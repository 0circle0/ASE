using System;

namespace ASE {
    [Serializable]
    public struct ICCProfile {
        public uint length;
        public byte[] data;
    }
}