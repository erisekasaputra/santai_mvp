namespace Master.Data.API.Models;

public class BasicInspection
{
    public string Description { get; private set; }
    public string Parameter { get; private set; }
    public int Value { get; private set; }

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
