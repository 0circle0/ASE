using System;
using System.Collections.Generic;

namespace ASE {
    [Serializable]
    public struct PaletteChunk : Chunk { //0x2019
        public uint new_palette_size;
        public uint start;
        public uint end;
        [NonSerialized]
        public byte[] for_future; //size 8
        public List<PaletteEntry> palette_entries;

        public void GenerateChunk(ref byte[] chunkData) {
            new_palette_size = Read.DWORD(ref chunkData);
            start = Read.DWORD(ref chunkData);
            end = Read.DWORD(ref chunkData);
            for_future = Read.BYTEARRAY(ref chunkData, 8);

            palette_entries = new List<PaletteEntry>();
            for (int i = 0; i < end - start + 1; i++) {
                PaletteEntry paletteEntry = new PaletteEntry() {
                    entry_flags = Read.WORD(ref chunkData),
                    r = Read.BYTE(ref chunkData),
                    g = Read.BYTE(ref chunkData),
                    b = Read.BYTE(ref chunkData),
                    a = Read.BYTE(ref chunkData)
                };
                if (paletteEntry.entry_flags == 1) {
                    paletteEntry.name = Read.STRING(ref chunkData);
                }
                palette_entries.Add(paletteEntry);
            }
        }
    }
}