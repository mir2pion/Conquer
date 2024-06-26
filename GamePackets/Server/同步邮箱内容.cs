namespace GamePackets.Server;

[PacketInfo(Source = PacketSource.Server, ID = 575, Length = 0, Description = "查询邮箱内容")]
public sealed class 同步邮箱内容 : GamePacket
{
    [FieldAttribute(Position = 4, Length = 2)]
    public ushort MailCount;

    [FieldAttribute(Position = 6, Length = 0)]
    public byte[] Description;
}
