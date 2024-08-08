using Account.Domain.Aggregates.ReferralAggregate;
using MediatR;

namespace Account.Domain.Events;

public record ReferralProgramAddedDomainEvent(ReferralProgram ReferralProgram) : INotification;
