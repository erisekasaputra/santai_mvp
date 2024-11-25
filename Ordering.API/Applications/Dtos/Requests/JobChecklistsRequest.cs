namespace Ordering.API.Applications.Dtos.Requests;

public class JobChecklistsRequest
{
    public required IEnumerable<JobChecklistRequest> JobChecklists { get; set; }
    public JobChecklistsRequest(
        IEnumerable<JobChecklistRequest> jobChecklists)
    {
        JobChecklists = jobChecklists;
    }
}
