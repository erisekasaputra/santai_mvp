namespace Account.Domain.Aggregates.AccountAggregates;

public class SecuritySetting
{
    public int SecuritySettingsId { get; private set; }
    public int AccountId { get; set; }
    public bool TwoFactorAuthEnabled { get; set; }
    public string LastLoginIp { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public int FailedLoginAttempts { get; set; }
}