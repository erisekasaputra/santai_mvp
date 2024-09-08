namespace Account.API.Applications.Services.Interfaces;

public interface IHashService
{
    Task<string> Hash(string input);
}

