using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtspViewer.GUI.Services.IO
{
    public class CsvService<T> : ICsvService<T>
    {
        public IEnumerable<T> Read(string path)
        {
            using (var reader = new StreamReader(path, encoding: Encoding.UTF8))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture)))
            return csv.GetRecords<T>().ToList();
        }

        public async Task Write(IEnumerable<T> records, string path)
        {
            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.CurrentCulture)))
            await csv.WriteRecordsAsync(records);
        }
    }
}
