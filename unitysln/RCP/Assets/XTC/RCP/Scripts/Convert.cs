namespace XTC.RCP
{

    public class Convert
    {
        public static int BytesToInt32(byte[] _data)
        {
            int value = 0;
            value |= ((int)_data[0]);
            value |= ((int)_data[1]) << 8;
            value |= ((int)_data[2]) << 16;
            value |= ((int)_data[3]) << 24;
            return value;
        }

        public static int BytesToInt32(byte[] _data, int _offset)
        {
            int value = 0;
            value |= ((int)_data[_offset + 0]);
            value |= ((int)_data[_offset + 1]) << 8;
            value |= ((int)_data[_offset + 2]) << 16;
            value |= ((int)_data[_offset + 3]) << 24;
            return value;
        }

        public static long BytesToInt64(byte[] _data)
        {
            long value = 0;
            value |= ((long)_data[0]);
            value |= ((long)_data[1]) << 8;
            value |= ((long)_data[2]) << 16;
            value |= ((long)_data[3]) << 24;
            value |= ((long)_data[4]) << 32;
            value |= ((long)_data[5]) << 40;
            value |= ((long)_data[6]) << 48;
            value |= ((long)_data[7]) << 56;
            return value;
        }

        public static long BytesToInt64(byte[] _data, int _offset)
        {
            long value = 0;
            value |= ((long)_data[_offset + 0]);
            value |= ((long)_data[_offset + 1]) << 8;
            value |= ((long)_data[_offset + 2]) << 16;
            value |= ((long)_data[_offset + 3]) << 24;
            value |= ((long)_data[_offset + 4]) << 32;
            value |= ((long)_data[_offset + 5]) << 40;
            value |= ((long)_data[_offset + 6]) << 48;
            value |= ((long)_data[_offset + 7]) << 56;
            return value;
        }


        public static byte[] Int32ToBytes(int _value)
        {
            byte[] data = new byte[4];
            data[0] = (byte)_value;
            data[1] = (byte)(_value >> 8);
            data[2] = (byte)(_value >> 16);
            data[3] = (byte)(_value >> 24);
            return data;
        }

        public static byte[] Int64ToBytes(long _value)
        {
            byte[] data = new byte[8];
            data[0] = (byte)_value;
            data[1] = (byte)(_value >> 8);
            data[2] = (byte)(_value >> 16);
            data[3] = (byte)(_value >> 24);
            data[4] = (byte)(_value >> 32);
            data[5] = (byte)(_value >> 40);
            data[6] = (byte)(_value >> 48);
            data[7] = (byte)(_value >> 56);
            return data;
        }
    }
}
