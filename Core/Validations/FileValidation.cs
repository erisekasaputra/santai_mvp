using Microsoft.AspNetCore.Http;

namespace Core.Validations; 
public class FileValidation
{
    private static readonly string[] PermittedExtensions =
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"
    };

    private static readonly string[] PermittedMimeTypes =
    {
        "image/jpeg",    // .jpg, .jpeg
        "image/png",     // .png
        "image/gif",     // .gif
        "image/webp",    // .webp
        "image/svg+xml"  // .svg
    };


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
}
