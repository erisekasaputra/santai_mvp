using Account.API.Applications.Commands.StaffCommand.ResetPhoneNumberByStaffId;
using Account.API.Applications.Commands.UserCommand.ResetPhoneNumberByUserId; 
using Identity.Contracts.Enumerations;
using Identity.Contracts.IntegrationEvent;
using MassTransit;
using MediatR;

namespace Account.API.Consumers;

public class PhoneNumberDuplicateIntegrationEventConsumer(
    IMediator mediator, 
    ILogger<PhoneNumberDuplicateIntegrationEventConsumer> logger) : IConsumer<PhoneNumberDuplicateIntegrationEvent>
{  
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<PhoneNumberDuplicateIntegrationEventConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<PhoneNumberDuplicateIntegrationEvent> context)
    { 
        foreach(var user in context.Message.DuplicateUsers)
        {  
            if (user.UserType == UserType.BusinessUser)
            {
                var command = new ResetPhoneNumberByUserIdCommand(user.Id);
                var response = await _mediator.Send(command);
                
                if (!response.IsSuccess)
                {
                    _logger.LogError("An error occured. error: '{Message}'. When reseting the phone number: {phoneNumber} for registered user id: {Id}", response.Message, user.PhoneNumber, user.Id);
                }
            }

            if (user.UserType == UserType.StaffUser)
            {
                var command = new ResetPhoneNumberByStaffIdCommand(user.Id);
                var response = await _mediator.Send(command);

                if (!response.IsSuccess)
                {
                    _logger.LogError("An error occured. error: '{Message}'. When reseting the phone number: {phoneNumber} for registered user id: {Id}", response.Message, user.PhoneNumber, user.Id);
                }
            } 
        }
    }
}
    