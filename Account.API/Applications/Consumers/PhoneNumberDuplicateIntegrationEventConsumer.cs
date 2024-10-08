using Account.API.Applications.Commands.BusinessUserCommand.DeleteBusinessUserByUserId;
using Account.API.Applications.Commands.MechanicUserCommand.DeleteMechanicUserByUserId;
using Account.API.Applications.Commands.RegularUserCommand.DeleteRegularUserByUserId;
using Account.API.Applications.Commands.StaffCommand.RemoveStaffByUserId;
using Core.Enumerations;
using Core.Events.Identity;
using Core.Results;
using MassTransit;
using MediatR;  

namespace Account.API.Applications.Consumers;

public class PhoneNumberDuplicateIntegrationEventConsumer(
    IMediator mediator,
    ILogger<PhoneNumberDuplicateIntegrationEventConsumer> logger) : IConsumer<PhoneNumberDuplicateIntegrationEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<PhoneNumberDuplicateIntegrationEventConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<PhoneNumberDuplicateIntegrationEvent> context)
    { 
        foreach (var user in context.Message.DuplicateUsers)
        {  
            if (user.UserType == UserType.BusinessUser)
            {
                var command = new DeleteBusinessUserByUserIdCommand(user.Id);
                var response = await _mediator.Send(command);

                if (!response.IsSuccess)
                {
                    _logger.LogError("An error occured. error: '{Message}'. When reseting the phone number: {phoneNumber} for registered user id: {Id}", response.Message, user.PhoneNumber, user.Id);

                    if (response.ResponseStatus is not ResponseStatus.NotFound and ResponseStatus.BadRequest)
                    {
                        throw new Exception(response.Message);
                    } 
                }
            }

            if (user.UserType == UserType.StaffUser)
            {
                var command = new RemoveStaffByUserIdCommand(user.Id);
                var response = await _mediator.Send(command);

                if (!response.IsSuccess)
                {
                    _logger.LogError("An error occured. error: '{Message}'. When reseting the phone number: {phoneNumber} for registered user id: {Id}", response.Message, user.PhoneNumber, user.Id);

                    if (response.ResponseStatus is not ResponseStatus.NotFound and ResponseStatus.BadRequest)
                    {
                        throw new Exception(response.Message);
                    }
                }
            }

            if (user.UserType == UserType.RegularUser)
            {
                var command = new DeleteRegularUserByUserIdCommand(user.Id);
                var response = await _mediator.Send(command);

                if (!response.IsSuccess)
                {
                    _logger.LogError("An error occured. error: '{Message}'. When reseting the phone number: {phoneNumber} for registered user id: {Id}", response.Message, user.PhoneNumber, user.Id);

                    if (response.ResponseStatus is not ResponseStatus.NotFound and ResponseStatus.BadRequest)
                    {
                        throw new Exception(response.Message);
                    }
                }
            }

            if (user.UserType == UserType.MechanicUser)
            {
                var command = new DeleteMechanicUserByUserIdCommand(user.Id);
                var response = await _mediator.Send(command);

                if (!response.IsSuccess)
                {
                    _logger.LogError("An error occured. error: '{Message}'. When reseting the phone number: {phoneNumber} for registered user id: {Id}", response.Message, user.PhoneNumber, user.Id);

                    if (response.ResponseStatus is not ResponseStatus.NotFound and ResponseStatus.BadRequest)
                    {
                        throw new Exception(response.Message);
                    }
                }
            } 
        }
    }
}
