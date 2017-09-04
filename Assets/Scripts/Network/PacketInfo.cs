using System.Runtime.InteropServices;

namespace PacketInfo
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketHeader
    {
        public int packetId;
        public int bodySize;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Packet
    {
        public int packetId;
        public int bodySize;
        public string data;
    }


}