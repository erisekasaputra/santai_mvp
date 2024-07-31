namespace TimedGuid; 
public class TimedGuid
{
    public DateTime Timestamp { get; }
    public Guid Id { get; }

    // Constructor
    private TimedGuid(DateTime timestamp, Guid id)
    {
        Timestamp = timestamp;
        Id = id;
    }

    // Static method to create a new TimedGuid
    public static TimedGuid NewTimedGuid()
    {
        var timestamp = DateTime.UtcNow;
        var id = Guid.NewGuid();
        return new TimedGuid(timestamp, id);
    }

    // Method to generate a string representation of TimedGuid
    public override string ToString()
    {
        // Format timestamp as sortable string and combine with GUID
        return $"{Timestamp:yyyyMMddHHmmssfff}-{Id}";
    }

    // Method to parse a TimedGuid from its string representation
    public static TimedGuid Parse(string value)
    {
        var parts = value.Split('-');
        if (parts.Length != 2)
            throw new FormatException("Invalid TimedGuid format.");

        var timestampString = parts[0];
        var guidString = parts[1];

        var timestamp = DateTime.ParseExact(timestampString, "yyyyMMddHHmmssfff", null);
        var guid = Guid.Parse(guidString);

        return new TimedGuid(timestamp, guid);
    }
}