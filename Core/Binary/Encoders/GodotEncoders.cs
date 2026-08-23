#if GODOT4_0_OR_GREATER
namespace Cutulu.Core;

using System.IO;
using System;
using Godot;

/// <summary>
/// Use static method Register() to see more.
/// </summary>
public static class GodotEncoders
{
    #region Vector3         ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    class Vector3IEncoder() : BinaryEncoder(typeof(Vector3I))
    {
        public override void Encode(Encoder.Marshal writer, Type type, object value)
        {
            Vector3I _ = (Vector3I)value;
            for (int i = 0; i < 3; i++)
            {
                writer.Writer.Write(_[i]);
            }
        }

        public override object Decode(Decoder.Marshal marshal, Type type)
        {
            return new Vector3I(marshal.Reader.ReadInt32(), marshal.Reader.ReadInt32(), marshal.Reader.ReadInt32());
        }
    }

    class Vector3Formatter() : BinaryEncoder(typeof(Vector3))
    {
        public override void Encode(Encoder.Marshal writer, Type type, object value)
        {
            var _ = (Vector3)value;
            for (int i = 0; i < 3; i++)
            {
                writer.Writer.Write(_[i]);
            }
        }

        public override object Decode(Decoder.Marshal marshal, Type type)
        {
            return new Vector3(marshal.Reader.ReadSingle(), marshal.Reader.ReadSingle(), marshal.Reader.ReadSingle());
        }
    }
    #endregion

    #region Vector2         ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    class Vector2IFormatter() : BinaryEncoder(typeof(Vector2I))
    {
        public override void Encode(Encoder.Marshal writer, Type type, object value)
        {
            Vector2I _ = (Vector2I)value;
            for (int i = 0; i < 2; i++)
            {
                writer.Writer.Write(_[i]);
            }
        }

        public override object Decode(Decoder.Marshal marshal, Type type)
        {
            return new Vector2I(marshal.Reader.ReadInt32(), marshal.Reader.ReadInt32());
        }
    }

    class Vector2Formatter() : BinaryEncoder(typeof(Vector2))
    {
        public override void Encode(Encoder.Marshal writer, Type type, object value)
        {
            Vector2 _ = (Vector2)value;
            for (int i = 0; i < 2; i++)
            {
                writer.Writer.Write(_[i]);
            }
        }

        public override object Decode(Decoder.Marshal marshal, Type type)
        {
            return new Vector2(marshal.Reader.ReadSingle(), marshal.Reader.ReadSingle());
        }
    }
    #endregion

    class ColorFormatter() : BinaryEncoder(typeof(Color))
    {
        public override void Encode(Encoder.Marshal writer, Type type, object value)
        {
            Color _ = (Color)value;
            for (int i = 0; i < 4; i++)
            {
                writer.Writer.Write(_[i]);
            }
        }

        public override object Decode(Decoder.Marshal marshal, Type type)
        {
            return new Color(marshal.Reader.ReadSingle(), marshal.Reader.ReadSingle(), marshal.Reader.ReadSingle(), marshal.Reader.ReadSingle());
        }
    }
}
#endif