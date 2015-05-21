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
using System.IO;
using Rug.Cmd;

namespace Rpx.Packing.Helpers
{
    internal class DisguiseHelper
    {
        public static void DisguiseFile(string path)
        {
            if (!File.Exists(path))
                throw new Exception(string.Format(Rpx.Strings.Disguise_ErrorPathDoesNotExist, path));

            CmdHelper.WriteInfoToConsole(Rpx.Strings.Hide_Protection, Rpx.Strings.Hide_Disguised, RC.Theme[ConsoleThemeColor.SubTextNutral]);            
                                                                                             
            byte[] bytes = File.ReadAllBytes(path);
            
            Array.Reverse(bytes);

            File.WriteAllBytes(path, bytes);
        }
    }
}
