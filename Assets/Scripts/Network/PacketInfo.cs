using System.Runtime.InteropServices;

namespace PacketInfo
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PacketHeader
    {
        public int id;
        public int bodySize;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class Packet
    {
        public int packetId;
        public int bodySize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public string data;
    }


}