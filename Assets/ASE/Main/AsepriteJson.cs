using System;
using System.Collections.Generic;
namespace ASE {
    [Serializable]
    public class AsepriteObj {
        public Header header;
        public List<Frame> frames;

        public AsepriteObj() {
            frames = new List<Frame>();
        }
    }
}