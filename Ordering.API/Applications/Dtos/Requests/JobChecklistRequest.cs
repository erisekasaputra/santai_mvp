namespace Ordering.API.Applications.Dtos.Requests
{
    public class JobChecklistRequest
    {
        public required string Description { get; set; }
        public required string Parameter { get; set; }
        public required bool Value { get; set; } 
        public JobChecklistRequest(
            string description,
            string parameter,
            bool value)
        {
            Description = description;
            Parameter = parameter;
            Value = value; 
        }
    }
} 