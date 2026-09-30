#if GODOT4_0_OR_GREATER
namespace Cutulu.Core;

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

    public class ImageEncoder() : BinaryEncoder(typeof(Image))
    {
        public static BufferEnum BufferType { get; set; } = BufferEnum.Storage;
        public static FileEnum FileType { get; set; } = FileEnum.Png;

        public override object Decode(Decoder.Marshal reader, Type type)
        {
            byte[] buffer = BufferType switch
            {
                BufferEnum.Storage => reader.Reader.ReadBytes((int)reader.Length),
                BufferEnum.Transfer => reader.Reader.ReadBytes((int)reader.Decode<uint>()),
                _ => throw new Exception($"Unknown buffer type: {BufferType}")
            };

            var image = Image.CreateEmpty(0, 0, false, default);
            Error error = FileType switch
            {
                FileEnum.Png => image.LoadPngFromBuffer(buffer),
                FileEnum.Jpg => image.LoadJpgFromBuffer(buffer),
                FileEnum.WebP => image.LoadWebpFromBuffer(buffer),
                FileEnum.Exr => image.LoadExrFromBuffer(buffer),
                FileEnum.Dds => image.LoadDdsFromBuffer(buffer),
                FileEnum.Ktx_READONLY => image.LoadKtxFromBuffer(buffer),
                FileEnum.Bmp_READONLY => image.LoadBmpFromBuffer(buffer),
                FileEnum.Tga_READONLY => image.LoadTgaFromBuffer(buffer),
                _ => throw new Exception($"Unknown image type: {FileType}")
            };

            if (error != Error.Ok) throw new Exception($"Could not load image: {error}");

            return image;
        }

        public override void Encode(Encoder.Marshal writer, Type type, object value)
        {
            var image = (Image)value;
            long position = default;

            if (BufferType == BufferEnum.Transfer)
            {
                position = writer.Position;
                writer.Encode(uint.MaxValue);
            }

            writer.Writer.Write(FileType switch
            {
                FileEnum.Png => image.SavePngToBuffer(),
                FileEnum.Jpg => image.SaveJpgToBuffer(),
                FileEnum.WebP => image.SaveWebpToBuffer(),
                FileEnum.Exr => image.SaveExrToBuffer(),
                FileEnum.Dds => image.SaveDdsToBuffer(),
                _ => throw new Exception($"Can't encode image of type: {FileType}")
            });

            if (BufferType == BufferEnum.Transfer)
            {
                writer.Position = position;
                writer.Encode(writer.Length - position - 4/*amount of bytes in uint*/);
                writer.Position = writer.Length;
            }
        }

        public enum FileEnum : byte
        {
            Png,
            Jpg,
            WebP,
            Exr,
            Dds,
            Ktx_READONLY,
            Bmp_READONLY,
            Tga_READONLY,
        }

        public enum BufferEnum : byte
        {
            /// <summary> The image will be en/decoded to/from it's raw data. </summary>
            Storage,

            /// <summary> The image will be en/decoded to/from a header + it's raw data. </summary>
            Transfer,
        }
    }
}
#endif