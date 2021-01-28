using System;

namespace ASE {
    [Serializable]
    public struct SliceKey {
        public uint frame_number;
        public long slice_x_origin;
        public long slice_y_origin;
        public uint slice_width;
        public uint slice_height;

        //if SliceChunk flags = 1
        public long center_x_position;
        public long center_y_position;
        public uint center_width;
        public uint center_height;

        //if SliceChunk flags = 2
        public long pivot_x_position;
        public long pivot_y_position;
    }
}