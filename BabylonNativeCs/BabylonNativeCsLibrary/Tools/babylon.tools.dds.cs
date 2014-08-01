using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Web;
namespace BABYLON.Internals
{
    public partial interface DDSInfo
    {
        int width
        {
            get;
        }
        int height
        {
            get;
        }
        int mipmapCount
        {
            get;
        }
        bool isFourCC
        {
            get;
        }
        bool isRGB
        {
            get;
        }
        bool isLuminance
        {
            get;
        }
        bool isCube
        {
            get;
        }
    }
    public partial class DDSTools
    {
        public const int DDS_MAGIC = 0x20534444;

        public const int DDSD_CAPS = 0x1;
        public const int DDSD_HEIGHT = 0x2;
        public const int DDSD_WIDTH = 0x4;
        public const int DDSD_PITCH = 0x8;
        public const int DDSD_PIXELFORMAT = 0x1000;
        public const int DDSD_MIPMAPCOUNT = 0x20000;
        public const int DDSD_LINEARSIZE = 0x80000;
        public const int DDSD_DEPTH = 0x800000;

        public const int DDSCAPS_COMPLEX = 0x8;
        public const int DDSCAPS_MIPMAP = 0x400000;
        public const int DDSCAPS_TEXTURE = 0x1000;

        public const int DDSCAPS2_CUBEMAP = 0x200;
        public const int DDSCAPS2_CUBEMAP_POSITIVEX = 0x400;
        public const int DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800;
        public const int DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000;
        public const int DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000;
        public const int DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000;
        public const int DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000;
        public const int DDSCAPS2_VOLUME = 0x200000;

        public const int DDPF_ALPHAPIXELS = 0x1;
        public const int DDPF_ALPHA = 0x2;
        public const int DDPF_FOURCC = 0x4;
        public const int DDPF_RGB = 0x40;
        public const int DDPF_YUV = 0x200;
        public const int DDPF_LUMINANCE = 0x20000;

        public const int headerLengthInt = 31; // The header length in 32 bit ints

        // Offsets into the header array
        public const int off_magic = 0;

        public const int off_size = 1;
        public const int off_flags = 2;
        public const int off_height = 3;
        public const int off_width = 4;

        public const int off_mipmapCount = 7;

        public const int off_pfFlags = 20;
        public const int off_pfFourCC = 21;
        public const int off_RGBbpp = 22;
        public const int off_RMask = 23;
        public const int off_GMask = 24;
        public const int off_BMask = 25;
        public const int off_AMask = 26;
        public const int off_caps1 = 27;
        public const int off_caps2 = 28;

        public const int FOURCC_DXT1 = 'D' + ('X' << 8) + ('T' << 16) + ('1' << 24);
        public const int FOURCC_DXT3 = 'D' + ('X' << 8) + ('T' << 16) + ('3' << 24);
        public const int FOURCC_DXT5 = 'D' + ('X' << 8) + ('T' << 16) + ('5' << 24);

        private static Web.Console console;

