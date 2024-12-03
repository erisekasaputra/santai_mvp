namespace Master.Data.API.Domain.Entity;

public class Banner
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImagePath { get; private set; }
    public bool IsActive { get; private set; }

    public Banner()
    {
        Name = string.Empty;
        Description = string.Empty;
        ImagePath = string.Empty;
        IsActive = false;
    }

    public Banner(  
        string name, 
        string description,
        string imagePath,
        bool isActive)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        ImagePath = imagePath;
        IsActive = isActive;
    }

    public void Update(
        string name, 
        string description,
        string imagePath,
        bool isActive)
    {
        Name = name;
        Description = description;
        ImagePath = imagePath;
        IsActive = isActive;
    }
}
