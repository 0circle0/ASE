using System;

namespace ASE {
    [Serializable]
    public struct UserDataChunk : Chunk {//0x2020
        public uint flags; //1 has text, 2 has color
                           //if 1
        public string text;
        //if 2
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public void GenerateChunk(ref byte[] chunkData) {
            flags = Read.DWORD(ref chunkData);

            if (flags == 1) {
                text = Read.STRING(ref chunkData);
            } else if (flags == 2) {
                r = Read.BYTE(ref chunkData);
                g = Read.BYTE(ref chunkData);
                b = Read.BYTE(ref chunkData);
                a = Read.BYTE(ref chunkData);
            }
        }
    }
}