namespace Ordering.API.Applications.Dtos.Requests;

public class JobChecklistsRequest
{
    public required IEnumerable<JobChecklistRequest> JobChecklists { get; set; }
    public required string Comment { get; set; }
    public JobChecklistsRequest(
        IEnumerable<JobChecklistRequest> jobChecklists, string comment)
    {
        JobChecklists = jobChecklists;
        Comment = comment;
    }
}
