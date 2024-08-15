namespace FileHub.API.Validations.Abstraction;

public interface IFileValidation
{
    bool IsValidImage(IFormFile file);
}
