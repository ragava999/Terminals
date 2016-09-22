using System;
using System.Collections;
using System.Windows.Forms;

namespace WalburySoftware
{
    public partial class TerminalEmulator : Control
    {
        private class uc_Keyboard
        {
            private readonly uc_KeyMap KeyMap = new uc_KeyMap();
            private readonly TerminalEmulator Parent;
            private bool AltIsDown;
            private bool CtrlIsDown;
            private Boolean LastKeyDownSent; // next WM_CHAR ignored if true 
            private bool ShiftIsDown;

            public uc_Keyboard(TerminalEmulator p1)
            {
                this.Parent = p1;
            }

            public bool UpArrow { get; set; }
            public bool DownArrow { get; set; }
            public bool LeftArrow { get; set; }
            public bool RightArrow { get; set; }
            public event KeyboardEventHandler KeyboardEvent;

            public void KeyDown(Message KeyMess)
            {
                Byte[] lBytes;
                Byte[] wBytes;
                UInt16 KeyValue = 0;
                UInt16 RepeatCount = 0;
                Byte ScanCode = 0;
                Byte AnsiChar = 0;
                UInt16 UniChar = 0;
                Byte Flags = 0;

                lBytes = BitConverter.GetBytes(KeyMess.LParam.ToInt64());
                wBytes = BitConverter.GetBytes(KeyMess.WParam.ToInt64());
                RepeatCount = BitConverter.ToUInt16(lBytes, 0);
                ScanCode = lBytes[2];
                Flags = lBytes[3];

                // key down messages send the scan code in wParam whereas
                // key press messages send the char and unicode values in this word
                if (KeyMess.Msg == WMCodes.WM_SYSKEYDOWN ||
                    KeyMess.Msg == WMCodes.WM_KEYDOWN)
                {
                    KeyValue = BitConverter.ToUInt16(wBytes, 0);

                    // set the key down flags. I know you can get alt from lparam flags
                    // but this feels more consistent
                    switch (KeyValue)
                    {
                        case 16: // Shift Keys
                        case 160: // L Shift Key
                        case 161: // R Shift Keys
                            this.ShiftIsDown = true;
                            return;

                        case 17: // Ctrl Keys
                        case 162: // L Ctrl Key
                        case 163: // R Ctrl Key
                            this.CtrlIsDown = true;
                            return;

                        case 18: // Alt Keys (Menu)
                        case 164: // L Alt Key
                        case 165: // R Ctrl Key
                            this.AltIsDown = true;
                            return;

                        case 38: //up arrow
                            this.UpArrow = true;
                            break;
                        case 40: //down arrow
                            this.DownArrow = true;
                            break;
                        case 37: //left arrow
                            this.LeftArrow = true;
                            break;
                        case 39: //right arrow
                            this.RightArrow = true;
                            break;

                        case 45: // Shift-Insert for pasting
                            if (this.ShiftIsDown)
                            {
                                this.KeyboardEvent(this, "Paste");
                                this.LastKeyDownSent = true;
                                return;
                            }
                            break;
                        case 86: // Ctrl-V for pasting
                            if (this.CtrlIsDown)
                            {
                                this.KeyboardEvent(this, "Paste");
                                this.LastKeyDownSent = true;
                                return;
                            }
                            break;

                        default:
                            break;
                    }

                    String Modifier = "Key";

                    if (this.ShiftIsDown)
                    {
                        Modifier = "Shift";
                    }
                    else if (this.CtrlIsDown)
                    {
                        Modifier = "Ctrl";
                    }
                    else if (this.AltIsDown)
                    {
                        Modifier = "Alt";
                    }

                    // the key pressed was not a modifier so check for an override string
                    String OutString = this.KeyMap.Find(ScanCode, Convert.ToBoolean(Flags & 0x01), Modifier,
                                                        this.Parent.Modes.Flags);

                    this.LastKeyDownSent = false;

                    if (OutString != String.Empty)
                    {
                        // Flag the event so that the associated WM_CHAR event (if any) is ignored
                        this.LastKeyDownSent = true;

                        this.KeyboardEvent(this, OutString);
                    }
                }
                else if (KeyMess.Msg == WMCodes.WM_SYSKEYUP ||
                         KeyMess.Msg == WMCodes.WM_KEYUP)
                {
                    KeyValue = BitConverter.ToUInt16(wBytes, 0);

                    switch (KeyValue)
                    {
                        case 16: // Shift Keys
                        case 160: // L Shift Key
                        case 161: // R Shift Keys
                            this.ShiftIsDown = false;
                            break;

                        case 17: // Ctrl Keys
                        case 162: // L Ctrl Key
                        case 163: // R Ctrl Key
                            this.CtrlIsDown = false;
                            break;

                        case 18: // Alt Keys (Menu)
                        case 164: // L Alt Key
                        case 165: // R Ctrl Key
                            this.AltIsDown = false;
                            break;

                        case 38: //up arrow
                            this.UpArrow = false;
                            break;
                        case 40: //down arrow
                            this.DownArrow = false;
                            break;
                        case 37: //left arrow
                            this.LeftArrow = false;
                            break;
                        case 39: //right arrow
                            this.RightArrow = false;
                            break;

                        default:
                            break;
                    }
                }

                else if (KeyMess.Msg == WMCodes.WM_SYSCHAR ||
                         KeyMess.Msg == WMCodes.WM_CHAR)
                {
                    AnsiChar = wBytes[0];
                    UniChar = BitConverter.ToUInt16(wBytes, 0);

                    // if there's a string mapped to this key combo we want to ignore the character
                    // as it has been overriden in the keydown event

                    // only send the windows generated char if there was no custom
                    // string sent by the keydown event
                    if (this.LastKeyDownSent == false)
                    {
                        // send the character straight to the host if we haven't already handled the actual key press
                        this.KeyboardEvent(this, Convert.ToChar(AnsiChar).ToString());
                    }
                }

                ////Console.Write ("AnsiChar = {0} Result = {1} ScanCode = {2} KeyValue = {3} Flags = {4} Repeat = {5}\n ", 
                ////AnsiChar, KeyMess.Result, ScanCode, KeyValue, Flags, RepeatCount);
            }

