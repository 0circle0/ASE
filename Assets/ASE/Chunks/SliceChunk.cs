using System;
using System.Collections.Generic;

namespace ASE {
    [Serializable]
    public struct SliceChunk : Chunk {//0x2022
        public uint number_of_slice_keys;
        public uint flags; //1= 9-patch slice, 2 = has pivot info
        [NonSerialized]
        public uint reserved;
        public string name;
        public List<SliceKey> slice_keys;

        public void GenerateChunk(ref byte[] chunkData) {
            number_of_slice_keys = Read.DWORD(ref chunkData);
            flags = Read.DWORD(ref chunkData);
            reserved = Read.DWORD(ref chunkData);
            name = Read.STRING(ref chunkData);
            slice_keys = new List<SliceKey>();
            for (int i = 0; i < number_of_slice_keys; i++) {
                SliceKey sliceKey = new SliceKey() {
                    frame_number = Read.DWORD(ref chunkData),
                    slice_x_origin = Read.LONG(ref chunkData),
                    slice_y_origin = Read.LONG(ref chunkData),
                    slice_width = Read.DWORD(ref chunkData),
                    slice_height = Read.DWORD(ref chunkData)
                };
                if (flags == 1) {
                    sliceKey.center_x_position = Read.LONG(ref chunkData);
                    sliceKey.center_y_position = Read.LONG(ref chunkData);
                    sliceKey.center_width = Read.DWORD(ref chunkData);
                    sliceKey.center_height = Read.DWORD(ref chunkData);
                } else if (flags == 2) {
                    sliceKey.pivot_x_position = Read.LONG(ref chunkData);
                    sliceKey.pivot_y_position = Read.LONG(ref chunkData);
                }

                slice_keys.Add(sliceKey);
            }
        }
    }
}