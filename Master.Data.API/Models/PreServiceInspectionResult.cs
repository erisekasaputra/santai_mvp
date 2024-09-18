namespace Master.Data.API.Models;

public class PreServiceInspectionResult
{
    public string Description { get; private set; }
    public string Parameter { get; private set; }
    public bool IsWorking { get; private set; }
    public PreServiceInspectionResult(
        string description,
        string parameter,
        bool isWorking)
    {
        Description = description;
        Parameter = parameter;
        IsWorking = isWorking;
    }
}