            private class uc_KeyInfo
            {
                public readonly Boolean ExtendFlag;
                public readonly UInt32 Flag;
                public readonly UInt32 FlagValue;
                public readonly String Modifier;
                public readonly String OutString;
                public readonly UInt16 ScanCode;

                public uc_KeyInfo(
                    UInt16 p1,
                    Boolean p2,
                    String p3,
                    String p4,
                    UInt32 p5,
                    UInt32 p6)
                {
                    this.ScanCode = p1;
                    this.ExtendFlag = p2;
                    this.Modifier = p3;
                    this.OutString = p4;
                    this.Flag = p5;
                    this.FlagValue = p6;
                }
            }

            private class uc_KeyMap
            {
                public readonly ArrayList Elements = new ArrayList();

                public uc_KeyMap()
                {
                    this.SetToDefault();
                }

                // set the key mapping up to emulate most keys on a vt420
                public void SetToDefault()
                {
                    // add the default key mappings here
                    this.Elements.Clear();

                    // TOPNZ Customisations these should be commented out
                    //Elements.Add (new uc_KeyInfo (15,  false, "Shift", "\x1B[OI~", uc_Mode.Any,       0)); //ShTab
                    //Elements.Add (new uc_KeyInfo (63,  false, "Key",   "\x1BOT",   uc_Mode.Any,       0)); //F5
                    //Elements.Add (new uc_KeyInfo (64,  false, "Key",   "\x1BOU",   uc_Mode.Any,       0)); //F6
                    //Elements.Add (new uc_KeyInfo (65,  false, "Key",   "\x1BOV",   uc_Mode.Any,       0)); //F7
                    //Elements.Add (new uc_KeyInfo (66,  false, "Key",   "\x1BOW",   uc_Mode.Any,       0)); //F8
                    //Elements.Add (new uc_KeyInfo (67,  false, "Key",   "\x1BOX",   uc_Mode.Any,       0)); //F9
                    //Elements.Add (new uc_KeyInfo (68,  false, "Key",   "\x1BOY",   uc_Mode.Any,       0)); //F10
                    //Elements.Add (new uc_KeyInfo (59,  false, "Shift", "\x1B[25~", uc_Mode.Any,       0)); //ShF1
                    //Elements.Add (new uc_KeyInfo (60,  false, "Shift", "\x1B[26~", uc_Mode.Any,       0)); //ShF2
                    //Elements.Add (new uc_KeyInfo (61,  false, "Shift", "\x1B[28~", uc_Mode.Any,       0)); //ShF3
                    //Elements.Add (new uc_KeyInfo (62,  false, "Shift", "\x1B[29~", uc_Mode.Any,       0)); //ShF4
                    //Elements.Add (new uc_KeyInfo (63,  false, "Shift", "\x1B[31~", uc_Mode.Any,       0)); //ShF5
                    //Elements.Add (new uc_KeyInfo (64,  false, "Shift", "\x1B[32~", uc_Mode.Any,       0)); //ShF6
                    //Elements.Add (new uc_KeyInfo (65,  false, "Shift", "\x1B[33~", uc_Mode.Any,       0)); //ShF7
                    //Elements.Add (new uc_KeyInfo (66,  false, "Shift", "\x1B[34~", uc_Mode.Any,       0)); //ShF8
                    //Elements.Add (new uc_KeyInfo (67,  false, "Shift", "\x1B[36~", uc_Mode.Any,       0)); //ShF9
                    //Elements.Add (new uc_KeyInfo (68,  false, "Shift", "\x1B[37~", uc_Mode.Any,       0)); //ShF10
                    //Elements.Add (new uc_KeyInfo (87,  false, "Shift", "\x1B[38~", uc_Mode.Any,       0)); //ShF11
                    //Elements.Add (new uc_KeyInfo (88,  false, "Shift", "\x1B[39~", uc_Mode.Any,       0)); //ShF12


                    // this is the initial list of keyboard codes
                    this.Elements.Add(new uc_KeyInfo(15, false, "Shift", "\x1B[Z", uc_Mode.Any, 0)); //ShTab
                    this.Elements.Add(new uc_KeyInfo(28, false, "Key", "\x0D", uc_Mode.Any, 0)); //Return
                    this.Elements.Add(new uc_KeyInfo(59, false, "Key", "\x1BOP", uc_Mode.Any, 0)); //F1->PF1
                    this.Elements.Add(new uc_KeyInfo(60, false, "Key", "\x1BOQ", uc_Mode.Any, 0)); //F2->PF2
                    this.Elements.Add(new uc_KeyInfo(61, false, "Key", "\x1BOR", uc_Mode.Any, 0)); //F3->PF3
                    this.Elements.Add(new uc_KeyInfo(62, false, "Key", "\x1BOS", uc_Mode.Any, 0)); //F4->PF4
                    this.Elements.Add(new uc_KeyInfo(63, false, "Key", "\x1B[15~", uc_Mode.Any, 0)); //F5
                    this.Elements.Add(new uc_KeyInfo(64, false, "Key", "\x1B[17~", uc_Mode.Any, 0)); //F6
                    this.Elements.Add(new uc_KeyInfo(65, false, "Key", "\x1B[18~", uc_Mode.Any, 0)); //F7
                    this.Elements.Add(new uc_KeyInfo(66, false, "Key", "\x1B[19~", uc_Mode.Any, 0)); //F8
                    this.Elements.Add(new uc_KeyInfo(67, false, "Key", "\x1B[20~", uc_Mode.Any, 0)); //F9
                    this.Elements.Add(new uc_KeyInfo(68, false, "Key", "\x1B[21~", uc_Mode.Any, 0)); //F10
                    this.Elements.Add(new uc_KeyInfo(87, false, "Key", "\x1B[23~", uc_Mode.Any, 0)); //F11
                    this.Elements.Add(new uc_KeyInfo(88, false, "Key", "\x1B[24~", uc_Mode.Any, 0)); //F12
                    this.Elements.Add(new uc_KeyInfo(61, false, "Shift", "\x1B[25~", uc_Mode.Any, 0)); //ShF3 ->F13
                    this.Elements.Add(new uc_KeyInfo(62, false, "Shift", "\x1B[26~", uc_Mode.Any, 0)); //ShF4 ->F14
                    this.Elements.Add(new uc_KeyInfo(63, false, "Shift", "\x1B[28~", uc_Mode.Any, 0)); //ShF5 ->F15
                    this.Elements.Add(new uc_KeyInfo(64, false, "Shift", "\x1B[29~", uc_Mode.Any, 0)); //ShF6 ->F16
                    this.Elements.Add(new uc_KeyInfo(65, false, "Shift", "\x1B[31~", uc_Mode.Any, 0)); //ShF7 ->F17
                    this.Elements.Add(new uc_KeyInfo(66, false, "Shift", "\x1B[32~", uc_Mode.Any, 0)); //ShF8 ->F18
                    this.Elements.Add(new uc_KeyInfo(67, false, "Shift", "\x1B[33~", uc_Mode.Any, 0)); //ShF9 ->F19
                    this.Elements.Add(new uc_KeyInfo(68, false, "Shift", "\x1B[34~", uc_Mode.Any, 0)); //ShF10->F20
                    this.Elements.Add(new uc_KeyInfo(87, false, "Shift", "\x1B[28~", uc_Mode.Any, 0)); //ShF11->Help
                    this.Elements.Add(new uc_KeyInfo(88, false, "Shift", "\x1B[29~", uc_Mode.Any, 0)); //ShF12->Do
                    this.Elements.Add(new uc_KeyInfo(71, true, "Key", "\x1B[1~", uc_Mode.Any, 0)); //Home
                    this.Elements.Add(new uc_KeyInfo(82, true, "Key", "\x1B[2~", uc_Mode.Any, 0)); //Insert
                    this.Elements.Add(new uc_KeyInfo(83, true, "Key", "\x1B[3~", uc_Mode.Any, 0)); //Delete
                    this.Elements.Add(new uc_KeyInfo(79, true, "Key", "\x1B[4~", uc_Mode.Any, 0)); //End
                    this.Elements.Add(new uc_KeyInfo(73, true, "Key", "\x1B[5~", uc_Mode.Any, 0)); //PageUp
                    this.Elements.Add(new uc_KeyInfo(81, true, "Key", "\x1B[6~", uc_Mode.Any, 0)); //PageDown
                    this.Elements.Add(new uc_KeyInfo(72, true, "Key", "\x1B[A", uc_Mode.CursorAppln, 0)); //CursorUp
                    this.Elements.Add(new uc_KeyInfo(80, true, "Key", "\x1B[B", uc_Mode.CursorAppln, 0)); //CursorDown
                    this.Elements.Add(new uc_KeyInfo(77, true, "Key", "\x1B[C", uc_Mode.CursorAppln, 0));
                    //CursorKeyRight
                    this.Elements.Add(new uc_KeyInfo(75, true, "Key", "\x1B[D", uc_Mode.CursorAppln, 0));
                    //CursorKeyLeft
                    this.Elements.Add(new uc_KeyInfo(72, true, "Key", "\x1BOA", uc_Mode.CursorAppln, 1)); //CursorUp
                    this.Elements.Add(new uc_KeyInfo(80, true, "Key", "\x1BOB", uc_Mode.CursorAppln, 1)); //CursorDown
                    this.Elements.Add(new uc_KeyInfo(77, true, "Key", "\x1BOC", uc_Mode.CursorAppln, 1));
                    //CursorKeyRight
                    this.Elements.Add(new uc_KeyInfo(75, true, "Key", "\x1BOD", uc_Mode.CursorAppln, 1));
                    //CursorKeyLeft
                    this.Elements.Add(new uc_KeyInfo(82, false, "Key", "\x1BOp", uc_Mode.KeypadAppln, 1)); //Keypad0
                    this.Elements.Add(new uc_KeyInfo(79, false, "Key", "\x1BOq", uc_Mode.KeypadAppln, 1)); //Keypad1
                    this.Elements.Add(new uc_KeyInfo(80, false, "Key", "\x1BOr", uc_Mode.KeypadAppln, 1)); //Keypad2
                    this.Elements.Add(new uc_KeyInfo(81, false, "Key", "\x1BOs", uc_Mode.KeypadAppln, 1)); //Keypad3
                    this.Elements.Add(new uc_KeyInfo(75, false, "Key", "\x1BOt", uc_Mode.KeypadAppln, 1)); //Keypad4
                    this.Elements.Add(new uc_KeyInfo(76, false, "Key", "\x1BOu", uc_Mode.KeypadAppln, 1)); //Keypad5
                    this.Elements.Add(new uc_KeyInfo(77, false, "Key", "\x1BOv", uc_Mode.KeypadAppln, 1)); //Keypad6
                    this.Elements.Add(new uc_KeyInfo(71, false, "Key", "\x1BOw", uc_Mode.KeypadAppln, 1)); //Keypad7
                    this.Elements.Add(new uc_KeyInfo(72, false, "Key", "\x1BOx", uc_Mode.KeypadAppln, 1)); //Keypad8
                    this.Elements.Add(new uc_KeyInfo(73, false, "Key", "\x1BOy", uc_Mode.KeypadAppln, 1)); //Keypad9
                    this.Elements.Add(new uc_KeyInfo(74, false, "Key", "\x1BOm", uc_Mode.KeypadAppln, 1)); //Keypad-
                    this.Elements.Add(new uc_KeyInfo(78, false, "Key", "\x1BOl", uc_Mode.KeypadAppln, 1));
                    //Keypad+ (use instead of comma)
                    this.Elements.Add(new uc_KeyInfo(83, false, "Key", "\x1BOn", uc_Mode.KeypadAppln, 1)); //Keypad.
                    this.Elements.Add(new uc_KeyInfo(28, true, "Key", "\x1BOM", uc_Mode.KeypadAppln, 1)); //Keypad Enter
                    this.Elements.Add(new uc_KeyInfo(03, false, "Ctrl", "\x00", uc_Mode.Any, 0)); //Ctrl2->Null
                    this.Elements.Add(new uc_KeyInfo(57, false, "Ctrl", "\x00", uc_Mode.Any, 0)); //CtrlSpaceBar->Null
                    this.Elements.Add(new uc_KeyInfo(04, false, "Ctrl", "\x1B", uc_Mode.Any, 0)); //Ctrl3->Escape
                    this.Elements.Add(new uc_KeyInfo(05, false, "Ctrl", "\x1C", uc_Mode.Any, 0)); //Ctrl4->FS
                    this.Elements.Add(new uc_KeyInfo(06, false, "Ctrl", "\x1D", uc_Mode.Any, 0)); //Ctrl5->GS
                    this.Elements.Add(new uc_KeyInfo(07, false, "Ctrl", "\x1E", uc_Mode.Any, 0)); //Ctrl6->RS
                    this.Elements.Add(new uc_KeyInfo(08, false, "Ctrl", "\x1F", uc_Mode.Any, 0)); //Ctrl7->US
                    this.Elements.Add(new uc_KeyInfo(09, false, "Ctrl", "\x7F", uc_Mode.Any, 0)); //Ctrl8->DEL
                }

