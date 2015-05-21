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

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace Rpx.Packing.PEFile
{
    #region Structures

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ImageResourceDirectory
    {
        public uint Characteristics;
        public uint TimeDateStamp;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public ushort NumberOfNamedEntries;
        public ushort NumberOfIdEntries;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ImageResourceDirectoryEntry
    {
        public uint Name;
        public uint OffsetToData;

        public uint GetOffset(out bool isDir)
        {
            if ((OffsetToData & 0x80000000) == 0x80000000)
            {
                isDir = true;
                return OffsetToData & 0x7FFFFFFF;
            }
            else
            {
                isDir = false;
                return OffsetToData;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ImageResourceDataEntry
    {
        public uint OffsetToData;
        public uint Size;
        public uint CodePage;
        public uint Reserved;

        public uint GetOffset(out bool isDir) 
        {
            if ((OffsetToData & 0x80000000) == 0x80000000)
            {
                isDir = true;
                return OffsetToData & 0x7FFFFFFF;
            }
            else
            {
                isDir = true;
                return OffsetToData;
            }
        }
    }


    #endregion 

    internal class ResourceDirectory
    {
        internal ImageResourceDirectory ResourceDirectoryInfo;
        internal ImageResourceDirectoryEntry DirectoryEntry; 
        
        internal List<ResourceDirectory> Directorys = new List<ResourceDirectory>(); 
        internal List<ResourceEntry> Entries = new List<ResourceEntry>();
        
        Stream m_Stream;
        long m_BaseAddress;

        public ResourceDirectory(Stream stream, long baseAddress)
        {
            this.m_Stream = stream;
            this.m_BaseAddress = baseAddress; 
        }

        public ResourceDirectory(ImageResourceDirectoryEntry DirectoryEntry, Stream stream, long baseAddress) 
            :this(stream, baseAddress)
        {
            this.DirectoryEntry = DirectoryEntry; 
        }

        public void Seek()
        {
            bool isDir; 

            uint dirLoc = DirectoryEntry.GetOffset(out isDir);

            m_Stream.Seek(m_BaseAddress + dirLoc, SeekOrigin.Begin);
        }

        public void Read(BinaryReader reader, bool isRoot, uint parentName)
        {
            ResourceDirectoryInfo = PEHeader.FromBinaryReader<ImageResourceDirectory>(reader);

            List<ImageResourceDirectoryEntry> dirs = new List<ImageResourceDirectoryEntry>();
            List<ImageResourceDataEntry> entrys = new List<ImageResourceDataEntry>();

            for (int i = 0; i < ResourceDirectoryInfo.NumberOfNamedEntries; i++)
            {
                entrys.Add(PEHeader.FromBinaryReader<ImageResourceDataEntry>(reader));
            }

            for (int i = 0; i < ResourceDirectoryInfo.NumberOfIdEntries; i++)
            {
                if (isRoot)
                {
                    ImageResourceDirectoryEntry dirEntry = PEHeader.FromBinaryReader<ImageResourceDirectoryEntry>(reader);

                    if (dirEntry.Name == (uint)Win32ResourceType.RT_ICON ||
                        dirEntry.Name == (uint)Win32ResourceType.RT_GROUP_ICON)
                    {
                        dirs.Add(dirEntry);
                    }
                }
                else
                {
                    dirs.Add(PEHeader.FromBinaryReader<ImageResourceDirectoryEntry>(reader));
                }
            }

            foreach (ImageResourceDataEntry e in entrys)
            {
                bool isDir;

                uint entryLoc = e.GetOffset(out isDir);
                uint entrySize = e.Size;

                ResourceEntry entryInfo = new ResourceEntry(e, m_Stream, parentName);

                Entries.Add(entryInfo);                 
            }

            foreach (ImageResourceDirectoryEntry d in dirs)
            {
                bool isDir;

                uint dirLoc = d.GetOffset(out isDir);
                
                ResourceDirectory dirInfo = new ResourceDirectory(d, m_Stream, m_BaseAddress);

                if (isDir)
                {
                    Directorys.Add(dirInfo);

                    dirInfo.Seek();

                    dirInfo.Read(reader ,false, d.Name != 0 ? d.Name : parentName);
                }
                else
                {
                    dirInfo.Seek();

                    ImageResourceDataEntry entry = PEHeader.FromBinaryReader<ImageResourceDataEntry>(reader);

                    uint entryLoc = entry.GetOffset(out isDir);
                    uint entrySize = entry.Size;

                    ResourceEntry entryInfo = new ResourceEntry(entry, m_Stream, parentName);

                    entryInfo.Seek();

                    Entries.Add(entryInfo);   
                }
            }
        }

        internal ResourceEntry GetFirstEntry()
        {
            if (Entries.Count > 0)
                return Entries[0];

            foreach (ResourceDirectory dir in Directorys)
            {
                ResourceEntry firstEntry = dir.GetFirstEntry();

                if (firstEntry != null)
                    return firstEntry; 
            }

            return null;
        }

        internal List<ResourceEntry> GetAllEntrys()
        {
            List<ResourceEntry> list = new List<ResourceEntry>();

            return GetAllEntrys(list); 
        }

        private List<ResourceEntry> GetAllEntrys(List<ResourceEntry> list)
        {
            list.AddRange(Entries); 

            foreach (ResourceDirectory dir in Directorys)
            {
                dir.GetAllEntrys(list);
            }

            return list; 
        }
    }

    #region ResourceEntry
    
    /// <summary>
    /// 
    /// </summary>
    internal class ResourceEntry
    {
        internal ImageResourceDataEntry Entry;
        internal uint Name; 

        Stream m_Stream;

        public ResourceEntry(ImageResourceDataEntry Entry, Stream stream, uint Name)
        {
            this.Entry = Entry;

            this.m_Stream = stream;
            this.Name = Name; 
        }

        public void Seek()
        {
            m_Stream.Seek(DataAddress, SeekOrigin.Begin);
        }

        public long DataAddress
        {
            get
            {
                bool isDir;

                return (long)Entry.GetOffset(out isDir);
            }
        }
    }

    #endregion
}
