using System;
using System.Net;
using System.Windows;
using RtspClientSharp;
using RtspViewer.GUI.Models;
using RtspViewer.ViewModels.Base;
using System.Threading.Tasks;
using RtspViewer.GUI.Services.Dialogs;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using RtspViewer.GUI.Services.IO;
using System.Text;
using System.Threading;
using System.Collections;
using RtspViewer.Properties;

namespace RtspViewer.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private const string RtspPrefix = "rtsp://";
        private const string HttpPrefix = "http://";

        private readonly DatabaseContext _db;
        private readonly ISourceDialogService _sourceDialogService;
        private readonly IFileDialogService _fileDialogService;
        private readonly IFolderDialogService _folderDialogService;
        private readonly ICsvService<Source> _sourceCsvService;
        private readonly ICsvService<Report> _reportCsvService;
        private readonly IRtspConnection _rtspConnection;
        private ObservableCollection<Source> _sources = null!;
        private Source? _selectedSource;
        private string _title = "Прямой эфир";
        private string _status = string.Empty;

        public ObservableCollection<Source> Sources
        {
            get => _sources;
            set => Set(ref _sources, value);
        }
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }
        public Source? SelectedSource
        {
            get => _selectedSource;
            set
            {
                Set(ref _selectedSource, value);
                Application.Current.Dispatcher.InvokeAsync(() => TryRestartStream());
            }
        }
        public bool IsTcpConnection
        {
            get => Settings.Default.UseTcp;
            set
            {
                Settings.Default.UseTcp = value;
                Settings.Default.Save();
                OnPropertyChanged();
                Application.Current.Dispatcher.InvokeAsync(() => TryRestartStream());
            }
        }
        private RtpTransportProtocol _rtpTransportProtocol => IsTcpConnection ? RtpTransportProtocol.TCP : RtpTransportProtocol.UDP;

        public IVideoSource VideoSource => _rtspConnection.VideoSource;

        public RelayCommand ClosingCommand { get; }
        public RelayCommand AddSourceCommand { get; }
        public RelayCommand EditSourceCommand { get; }
        public RelayCommand RemoveSourceCommand { get; }
        public RelayCommand PositiveReportCommand { get; }
        public RelayCommand EmptyReportCommand { get; }
        public RelayCommand NegativeReportCommand { get; }
        public RelayCommand ImportSourcesCommand { get; }
        public RelayCommand CreateImportSourcesTemplateCommand { get; }
        public RelayCommand ExportReportsCommand { get; }
        public RelayCommand StopVideoCommand { get; }
        public RelayCommand RestartVideoCommand { get; }
        public RelayCommand ChangeTcpConnectionStateCommand { get; }

        public MainWindowViewModel(DatabaseContext db, IRtspConnection rtspConnection,
            ISourceDialogService sourceDialogService, IFileDialogService fileDialogService, IFolderDialogService folderDialogService,
            ICsvService<Source> sourceCsvService, ICsvService<Report> reportCsvService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _sourceDialogService = sourceDialogService ?? throw new ArgumentNullException(nameof(sourceDialogService));
            _fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            _folderDialogService = folderDialogService ?? throw new ArgumentNullException(nameof(folderDialogService));
            _rtspConnection = rtspConnection ?? throw new ArgumentNullException(nameof(rtspConnection));
            _sourceCsvService = sourceCsvService ?? throw new ArgumentNullException(nameof(sourceCsvService));
            _reportCsvService = reportCsvService ?? throw new ArgumentNullException(nameof(reportCsvService));

            _db.Sources.Load();
            Sources = _db.Sources.Local.ToObservableCollection();

            ClosingCommand = new RelayCommand(_ => OnClosing());
            AddSourceCommand = new RelayCommand(async _ => await OnAddSourceButtonClick());
            EditSourceCommand = new RelayCommand(async _ => await OnEditSourceButtonClick(), _ => SelectedSource != null);
            RemoveSourceCommand = new RelayCommand(async _ => await OnRemoveSourceButtonClick(), _ => SelectedSource != null);
            PositiveReportCommand = new RelayCommand(async _ => await OnReportButtonClick(true), _ => SelectedSource != null);
            EmptyReportCommand = new RelayCommand(async _ => await OnReportButtonClick(null), _ => SelectedSource != null);
            NegativeReportCommand = new RelayCommand(async _ => await OnReportButtonClick(false), _ => SelectedSource != null);
            ImportSourcesCommand = new RelayCommand(async _ => await OnImportSourcesButtonClick());
            CreateImportSourcesTemplateCommand = new RelayCommand(async _ => await OnCreateImportSourcesTemplateButtonClick());
            ExportReportsCommand = new RelayCommand(async _ => await OnExportReportsButtonClick(), _ => _db.Reports.Any());
            StopVideoCommand = new RelayCommand(_ => StopStream(), _ => SelectedSource != null);
            RestartVideoCommand = new RelayCommand(async _ => await TryRestartStream(), _ => SelectedSource != null);
            ChangeTcpConnectionStateCommand = new RelayCommand(_ => IsTcpConnection = !IsTcpConnection);
        }

        private void StopStream()
        {
            _rtspConnection.Stop();
            _rtspConnection.StatusChanged -= MainWindowModelOnStatusChanged;
            Status = string.Empty;
        }

        private async Task TryRestartStream()
        {
            StopStream();

            if (_selectedSource == null) return;
            if (!await ValidateStreamSource(_selectedSource.Address, _selectedSource.Login, _selectedSource.Password)) 
            {
                _sourceDialogService.ShowErrorMessage("Не удалось установить соединение по указанным данным");
                return;
            }
            var address = _selectedSource.Address;
            if (!address.StartsWith(RtspPrefix) && !address.StartsWith(HttpPrefix))
                address = RtspPrefix + address;
            var deviceUri = new Uri(address, UriKind.Absolute);
            var credential = new NetworkCredential(_selectedSource.Login, _selectedSource.Password);
            var connectionParameters = !string.IsNullOrEmpty(deviceUri.UserInfo)
                ? new ConnectionParameters(deviceUri)
                : new ConnectionParameters(deviceUri, credential);
            connectionParameters.RtpTransport = _rtpTransportProtocol;
            connectionParameters.CancelTimeout = TimeSpan.FromSeconds(1);
            _rtspConnection.Start(connectionParameters);
            _rtspConnection.StatusChanged += MainWindowModelOnStatusChanged;
        }

        private async Task<bool> ValidateStreamSource(string address, string? login, string? password)
        {
            if (!address.StartsWith(RtspPrefix) && !address.StartsWith(HttpPrefix))
                address = RtspPrefix + address;

            if (!Uri.TryCreate(address, UriKind.Absolute, out Uri? deviceUri))
                return false;

            var credential = new NetworkCredential(login, password);
            var connectionParameters = !string.IsNullOrEmpty(deviceUri.UserInfo) ? new ConnectionParameters(deviceUri) :
                new ConnectionParameters(deviceUri, credential);
            var cancellationTokenSource = new CancellationTokenSource();

            using (RtspClient rtspClient = new RtspClient(connectionParameters))
            {
                try { await rtspClient.ConnectAsync(cancellationTokenSource.Token); }
                catch { cancellationTokenSource.Cancel(); return false; }
            }

            return true;
        }

        private async Task OnAddSourceButtonClick()
        {
            SelectedSource = null;
            if (!_sourceDialogService.ModifySourceDialog(new Source())) return;
            if (!await ValidateStreamSource(_sourceDialogService.ModifiedSource!.Address,
                _sourceDialogService.ModifiedSource.Login,
                _sourceDialogService.ModifiedSource.Password))
            {
                _sourceDialogService.ShowErrorMessage("Не удалось установить соединение по указанным данным\nИнформация не была сохранена");
                return;
            }
            _db.Sources.Add(new Source()
            {
                Name = _sourceDialogService.ModifiedSource!.Name,
                Address = _sourceDialogService.ModifiedSource.Address,
                Login = _sourceDialogService.ModifiedSource.Login,
                Password = _sourceDialogService.ModifiedSource.Password
            });
            await _db.SaveChangesAsync();
        }

        private async Task OnEditSourceButtonClick()
        {
            StopStream();
            if (!_sourceDialogService.ModifySourceDialog(new Source()
            {
                Name = SelectedSource!.Name,
                Login = SelectedSource.Login,
                Password = SelectedSource.Password,
                Address = SelectedSource.Address
            }))
                return;
            if (!await ValidateStreamSource(_sourceDialogService.ModifiedSource!.Address,
                _sourceDialogService.ModifiedSource.Login,
                _sourceDialogService.ModifiedSource.Password))
            {
                _sourceDialogService.ShowErrorMessage("Не удалось установить соединение по указанным данным\nИнформация не была сохранена");
                return;
            }
            SelectedSource.Name = _sourceDialogService.ModifiedSource!.Name;
            SelectedSource.Login = _sourceDialogService.ModifiedSource.Login;
            SelectedSource.Password = _sourceDialogService.ModifiedSource.Password;
            SelectedSource.Address = _sourceDialogService.ModifiedSource.Address;
            await _db.SaveChangesAsync();
            await TryRestartStream();
        }

        private async Task OnRemoveSourceButtonClick()
        {
            StopStream();
            if (!_sourceDialogService.ShowConfirmationMessage("Удалить выбранный источник?")) return;
            _db.Reports.RemoveRange(_db.Reports.Where(r => r.SourceId == SelectedSource!.Id));
            _db.Sources.Remove(SelectedSource!);
            await _db.SaveChangesAsync();
            SelectedSource = null;
        }

        private async Task OnReportButtonClick(bool? success)
        {
            if (SelectedSource == null) return;
            _db.Reports.Add(new Report
            {
                CreatedAt = DateTime.UtcNow,
                SourceId = SelectedSource.Id,
                Success = success
            });
            await _db.SaveChangesAsync();
        }

        public async Task OnImportSourcesButtonClick()
        {
            SelectedSource = null;
            if (!_fileDialogService.SelectFileDialog("csv")) return;
            System.Collections.Generic.IEnumerable<Source> sources;
            try
            {
                sources = _sourceCsvService.Read(_fileDialogService.SelectedFile);
            }
            catch (Exception ex)
            {
                _fileDialogService.ShowErrorMessage("Ошибка при чтении файла\nУбедитесь, что файл имеет кодировку UTF-8 и стандартный разделитель значений - шаблон можно сгенерировать через пункт меню");
                _fileDialogService.ShowErrorMessage(ex.Message);
                return;
            }
            if (!sources.Any()) return;
            if (_db.Sources.Local.Any() && _fileDialogService.ShowConfirmationMessage("Удалить уже добавленные источники?"))
            {
                _db.RemoveRange(_db.Reports);
                _db.RemoveRange(_db.Sources.Local);
                await _db.SaveChangesAsync();
            }
            var errCounter = 0;
            foreach (Source source in sources)
            {
                if (!await ValidateStreamSource(source.Address, source.Login, source.Password))
                {
                    errCounter++;
                    continue;
                }
                await _db.Sources.AddAsync(source);
            }
            await _db.SaveChangesAsync();
            if (errCounter > 0)
                _sourceDialogService.ShowErrorMessage($"Количество источников, с которыми не удалось установить соединение: {errCounter}\nОни не были импортированы");
        }

        public async Task OnCreateImportSourcesTemplateButtonClick()
        {
            if (!_folderDialogService.SelectFolderDialog()) return;
            try
            {
                var path = @$"{_folderDialogService.SelectedFolder}\Шаблон {Guid.NewGuid()}.csv";
                await _sourceCsvService.Write(Enumerable.Empty<Source>(), path);
                _fileDialogService.ShowInformationMessage($"Шаблон сохранён в {path}");
            }
            catch (Exception ex)
            {
                _fileDialogService.ShowErrorMessage("Ошибка при записи файла");
                _fileDialogService.ShowErrorMessage(ex.Message);
            }
        }

        public async Task OnExportReportsButtonClick()
        {
            if (!_folderDialogService.SelectFolderDialog()) return;
            var reports = _db.Reports.Include(r => r.Source).OrderByDescending(r => r.Id);
            if (!reports.Any())
            {
                _fileDialogService.ShowInformationMessage("Оценок нет");
                return;
            }
            try
            {
                var path = @$"{_folderDialogService.SelectedFolder}\Оценки на {DateTime.Now.ToString("dd-MM-yyyy")} {Guid.NewGuid()}.csv";
                await _reportCsvService.Write(reports, path);
                _fileDialogService.ShowInformationMessage($"Оценки сохранены в {path}");
            }
            catch (Exception ex)
            {
                _fileDialogService.ShowErrorMessage("Ошибка при записи файла");
                _fileDialogService.ShowErrorMessage(ex.Message);
            }
        }

        private void MainWindowModelOnStatusChanged(object sender, string s)
        {
            Application.Current.Dispatcher.Invoke(() => Status = s);
        }

        private void OnClosing()
        {
            _rtspConnection.Stop();
        }
    }
}