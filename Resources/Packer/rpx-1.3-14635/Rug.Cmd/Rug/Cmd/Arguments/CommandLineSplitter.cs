namespace Rug.Cmd.Arguments
{
    using System;

    public class CommandLineSplitter
    {
        public static string[] SplitArguments(string commandLine)
        {
            char[] chArray = commandLine.ToCharArray();
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < chArray.Length; i++)
            {
                if (chArray[i] == '"')
                {
                    if (!flag2)
                    {
                        flag = !flag;
                        chArray[i] = '\n';
                    }
                    else
                    {
                        flag2 = true;
                    }
                }
                else if ((chArray[i] == '\\') && flag)
                {
                    flag2 = true;
                }
                else if (!flag && (chArray[i] == ' '))
                {
                    chArray[i] = '\n';
                    flag2 = false;
                }
                else
                {
                    flag2 = false;
                }
            }
            return new string(chArray).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}

