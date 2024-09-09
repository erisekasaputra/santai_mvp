using Identity.API.Domain.Entities;
using MediatR;

namespace Identity.API.Domain.Events;

public record PhoneNumberDuplicateDomainEvent(IEnumerable<ApplicationUser> Users) : INotification;
