using System;
using UnityEngine;

namespace ASE {
    [Serializable]
    public struct PaletteEntry {
        public ushort entry_flags; //1 = has name
        public byte r;
        public byte g;
        public byte b;
        public byte a;
        public Color32 color;
        public string name; //entry_flags = 1 otherwise ignore
    }
}