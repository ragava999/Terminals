namespace Rug.Cmd.Colors
{
    using Rug.Cmd;
    using System;
    using System.IO;
    using System.Reflection;

    public sealed class ConsoleColorThemeDirectory
    {
        private const int Length = 0x200;
        private byte[] m_RawBytes = new byte[0x200];

        public static ConsoleColorThemeDirectory Read(Stream stream)
        {
            ConsoleColorThemeDirectory directory = new ConsoleColorThemeDirectory();
            if (stream.Length == 0x200L)
            {
                stream.Read(directory.m_RawBytes, 0, 0x200);
            }
            return directory;
        }

        public static ConsoleColorThemeDirectory Read(string path)
        {
            FileInfo info = new FileInfo(path);
            if (!info.Exists)
            {
                throw new Exception("");
            }
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return Read(stream);
            }
        }

        public void Write(Stream stream)
        {
            stream.Write(this.m_RawBytes, 0, 0x200);
        }

        public void Write(string path)
        {
            FileInfo info = new FileInfo(path);
            if (info.Exists)
            {
                info.Delete();
            }
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                this.Write(stream);
            }
        }

        public ConsoleColorTheme this[System.ConsoleColor background]
        {
            get
            {
                ConsoleColorTheme theme = new ConsoleColorTheme();
                
                //int num = (int) (background * ((System.ConsoleColor) 0x20));
                int num = (int) (((int)background) * 0x20);
                for (int i = 0; i < 0x20; i++)
                {
                    theme.Mappings[i] = (ConsoleColorExt) this.m_RawBytes[num++];
                }
                return theme;
            }
            set
            {
                //int num = (int) (background * ((System.ConsoleColor) 0x20));
                int num = (int) (((int)background) * 0x20);
                for (int i = 0; i < 0x20; i++)
                {
                    this.m_RawBytes[num++] = (byte) value.Mappings[i];
                }
            }
        }
    }
}

