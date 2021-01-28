using System;

namespace ASE {
    [Serializable]
    public struct Tag {
        public ushort from_frame;
        public ushort to_frame;
        public byte loop_animation_direction; //0 forward, 1 reverse, 2 pingpong
        public byte[] for_future; //size 8
        public byte[] RBG_tag_color; //3 no alpha
        public byte extra_byte;
        public string tag_name;
    }
}