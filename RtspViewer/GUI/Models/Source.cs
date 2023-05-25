using RtspViewer.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RtspViewer.GUI.Models
{
    public class Source : Model, IDataErrorInfo
    {
        [CsvHelper.Configuration.Attributes.Ignore]
        int _id;
        string _name = null!;
        string _address = null!;
        string? _login;
        string? _password;

        [CsvHelper.Configuration.Attributes.Ignore]
        public List<Report> Reports { get; set; } = null!;
        [CsvHelper.Configuration.Attributes.Ignore]
        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }
        [CsvHelper.Configuration.Attributes.Name("Название")]
        [StringLength(15, MinimumLength = 1)]
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        [CsvHelper.Configuration.Attributes.Name("Адрес")]
        [StringLength(2000, MinimumLength = 5)]
        public string Address
        {
            get => _address;
            set => Set(ref _address, value);
        }
        [CsvHelper.Configuration.Attributes.Name("Логин")]
        [StringLength(150)]
        public string? Login
        {
            get => _login;
            set => Set(ref _login, value);
        }
        [CsvHelper.Configuration.Attributes.Name("Пароль")]
        [StringLength(150)]
        public string? Password
        {
            get => _password; 
            set => Set(ref _password, value);
        }

        private StringBuilder _errorMessage = new StringBuilder();
        [CsvHelper.Configuration.Attributes.Ignore]
        string IDataErrorInfo.Error
        {
            get { return _errorMessage.ToString(); }
        }
        [CsvHelper.Configuration.Attributes.Ignore]
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                _errorMessage.Clear();
                switch (columnName)
                {
                    case "Name":
                        {
                            if (string.IsNullOrEmpty(Name))
                                _errorMessage.AppendLine("Название не может быть пустым");
                            else if (Name.Length > 15 || Name.Length < 1)
                                _errorMessage.AppendLine("Длина названия должна составлять от 1 до 15 символов");
                        }
                        break;
                    case "Address":
                        {
                            if (string.IsNullOrEmpty(Address))
                                _errorMessage.AppendLine("Адрес не может быть пустым");
                            else if (Address.Length > 2000 || Address.Length < 5)
                                _errorMessage.AppendLine("Длина адреса должна составлять от 5 до 2000 символов");
                        }
                        break;
                    case "Login":
                        {
                            if (string.IsNullOrEmpty(Login))
                                break;
                            if (Login.Length > 150)
                                _errorMessage.AppendLine("Длина логина не должна превышать 150 символов");
                        }
                        break;
                    case "Password":
                        {
                            if (string.IsNullOrEmpty(Password))
                                break;
                            if (Password.Length > 150)
                                _errorMessage.AppendLine("Длина пароля не должна превышать 150 символов");
                        }
                        break;
                    default: break;
                }
                return _errorMessage.ToString();
            }
        }
    }
}
