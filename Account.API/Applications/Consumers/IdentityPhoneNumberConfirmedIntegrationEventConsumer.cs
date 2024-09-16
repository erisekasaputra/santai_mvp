using Account.API.Applications.Commands.StaffCommand.ConfirmStaffPhoneNumberByStaffId;
using Account.API.Applications.Commands.UserCommand.ConfirmUserPhoneNumberByUserId;
using Azure;
using Core.Enumerations;
using Core.Events;
using Core.Results;
using MassTransit;
using MediatR;

namespace Account.API.Applications.Consumers;

public class IdentityPhoneNumberConfirmedIntegrationEventConsumer : IConsumer<IdentityPhoneNumberConfirmedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<IdentityPhoneNumberConfirmedIntegrationEventConsumer> _logger;
    public IdentityPhoneNumberConfirmedIntegrationEventConsumer(
        IMediator mediator,
        ILogger<IdentityPhoneNumberConfirmedIntegrationEventConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IdentityPhoneNumberConfirmedIntegrationEvent> context)
    {
        var user = context.Message;

        if (user.UserType == UserType.StaffUser)
        {
            var commandStaff = new ConfirmStaffPhoneNumberByStaffIdCommand(user.Id);
            var responseStaff = await _mediator.Send(commandStaff);

            if (!responseStaff.IsSuccess)
            {
                _logger.LogError("An error occured. error: '{Message}'. When confirming the phone number: {phoneNumber} for registered staff user id: {Id}", responseStaff.Message, user.PhoneNumber, user.Id); 
            }

            return;
        }


        if (user.UserType == UserType.BusinessUser)
        {
            var command = new ConfirmUserPhoneNumberByUserIdCommand(user.Id);
            var response = await _mediator.Send(command);

            if (!response.IsSuccess)
            {
                _logger.LogError("An error occured. error: '{Message}'. When confirming the phone number: {phoneNumber} for registered user id: {Id}", response.Message, user.PhoneNumber, user.Id); 
            }

            return;
        }


        var commandForAll = new ConfirmUserPhoneNumberByUserIdCommand(user.Id);
        var responseForAll = await _mediator.Send(commandForAll);

        if (!responseForAll.IsSuccess)
        {
            _logger.LogError("An error occured. error: '{Message}'. When confirming the phone number: {phoneNumber} for registered user id: {Id}", responseForAll.Message, user.PhoneNumber, user.Id);

            if (responseForAll.ResponseStatus is not ResponseStatus.NotFound and ResponseStatus.BadRequest)
            {
                throw new Exception(responseForAll.Message);
            }
        }
    }
}
