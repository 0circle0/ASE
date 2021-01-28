using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASE {
    [Serializable]
    public struct OldPaletteChunkPacket {
        public byte number_of_palette_entries;
        public byte number_of_colors;
        public List<Color32> colors;
    }
}