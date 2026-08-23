#if GODOT4_0_OR_GREATER
namespace Cutulu.Core;

using System;

public partial struct ActionTimeStamp
{
    public ushort MilliSecond { get; set; }
    public byte Second { get; set; }
    public ushort Minute { get; set; }

    public ActionTimeStamp() { }

    public static ActionTimeStamp GetTimeStamp() //int offset = 0)
    {
        var time = Godot.Time.GetTicksMsec(); //+ (uint)int.Max(0, offset);

        var mil = time % 1000;
        time -= mil;

        var sec = time % 60;
        time -= sec;

        return new()
        {
            MilliSecond = (ushort)mil,
            Second = (byte)sec,
            Minute = (ushort)time,
        };
    }

    class Encoder() : BinaryEncoder(typeof(ActionTimeStamp))
    {
        public override void Encode(Core.Encoder.Marshal writer, Type type, object value)
        {
            if (value is not ActionTimeStamp t) return;

            writer.Writer.Write(t.MilliSecond);
            writer.Writer.Write(t.Second);
            writer.Writer.Write(t.Minute);
        }

        public override object Decode(Decoder.Marshal marshal, Type type)
        {
            return new ActionTimeStamp()
            {
                MilliSecond = marshal.Reader.ReadUInt16(),
                Second = marshal.Reader.ReadByte(),
                Minute = marshal.Reader.ReadUInt16(),
            };
        }
    }
}
#endif