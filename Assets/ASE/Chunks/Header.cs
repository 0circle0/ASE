using System;
using UnityEngine;

namespace ASE {
    [Serializable]
    public struct Header : Chunk {
        public uint filesize;
        public ushort magic_number; //Magic number (0xA5E0)
        public ushort frames;
        public ushort width_in_pixels;
        public ushort height_in_pixels;
        public ushort color_depth; //Color depth(bits per pixel)
        public uint flags; //Flags: 1 = Layer opacity has valid value
        public ushort speed; //DEPRECATED: You should use the frame duration field from each frame header
        public uint _empty1; //set to 0
        public uint _empty2; //set to 0
        public byte palette_index; //Palette entry (index) which represent transparent color in all non-background layers(only for Indexed sprites).
        public byte[] ignore_these_bytes;
        public ushort number_of_colors; //Number of colors (0 means 256 for old sprites)
        public byte pixel_width; //Pixel width (pixel ratio is "pixel width/pixel height"). If this or pixel height field is zero, pixel ratio is 1:1
        public byte pixel_height;
        public short x_position_on_grid;
        public short y_position_on_grid;
        public ushort grid_width; //0 if no grid
        public ushort grid_height;
        public byte[] for_future; //set to 0
        public Color32[] pixels;
        public string headerHex;

        public void GenerateChunk(ref byte[] chunkData) {
            filesize = Read.DWORD(ref chunkData);
            magic_number = Read.WORD(ref chunkData);
            frames = Read.WORD(ref chunkData);
            width_in_pixels = Read.WORD(ref chunkData);
            height_in_pixels = Read.WORD(ref chunkData);
            color_depth = Read.WORD(ref chunkData);
            flags = Read.DWORD(ref chunkData);
            speed = Read.WORD(ref chunkData);
            _empty1 = Read.DWORD(ref chunkData);
            _empty2 = Read.DWORD(ref chunkData);
            palette_index = Read.BYTE(ref chunkData);
            ignore_these_bytes = Read.BYTEARRAY(ref chunkData, 3);
            number_of_colors = Read.WORD(ref chunkData);
            pixel_width = Read.BYTE(ref chunkData);
            pixel_height = Read.BYTE(ref chunkData);
            x_position_on_grid = Read.SHORT(ref chunkData);
            y_position_on_grid = Read.SHORT(ref chunkData);
            grid_width = Read.WORD(ref chunkData);
            grid_height = Read.WORD(ref chunkData);
            for_future = Read.BYTEARRAY(ref chunkData, 84);
            headerHex = magic_number.ToString("x");
            pixels = new Color32[width_in_pixels * height_in_pixels];
        }
    }
}