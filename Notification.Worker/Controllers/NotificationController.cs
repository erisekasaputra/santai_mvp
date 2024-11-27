using Core.CustomMessages;
using Core.Results;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification.Worker.Services.Interfaces;

namespace Notification.Worker.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IUserInfoService _userInfoService;
    public NotificationController(
        INotificationService notificationService, 
        IUserInfoService userInfoService)
    {
        _notificationService = notificationService;
        _userInfoService = userInfoService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IResult> GetNotifications(long lastTimestamp)
    {
        try
        {
            var userClaim = _userInfoService.GetUserInfo();
            if (userClaim is null)
            {
                return TypedResults.Forbid();
            }

            var notifications = await _notificationService.GetNotifications(userClaim.Sub.ToString(), 10, lastTimestamp);

            if (notifications is null || notifications.Count == 0)
            {
                return TypedResults.NotFound(); 
            }

            return TypedResults.Ok(Result.Success(notifications));
        }
        catch (Exception) 
        {
            return TypedResults.InternalServerError(Messages.InternalServerError);
        }
    }
}
