namespace Master.Data.API.Models;

public class BasicInspection
{
    public string Description { get; set; }
    public string Parameter { get; set; }
    public int Value { get; set; }

    public BasicInspection(
        string description,
        string parameter,
        int value)
    {
        Description = description;
        Parameter = parameter;
        Value = value;
    }
}
