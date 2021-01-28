using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using UnityEngine.UI;
using ASE;

public class OpenJSON : MonoBehaviour {
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

    public Image image;
    public byte[] data;

    public AsepriteObj asepriteObj;
    private void Awake() {
        StartCoroutine(Load());
    }

    private IEnumerator Load() {

        //aseprite = new Aseprite(@"c://Sprite-0001.aseprite", true);

        using UnityWebRequest w = UnityWebRequest.Get("file:///c://Sprite-0001.aseprite");
        yield return w.SendWebRequest();

        data = w.downloadHandler.data;
        if (data.Length < 128) {
            Debug.Log("File too small.");
            yield break;
        }

        asepriteObj = new AsepriteObj {
            header = new Header()
        };
        var header = asepriteObj.header;
        header.GenerateChunk(ref data);

        if (!header.headerHex.Equals("a5e0")) {
            Debug.Log("Only supports aseprite files.");
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
                Frame frame = new Frame(bytesInFrame, magicNumber);
                frame.GenerateChunk(ref frameData);

                while (frameData.Length > 1) {
                    var chunkSize = Read.DWORD(ref frameData);
                    var chunkType = Read.WORD(ref frameData);
                    var chunkTypeHex = chunkType.ToString("x");
                    var chunkData = Read.BYTEARRAY(ref frameData, (int)chunkSize - (Read.DWORD_LENGTH + Read.WORD_LENGTH));

                    //chunkData is used from here on
                    switch (chunkTypeHex) {
                        case OLD_PALETTE2_MAGIC: //11
                            OldPaletteChunk oldPaletteChunk2 = new OldPaletteChunk();
                            oldPaletteChunk2.GenerateChunk(ref chunkData);
                            frame.oldPaletteChunks.Add(oldPaletteChunk2);
                            break;

                        //Blank 11 == 4
                        case OLD_PALETTE_MAGIC: //4
                            OldPaletteChunk oldPaletteChunk = new OldPaletteChunk();
                            oldPaletteChunk.GenerateChunk(ref chunkData);
                            frame.oldPaletteChunks.Add(oldPaletteChunk);
                            break;

                        case LAYER_MAGIC:
                            LayerChunk layerChunk = new LayerChunk();
                            layerChunk.GenerateChunk(ref chunkData);
                            frame.layerChunks.Add(layerChunk);
                            break;

                        case CEL_MAGIC:
                            CelChunk celChunk = new CelChunk(header.color_depth);
                            celChunk.GenerateChunk(ref chunkData);

                            //testing
                            image.sprite = celChunk.sprite;

                            frame.celChunks.Add(celChunk);
                            break;

                        case CEL_EXTRA_MAGIC:
                            CelExtraChunk celExtraChunk = new CelExtraChunk();
                            celExtraChunk.GenerateChunk(ref chunkData);
                            frame.celExtraChunks.Add(celExtraChunk);
                            break;

                        case COLOR_MAGIC:
                            ColorProfileChunk colorProfileChunk = new ColorProfileChunk();
                            colorProfileChunk.GenerateChunk(ref chunkData);
                            frame.colorProfileChunks.Add(colorProfileChunk);
                            break;

                        case MASK_MAGIC:
                            MaskChunk maskChunk = new MaskChunk();
                            maskChunk.GenerateChunk(ref chunkData);
                            frame.maskChunks.Add(maskChunk);
                            break;

                        case PATH_MAGIC:
                            //Never Used
                            break;

                        case TAG_MAGIC:
                            TagChunk tagChunk = new TagChunk();
                            tagChunk.GenerateChunk(ref chunkData);
                            frame.tagChunks.Add(tagChunk);
                            break;

                        case PALETTE_MAGIC:
                            PaletteChunk paletteChunk = new PaletteChunk();
                            paletteChunk.GenerateChunk(ref chunkData);
                            frame.paletteChunks.Add(paletteChunk);
                            break;

                        case USERDATA_MAGIC:
                            UserDataChunk userDataChunk = new UserDataChunk();
                            userDataChunk.GenerateChunk(ref chunkData);
                            frame.userDataChunks.Add(userDataChunk);
                            break;

                        case SLICE_MAGIC:
                            SliceChunk sliceChunk = new SliceChunk();
                            sliceChunk.GenerateChunk(ref chunkData);
                            frame.sliceChunks.Add(sliceChunk);
                            break;

                        default:
                            Debug.LogError($"Unknown magic number: {chunkTypeHex}");
                            Debug.LogError($"Skipping data chunk of size: {chunkSize}");
                            break;
                    }

                }
                asepriteObj.frames.Add(frame);
            }

        }

        Debug.Log("DONE");
    }



    //private delegate void Blend(ref Color32 dest, Color32 src, byte opacity);
    //
    //private static Blend[] BlendModes = new Blend[]
    //{
    //        // 0 - NORMAL
    //        (ref Color32 dest, Color32 src, byte opacity) =>
    //        {
    //            int r, g, b, a;
    //
    //            if (dest.a == 0)
    //            {
    //                r = src.r;
    //                g = src.g;
    //                b = src.b;
    //            }
    //            else if (src.a == 0)
    //            {
    //                r = dest.r;
    //                g = dest.g;
    //                b = dest.b;
    //            }
    //            else
    //            {
    //                r = (dest.r + MUL_UN8((src.r - dest.r), opacity));
    //                g = (dest.g + MUL_UN8((src.g - dest.g), opacity));
    //                b = (dest.b + MUL_UN8((src.b - dest.b), opacity));
    //            }
    //
    //            a = (dest.a + MUL_UN8((src.a - dest.a), opacity));
    //            if (a == 0)
    //                r = g = b = 0;
    //
    //            dest.r = (byte)r;
    //            dest.g = (byte)g;
    //            dest.b = (byte)b;
    //            dest.a = (byte)a;
    //        }
    //};
    //private static int MUL_UN8(int a, int b) {
    //    var t = (a * b) + 0x80;
    //    return (((t >> 8) + t) >> 8);
    //}
    //
    ///// <summary>
    ///// Applies a Cel's pixels to the Frame, using its Layer's BlendMode & Alpha
    ///// </summary>
    //private void CelToFrame(Header header, Frame frame, CelChunk cel) {
    //    var opacity = (byte)((cel.opacity * frame.layerChunks[cel.layer_index].opacity) * 255);
    //    var blend = BlendModes[0];
    //
    //    for (int sx = 0; sx < cel.width_in_pixels; sx++) {
    //        int dx = cel.x_position + sx;
    //        int dy = cel.y_position * header.width_in_pixels;
    //
    //        for (int i = 0, sy = 0; i < cel.height_in_pixels; i++, sy += cel.width_in_pixels, dy += header.width_in_pixels)
    //            blend(ref header.pixels[dx + dy], cel.pixels[sx + sy], opacity);
    //    }
    //}

}
