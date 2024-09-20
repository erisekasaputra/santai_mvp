namespace Core.Events.Models;

public record AddressIntegrationEvent(
    string AddressLine1,
    string? AddressLine2,
    string? AddressLine3,
    string City,
    string State,
    string PostalCode,
    string Country);