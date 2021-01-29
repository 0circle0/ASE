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
        public string layer_name;
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
            layer_name = "null";
        }
        //TODO GenerateChunk could be combined with the constructor with the removal of the interface
        public void GenerateChunk(ref byte[] chunkData) {

            BuildChunkVariables(ref chunkData);

            if (cel_type == 0) {
                //TODO Figure out what/when this is part of the save file
                ProcessCell(ref chunkData, false);

            } else if (cel_type == 1) {
                //TODO Figure out what/when this is part of the save file No Image in cel??Doesn't make sense
                frame_position_to_link_with = Read.WORD(ref chunkData);
                return;
            } else if (cel_type == 2) {
                //All my saves have been compressed.
                ProcessCell(ref chunkData, true);
            }

        }

        private void BuildChunkVariables(ref byte[] chunkData) {
            layer_index = Read.WORD(ref chunkData);
            x_position = Read.SHORT(ref chunkData);
            y_position = Read.SHORT(ref chunkData);
            opacity = Read.BYTE(ref chunkData);
            cel_type = Read.WORD(ref chunkData);
            for_future = Read.BYTEARRAY(ref chunkData, 7);
        }

        private void ProcessCell(ref byte[] chunkData, bool compressed) {
            BuildWidthHeight(ref chunkData);

            if (compressed) {
                raw_cel_compressed = chunkData;

                int count = width_in_pixels * height_in_pixels * (color_depth / 8);
                raw_data = Read.DecompressImageBytes(raw_cel_compressed, count);
            } else {
                raw_data = Read.BYTEARRAY(ref chunkData, width_in_pixels * height_in_pixels);
            }

            BuildColor32Direct();
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
            FlipColor32();
        }

        public void BuildSprite(int width, int height) {
            Texture2D tex = new Texture2D(width, height) {
                filterMode = FilterMode.Point
            };
            var clear = new Color32[width*height];
            for(int x = 0; x < width; x++)
                for (int y = 0; y < height; y++) {
                    clear[x+y] = Color.clear;
                }

            tex.SetPixels32(clear);
            var flip = height - height_in_pixels;
            tex.SetPixels32(x_position, flip - y_position, width_in_pixels, height_in_pixels, pixels);
            //tex.SetPixels32(newColors);
            tex.Apply();

            Rect r = new Rect(0f, 0f, width, height);
            sprite = Sprite.Create(tex, r, new Vector2(0.5f, 0.5f), 100.0f, 0, SpriteMeshType.FullRect);
        }


        //Aseprite Images origin pixel is bottom left but we need it to be top left.
        public void FlipColor32() {
            var flipped = new Color32[raw_data.Length / 4];

            for (int i = 0, j = pixels.Length - width_in_pixels; i < pixels.Length; i += width_in_pixels, j -= width_in_pixels) {
                for (int k = 0; k < width_in_pixels; ++k) {
                    flipped[i + k] = pixels[j + k];
                }
            }
            pixels = flipped;
        }
    }
}