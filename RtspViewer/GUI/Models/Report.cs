using RtspViewer.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtspViewer.GUI.Models
{
    public class Report : Model
    {
        bool? _success;
        DateTime _createdAt = DateTime.UtcNow;
        int _sourceId;
        Source _source;

        [CsvHelper.Configuration.Attributes.Ignore]
        public int Id { get; set; }
        [CsvHelper.Configuration.Attributes.Ignore]
        public int SourceId
        {
            get => _sourceId;
            set => Set(ref _sourceId, value);
        }
        public Source Source
        {
            get => _source;
            set => Set(ref _source, value);
        }
        [CsvHelper.Configuration.Attributes.Ignore]
        public bool? Success
        {
            get => _success;
            set => Set(ref _success, value);
        }
        [CsvHelper.Configuration.Attributes.Name("Оценка")]
        public string SuccessMessage => Success switch {
            true => "хорошо",
            false => "плохо",
            _ => "не знаю"
        };
        [CsvHelper.Configuration.Attributes.Name("Дата создания")]
        public DateTime CreatedAt
        {
            get => _createdAt;
            set => Set(ref _createdAt, value);
        }
    }
}
