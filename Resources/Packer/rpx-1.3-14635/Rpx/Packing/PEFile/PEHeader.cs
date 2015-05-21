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
    #region File Header Structures

    /// <summary>
    /// DOS .EXE Header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ImageDosHeader
    {      
        /// <summary>
        /// Magic number
        /// </summary>
        public ushort MagicNumber;              

        /// <summary>
        /// Bytes on last page of file
        /// </summary>
        public ushort BytesOnLastPage;               

        /// <summary>
        /// Pages in file
        /// </summary>
        public ushort PagesInFile;
        
        /// <summary>
        /// Relocations
        /// </summary>
        public ushort Relocations;          

        /// <summary>
        /// Size of header in paragraphs
        /// </summary>
        public ushort SizeOfHeader;   
        
        /// <summary>
        /// Minimum extra paragraphs needed
        /// </summary>
        public ushort MinExtraAlloc;  
        
        /// <summary>
        /// Maximum extra paragraphs needed
        /// </summary>
        public ushort MaxExtraAlloc;      
     
        /// <summary>
        /// Initial (relative) SS value
        /// </summary>
        public ushort SS;                 

        /// <summary>
        /// Initial SP value
        /// </summary>
        public ushort SP;    
        
        /// <summary>
        /// Checksum
        /// </summary>
        public ushort Checksum;     
        
        /// <summary>
        /// Initial IP value
        /// </summary>
        public ushort IP;                

        /// <summary>
        /// Initial (relative) CS value
        /// </summary>
        public ushort CS;                

        /// <summary>
        /// File address of relocation table
        /// </summary>
        public ushort RelocationTableAddress;      
       
        /// <summary>
        /// Overlay number
        /// </summary>
        public ushort OverlayNumber;              

        /// <summary>
        /// Reserved words
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] ReservedWords;    
        
        /// <summary>
        /// OEM identifier (for OEMInformation)
        /// </summary>
        public ushort OEMIdentifier;

        /// <summary>
        /// OEM information (OEMIdentifier specific) 
        /// </summary>
        public ushort OEMInformation;            

        /// <summary>
        /// Reserved words
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] ReservedWords2;             

        /// <summary>
        /// File address of new exe header
        /// </summary>
        public ushort ExeHeaderAddress;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ImageOptionalHeader32
    {
        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public uint BaseOfData;
        public uint ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public uint SizeOfStackReserve;
        public uint SizeOfStackCommit;
        public uint SizeOfHeapReserve;
        public uint SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;

        public ImageDataDirectory Data_ExportTable;
        public ImageDataDirectory Data_ImportTable;
        public ImageDataDirectory Data_ResourceTable;
        public ImageDataDirectory Data_ExceptionTable;
        public ImageDataDirectory Data_CertificateTable;
        public ImageDataDirectory Data_BaseRelocation;
        public ImageDataDirectory Data_DebuggingInformation;
        public ImageDataDirectory Data_ArchitectureSpecific;
        public ImageDataDirectory Data_GlobalPointerRegister;
        public ImageDataDirectory Data_ThreadLocalStorage;
        public ImageDataDirectory Data_LoadConfigurationTable;
        public ImageDataDirectory Data_BoundImportTable;
        public ImageDataDirectory Data_ImportAddressTable;
        public ImageDataDirectory Data_DelayImportDescriptor;
        public ImageDataDirectory Data_CLRHeader;
        public ImageDataDirectory Data_Reserved;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ImageOptionalHeader64
    {
        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public ulong ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public ulong SizeOfStackReserve;
        public ulong SizeOfStackCommit;
        public ulong SizeOfHeapReserve;
        public ulong SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;

        public ImageDataDirectory Data_ExportTable;
        public ImageDataDirectory Data_ImportTable;
        public ImageDataDirectory Data_ResourceTable;
        public ImageDataDirectory Data_ExceptionTable;
        public ImageDataDirectory Data_CertificateTable;
        public ImageDataDirectory Data_BaseRelocation;
        public ImageDataDirectory Data_DebuggingInformation;
        public ImageDataDirectory Data_ArchitectureSpecific;
        public ImageDataDirectory Data_GlobalPointerRegister;
        public ImageDataDirectory Data_ThreadLocalStorage;
        public ImageDataDirectory Data_LoadConfigurationTable;
        public ImageDataDirectory Data_BoundImportTable;
        public ImageDataDirectory Data_ImportAddressTable;
        public ImageDataDirectory Data_DelayImportDescriptor;
        public ImageDataDirectory Data_CLRHeader;
        public ImageDataDirectory Data_Reserved;

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ImageFileHeader
    {
        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort SizeOfOptionalHeader;
        public ushort Characteristics;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ImageDataDirectory
    {
        public uint VirtualAddress;
        public uint Size;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct ImageSectionHeader
    {
        public const int IMAGE_SIZEOF_SHORT_NAME = 8;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Name;        
        public uint VirtualSize;
        public uint VirtualAddress;
        public uint RawDataSize;
        public uint RawDataAddress;
        public uint RelocationsAddress;
        public uint Linenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public uint Characteristics;

        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < Name.Length; i++)
            {
                if (Name[i] == 0)
                    break;

                str += (char)Name[i];
            }

            return str;
        }
    }

    #endregion

    #region Portable Executable Header

    /// <summary>
    /// Reads the header of a portable executable file and allows extraction of its default icon
    /// </summary>
    internal class PEHeader
    {      
        #region Private Members

        // subsystem type of the executable
        private SubsystemTypes m_SubsystemType = SubsystemTypes.Unknown; 

        // The DOS header
        private ImageDosHeader m_DosHeader;

        // The file header
        private ImageFileHeader m_FileHeader;
        
        // Optional 32 bit file header
        private ImageOptionalHeader32 m_OptionalHeader32;
        
        // Optional 64 bit file header
        private ImageOptionalHeader64 m_OptionalHeader64;
        
        // Section header
        private ImageSectionHeader m_Section;

        // Resource data
        private ImageDataDirectory ReourcesDataTable;

        // was the header read correctly 
        private bool m_FoundSection;

        #endregion

        #region Properties

        /// <summary>
        /// Get the executables subsystem type
        /// </summary>
        public SubsystemTypes SubsystemType 
        { 
            get 
            { 
                return m_SubsystemType; 
            } 
        }

        /// <summary>
        /// Gets if the file header is 32 bit or not
        /// </summary>
        public bool Is32BitHeader
        {
            get
            {
                UInt16 IMAGE_FILE_32BIT_MACHINE = 0x0100;
                return (IMAGE_FILE_32BIT_MACHINE & FileHeader.Characteristics) == IMAGE_FILE_32BIT_MACHINE;
            }
        }

        /// <summary>
        /// Gets the file header
        /// </summary>
        public ImageFileHeader FileHeader
        {
            get
            {
                return m_FileHeader;
            }
        }

        /// <summary>
        /// Gets the optional 32 header
        /// </summary>
        public ImageOptionalHeader32 OptionalHeader32
        {
            get
            {
                return m_OptionalHeader32;
            }
        }

        /// <summary>
        /// Gets the optional 64 header 
        /// </summary>
        public ImageOptionalHeader64 OptionalHeader64
        {
            get
            {
                return m_OptionalHeader64;
            }
        }

        /// <summary>
        /// Gets the timestamp from the file header
        /// </summary>
        public DateTime TimeStamp
        {
            get
            {
                // Timestamp is a date offset from 1970
                DateTime returnValue = new DateTime(1970, 1, 1, 0, 0, 0);

                // Add in the number of seconds since 1970/1/1
                returnValue = returnValue.AddSeconds(m_FileHeader.TimeDateStamp);
                // Adjust to local timezone
                returnValue += TimeZone.CurrentTimeZone.GetUtcOffset(returnValue);

                return returnValue;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the PE header from the bytes of an assembly
        /// </summary>
        /// <param name="bytes"></param>
        public PEHeader(byte[] bytes)
        {
            m_FoundSection = false;

            // Read in the DLL or EXE and get the timestamp
            using (MemoryStream stream = new MemoryStream(bytes, false))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    m_DosHeader = FromBinaryReader<ImageDosHeader>(reader);

                    // Add 4 bytes to the offset
                    stream.Seek(m_DosHeader.ExeHeaderAddress, SeekOrigin.Begin);

                    UInt32 ntHeadersSignature = reader.ReadUInt32();

                    m_FileHeader = FromBinaryReader<ImageFileHeader>(reader);

                    if (this.Is32BitHeader)
                    {
                        m_OptionalHeader32 = FromBinaryReader<ImageOptionalHeader32>(reader);
                        m_SubsystemType = (SubsystemTypes)m_OptionalHeader32.Subsystem;

                        ReourcesDataTable = m_OptionalHeader32.Data_ResourceTable;
                    }
                    else
                    {
                        m_OptionalHeader64 = FromBinaryReader<ImageOptionalHeader64>(reader);
                        m_SubsystemType = (SubsystemTypes)m_OptionalHeader64.Subsystem;

                        ReourcesDataTable = m_OptionalHeader32.Data_ResourceTable;
                    }

                    for (int i = 0; i < m_FileHeader.NumberOfSections; i++)
                    {
                        ImageSectionHeader section = FromBinaryReader<ImageSectionHeader>(reader);

                        if (section.ToString() == ".rsrc")
                        {
                            m_Section = section;
                            m_FoundSection = true;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="writeToPath"></param>
        /// <returns></returns>
        public bool TryExtractIconFromExe(byte[] bytes, string writeToPath)
        {
            if (m_FoundSection)
            {
                using (MemoryStream stream = new MemoryStream(bytes, false))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        uint rva = ReourcesDataTable.VirtualAddress;

                        uint size = m_Section.VirtualSize > 0 ? m_Section.VirtualSize : m_Section.RawDataSize;

                        if (rva >= m_Section.VirtualAddress && rva < m_Section.VirtualAddress + size)
                        {
                            long baseAddress = stream.Seek(m_Section.RawDataAddress + (rva - m_Section.VirtualAddress), SeekOrigin.Begin);

                            ResourceDirectory dirInfo = new ResourceDirectory(stream, baseAddress);

                            dirInfo.Read(reader, true, 0);

                            ResourceEntry IconGroup = null;
                            List<ResourceEntry> IconImages = new List<ResourceEntry>();

                            foreach (ResourceDirectory dir in dirInfo.Directorys)
                            {
                                if (dir.DirectoryEntry.Name == (uint)Win32ResourceType.RT_GROUP_ICON)
                                {
                                    IconGroup = dir.GetFirstEntry();
                                    break;
                                }
                            }

                            foreach (ResourceDirectory dir in dirInfo.Directorys)
                            {
                                if (dir.DirectoryEntry.Name == (uint)Win32ResourceType.RT_ICON)
                                {
                                    IconImages = dir.GetAllEntrys();

                                    break;
                                }
                            }

                            if (IconGroup != null)
                            {
                                if (RC.ShouldWrite(ConsoleVerbosity.Verbose))
                                {
                                    RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubTextGood, " - " + Rpx.Strings.Reflection_FoundIconResource);
                                }
                                else
                                {
                                    RC.WriteLine(ConsoleVerbosity.Minimal, ConsoleThemeColor.SubTextGood, " " + Rpx.Strings.Reflection_FoundIconResource);                                    
                                }
                                
                                RC.WriteLine(ConsoleVerbosity.Verbose, ConsoleThemeColor.SubText, " - " + string.Format(Rpx.Strings.Reflection_FoundNImageLayers, IconImages.Count)); 

                                IconResource icon = new IconResource(stream, IconGroup.DataAddress, m_Section.RawDataAddress, m_Section.VirtualAddress);

                                icon.Seek();

                                if (!icon.Read(reader, IconImages))
                                    return false; 
                                
                                icon.Write(writeToPath, reader);
                                return true;                                 
                            }
                        }
                    }
                }
            }

            return false; 
        }

        /// <summary>
        /// Reads in a block from a file and converts it to the struct
        /// type specified by the template parameter
        /// </summary>
        /// <typeparam name="T">type of the struct to read</typeparam>
        /// <param name="reader">reader</param>
        /// <returns>a instance of the struct T cast from the data in the reader</returns>
        public static T FromBinaryReader<T>(BinaryReader reader)
        {
            // Read in a byte array
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            // Pin the managed memory while, copy it out the data, then unpin it
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        #endregion
    }

    #endregion
}