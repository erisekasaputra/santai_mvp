using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Core.Validations; 
public class FileValidation
{
    private static readonly string[] PermittedExtensions =
    [
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"
    ];

    private static readonly string[] PermittedMimeTypes =
    [
        "image/jpeg",    // .jpeg
        "image/png",     // .png
        "image/gif",     // .gif
        "image/webp",    // .webp
        "image/svg+xml",  // .svg
        "image/jpg"
    ];


    public static bool IsValidImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        // Validate MIME type
        if (!PermittedMimeTypes.Contains(file.ContentType.ToLower()))
            return false;

        // Validate file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !PermittedExtensions.Contains(extension))
            return false;

        return true;
    }  

    public static bool IsValidImageFileName(string fileName)
    { 
        string pattern = @"^[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}\.(jpg|jpeg|png|gif|webp|svg)$";

        return Regex.IsMatch(fileName, pattern);
    } 
}
