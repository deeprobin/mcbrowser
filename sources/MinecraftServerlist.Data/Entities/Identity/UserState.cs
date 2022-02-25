namespace MinecraftServerlist.Data.Entities.Identity;

public enum UserState : byte
{
    PendingEmailVerification = 0,
    Enabled = 1,
    DisabledByUser = 100,
    DisabledDueToS = 101,
    DisabledDueLaw = 102
}