                public String Find(
                    UInt16 ScanCode,
                    Boolean ExtendFlag,
                    String Modifier,
                    UInt32 ModeFlags)
                {
                    String OutString = String.Empty;

                    for (Int32 i = 0; i < this.Elements.Count; i++)
                    {
                        uc_KeyInfo Element = (uc_KeyInfo)this.Elements[i];

                        if (Element.ScanCode == ScanCode &&
                            Element.ExtendFlag == ExtendFlag &&
                            Element.Modifier == Modifier &&
                            (Element.Flag == uc_Mode.Any ||
                             ((Element.Flag & ModeFlags) == Element.Flag && Element.FlagValue == 1) ||
                             ((Element.Flag & ModeFlags) == 0 && Element.FlagValue == 0)))
                        {
                            OutString = Element.OutString;
                            return OutString;
                        }
                    }

                    return OutString;
                }
            }
        }

        private class uc_Mode
        {
            public static UInt32 Locked = 1; // Unlocked           = off 
            public static UInt32 BackSpace = 2; // Delete             = off 
            public static UInt32 NewLine = 4; // Line Feed          = off 
            public static UInt32 Repeat = 8; // No Repeat          = off 
            public static UInt32 AutoWrap = 16; // No AutoWrap        = off 
            public static UInt32 CursorAppln = 32; // Std Cursor Codes   = off 
            public static UInt32 KeypadAppln = 64; // Std Numeric Codes  = off 
            public static UInt32 DataProcessing = 128; // Typewriter         = off 
            public static UInt32 PositionReports = 256; // CharacterCodes     = off
            public static UInt32 LocalEchoOff = 512; // LocalEchoOn        = off
            public static UInt32 OriginRelative = 1024; // OriginAbsolute     = off
            public static UInt32 LightBackground = 2048; // DarkBackground     = off
            public static UInt32 National = 4096; // Multinational      = off
            public static UInt32 Any = 0x80000000; // Any Flags

            public UInt32 Flags;

            public uc_Mode(UInt32 InitialFlags)
            {
                this.Flags = InitialFlags;
            }

            public uc_Mode()
            {
                this.Flags = 0;
            }
        }
    }
}