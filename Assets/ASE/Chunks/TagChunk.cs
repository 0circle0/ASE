using System;
using System.Collections.Generic;

namespace ASE {
    [Serializable]
    public struct TagChunk : Chunk {//0x2018
        public ushort number_of_tags;
        [NonSerialized]
        public byte[] for_future; //size 8
        public List<Tag> tags;

        public void GenerateChunk(ref byte[] chunkData) {
            number_of_tags = Read.WORD(ref chunkData);
            for_future = Read.BYTEARRAY(ref chunkData, 8);
            tags = new List<Tag>();
            for (int i = 0; i < number_of_tags; i++) {
                Tag tag = new Tag() {
                    from_frame = Read.WORD(ref chunkData),
                    to_frame = Read.WORD(ref chunkData),
                    loop_animation_direction = Read.BYTE(ref chunkData),
                    for_future = Read.BYTEARRAY(ref chunkData, 8),
                    RBG_tag_color = Read.BYTEARRAY(ref chunkData, 3),
                    extra_byte = Read.BYTE(ref chunkData),
                    tag_name = Read.STRING(ref chunkData)
                };
                tags.Add(tag);
            }
        }
    }
}