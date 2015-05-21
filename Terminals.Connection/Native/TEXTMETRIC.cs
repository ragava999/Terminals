namespace Terminals.Connection.Native
{
    // .NET namespaces
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The TEXTMETRIC structure holds the text metrics of a font.
    /// The structure describes many physical attributes of the font.
    /// Unless otherwise specified, all size measurements of the font
    /// stored in the structure are in the logical units of the device
    /// the font is being used on, so the same font may have a different
    /// "scale" if used on a different device.
    /// </summary>
    /// <remarks>
    ///     Const FW_DONTCARE = 0
    ///     Const FW_THIN = 100
    ///     Const FW_EXTRALIGHT = 200
    ///     Const FW_ULTRALIGHT = 200
    ///     Const FW_LIGHT = 300
    ///     Const FW_NORMAL = 400
    ///     Const FW_REGULAR = 400
    ///     Const FW_MEDIUM = 500
    ///     Const FW_SEMIBOLD = 600
    ///     Const FW_DEMIBOLD = 600
    ///     Const FW_BOLD = 700
    ///     Const FW_EXTRABOLD = 800
    ///     Const FW_ULTRABOLD = 800
    ///     Const FW_HEAVY = 900
    ///     Const FW_BLACK = 900
    ///     Const DEFAULT_PITCH = 0
    ///     Const FIXED_PITCH = 1
    ///     Const VARIABLE_PITCH = 2
    ///     Const FF_DECORATIVE = 80
    ///     Const FF_DONTCARE = 0
    ///     Const FF_ROMAN = 16
    ///     Const FF_SCRIPT = 64
    ///     Const FF_SWISS = 32
    ///     Const ANSI_CHARSET = 0
    ///     Const ARABIC_CHARSET = 178
    ///     Const BALTIC_CHARSET = 186
    ///     Const CHINESEBIG5_CHARSET = 136
    ///     Const DEFAULT_CHARSET = 1
    ///     Const EASTEUROPE_CHARSET = 238
    ///     Const GB2312_CHARSET = 134
    ///     Const GREEK_CHARSET = 161
    ///     Const HANGEUL_CHARSET = 129
    ///     Const HEBREW_CHARSET = 177
    ///     Const JOHAB_CHARSET = 130
    ///     Const MAC_CHARSET = 77
    ///     Const OEM_CHARSET = 255
    ///     Const RUSSIAN_CHARSET = 204
    ///     Const SHIFTJIS_CHARSET = 128
    ///     Const SYMBOL_CHARSET = 2
    ///     Const THAI_CHARSET = 222
    ///     Const TURKISH_CHARSET = 162
    /// </remarks>
    [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TextMetric
    {
        /// <summary>
        /// The height of the font's characters (equal to the sum of tmAscent and tmDescent). 
        /// </summary>
        public int tmHeight;

        /// <summary>
        /// The ascent (units above the base line) of the font's characters. 
        /// </summary>
        public int tmAscent;

        /// <summary>
        /// The descent (units below the base line) of the font's characters. 
        /// </summary>
        public int tmDescent;

        /// <summary>
        /// The amount of leading space inside the bounds of tmHeight where accent marks or other diacritical marks may appear. 
        /// </summary>
        public int tmInternalLeading;

        /// <summary>
        /// The amount of leading space placed between rows of text in which the font does not draw. 
        /// </summary>
        public int tmExternalLeading;

        /// <summary>
        /// The average width of the font's characters, usually identified as the width of the "x" character. This value does not include extra space required for bold or italic characters. 
        /// </summary>
        public int tmAveCharWidth;

        /// <summary>
        /// The width of the widest character of the font. This value does not include extra space required for bold or italics. 
        /// </summary>
        public int tmMaxCharWidth;

        /// <summary>
        /// One of the following flags specifying the boldness (weight) of the font:
        /// 
        ///     FW_DONTCARE             Default weight.
        ///     FW_THIN                 Thin weight.
        ///     FW_EXTRALIGHT           Extra-light weight.
        ///     FW_ULTRALIGHT           Same as FW_EXTRALIGHT.
        ///     FW_LIGHT                Light weight.
        ///     FW_NORMAL               Normal weight.
        ///     FW_REGULAR              Same as FW_NORMAL.
        ///     FW_MEDIUM               Medium weight.
        ///     FW_SEMIBOLD             Semi-bold weight.
        ///     FW_DEMIBOLD             Same As FW_SEMIBOLD.
        ///     FW_BOLD                 Bold weight.
        ///     FW_EXTRABOLD            Extra-bold weight.
        ///     FW_ULTRABOLD            Same as FW_EXTRABOLD.
        ///     FW_HEAVY                Heavy weight.
        ///     FW_BLACK                Same as FW_HEAVY. 
        /// 
        /// </summary>
        public int tmWeight;

        /// <summary>
        /// The extra width per string added to the font when synthesizing attributes such as boldface and italics. For a boldface effect, this is the distance by which the overstrike is offset. For an italics effect, this is the amount that the top of the character is sheared past the bottom of the character. 
        /// </summary>
        public int tmOverhang;

        /// <summary>
        /// The horizontal aspect of the device for which the font was designed. 
        /// </summary>
        public int tmDigitizedAspectX;

        /// <summary>
        /// The vertical aspect of the device for which the font was designed. 
        /// </summary>
        public int tmDigitizedAspectY;

        /// <summary>
        /// The value of the first character defined in the font. 
        /// </summary>
        public char tmFirstChar;

        /// <summary>
        /// The value of the last character defined in the font. 
        /// </summary>
        public char tmLastChar;

        /// <summary>
        /// The value of the character to substitute for characters not present in the font. 
        /// </summary>
        public char tmDefaultChar;

        /// <summary>
        /// The value of the character to be used for work breaks for text justification. 
        /// </summary>
        public char tmBreakChar;

        /// <summary>
        /// If zero, the font is not an italic font. If a non-zero value, the font is an italic font. 
        /// </summary>
        public byte tmItalic;

        /// <summary>
        /// If zero, the font is not an underlined font. If a non-zero value, the font is an underlined font. 
        /// </summary>
        public byte tmUnderlined;

        /// <summary>
        /// If zero, the font is not a strikeout font. If a non-zero value, the font is a strikeout font. 
        /// </summary>
        public byte tmStruckOut;

        /// <summary>
        /// A bitwise OR combination of exactly one *_PITCH flag specifying the pitch of the font
        /// and exactly one FF_* flag specifying the font face family of the font:
        /// 
        ///     DEFAULT_PITCH           The default pitch.
        ///     FIXED_PITCH             Fixed pitch.
        ///     VARIABLE_PITCH          Variable pitch.
        ///     FF_DECORATIVE           Showy, decorative font face.
        ///     FF_DONTCARE             Do not care about the font face.
        ///     FF_MODERN               Modern font face (monospaced, sans serif font).
        ///     FF_ROMAN                Roman font face (proportional-width, serif font).
        ///     FF_SCRIPT               Script font face which imitates script handwriting.
        ///     FF_SWISS                Swiss font face (proportional-width, sans serif font). 
        /// 
        /// </summary>

        public byte tmPitchAndFamily;


        /// <summary>
        /// One of the following flags identifying the character set of the font:
        /// 
        ///     ANSI_CHARSET            ANSI character set.
        ///     ARABIC_CHARSET          Windows NT, 2000: Arabic character set.
        ///     BALTIC_CHARSET          Windows 95, 98: Baltic character set.
        ///     CHINESEBIG5_CHARSET     Chinese Big 5 character set.
        ///     DEFAULT_CHARSET         Default character set.
        ///     EASTEUROPE_CHARSET      Windows 95, 98: Eastern European character set.
        ///     GB2312_CHARSET          GB2312 character set.
        ///     GREEK_CHARSET           Windows 95, 98: Greek character set.
        ///     HANGEUL_CHARSET         HANDEUL character set.
        ///     HEBREW_CHARSET          Windows NT, 2000: Hebrew character set.
        ///     JOHAB_CHARSET           Windows 95, 98: Johab character set.
        ///     MAC_CHARSET             Windows 95, 98: Mac character set.
        ///     OEM_CHARSET             Original equipment manufacturer (OEM) character set.
        ///     RUSSIAN_CHARSET         Windows 95, 98: Russian character set.
        ///     SHIFTJIS_CHARSET        ShiftJis character set.
        ///     SYMBOL_CHARSET          Symbol character set.
        ///     THAI_CHARSET            Windows NT, 2000: Thai character set.
        ///     TURKISH_CHARSET         Windows 95, 98: Turkish character set. 
        /// </summary>
        public byte tmCharSet;
    }
}