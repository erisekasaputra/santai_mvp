using Chat.API.Domain.Enumerations;

namespace Chat.API.Applications.Dtos.Request;

public class ChatContactRequest
{ 
    public ChatContactUserType ChatContactUserType { get; set; }

    public ChatContactRequest()
    {

    }
    public ChatContactRequest( 
        ChatContactUserType chatContactUserType)
    {
        ChatContactUserType = chatContactUserType;
    }
}
