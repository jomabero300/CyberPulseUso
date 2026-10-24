using System.Globalization;
using System.Text.RegularExpressions;

namespace CyberPulseUso.Backend.Helpers;

public static partial class HtmlUtilities
{
    public static string StripTags(string? html)
    {
        if (string.IsNullOrEmpty(html))
            return html ?? string.Empty;

        html = Regex.Replace(html, @"<\s*br\s*/?\s*>", "\n", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"</\s*(p|div|li|h[1-6])\s*>", "\n", RegexOptions.IgnoreCase);
        html = Regex.Replace(html, @"<[^>]*(>|$)", string.Empty);
        return System.Net.WebUtility.HtmlDecode(html);
    }

    public static string ToTitleCase(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(value);
    }

    public static async Task<string> UploadImageAsync(string image, string directoryFolder, string imageUrl = "")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(image);
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryFolder);

        var base64 = image;
        var commaIndex = image.IndexOf(',');
        if (image.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex >= 0)
            base64 = image[(commaIndex + 1)..];

        var imageBytes = Convert.FromBase64String(base64);
        Directory.CreateDirectory(directoryFolder);

        var fileName = $"{Guid.NewGuid():N}.jpg";
        var fullPath = Path.Combine(directoryFolder, fileName);
        await File.WriteAllBytesAsync(fullPath, imageBytes);

        // Solo elimina la imagen anterior si se encuentra dentro de la carpeta destino.
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            var oldFullPath = Path.GetFullPath(imageUrl);
            var destinationFullPath = Path.GetFullPath(directoryFolder) + Path.DirectorySeparatorChar;
            if (oldFullPath.StartsWith(destinationFullPath, StringComparison.OrdinalIgnoreCase) && File.Exists(oldFullPath))
                File.Delete(oldFullPath);
        }

        return fileName;
    }
}
