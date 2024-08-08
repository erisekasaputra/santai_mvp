using Microsoft.EntityFrameworkCore.Storage.ValueConversion; 

namespace Account.Infrastructure.Extensions;

public class StringListConverter : ValueConverter<ICollection<string>?, string>
{
    public StringListConverter()
      : base(
          v => v == null || !v.Any() ? "" : string.Join(",", v),  
          v => v == null || string.IsNullOrWhiteSpace(v) ? new List<string>() : v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()) 
    {
    }
}
