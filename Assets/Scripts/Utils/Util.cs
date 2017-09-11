using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Util
{
    public static class ByteTranslator
    {
        public static object ByteToObject(byte[] buffer)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(buffer))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    stream.Position = 0;
                    return binaryFormatter.Deserialize(stream);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            return null;
        }

        public static object GetObjectFromBytes(byte[] buffer, Type objType)
        {
            object obj = null;
            if ((buffer != null) && (buffer.Length > 0))
            {
                IntPtr ptrObj = IntPtr.Zero;
                try
                {
                    int objSize = Marshal.SizeOf(objType);
                    if (objSize > 0)
                    {
                        if (buffer.Length < objSize)
                        {
                            throw new Exception(String.Format("Buffer smaller than needed for creation of object of type {0}", objType));
                        }

                        ptrObj = Marshal.AllocHGlobal(objSize);

                        if (ptrObj != IntPtr.Zero)
                        {
                            Marshal.Copy(buffer, 0, ptrObj, objSize);
                            obj = Marshal.PtrToStructure(ptrObj, objType);
                        }
                        else
                        {
                            throw new Exception(String.Format("Couldn't allocate memory to create object of type {0}", objType));
                        }
                    }
                }
                finally
                {
                    if (ptrObj != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(ptrObj);
                    }
                }
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
