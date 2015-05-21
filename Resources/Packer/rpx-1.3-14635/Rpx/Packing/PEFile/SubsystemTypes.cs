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

namespace Rpx.Packing.PEFile
{
    [Serializable]
    public enum SubsystemTypes
    {
        /// <summary>
        /// Unknown subsystem.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Image doesn't require a subsystem.
        /// </summary>
        Native = 1,
        /// <summary>
        /// Image runs in the Windows GUI subsystem. (Forms)
        /// </summary>
        WindowsGui = 2,
        /// <summary>
        /// Image runs in the Windows character subsystem. (Console)
        /// </summary>
        WindowsCni = 3,
        /// <summary>
        /// Image runs in the OS/2 character subsystem.
        /// </summary>
        Os2Cui = 5,
        /// <summary>
        /// Image runs in the Posix character subsystem.
        /// </summary>
        PosixCui = 7,
        /// <summary>
        /// Image is a native Win9x driver.
        /// </summary>
        NativeWindows = 8,
        /// <summary>
        /// Image runs in the Windows CE subsystem.
        /// </summary>
        WindowsCEGui = 9
    }
    
}
