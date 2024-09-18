namespace Master.Data.API.Models;

public class PreServiceInspection
{
    public string Description { get; private set; }
    public string Parameter { get; private set; }
    public int Rating { get; private set; }
    public IEnumerable<PreServiceInspectionResult> PreServiceInspectionResults { get; private set; }
    public PreServiceInspection(
        string description,
        string parameter,
        int rating,
        IEnumerable<PreServiceInspectionResult> preServiceInspectionResults)
    {
        Description = description;
        Parameter = parameter;
        Rating = rating;
        PreServiceInspectionResults = preServiceInspectionResults;
    }
}
