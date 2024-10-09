namespace Master.Data.API.Models;

public class PreServiceInspection
{
    public string Description { get; set; }
    public string Parameter { get; set; }
    public int Rating { get; set; }
    public IEnumerable<PreServiceInspectionResult> PreServiceInspectionResults { get; set; }

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
