using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ASE {
    public static class Read {

        public const int DWORD_LENGTH = 4;
        public const int WORD_LENGTH = 2;
        public const int BYTE_LENGTH = 1;
        public const int SHORT_LENGTH = 2;
        public const int FIXED_LENGTH = 4;
        public const int LONG_LENGTH = 4;
        public const int PIXEL_RGBA_LENGTH = 4;
        public const int PIXEL_GRAYSCALE_LENGTH = 2;
        public const int PIXEL_INDEXED_LENGTH = 1;

        public static byte[] DecompressImageBytes(byte[] data, int count) {
            using MemoryStream stream = new MemoryStream(data);
            var reader = new BinaryReader(stream);
            reader.BaseStream.Position += 2;
            var deflate = new DeflateStream(reader.BaseStream, CompressionMode.Decompress);
            byte[] ret = new byte[count];
            deflate.Read(ret, 0, count);
            return ret;
        }

        public static byte[] BYTEARRAY(ref byte[] data, int size) {
            byte[] _array = new byte[size];
            Buffer.BlockCopy(data, 0, _array, 0, _array.Length);

            AdjustData(ref data, size);
            return _array;
        }

        public static string STRING(ref byte[] data) {
            ushort _length = WORD(ref data);
            byte[] _string = BYTEARRAY(ref data, _length);
            //Might have a '/0'
            return Encoding.UTF8.GetString(_string);
        }

        public static byte BYTE(ref byte[] data) {
            byte[] _byte = new byte[1];
            Buffer.BlockCopy(data, 0, _byte, 0, _byte.Length);

            AdjustData(ref data, 1);
            return _byte[0];
        }

        public static short SHORT(ref byte[] data) {
            byte[] _short = new byte[2];
            Buffer.BlockCopy(data, 0, _short, 0, _short.Length);

            AdjustData(ref data, 2);
            return BitConverter.ToInt16(_short, 0);
        }

        public static long LONG(ref byte[] data) {
            byte[] _long = new byte[8];
            Buffer.BlockCopy(data, 0, _long, 0, _long.Length);

            AdjustData(ref data, 8);
            return BitConverter.ToInt64(_long, 0);
        }

        public static float FIXED(ref byte[] data) {
            byte[] _fixed = new byte[4];
            Buffer.BlockCopy(data, 0, _fixed, 0, _fixed.Length);

            AdjustData(ref data, 4);
            return BitConverter.ToSingle(_fixed, 0);
        }

        public static ushort WORD(ref byte[] data) {
            byte[] _ushort = new byte[2];
            Buffer.BlockCopy(data, 0, _ushort, 0, _ushort.Length);

            AdjustData(ref data, 2);
            return BitConverter.ToUInt16(_ushort, 0);
        }

        public static uint DWORD(ref byte[] data) {
            byte[] _dword = new byte[4];
            Buffer.BlockCopy(data, 0, _dword, 0, _dword.Length);

            AdjustData(ref data, 4);
            return BitConverter.ToUInt32(_dword, 0);
        }

        public static void AdjustData(ref byte[] data, int size) {
            byte[] newData = new byte[data.Length - size];
            Buffer.BlockCopy(data, size, newData, 0, newData.Length);
            data = newData;
        }
    }
}