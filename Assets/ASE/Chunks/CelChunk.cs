using System;
using UnityEngine;

namespace ASE {
    [Serializable]
    public struct CelChunk : Chunk { //0x2005
        public ushort layer_index;
        public short x_position;
        public short y_position;
        public byte opacity;
        public ushort cel_type;
        public byte[] for_future; //size 7

        //for cel type = 0
        public ushort width_in_pixels;
        public ushort height_in_pixels;
        public Color32[] pixels;
        public byte[] raw_data;
        public Sprite sprite;

        //for cel type = 1
        public ushort frame_position_to_link_with;

        //for cel type = 2
        public byte[] raw_cel_compressed;

        //ASE helper
        public ushort color_depth;
        public CelChunk(ushort colorDepth) {
            color_depth = colorDepth;
            layer_index = 0;
            x_position = 0;
            y_position = 0;
            opacity = 0;
            cel_type = 0;
            for_future = new byte[7];
            width_in_pixels = 0;
            height_in_pixels = 0;
            pixels = new Color32[0];
            raw_data = new byte[0];
            sprite = null;
            frame_position_to_link_with = 0;
            raw_cel_compressed = new byte[0];
        }

        public void GenerateChunk(ref byte[] chunkData) {

            BuildChunkVariables(ref chunkData);

            if (cel_type == 0) {
                //TODO Figure out what/when this is part of the save file
                BuildRawCell(ref chunkData);

            } else if (cel_type == 1) {
                //TODO Figure out what/when this is part of the save file No Image in cel??Doesn't make sense
                frame_position_to_link_with = Read.WORD(ref chunkData);
                return;
            } else if (cel_type == 2) {
                //All my saves have been compressed.
                ProcessCompressCell(ref chunkData);
            }

            //puts the cel on the frame. I want each cell on a blank new layer. Need to convert
            //CelToFrame(asepriteObj.header, frame, celChunk);
            //Once CelToFrame has been applied we need to adjust width_in_pixels and height_in_pixels to that of the frame.

            //This could happen with blank layers?
            if (pixels.Length == 0)
                return;

            //Create just this cel. Will not be dimentions of aseprite. Compressed to smalled size possible CelToFrame solves this
            BuildSprite();
        }

        private void BuildChunkVariables(ref byte[] chunkData) {
            layer_index = Read.WORD(ref chunkData);
            x_position = Read.SHORT(ref chunkData);
            y_position = Read.SHORT(ref chunkData);
            opacity = Read.BYTE(ref chunkData);
            cel_type = Read.WORD(ref chunkData);
            for_future = Read.BYTEARRAY(ref chunkData, 7);
        }

        private void BuildRawCell(ref byte[] chunkData) {
            BuildWidthHeight(ref chunkData);

            raw_data = Read.BYTEARRAY(ref chunkData, width_in_pixels * height_in_pixels);

            //BuildColor32AlphaBlended();
            BuildColor32Direct();
        }

        private void ProcessCompressCell(ref byte[] chunkData) {
            BuildWidthHeight(ref chunkData);

            raw_cel_compressed = chunkData;

            int count = width_in_pixels * height_in_pixels * (color_depth / 8);
            raw_data = Read.DecompressImageBytes(raw_cel_compressed, count);

            BuildColor32AlphaBlended();
            //BuildColor32Direct();
        }

        private void BuildWidthHeight(ref byte[] chunkData) {
            width_in_pixels = Read.WORD(ref chunkData);
            height_in_pixels = Read.WORD(ref chunkData);
        }

        private void BuildColor32Direct() {
            pixels = new Color32[raw_data.Length / 4];
            for (int i = 0, b = 0; i < raw_data.Length; i += 4, b++) {
                pixels[b] = new Color32(raw_data[i], raw_data[i + 1], raw_data[i + 2], raw_data[i + 3]);
            }
        }

        private void BuildColor32AlphaBlended() {
            pixels = new Color32[raw_data.Length / 4];
            var len = width_in_pixels * height_in_pixels;
            for (int p = 0, b = 0; p < len; p++, b += 4) {
                //Not sure I care about opacity.
                var red = (byte)(raw_data[b + 0] * raw_data[b + 3] / 255);
                var green = (byte)(raw_data[b + 1] * raw_data[b + 3] / 255);
                var blue = (byte)(raw_data[b + 2] * raw_data[b + 3] / 255);
                var alpha = raw_data[b + 3];
                pixels[p] = new Color32(red, green, blue, alpha);
            }
        }

        private void BuildSprite() {
            Texture2D tex = new Texture2D(width_in_pixels, height_in_pixels) {
                filterMode = FilterMode.Point
            };

            pixels = FlipColor32(pixels, width_in_pixels);
            tex.SetPixels32(pixels);
            tex.Apply();

            Rect r = new Rect(0f, 0f, width_in_pixels, height_in_pixels);
            sprite = Sprite.Create(tex, r, new Vector2(0.5f, 0.5f), 100.0f, 0, SpriteMeshType.FullRect);
        }

        //Aseprite Images origin pixel is bottom left but we need it to be top left.
        //This could be built directly into the part where colors are first created Skipping needing to do this
        public Color32[] FlipColor32(Color32[] bytes, int width) {
            Color32[] flippedBytes = new Color32[bytes.Length];

            for (int i = 0, j = bytes.Length - width; i < bytes.Length; i += width, j -= width) {
                for (int k = 0; k < width; ++k) {
                    flippedBytes[i + k] = bytes[j + k];
                }
            }
            return flippedBytes;
        }
    }
}