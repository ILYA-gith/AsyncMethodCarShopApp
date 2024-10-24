using CarShop;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

public class StreamService<T>
{
    public async Task WriteToStreamAsync(Stream stream, IEnumerable<T> data, IProgress<string> progress, IProgress<int> percentage)
    {
        using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true))
        {
            int total = data.Count();
            int count = 0;

            foreach (var item in data)
            {
                await Task.Delay(5000);
     
                await writer.WriteLineAsync(item.ToString());
                count++;
                int percentComplete = (count * 100) / total;
                progress.Report($"Поток {Thread.CurrentThread.ManagedThreadId}: Запись элемента: {item}");
                percentage.Report(percentComplete);
            }
        }
    }

    public async Task CopyFromStreamAsync(Stream stream, string filename, IProgress<string> progress, IProgress<int> percentage)
    {
        using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
        {
            long totalBytes = stream.Length;
            long copiedBytes = 0;
            byte[] buffer = new byte[81920];
            int bytesRead;

            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                copiedBytes += bytesRead;
                int percentComplete = (int)((copiedBytes * 100) / totalBytes);
                progress.Report($"Поток {Thread.CurrentThread.ManagedThreadId}: Копирование данных...");
                percentage.Report(percentComplete);
            }

            progress.Report($"Поток {Thread.CurrentThread.ManagedThreadId}: Копирование завершено в файл: {filename}");
        }
    }

    public async Task<int> GetStatisticsAsync(string fileName, Func<T, bool> filter)
    {
        int count = 0;
        using (StreamReader reader = new StreamReader(fileName))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                var parts = line.Split(',');
                var car = new Car
                {
                    Id = int.Parse(parts[0]),
                    Model = parts[1],
                    Engine = double.Parse(parts[2])
                };
                if (filter((T)(object)car))
                {
                    count++;
                }
            }
        }
        return count;
    }
}
