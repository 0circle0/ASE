using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASE {
    [Serializable]
    public struct OldPaletteChunk : Chunk { //0x0004 & 0x0011 //0x2019 ignore 0x0004 & 0x0011 if 0x2019 exists
        public ushort number_of_packets;
        public List<OldPaletteChunkPacket> old_palette_chunk_packets;

        public void GenerateChunk(ref byte[] chunkData) {
            number_of_packets = Read.WORD(ref chunkData);
            old_palette_chunk_packets = new List<OldPaletteChunkPacket>();
            for (int i = 0; i < number_of_packets; i++) {
                OldPaletteChunkPacket oldPaletteChunkPacket = new OldPaletteChunkPacket() {
                    number_of_palette_entries = Read.BYTE(ref chunkData),
                    number_of_colors = Read.BYTE(ref chunkData),
                    colors = new List<Color32>()
                };
                Color32 color = new Color32(Read.BYTE(ref chunkData), Read.BYTE(ref chunkData), Read.BYTE(ref chunkData), 1);
                oldPaletteChunkPacket.colors.Add(color);
            }
        }
    }
}