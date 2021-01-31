using System;
using UnityEngine;

namespace ASE {
    [Serializable]
    public struct ColorProfileChunk : Chunk {//0x2007
        public ushort type;
        public ushort flags;
        public float fixed_gamma;
        [HideInInspector]
        public byte[] reserved; // size 8
                                //if type 2
        public uint icc_profile_length;
        [HideInInspector]
        public byte[] icc_profile_data;

        public void GenerateChunk(ref byte[] chunkData) {
            type = Read.WORD(ref chunkData);
            flags = Read.WORD(ref chunkData);
            fixed_gamma = Read.FIXED(ref chunkData);
            reserved = Read.BYTEARRAY(ref chunkData, 8);
            if (type == 2) {
                icc_profile_length = Read.DWORD(ref chunkData);
                icc_profile_data = Read.BYTEARRAY(ref chunkData, (int)icc_profile_length - (ASE.Read.DWORD_LENGTH));
            }
        }
    }
}