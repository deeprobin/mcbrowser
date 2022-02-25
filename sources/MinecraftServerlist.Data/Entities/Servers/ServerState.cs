namespace MinecraftServerlist.Data.Entities.Servers;

public enum ServerState : byte
{
    Default = 0,
    PendingActivation = 1,
    Enabled = 2,
    DisabledByUser = 100,
    DisabledDueToS = 101,
    DisabledDueLaw = 102
}