        public static DDSInfo GetDDSInfo(ArrayBuffer arrayBuffer)
        {
            var header = new Int32Array(arrayBuffer, 0, headerLengthInt);
            var mipmapCount = 1;
            if ((header[off_flags] & DDSD_MIPMAPCOUNT) > 0)
            {
                mipmapCount = Math.Max(1, header[off_mipmapCount]);
            }

            return new DDSInfoDts
            {
                width = header[off_width],
                height = header[off_height],
                mipmapCount = mipmapCount,
                isFourCC = (header[off_pfFlags] & DDPF_FOURCC) == DDPF_FOURCC,
                isRGB = (header[off_pfFlags] & DDPF_RGB) == DDPF_RGB,
                isLuminance = (header[off_pfFlags] & DDPF_LUMINANCE) == DDPF_LUMINANCE,
                isCube = (header[off_caps2] & DDSCAPS2_CUBEMAP) == DDSCAPS2_CUBEMAP
            };
        }
        private static Uint8Array GetRGBAArrayBuffer(int width, int height, int dataOffset, int dataLength, ArrayBuffer arrayBuffer)
        {
            var byteArray = new Uint8Array(dataLength);
            var srcData = new Uint8Array(arrayBuffer);
            var index = 0;
            for (var y = height - 1; y >= 0; y--)
            {
                for (var x = 0; x < width; x++)
                {
                    var srcPos = dataOffset + (x + y * width) * 4;
                    byteArray[index + 2] = srcData[srcPos];
                    byteArray[index + 1] = srcData[srcPos + 1];
                    byteArray[index] = srcData[srcPos + 2];
                    byteArray[index + 3] = srcData[srcPos + 3];
                    index += 4;
                }
            }
            return byteArray;
        }
        private static Uint8Array GetRGBArrayBuffer(int width, int height, int dataOffset, int dataLength, ArrayBuffer arrayBuffer)
        {
            var byteArray = new Uint8Array(dataLength);
            var srcData = new Uint8Array(arrayBuffer);
            var index = 0;
            for (var y = height - 1; y >= 0; y--)
            {
                for (var x = 0; x < width; x++)
                {
                    var srcPos = dataOffset + (x + y * width) * 3;
                    byteArray[index + 2] = srcData[srcPos];
                    byteArray[index + 1] = srcData[srcPos + 1];
                    byteArray[index] = srcData[srcPos + 2];
                    index += 3;
                }
            }
            return byteArray;
        }
        private static Uint8Array GetLuminanceArrayBuffer(int width, int height, int dataOffset, int dataLength, ArrayBuffer arrayBuffer)
        {
            var byteArray = new Uint8Array(dataLength);
            var srcData = new Uint8Array(arrayBuffer);
            var index = 0;
            for (var y = height - 1; y >= 0; y--)
            {
                for (var x = 0; x < width; x++)
                {
                    var srcPos = dataOffset + (x + y * width);
                    byteArray[index] = srcData[srcPos];
                    index++;
                }
            }
            return byteArray;
        }
        public static void UploadDDSLevels(WebGLRenderingContext gl, WEBGL_compressed_texture_s3tc ext, ArrayBuffer arrayBuffer, DDSInfo info, bool loadMipmaps, int faces)
        {
            var header = new Int32Array(arrayBuffer, 0, headerLengthInt);
            int fourCC;
            int blockBytes;
            int internalFormat;
            int width;
            int height;
            int dataLength;
            int dataOffset;
            Uint8Array byteArray;
            int mipmapCount;
            int i;
            if (header[off_magic] != DDS_MAGIC)
            {
                Tools.Error("Invalid magic number in DDS header");
                return;
            }
            if (!info.isFourCC && !info.isRGB && !info.isLuminance)
            {
                Tools.Error("Unsupported format, must contain a FourCC, RGB or LUMINANCE code");
                return;
            }
            if (info.isFourCC)
            {
                fourCC = header[off_pfFourCC];
                switch (fourCC)
                {
                    case FOURCC_DXT1:
                        blockBytes = 8;
                        internalFormat = ext.COMPRESSED_RGBA_S3TC_DXT1_EXT;
                        break;
                    case FOURCC_DXT3:
                        blockBytes = 16;
                        internalFormat = ext.COMPRESSED_RGBA_S3TC_DXT3_EXT;
                        break;
                    case FOURCC_DXT5:
                        blockBytes = 16;
                        internalFormat = ext.COMPRESSED_RGBA_S3TC_DXT5_EXT;
                        break;
                    default:
                        console.error("Unsupported FourCC code:", fourCC);
                        return;
                }
            }
            mipmapCount = 1;
            if ((header[off_flags] & DDSD_MIPMAPCOUNT) > 0 && loadMipmaps)
            {
                mipmapCount = Math.Max(1, header[off_mipmapCount]);
            }
            var bpp = header[off_RGBbpp];
            for (var face = 0; face < faces; face++)
            {
                var sampler = (faces == 1) ? gl.TEXTURE_2D : (gl.TEXTURE_CUBE_MAP_POSITIVE_X + face);
                width = header[off_width];
                height = header[off_height];
                dataOffset = header[off_size] + 4;
                for (i = 0; i < mipmapCount; ++i)
                {
                    if (info.isRGB)
                    {
                        if (bpp == 24)
                        {
                            dataLength = width * height * 3;
                            byteArray = DDSTools.GetRGBArrayBuffer(width, height, dataOffset, dataLength, arrayBuffer);
                            gl.texImage2D(sampler, i, gl.RGB, width, height, 0, gl.RGB, gl.UNSIGNED_BYTE, byteArray);
                        }
                        else
                        {
                            dataLength = width * height * 4;
                            byteArray = DDSTools.GetRGBAArrayBuffer(width, height, dataOffset, dataLength, arrayBuffer);
                            gl.texImage2D(sampler, i, gl.RGBA, width, height, 0, gl.RGBA, gl.UNSIGNED_BYTE, byteArray);
                        }
                    }
                    else
                        if (info.isLuminance)
                        {
                            var unpackAlignment = (int)gl.getParameter(gl.UNPACK_ALIGNMENT);
                            var unpaddedRowSize = width;
                            var paddedRowSize = (int)Math.Floor((width + unpackAlignment - 1) / unpackAlignment) * unpackAlignment;
                            dataLength = paddedRowSize * (height - 1) + unpaddedRowSize;
                            byteArray = DDSTools.GetLuminanceArrayBuffer(width, height, dataOffset, dataLength, arrayBuffer);
                            gl.texImage2D(sampler, i, gl.LUMINANCE, width, height, 0, gl.LUMINANCE, gl.UNSIGNED_BYTE, byteArray);
                        }
                        else
                        {
                            dataLength = Math.Max(4, width) / 4 * Math.Max(4, height) / 4 * blockBytes;
                            byteArray = new Uint8Array(arrayBuffer, dataOffset, dataLength);
                            gl.compressedTexImage2D(sampler, i, internalFormat, width, height, 0, byteArray);
                        }
                    dataOffset += dataLength;
                    width /= 2;
                    height /= 2;
                    width = Math.Max(1, width);
                    height = Math.Max(1, height);
                }
            }
        }
    }
}