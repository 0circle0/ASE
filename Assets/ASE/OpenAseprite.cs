using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace ASE {

    public class OpenAseprite : MonoBehaviour {
        public const string HEADER_MAGIC = "a5e0";
        public const string FRAME_MAGIC = "f1fa";
        public const string OLD_PALETTE_MAGIC = "4";
        public const string OLD_PALETTE2_MAGIC = "11";
        public const string LAYER_MAGIC = "2004";
        public const string CEL_MAGIC = "2005";
        public const string CEL_EXTRA_MAGIC = "2006";
        public const string COLOR_MAGIC = "2007";
        public const string MASK_MAGIC = "2016";//depricated
        public const string PATH_MAGIC = "2017"; //not used?
        public const string TAG_MAGIC = "2018";
        public const string PALETTE_MAGIC = "2019";
        public const string USERDATA_MAGIC = "2020";
        public const string SLICE_MAGIC = "2022";
        [HideInInspector]
        public Dictionary<string, Action<byte[], Frame>> Magic;

        public Action<string> OnError = delegate { };
        public Action<AsepriteObj> OnSuccess = delegate { };

        public Image image;
        [HideInInspector]
        public byte[] data;
        public List<string> LayerNames;

        //Seeing in inspector is the point
        public AsepriteObj asepriteObj;

        private void Awake() {
            Magic = new Dictionary<string, Action<byte[], Frame>> {
                { OLD_PALETTE_MAGIC, (chunkData, frame) => CreateOldPalette(ref chunkData, frame) },
                { OLD_PALETTE2_MAGIC, (chunkData, frame) => CreateOldPalette(ref chunkData, frame) },
                { LAYER_MAGIC, (chunkData, frame) => CreateLayer(ref chunkData, frame) },
                { CEL_MAGIC, (chunkData, frame) => CreateCel(ref chunkData, frame) },
                { CEL_EXTRA_MAGIC, (chunkData, frame) => CreateCelExtra(ref chunkData, frame) },
                { COLOR_MAGIC, (chunkData, frame) => CreateColor(ref chunkData, frame) },
                { MASK_MAGIC, (chunkData, frame) => CreateMask(ref chunkData, frame) },
                { PATH_MAGIC, (chunkData, frame) => CreatePath(ref chunkData, frame) },
                { TAG_MAGIC, (chunkData, frame) => CreateTag(ref chunkData, frame) },
                { PALETTE_MAGIC, (chunkData, frame) => CreatePalatte(ref chunkData, frame) },
                { USERDATA_MAGIC, (chunkData, frame) => CreateUser(ref chunkData, frame) },
                { SLICE_MAGIC, (chunkData, frame) => CreateSlice(ref chunkData, frame) }
            };

            StartCoroutine(Load("file:///c://Sprite-0001.aseprite"));
        }
        //Keep track of the start time of coroutine
        //yield when the coroutine is taking too long.
        public IEnumerator Load(string path) {
            
            LayerNames = new List<string>();
            using UnityWebRequest w = UnityWebRequest.Get(path);
            yield return w.SendWebRequest();

            data = w.downloadHandler.data;

            if (data.Length < 128) {
                //Debug.Log("File too small.");
                OnError?.Invoke("File too small.");
                yield break;
            }

            //I want a perm object
            asepriteObj = new AsepriteObj {
                header = new Header()
            };


            asepriteObj.header.GenerateChunk(ref data);


            if (!asepriteObj.header.headerHex.Equals(HEADER_MAGIC)) {
                //Debug.Log("Only supports aseprite files.");
                OnError?.Invoke("Only supports aseprite files.");
                yield break;
            }

            while (data.Length > 1) {
                //Expect a frame
                var bytesInFrame = Read.DWORD(ref data);
                var magicNumber = Read.WORD(ref data);
                var magicNumberHex = magicNumber.ToString("x");

                //frameData bytesInFrame include the bytesInFrame and MagicNumber. Since we already have them adjust the length.
                var frameData = Read.BYTEARRAY(ref data, (int)bytesInFrame - (Read.DWORD_LENGTH + Read.WORD_LENGTH));

                //Making sure we have an aseprite file frame. This is guarenteed if a real aseprite file was loaded
                if (magicNumberHex.Equals(FRAME_MAGIC)) {
                    Frame frame = new Frame(bytesInFrame, magicNumber, asepriteObj.header.color_depth) {
                        width_in_pixels = asepriteObj.header.width_in_pixels,
                        height_in_pixels = asepriteObj.header.height_in_pixels
                    };
                    frame.GenerateChunk(ref frameData);

                    while (frameData.Length > 1) {
                        var chunkSize = Read.DWORD(ref frameData);
                        var chunkType = Read.WORD(ref frameData);
                        var chunkTypeHex = chunkType.ToString("x");
                        var chunkData = Read.BYTEARRAY(ref frameData, (int)chunkSize - (Read.DWORD_LENGTH + Read.WORD_LENGTH));

                        Magic.TryGetValue(chunkTypeHex, out Action<byte[], Frame> Create);
                        Create(chunkData, frame);

                    }
                    asepriteObj.frames.Add(frame);
                }
            }
            OnSuccess?.Invoke(asepriteObj);
        }

        private void CreateOldPalette(ref byte[] chunkData, Frame frame) {
            OldPaletteChunk oldPaletteChunk = new OldPaletteChunk();
            oldPaletteChunk.GenerateChunk(ref chunkData);
            frame.oldPaletteChunks.Add(oldPaletteChunk);
        }

        private void CreateLayer(ref byte[] chunkData, Frame frame) {
            LayerChunk layerChunk = new LayerChunk();
            layerChunk.GenerateChunk(ref chunkData);
            frame.layerChunks.Add(layerChunk);
            LayerNames.Add(layerChunk.layer_name);
        }

        private void CreateCel(ref byte[] chunkData, Frame frame) {
            CelChunk celChunk = new CelChunk(frame.color_depth);
            celChunk.GenerateChunk(ref chunkData);
            //Debug.Log(celChunk.layer_index);
            //Debug.Log(frame.layerChunks[celChunk.layer_index].layer_name);
            ////testing
            celChunk.layer_name = LayerNames[celChunk.layer_index];
            celChunk.BuildSprite(frame.width_in_pixels, frame.height_in_pixels);
            image.sprite = celChunk.sprite;

            frame.celChunks.Add(celChunk);
        }

        private void CreateCelExtra(ref byte[] chunkData, Frame frame) {
            CelExtraChunk celExtraChunk = new CelExtraChunk();
            celExtraChunk.GenerateChunk(ref chunkData);
            frame.celExtraChunks.Add(celExtraChunk);
        }

        private void CreateColor(ref byte[] chunkData, Frame frame) {
            ColorProfileChunk colorProfileChunk = new ColorProfileChunk();
            colorProfileChunk.GenerateChunk(ref chunkData);
            frame.colorProfileChunks.Add(colorProfileChunk);
        }

        private void CreateMask(ref byte[] chunkData, Frame frame) {
            MaskChunk maskChunk = new MaskChunk();
            maskChunk.GenerateChunk(ref chunkData);
            frame.maskChunks.Add(maskChunk);
        }

        private void CreatePath(ref byte[] chunkData, Frame frame) {
            //Left empty for future
        }

        private void CreateTag(ref byte[] chunkData, Frame frame) {
            TagChunk tagChunk = new TagChunk();
            tagChunk.GenerateChunk(ref chunkData);
            frame.tagChunks.Add(tagChunk);
        }

        private void CreatePalatte(ref byte[] chunkData, Frame frame) {
            PaletteChunk paletteChunk = new PaletteChunk();
            paletteChunk.GenerateChunk(ref chunkData);
            frame.paletteChunks.Add(paletteChunk);
        }

        private void CreateUser(ref byte[] chunkData, Frame frame) {
            UserDataChunk userDataChunk = new UserDataChunk();
            userDataChunk.GenerateChunk(ref chunkData);
            frame.userDataChunks.Add(userDataChunk);
        }

        private void CreateSlice(ref byte[] chunkData, Frame frame) {
            SliceChunk sliceChunk = new SliceChunk();
            sliceChunk.GenerateChunk(ref chunkData);
            frame.sliceChunks.Add(sliceChunk);
        }
    }
}