using Account.API.Applications.Commands.StaffCommand.AssignStaffEmailByUserId;
using Account.API.Applications.Commands.StaffCommand.ConfirmStaffPhoneNumberByStaffId;
using Account.API.Applications.Commands.UserCommand.AssignUserEmailByUserId;
using Account.API.Applications.Commands.UserCommand.ConfirmUserPhoneNumberByUserId;
using Identity.Contracts.Enumerations;
using Identity.Contracts.IntegrationEvent;
using MassTransit;
using MediatR;

namespace Account.API.Consumers;

public class IdentityEmailAssignedToAUserIntegrationEventConsumer : IConsumer<IdentityEmailAssignedToAUserIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<IdentityPhoneNumberConfirmedIntegrationEventConsumer> _logger;

    public IdentityEmailAssignedToAUserIntegrationEventConsumer(
        IMediator mediator,
        ILogger<IdentityPhoneNumberConfirmedIntegrationEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IdentityEmailAssignedToAUserIntegrationEvent> context)
    {
        var user = context.Message;

        if (user.UserType == UserType.StaffUser)
        {
            var commandStaff = new AssignStaffEmailByUserIdCommand(user.Id, user.Email);
            var responseStaff = await _mediator.Send(commandStaff);

            if (!responseStaff.IsSuccess)
            {
                _logger.LogError("An error occured. error: '{Message}'. When assigning the email: {email} for registered staff user id: {Id}", responseStaff.Message, user.Email, user.Id); 
            }
            return;
        }

        var command = new AssignUserEmailByUserIdCommand(user.Id, user.Email);
        var response = await _mediator.Send(command);

        if (!response.IsSuccess)
        {
            _logger.LogError("An error occured. error: '{Message}'. When assigning the email: {email} for registered user id: {Id}", response.Message, user.Email, user.Id); 
        }
    }
}
