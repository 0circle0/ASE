using System;
using UnityEngine;

namespace ASE {
    [Serializable]
    public struct MaskChunk : Chunk { //x2016 depricated
        public short x_position;
        public short y_position;
        public ushort width;
        public ushort height;
        [HideInInspector]
        public byte[] for_future; //size 8
        public string mask_name;
        [HideInInspector]
        public byte[] bit_map_data;

        public void GenerateChunk(ref byte[] chunkData) {
            x_position = Read.SHORT(ref chunkData);
            y_position = Read.SHORT(ref chunkData);
            width = Read.WORD(ref chunkData);
            height = Read.WORD(ref chunkData);
            for_future = Read.BYTEARRAY(ref chunkData, 8);
            mask_name = Read.STRING(ref chunkData);
            bit_map_data = Read.BYTEARRAY(ref chunkData, height * ((width + 7) / 8));
        }
    }
}