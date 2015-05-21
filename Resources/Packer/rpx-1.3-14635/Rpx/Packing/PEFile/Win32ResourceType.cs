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

namespace Rpx.Packing.PEFile
{

    internal enum Win32ResourceType
    {
        RT_CURSOR = 1,
        RT_FONT = 8,
        RT_BITMAP = 2,
        RT_ICON = 3,
        RT_MENU = 4,
        RT_DIALOG = 5,
        RT_STRING = 6,
        RT_FONTDIR = 7,
        RT_ACCELERATOR = 9,
        RT_RCDATA = 10,
        RT_MESSAGETABLE = 11,
        RT_GROUP_CURSOR = 12,
        RT_GROUP_ICON = 14,
        RT_VERSION = 16,
        RT_DLGINCLUDE = 17,
        RT_PLUGPLAY = 19,
        RT_VXD = 20,
        RT_ANICURSOR = 21,
        RT_ANIICON = 22,
        RT_HTML = 23,
    }
}
