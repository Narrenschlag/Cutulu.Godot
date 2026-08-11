namespace Cutulu.Network;

[System.Flags]
public enum UserFlagEnum : byte
{
    None = 0,
    Client = 1 << 0,
    Host = 1 << 1,
    Both = Client | Host,
}