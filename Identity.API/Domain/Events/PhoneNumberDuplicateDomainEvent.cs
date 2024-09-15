using Core.Models;
using Identity.API.Domain.Entities;
using MediatR;

namespace Identity.API.Domain.Events;

public record PhoneNumberDuplicateDomainEvent(IEnumerable<DuplicateUser> Users) : INotification;
