using Chat.API.Applications.Dtos.Request;
using Chat.API.Applications.Mapper;
using Chat.API.Applications.Services;
using Chat.API.Applications.Services.Interfaces;
using Chat.API.Domain.Enumerations;
using Chat.API.Domain.Models;
using Core.Results;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Apis;

public static class MapChatApi
{
    public static IEndpointRouteBuilder MapChatApiRouteBuilder(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("api/v1/chat");
         
        app.MapGet("/conversations", GetLatestChatByTimestamp); 
        app.MapGet("/contacts", GetChatContacts);
        app.MapGet("/contacts/order/{orderId}", GetChatContactByOrderId);

        return app;
    }

    private static async Task<IResult> GetLatestChat(
        [AsParameters] LatestChatRequest request,
        [FromServices] IChatService chatService,
        [FromServices] IUserInfoService userInfoService,
        [FromServices] IHubContext<ChatHub, IChatClient> _chatHub)
    {
        try
        { 
            var user = userInfoService.GetUserInfo();
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }
             
            await foreach (var conversation in chatService.GetMessageByOrderId(request.OrderId.ToString(), request.Forward))
            { 
                await _chatHub.Clients.User(user.Sub.ToString()).ReceiveMessage(conversation.ToResponse());
            }

            return TypedResults.Ok(Result.Success(null, ResponseStatus.Ok));
        }
        catch (Exception ex)
        {
            // Return an internal server error with the exception message
            return TypedResults.InternalServerError(Result.Failure(ex.Message, ResponseStatus.InternalServerError));
        }
    }



    private static async Task<IResult> GetLatestChatByTimestamp(
        [AsParameters] LatestChatByTimestampRequest request,
        [FromServices] IChatService chatService,
        [FromServices] IUserInfoService userInfoService,
        [FromServices] IHubContext<ChatHub, IChatClient> _chatHub)
    {
        try
        {
            var user = userInfoService.GetUserInfo();
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            List<Conversation> conversations = [];
            await foreach (var conversation in chatService.GetMessageByLastTimestamp(request.OrderId.ToString(), request.Timestamp, request.Forward))
            {
                conversations.Add(conversation); 
            }

            return TypedResults.Ok(Result.Success(new 
            {
                Conversations = conversations
            }, ResponseStatus.Ok));
        }
        catch (Exception ex)
        {
            // Return an internal server error with the exception message
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
