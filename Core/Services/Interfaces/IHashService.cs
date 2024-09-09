namespace Core.Services.Interfaces;

public interface IHashService
{ 
    Task<string> Hash(string input);
}
