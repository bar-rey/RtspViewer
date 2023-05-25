using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtspViewer.GUI.Services.IO
{
    /// <summary>
    /// Запись и чтение Csv файлов
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICsvService<T>
    {
        IEnumerable<T> Read(string path);
        Task Write(IEnumerable<T> records, string path);
    }
}
