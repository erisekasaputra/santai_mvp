namespace Core.Authentications;

public class ClientService
{
    public const string IdentityService = "IdentityService";
    public const string CatalogService = "CatalogService";
    public const string AccountService = "AccountService";
    public const string ChatService = "ChatService";
    public const string FileHubService = "FileHubService";
    public const string HistoryService = "HistoryService";
    public const string LocationService = "LocationService";

    public static string[] GetAll()
    {
        return [IdentityService, CatalogService, AccountService, ChatService, FileHubService, HistoryService, LocationService];
    }
}
