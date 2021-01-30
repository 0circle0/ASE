using System;

namespace ASE {
    [Serializable]
    public struct CelExtraChunk : Chunk {//0x2006
        public uint flags;
        public float precise_x_position;
        public float precise_y_position;
        public float width_of_cel_in_sprite;
        public float height_of_cel_in_sprite;
        [NonSerialized]
        public byte[] for_future; //size 16

        public void GenerateChunk(ref byte[] chunkData) {
            flags = Read.DWORD(ref chunkData);
            precise_x_position = Read.FIXED(ref chunkData);
            precise_y_position = Read.FIXED(ref chunkData);
            width_of_cel_in_sprite = Read.FIXED(ref chunkData);
            height_of_cel_in_sprite = Read.FIXED(ref chunkData);
            for_future = Read.BYTEARRAY(ref chunkData, 16);
        }
    }
}