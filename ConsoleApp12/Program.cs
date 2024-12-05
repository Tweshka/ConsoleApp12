using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

// Интерфейс команды
public interface ICommand
{
    Task Execute(YoutubeClient youtube, string videoUrl);
}

// Команда для получения информации о видео
public class GetVideoInfoCommand : ICommand
{
    public async Task Execute(YoutubeClient youtube, string videoUrl)
    {
        try
        {
            var videoInfo = await youtube.Videos.GetAsync(videoUrl);
            Console.WriteLine($"Название видео: {videoInfo.Title}");
            Console.WriteLine($"Описание видео: {videoInfo.Description}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении информации о видео: {ex.Message}");
        }
    }
}

// Команда для скачивания видео
public class DownloadVideoCommand : ICommand
{
    private readonly string _outputPath;

    public DownloadVideoCommand(string outputPath)
    {
        _outputPath = outputPath;
    }

    public async Task Execute(YoutubeClient youtube, string videoUrl)
    {
        try
        {
            await youtube.Videos.DownloadAsync(videoUrl, _outputPath, builder => builder.SetPreset(ConversionPreset.UltraFast));
            Console.WriteLine($"Видео успешно скачано в {_outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при скачивании видео: {ex.Message}");
        }
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        // Проверка наличия ссылки на видео
        if (args.Length == 0)
        {
            Console.WriteLine("Укажите ссылку на YouTube-видео в качестве аргумента командной строки.");
            return;
        }
        string videoUrl = args[0];

        // Инициализация YoutubeClient
        var youtube = new YoutubeClient();

        // Список команд
        var commands = new ICommand[]
        {
            new GetVideoInfoCommand(),
            new DownloadVideoCommand("output.mp4") // Укажите желаемый путь к файлу
        };

        // Выполнение команд
        foreach (var command in commands)
        {
            Console.WriteLine($"Выполнение команды: {command.GetType().Name}");
            await command.Execute(youtube, videoUrl);
            Console.WriteLine();
        }
    }
}