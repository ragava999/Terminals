/* 
 * RPX 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * 
 * Copyright (C) 2008 Phill Tew. All rights reserved.
 * 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using Rug.Cmd;

namespace Rpx.Packing.PEFile
{    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct GRPICONDIR
    {
        public ushort idReserved;   // Reserved (must be 0)
        public ushort idType;       // Resource type (1 for icons)
        public ushort idCount;      // How many images?
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct GRPICONDIRENTRY
    {
        public byte Width;               // Width, in pixels, of the image
        public byte Height;              // Height, in pixels, of the image
        public byte ColorCount;          // Number of colors in image (0 if >=8bpp)
        public byte Reserved;            // Reserved
        public ushort Planes;              // Color Planes
        public ushort BitCount;            // Bits per pixel
        public uint BytesInRes;         // how many bytes in this resource?
        public ushort ID;                  // the ID
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ICON_HEADER
    {
        public ushort idReserved;   // Reserved (must be 0)
        public ushort idType;       // Resource Type (1 for icons)
        public ushort idCount;      // How many images?
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ICON_DIRECTORY_ENTRY
    {
        public byte Width;          // Width, in pixels, of the image
        public byte Height;         // Height, in pixels, of the image
        public byte ColorCount;     // Number of colors in image (0 if >=8bpp)
        public byte Reserved;       // Reserved ( must be 0)
        public ushort Planes;         // Color Planes
        public ushort BitCount;       // Bits per pixel
        public uint BytesInRes;    // How many bytes in this resource?
        public uint ImageOffset;   // Where in the file is this image?
    }



    internal class IconImage
    {
        internal GRPICONDIRENTRY Entry;
        internal ResourceEntry Resource;

        public long GetResourceAddress(long SectionBaseAddress, long SectionVirtualAddress)
        {
            return SectionBaseAddress + (Resource.DataAddress - SectionVirtualAddress);  
        }

        public byte[] GetImageData(BinaryReader reader, Stream stream, long location)
        {
            stream.Seek(location, SeekOrigin.Begin); 

            uint size = Resource.Entry.Size; 

            return reader.ReadBytes((int)size);
        }
    }

    internal class IconResource
    {
        GRPICONDIR Group;
        List<IconImage> Entries = new List<IconImage>();

        Stream m_Stream;
        long m_BaseAddress;
        long VirtualAddress; 
        long SectionBaseAddress;
        long SectionVirtualAddress; 

        public IconResource(Stream stream, long VirtualAddress, long SectionBaseAddress, long SectionVirtualAddress)
        {
            this.m_Stream = stream;
            
            this.SectionBaseAddress = SectionBaseAddress;
            this.SectionVirtualAddress = SectionVirtualAddress;

            this.VirtualAddress = VirtualAddress;

            this.m_BaseAddress = SectionBaseAddress + (VirtualAddress - SectionVirtualAddress);
        }

        /// <summary>
        /// Move the position of the stream to the start of the structure
        /// </summary>
        public void Seek()
        {
            m_Stream.Seek(m_BaseAddress, SeekOrigin.Begin); 
        }

        /// <summary>
        /// Read icon group from PE file header
        /// </summary>
        /// <param name="reader">reader that holds the PE image</param>
        /// <param name="iconImageData">all the ResourceEntry objects that hold the image data for the icon</param>
        public bool Read(BinaryReader reader, List<ResourceEntry> iconImageData)
        {
            try
            {
                Group = PEHeader.FromBinaryReader<GRPICONDIR>(reader);

                if (Group.idReserved != 0)
                {
                    RC.WriteWarning(0109, Rpx.Strings.Error_0109);

                    return false;
                }

                if (Group.idType != 1)
                {
                    RC.WriteWarning(0110, Rpx.Strings.Error_0110);

                    return false;
                }

                for (int i = 0; i < Group.idCount; i++)
                {
                    GRPICONDIRENTRY entry = PEHeader.FromBinaryReader<GRPICONDIRENTRY>(reader);

                    IconImage image = new IconImage();
                    image.Entry = entry;

                    foreach (ResourceEntry bmp in iconImageData)
                    {
                        if (bmp.Name == entry.ID)
                        {
                            image.Resource = bmp;

                            Entries.Add(image);

                            break;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                RC.WriteException(0111, Rpx.Strings.Error_0111, ex);

                return false;
            }            
        }

        /// <summary>
        /// Writes the icon group as a .ico file
        /// </summary>
        /// <param name="path">path to write to</param>
        /// <param name="reader">reader that holds the PE image</param>
        public void Write(string path, BinaryReader reader)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    ICON_HEADER header = new ICON_HEADER();

                    header.idCount = (ushort)Entries.Count;
                    header.idReserved = Group.idReserved;
                    header.idType = Group.idType;

                    writer.Write(RawSerialize(header)); 

                    long size = Marshal.SizeOf(typeof(ICON_DIRECTORY_ENTRY));

                    long headerEnd = Marshal.SizeOf(typeof(ICON_HEADER)) + (size * Entries.Count);

                    long baseImageDataOffset = headerEnd;

                    foreach (IconImage entry in Entries)
                    {
                        ICON_DIRECTORY_ENTRY ent = new ICON_DIRECTORY_ENTRY();

                        ent.ColorCount = entry.Entry.ColorCount;
                        ent.Height = entry.Entry.Height;
                        ent.Reserved = entry.Entry.Reserved;
                        ent.Width = entry.Entry.Width;
                        ent.BytesInRes = entry.Entry.BytesInRes;
                        ent.BitCount = entry.Entry.BitCount;
                        ent.Planes = entry.Entry.Planes;
                        ent.ImageOffset = (uint)baseImageDataOffset;
                        baseImageDataOffset += entry.Entry.BytesInRes;

                        writer.Write(RawSerialize(ent)); 
                    }

                    foreach (IconImage entry in Entries)
                    {
                        writer.Write(entry.GetImageData(reader, m_Stream, entry.GetResourceAddress(SectionBaseAddress, SectionVirtualAddress)));
                    }
                }

                FileInfo info = new FileInfo(path);

                if (!info.Directory.Exists)
                    info.Directory.Create();

                if (info.Exists)
                {
                    info.Attributes = FileAttributes.Normal;
                    info.Delete(); 
                }

                File.WriteAllBytes(path, stream.ToArray()); 
            }
        }

        public static byte[] RawSerialize(object anything)
        {
            int rawsize = Marshal.SizeOf(anything);

            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(anything, buffer, false);

            byte[] rawdata = new byte[rawsize];
            
            Marshal.Copy(buffer, rawdata, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            
            return rawdata;
        }
    }
}
