﻿namespace Text2BinaryCameraPoses
{
    public sealed class LittleEndianBitConverter : EndianBitConverter
    {
        // Methods
        protected override void CopyBytesImpl(long value, int bytes, byte[] buffer, int index)
        {
            for (int i = 0; i < bytes; i++)
            {
                buffer[i + index] = (byte)(value & 0xffL);
                value = value >> 8;
            }
        }

        protected override long FromBytes(byte[] buffer, int startIndex, int bytesToConvert)
        {
            long num = 0L;
            for (int i = 0; i < bytesToConvert; i++)
            {
                num = (num << 8) | buffer[((startIndex + bytesToConvert) - 1) - i];
            }
            return num;
        }

        public sealed override bool IsLittleEndian()
        {
            return true;
        }

        // Properties
        public sealed override Endianness Endianness
        {
            get
            {
                return Endianness.LittleEndian;
            }
        }
    }
}
