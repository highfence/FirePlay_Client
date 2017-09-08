using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Util
{
    public static class Util
    {
        public static object ByteToStructure(byte[] data, Type type)
        {
            IntPtr buff = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, buff, data.Length);

            object obj = Marshal.PtrToStructure(buff, type);
            Marshal.FreeHGlobal(buff);

            if (Marshal.SizeOf(obj) != data.Length)
            {
                return null;
            }

            return obj;
        }

        public static byte[] StructureToByte(object obj)
        {
            int datasize = Marshal.SizeOf(obj);
            IntPtr buff = Marshal.AllocHGlobal(datasize);

            Marshal.StructureToPtr(obj, buff, false);
            byte[] data = new byte[datasize];

            Marshal.Copy(buff, data, 0, datasize);
            Marshal.FreeHGlobal(buff);

            return data;
        }

        public static string ByteToString(byte[] buff)
        {
            return Encoding.UTF8.GetString(buff);
        }

        public static byte[] StringToByte(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
