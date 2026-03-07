using System.Text.RegularExpressions;

namespace BuildingBlocks.Primitives.IO;

public static partial class FileHelper
{
    private static readonly Dictionary<string, string> MimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".txt"] = "text/plain",
        [".html"] = "text/html",
        [".htm"] = "text/html",
        [".css"] = "text/css",
        [".js"] = "application/javascript",
        [".json"] = "application/json",
        [".xml"] = "application/xml",
        [".csv"] = "text/csv",
        [".pdf"] = "application/pdf",
        [".zip"] = "application/zip",
        [".gz"] = "application/gzip",
        [".tar"] = "application/x-tar",
        [".png"] = "image/png",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".gif"] = "image/gif",
        [".svg"] = "image/svg+xml",
        [".webp"] = "image/webp",
        [".ico"] = "image/x-icon",
        [".mp3"] = "audio/mpeg",
        [".mp4"] = "video/mp4",
        [".webm"] = "video/webm",
        [".doc"] = "application/msword",
        [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        [".xls"] = "application/vnd.ms-excel",
        [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        [".ppt"] = "application/vnd.ms-powerpoint",
        [".pptx"] = "application/vnd.openxmlformats-officedocument.presentationml.presentation"
    };

    private static readonly string[] SizeUnits = ["B", "KB", "MB", "GB", "TB"];

    public static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);
        var extension = Path.GetExtension(fileName);

        var sanitized = InvalidFileNameChars().Replace(name, "_");
        sanitized = sanitized.Trim('.', '_', ' ');

        if (string.IsNullOrEmpty(sanitized))
            sanitized = "file";

        return sanitized + extension;
    }

    public static string GetMimeType(string fileNameOrExtension)
    {
        var extension = fileNameOrExtension.StartsWith('.')
            ? fileNameOrExtension
            : Path.GetExtension(fileNameOrExtension);

        return MimeTypes.GetValueOrDefault(extension, "application/octet-stream");
    }

    public static string FormatFileSize(long bytes)
    {
        if (bytes < 0)
            throw new ArgumentOutOfRangeException(nameof(bytes));

        if (bytes == 0)
            return "0 B";

        var unitIndex = 0;
        var size = (double)bytes;

        while (size >= 1024 && unitIndex < SizeUnits.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return unitIndex == 0
            ? string.Create(System.Globalization.CultureInfo.InvariantCulture, $"{size:0} {SizeUnits[unitIndex]}")
            : string.Create(System.Globalization.CultureInfo.InvariantCulture, $"{size:0.##} {SizeUnits[unitIndex]}");
    }

    [GeneratedRegex(@"[^a-zA-Z0-9_\-.]")]
    private static partial Regex InvalidFileNameChars();
}
