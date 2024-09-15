using Account.API.Applications.Commands.StaffCommand.AssignStaffEmailByUserId; 
using Account.API.Applications.Commands.UserCommand.AssignUserEmailByUserId; 
using Core.Enumerations;
using Core.Events; 
using MassTransit;
using MediatR;

namespace Account.API.Applications.Consumers;

public class IdentityEmailAssignedToAUserIntegrationEventConsumer(
    IMediator mediator,
    ILogger<IdentityPhoneNumberConfirmedIntegrationEventConsumer> logger) : IConsumer<IdentityEmailAssignedToAUserIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<IdentityPhoneNumberConfirmedIntegrationEventConsumer> _logger = logger;

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
                 
                throw new Exception(responseStaff.Message);
            }

            return;
        }

        var command = new AssignUserEmailByUserIdCommand(user.Id, user.Email);
        var response = await _mediator.Send(command);

        if (!response.IsSuccess)
        {
            _logger.LogError("An error occured. error: '{Message}'. When assigning the email: {email} for registered user id: {Id}", response.Message, user.Email, user.Id);
             
            throw new Exception(response.Message);
        }
    }
}
