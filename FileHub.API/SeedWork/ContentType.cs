namespace FileHub.API.SeedWork;

public static class ContentType
{
    private static Dictionary<string, string> _mimeTypes = new Dictionary<string, string>
    {
         { "png", "image/png" },
         { "jpg", "image/jpeg" },
         { "jpeg", "image/jpeg" },
         { "gif", "image/gif" },
         { "bmp", "image/bmp" },
         { "ico", "image/x-icon" },
         { "webp", "image/webp" },
         { "svg", "image/svg+xml" },
         { "tiff", "image/tiff" },
         { "tif", "image/tiff" },
         { "pdf", "application/pdf" },
         { "zip", "application/zip" },
         { "gzip", "application/gzip" },
         { "tar", "application/x-tar" },
         { "7z", "application/x-7z-compressed" },
         { "rar", "application/x-rar-compressed" },
         { "xhtml", "application/xhtml+xml" },
         { "xml", "application/xml" },
         { "json", "application/json" },
         { "csv", "text/csv" },
         { "html", "text/html" },
         { "txt", "text/plain" },
         { "rtf", "application/rtf" },
         { "md", "text/markdown" },
         { "css", "text/css" },
         { "js", "application/javascript" },
         { "mp3", "audio/mpeg" },
         { "wav", "audio/wav" },
         { "ogg", "audio/ogg" },
         { "mp4", "video/mp4" },
         { "avi", "video/x-msvideo" },
         { "mov", "video/quicktime" },
         { "mkv", "video/x-matroska" },
         { "webm", "video/webm" },
         { "flv", "video/x-flv" },
         { "wmv", "video/x-ms-wmv" },
         { "asf", "video/x-ms-asf" },
         { "m4v", "video/x-m4v" },
         { "mpg", "video/mpeg" },
         { "mpeg", "video/mpeg" },
         { "epub", "application/epub+zip" },
         { "mobi", "application/x-mobipocket-ebook" },
         { "azw", "application/vnd.amazon.ebook" },
         { "apk", "application/vnd.android.package-archive" },
         { "dmg", "application/x-apple-diskimage" },
         { "exe", "application/x-msdownload" },
         { "dll", "application/x-msdownload" },
         { "bin", "application/octet-stream" },
         { "iso", "application/x-iso9660-image" },
         { "torrent", "application/x-bittorrent" },
         { "psd", "image/vnd.adobe.photoshop" },
         { "ai", "application/postscript" },
         { "eps", "application/postscript" },
         { "ppt", "application/vnd.ms-powerpoint" },
         { "pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
         { "xls", "application/vnd.ms-excel" },
         { "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
         { "doc", "application/msword" },
         { "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
         { "odt", "application/vnd.oasis.opendocument.text" },
         { "ods", "application/vnd.oasis.opendocument.spreadsheet" }
    };  

    public static string GetContentType(string type)
    {
        string fileExtension = type.ToLower().TrimStart('.');

        if (_mimeTypes.TryGetValue(fileExtension, out string? contentType))
        {
            return contentType ?? "application/octet-stream";
        }

        return "application/octet-stream";
    }
}
