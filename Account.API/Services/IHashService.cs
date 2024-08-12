namespace Account.API.Services;

public interface IHashService
{ 
    Task<string> Hash(string input); 
}

