namespace Ordering.API.Applications.Dtos.Responses
{
    public class JobChecklistResponseDto
    {
        public string Description { get; set; }
        public string Parameter { get; set; }
        public bool Value { get; set; } 
        public JobChecklistResponseDto(
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
