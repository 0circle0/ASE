using System;

namespace ASE {
    [Serializable]
    public struct LayerChunk : Chunk {
        public ushort flags;
        public ushort layer_type;
        public ushort layer_child_level;
        public ushort default_layer_width_in_pixels;
        public ushort default_layer_height_in_pixels;
        public ushort blend_mode;
        public byte opacity;
        public byte[] for_future; //Set to 0
        public string layer_name;

        public void GenerateChunk(ref byte[] chunkData) {
            flags = Read.WORD(ref chunkData);
            layer_type = Read.WORD(ref chunkData);
            layer_child_level = Read.WORD(ref chunkData);
            default_layer_width_in_pixels = Read.WORD(ref chunkData);
            default_layer_height_in_pixels = Read.WORD(ref chunkData);
            blend_mode = Read.WORD(ref chunkData);
            opacity = Read.BYTE(ref chunkData);
            for_future = Read.BYTEARRAY(ref chunkData, 3);
            layer_name = Read.STRING(ref chunkData);
        }
    }
}