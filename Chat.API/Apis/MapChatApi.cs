using Chat.API.Applications.Dtos.Request;
using Chat.API.Applications.Services.Interfaces;
using Chat.API.Domain.Enumerations;
using Core.Results; 
using Core.Services.Interfaces; 
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Apis;

public static class MapChatApi
{
    public static IEndpointRouteBuilder MapChatApiRouteBuilder(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/chat");

        app.MapGet("/conversations", GetLatestChat); 
        app.MapGet("/contacts", GetChatContacts);
        app.MapGet("/contacts/order/{orderId}", GetChatContactByOrderId);

        return app;
    }

    private static async Task<IResult> GetLatestChat(
        [AsParameters] LatestChatRequest request,
        [FromServices] IChatService chatService)
    {
        try
        {
            var conversations = await chatService.GetMessageByOrderIdAndTimestamp(request.OrderId.ToString(), request.Timestamp, request.Forward);
            if (conversations is null || conversations.Count == 0)
            {
                return TypedResults.NotFound(
                    Result.Failure("There is no recent chat", ResponseStatus.NotFound));
            }

            return TypedResults.Ok(
                Result.Success(conversations, ResponseStatus.Ok));
        }
        catch(Exception ex)
        { 
            return TypedResults.InternalServerError(Result.Failure(ex.Message, ResponseStatus.InternalServerError));
        }
    }
     
    private static async Task<IResult> GetChatContacts(
        [AsParameters] ChatContactRequest request,
        [FromServices] IChatService chatService,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var user = userInfoService.GetUserInfo();
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }
             
            var contacts = request.ChatContactUserType == ChatContactUserType.User ? await chatService.GetChatContactsByBuyerId(user.Sub.ToString()) : await chatService.GetChatContactsByMechanicId(user.Sub.ToString());

            if (contacts is null || contacts.Count == 0)
            {
                return TypedResults.NotFound(
                    Result.Failure("There is no recent chat contact", ResponseStatus.NotFound));
            }

            return TypedResults.Ok(
                Result.Success(contacts, ResponseStatus.Ok));
        }
        catch (Exception ex)
        {
            return TypedResults.InternalServerError(Result.Failure(ex.Message, ResponseStatus.InternalServerError));
        }
    }

    private static async Task<IResult> GetChatContactByOrderId(
        Guid orderId,
        [FromServices] IChatService chatService,
        [FromServices] IUserInfoService userInfoService)
    {
        try
        {
            var user = userInfoService.GetUserInfo();
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            var contact = await chatService.GetChatContactByOrderId(orderId.ToString());

            if (contact is null)
            {
                return TypedResults.NotFound(
                    Result.Failure("There is no recent chat contact", ResponseStatus.NotFound));
            }

            return TypedResults.Ok(
                Result.Success(contact, ResponseStatus.Ok));
        }
        catch (Exception ex)
        {
            return TypedResults.InternalServerError(Result.Failure(ex.Message, ResponseStatus.InternalServerError));
        }
    }
}
