using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASE {
    [Serializable]
    public struct Frame : Chunk { //0xF1FA 61946
        public uint bytes_in_frame;
        public ushort magic_number;
        public ushort old_chunks;
        public ushort frame_duration;
        [HideInInspector]
        public byte[] for_future; //size 2
        public uint number_of_chunks; //if 0 use old_chunks
        public uint chunks;
        public ushort color_depth;
        public List<OldPaletteChunk> oldPaletteChunks;
        public List<LayerChunk> layerChunks;
        public List<CelChunk> celChunks;
        public List<CelExtraChunk> celExtraChunks;
        public List<ColorProfileChunk> colorProfileChunks;
        public List<MaskChunk> maskChunks;
        public List<PathChunk> pathChunks;
        public List<TagChunk> tagChunks;
        public List<PaletteChunk> paletteChunks;
        public List<UserDataChunk> userDataChunks;
        public List<SliceChunk> sliceChunks;
        public ushort width_in_pixels;
        public ushort height_in_pixels;

        public Frame(uint bytesInFrame, ushort magicNumber, ushort colorDepth) {
            bytes_in_frame = bytesInFrame;
            magic_number = magicNumber;
            old_chunks = 0;
            frame_duration = 0;
            for_future = new byte[0];
            number_of_chunks = 0;
            chunks = 0;
            oldPaletteChunks = new List<OldPaletteChunk>();
            layerChunks = new List<LayerChunk>();
            celChunks = new List<CelChunk>();
            celExtraChunks = new List<CelExtraChunk>();
            colorProfileChunks = new List<ColorProfileChunk>();
            maskChunks = new List<MaskChunk>();
            pathChunks = new List<PathChunk>();
            tagChunks = new List<TagChunk>();
            paletteChunks = new List<PaletteChunk>();
            userDataChunks = new List<UserDataChunk>();
            sliceChunks = new List<SliceChunk>();
            color_depth = colorDepth;
            width_in_pixels = 0;
            height_in_pixels = 0;
        }
        public void GenerateChunk(ref byte[] chunkData) {
            old_chunks = Read.WORD(ref chunkData);
            frame_duration = Read.WORD(ref chunkData);
            for_future = Read.BYTEARRAY(ref chunkData, 2);
            chunks = Read.DWORD(ref chunkData);
        }
    }
}