namespace Notification.Domain.Entities;

public class NotificationSetting
{
    public Guid NotificationSettingsId { get; private init; }
    public Guid AccountId { get; private set; }
    public bool EmailNotifications { get; private set; }
    public bool SmsNotifications { get; private set; }
    public bool PushNotifications { get; private set; }
    public bool WhatsappNotifications { get; private set; } 

    public NotificationSetting(Guid accountId, bool emailNotifications, bool smsNotification, bool pushNotification, bool whatsappNotification)
    {
        NotificationSettingsId = Guid.NewGuid();
        AccountId = accountId;
        EmailNotifications = emailNotifications;
        SmsNotifications = smsNotification;
        PushNotifications = pushNotification;
        WhatsappNotifications = whatsappNotification;
    }

    
}
