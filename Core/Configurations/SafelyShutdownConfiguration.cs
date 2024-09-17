namespace Core.Configurations;

public class SafelyShutdownConfiguration
{
    public const string SectionName = "SafelyShutdown";
    public bool Shutdown { get; set; } = false;    
}
