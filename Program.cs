using CarShop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var cars = new List<Car>();
        var random = new Random();
        for (int i = 0; i < 1000; i++)
        {
            cars.Add(new Car { Id = i, Model = $"Model {i}", Engine = random.NextDouble() * 3 });
        }

        var autoSalon = new AutoSalon { Id = 1, Name = "My Auto Salon", Cars = cars };

        var streamService = new StreamService<Car>();
        var memoryStream = new MemoryStream();
        var progress = new Progress<string>(message => Console.WriteLine(message));
        var percentage = new Progress<int>(percent => Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: {percent}% завершено"));

        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: Начало работы");

        var writeTask = streamService.WriteToStreamAsync(memoryStream, cars, progress, percentage);
        await Task.Delay(200); // Задержка для последовательности запуска
        var copyTask = streamService.CopyFromStreamAsync(memoryStream, "cars.txt", progress, percentage);

        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: Поток 1 запущен");
        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: Поток 2 запущен");

        await Task.WhenAll(writeTask, copyTask);

        int count = await streamService.GetStatisticsAsync("cars.txt", car => car.Engine > 2.0);
        Console.WriteLine($"Количество автомобилей с двигателем более двух литров: {count}");

        // Выводим автомобили с двигателем более двух литров
        var carsWithBigEngines = cars.Where(car => car.Engine > 2.0).ToList();
        Console.WriteLine("Автомобили с двигателем более двух литров:");
        foreach (var car in carsWithBigEngines)
        {
            Console.WriteLine($"Id: {car.Id}, Model: {car.Model}, Engine Capacity: {car.Engine}");
        }

        // Ожидание ввода от пользователя
        Console.WriteLine("Нажмите любую клавишу для завершения...");
        Console.ReadKey();
    }
}
