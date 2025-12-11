using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// Keys that may be present on a <see cref="IKeyboard"/> device.
/// </summary>
/// <remarks>This list of keys is just copied from SDL3. If you're implementing a non-SDL backend, well good luck.</remarks>
[PublicAPI]
public enum Keys
{
    /// <summary>
    ///  No key pressed.
    /// </summary>
    None = 0x00,
    /// <summary>
    /// <c>\b</c>
    /// </summary>
    Backspace = 8,
    /// <summary>
    /// <c>\t</c>
    /// </summary>
    Tab = 9,
    /// <summary>
    /// <c>\r</c>
    /// </summary>
    Return = 13, // 0x0000000D
    /// <summary>
    /// <c>\x1B</c>
    /// </summary>
    Escape = 27, // 0x0000001B
    /// <summary>' '</summary>
    Space = 32, // 0x00000020
    /// <summary>
    /// <c>!</c>
    /// </summary>
    Exclaim = 33, // 0x00000021
    /// <summary>
    /// <c>"</c>
    /// </summary>
    DblApostrophe = 34, // 0x00000022
    /// <summary>
    /// <c>#</c>
    /// </summary>
    Hash = 35, // 0x00000023
    /// <summary>
    /// <c>$</c>
    /// </summary>
    Dollar = 36, // 0x00000024
    /// <summary>
    /// <c>%</c>
    /// </summary>
    Percent = 37, // 0x00000025
    /// <summary>
    /// <c>&amp;</c>
    /// </summary>
    Ampersand = 38, // 0x00000026
    /// <summary>
    /// <c>\</c>
    /// </summary>
    Apostrophe = 39, // 0x00000027
    /// <summary>
    /// <c>(</c>
    /// </summary>
    LeftParen = 40, // 0x00000028
    /// <summary>
    /// <c>)</c>
    /// </summary>
    RightParen = 41, // 0x00000029
    /// <summary>
    /// <c>*</c>
    /// </summary>
    Asterisk = 42, // 0x0000002A
    /// <summary>
    /// <c>+</c>
    /// </summary>
    Plus = 43, // 0x0000002B
    /// <summary>
    /// <c>,</c>
    /// </summary>
    Comma = 44, // 0x0000002C
    /// <summary>
    /// <c>-</c>
    /// </summary>
    Minus = 45, // 0x0000002D
    /// <summary>
    /// <c>.</c>
    /// </summary>
    Period = 46, // 0x0000002E
    /// <summary>
    /// <c>/</c>
    /// </summary>
    Slash = 47, // 0x0000002F
    /// <summary>
    /// <c>0</c>
    /// </summary>
    Alpha0 = 48, // 0x00000030
    /// <summary>
    /// <c>1</c>
    /// </summary>
    Alpha1 = 49, // 0x00000031
    /// <summary>
    /// <c>2</c>
    /// </summary>
    Alpha2 = 50, // 0x00000032
    /// <summary>
    /// <c>3</c>
    /// </summary>
    Alpha3 = 51, // 0x00000033
    /// <summary>
    /// <c>4</c>
    /// </summary>
    Alpha4 = 52, // 0x00000034
    /// <summary>
    /// <c>5</c>
    /// </summary>
    Alpha5 = 53, // 0x00000035
    /// <summary>
    /// <c>6</c>
    /// </summary>
    Alpha6 = 54, // 0x00000036
    /// <summary>
    /// <c>7</c>
    /// </summary>
    Alpha7 = 55, // 0x00000037
    /// <summary>
    /// <c>8</c>
    /// </summary>
    Alpha8 = 56, // 0x00000038
    /// <summary>
    /// <c>9</c>
    /// </summary>
    Alpha9 = 57, // 0x00000039
    /// <summary>
    /// <c>:</c>
    /// </summary>
    Colon = 58, // 0x0000003A
    /// <summary>
    /// <c>;</c>
    /// </summary>
    Semicolon = 59, // 0x0000003B
    /// <summary>
    /// <c>&lt;</c>
    /// </summary>
    Less = 60, // 0x0000003C
    /// <summary>
    /// <c>=</c>
    /// </summary>
    Equals = 61, // 0x0000003D
    /// <summary>
    /// <c>&gt;</c>
    /// </summary>
    Greater = 62, // 0x0000003E
    /// <summary>
    /// <c>?</c>
    /// </summary>
    Question = 63, // 0x0000003F
    /// <summary>
    /// <c>@</c>
    /// </summary>
    At = 64, // 0x00000040
    /// <summary>
    /// <c>[</c>
    /// </summary>
    LeftBracket = 91, // 0x0000005B
    /// <summary>
    /// <c>\</c>
    /// </summary>
    Backslash = 92, // 0x0000005C
    /// <summary>
    /// <c>]</c>
    /// </summary>
    RightBracket = 93, // 0x0000005D
    /// <summary>
    /// <c>^</c>
    /// </summary>
    Caret = 94, // 0x0000005E
    /// <summary>
    /// <c>_</c>
    /// </summary>
    Underscore = 95, // 0x0000005F
    /// <summary>
    /// <c>`</c>
    /// </summary>
    Grave = 96, // 0x00000060
    /// <summary>
    /// <c>a</c>
    /// </summary>
    A = 97, // 0x00000061
    /// <summary>
    /// <c>b</c>
    /// </summary>
    B = 98, // 0x00000062
    /// <summary>
    /// <c>c</c>
    /// </summary>
    C = 99, // 0x00000063
    /// <summary>
    /// <c>d</c>
    /// </summary>
    D = 100, // 0x00000064
    /// <summary>
    /// <c>e</c>
    /// </summary>
    E = 101, // 0x00000065
    /// <summary>
    /// <c>f</c>
    /// </summary>
    F = 102, // 0x00000066
    /// <summary>
    /// <c>g</c>
    /// </summary>
    G = 103, // 0x00000067
    /// <summary>
    /// <c>h</c>
    /// </summary>
    H = 104, // 0x00000068
    /// <summary>
    /// <c>i</c>
    /// </summary>
    I = 105, // 0x00000069
    /// <summary>
    /// <c>j</c>
    /// </summary>
    J = 106, // 0x0000006A
    /// <summary>
    /// <c>k</c>
    /// </summary>
    K = 107, // 0x0000006B
    /// <summary>
    /// <c>l</c>
    /// </summary>
    L = 108, // 0x0000006C
    /// <summary>
    /// <c>m</c>
    /// </summary>
    M = 109, // 0x0000006D
    /// <summary>
    /// <c>n</c>
    /// </summary>
    N = 110, // 0x0000006E
    /// <summary>
    /// <c>o</c>
    /// </summary>
    O = 111, // 0x0000006F
    /// <summary>
    /// <c>p</c>
    /// </summary>
    P = 112, // 0x00000070
    /// <summary>
    /// <c>q</c>
    /// </summary>
    Q = 113, // 0x00000071
    /// <summary>
    /// <c>r</c>
    /// </summary>
    R = 114, // 0x00000072
    /// <summary>
    /// <c>s</c>
    /// </summary>
    S = 115, // 0x00000073
    /// <summary>
    /// <c>t</c>
    /// </summary>
    T = 116, // 0x00000074
    /// <summary>
    /// <c>u</c>
    /// </summary>
    U = 117, // 0x00000075
    /// <summary>
    /// <c>v</c>
    /// </summary>
    V = 118, // 0x00000076
    /// <summary>
    /// <c>w</c>
    /// </summary>
    W = 119, // 0x00000077
    /// <summary>
    /// <c>x</c>
    /// </summary>
    X = 120, // 0x00000078
    /// <summary>
    /// <c>y</c>
    /// </summary>
    Y = 121, // 0x00000079
    /// <summary>
    /// <c>z</c>
    /// </summary>
    Z = 122, // 0x0000007A
    /// <summary>
    /// <c>{</c>
    /// </summary>
    LeftBrace = 123, // 0x0000007B
    /// <summary>
    /// <c>|</c>
    /// </summary>
    Pipe = 124, // 0x0000007C
    /// <summary>
    /// <c>}</c>
    /// </summary>
    RightBrace = 125, // 0x0000007D
    /// <summary>
    /// <c>~</c>
    /// </summary>
    Tilde = 126, // 0x0000007E
    /// <summary>
    /// <c>\x7F</c>
    /// </summary>
    Delete = 127, // 0x0000007F
    /// <summary>
    /// <c>±</c>
    /// </summary>
    PlusMinus = 177, // 0x000000B1
    /// <summary>Extended key Left Tab</summary>
    LeftTab = 536870913, // 0x20000001
    /// <summary>Extended key Level 5 Shift</summary>
    Level5Shift = 536870914, // 0x20000002
    /// <summary>Extended key Multi-key Compose</summary>
    MultiKeyCompose = 536870915, // 0x20000003
    /// <summary>Extended key Left Meta</summary>
    LMeta = 536870916, // 0x20000004
    /// <summary>Extended key Right Meta</summary>
    RMeta = 536870917, // 0x20000005
    /// <summary>Extended key Left Hyper</summary>
    LHyper = 536870918, // 0x20000006
    /// <summary>Extended key Right Hyper</summary>
    RHyper = 536870919, // 0x20000007
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Capslock)</summary>
    Capslock = 1073741881, // 0x40000039
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F1)</summary>
    F1 = 1073741882, // 0x4000003A
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F2)</summary>
    F2 = 1073741883, // 0x4000003B
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F3)</summary>
    F3 = 1073741884, // 0x4000003C
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F4)</summary>
    F4 = 1073741885, // 0x4000003D
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F5)</summary>
    F5 = 1073741886, // 0x4000003E
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F6)</summary>
    F6 = 1073741887, // 0x4000003F
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F7)</summary>
    F7 = 1073741888, // 0x40000040
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F8)</summary>
    F8 = 1073741889, // 0x40000041
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F9)</summary>
    F9 = 1073741890, // 0x40000042
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F10)</summary>
    F10 = 1073741891, // 0x40000043
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F11)</summary>
    F11 = 1073741892, // 0x40000044
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F12)</summary>
    F12 = 1073741893, // 0x40000045
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.PrintScreen)</summary>
    PrintScreen = 1073741894, // 0x40000046
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.ScrollLock)</summary>
    ScrollLock = 1073741895, // 0x40000047
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Pause)</summary>
    Pause = 1073741896, // 0x40000048
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Insert)</summary>
    Insert = 1073741897, // 0x40000049
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Home)</summary>
    Home = 1073741898, // 0x4000004A
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Pageup)</summary>
    Pageup = 1073741899, // 0x4000004B
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.End)</summary>
    End = 1073741901, // 0x4000004D
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Pagedown)</summary>
    Pagedown = 1073741902, // 0x4000004E
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Right)</summary>
    Right = 1073741903, // 0x4000004F
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Left)</summary>
    Left = 1073741904, // 0x40000050
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Down)</summary>
    Down = 1073741905, // 0x40000051
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Up)</summary>
    Up = 1073741906, // 0x40000052
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.NumLockClear)</summary>
    NumLockClear = 1073741907, // 0x40000053
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpDivide)</summary>
    KpDivide = 1073741908, // 0x40000054
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpMultiply)</summary>
    KpMultiply = 1073741909, // 0x40000055
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpMinus)</summary>
    KpMinus = 1073741910, // 0x40000056
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpPlus)</summary>
    KpPlus = 1073741911, // 0x40000057
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpEnter)</summary>
    KpEnter = 1073741912, // 0x40000058
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp1)</summary>
    Kp1 = 1073741913, // 0x40000059
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp2)</summary>
    Kp2 = 1073741914, // 0x4000005A
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp3)</summary>
    Kp3 = 1073741915, // 0x4000005B
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp4)</summary>
    Kp4 = 1073741916, // 0x4000005C
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp5)</summary>
    Kp5 = 1073741917, // 0x4000005D
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp6)</summary>
    Kp6 = 1073741918, // 0x4000005E
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp7)</summary>
    Kp7 = 1073741919, // 0x4000005F
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp8)</summary>
    Kp8 = 1073741920, // 0x40000060
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp9)</summary>
    Kp9 = 1073741921, // 0x40000061
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp0)</summary>
    Kp0 = 1073741922, // 0x40000062
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpPeriod)</summary>
    KpPeriod = 1073741923, // 0x40000063
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Application)</summary>
    Application = 1073741925, // 0x40000065
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Power)</summary>
    Power = 1073741926, // 0x40000066
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpEquals)</summary>
    KpEquals = 1073741927, // 0x40000067
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F13)</summary>
    F13 = 1073741928, // 0x40000068
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F14)</summary>
    F14 = 1073741929, // 0x40000069
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F15)</summary>
    F15 = 1073741930, // 0x4000006A
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F16)</summary>
    F16 = 1073741931, // 0x4000006B
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F17)</summary>
    F17 = 1073741932, // 0x4000006C
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F18)</summary>
    F18 = 1073741933, // 0x4000006D
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F19)</summary>
    F19 = 1073741934, // 0x4000006E
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F20)</summary>
    F20 = 1073741935, // 0x4000006F
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F21)</summary>
    F21 = 1073741936, // 0x40000070
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F22)</summary>
    F22 = 1073741937, // 0x40000071
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F23)</summary>
    F23 = 1073741938, // 0x40000072
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.F24)</summary>
    F24 = 1073741939, // 0x40000073
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Execute)</summary>
    Execute = 1073741940, // 0x40000074
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Help)</summary>
    Help = 1073741941, // 0x40000075
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Menu)</summary>
    Menu = 1073741942, // 0x40000076
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Select)</summary>
    Select = 1073741943, // 0x40000077
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Stop)</summary>
    Stop = 1073741944, // 0x40000078
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Again)</summary>
    Again = 1073741945, // 0x40000079
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Undo)</summary>
    Undo = 1073741946, // 0x4000007A
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Cut)</summary>
    Cut = 1073741947, // 0x4000007B
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Copy)</summary>
    Copy = 1073741948, // 0x4000007C
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Paste)</summary>
    Paste = 1073741949, // 0x4000007D
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Find)</summary>
    Find = 1073741950, // 0x4000007E
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Mute)</summary>
    Mute = 1073741951, // 0x4000007F
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.VolumeUp)</summary>
    VolumeUp = 1073741952, // 0x40000080
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.VolumeDown)</summary>
    VolumeDown = 1073741953, // 0x40000081
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpComma)</summary>
    KpComma = 1073741957, // 0x40000085
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpEqualsAs400)</summary>
    KpEqualAas400 = 1073741958, // 0x40000086
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AltErase)</summary>
    AltErase = 1073741977, // 0x40000099
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.SysReq)</summary>
    SysReq = 1073741978, // 0x4000009A
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Cancel)</summary>
    Cancel = 1073741979, // 0x4000009B
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Clear)</summary>
    Clear = 1073741980, // 0x4000009C
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Prior)</summary>
    Prior = 1073741981, // 0x4000009D
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Return2)</summary>
    Return2 = 1073741982, // 0x4000009E
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Separator)</summary>
    Separator = 1073741983, // 0x4000009F
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Out)</summary>
    Out = 1073741984, // 0x400000A0
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Oper)</summary>
    Oper = 1073741985, // 0x400000A1
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.ClearAgain)</summary>
    ClearAgain = 1073741986, // 0x400000A2
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.CrSel)</summary>
    CrSel = 1073741987, // 0x400000A3
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.ExSel)</summary>
    ExSel = 1073741988, // 0x400000A4
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp00)</summary>
    Kp00 = 1073742000, // 0x400000B0
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Kp000)</summary>
    Kp000 = 1073742001, // 0x400000B1
    /// <summary>
    /// SDL.ScancodeToKeycode(SDL.Scancode.ThousandsSeparator)
    /// </summary>
    ThousandsSeparator = 1073742002, // 0x400000B2
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.DecimalSeparator)</summary>
    DecimalSeparator = 1073742003, // 0x400000B3
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.CurrencyUnit)</summary>
    CurrenCyUnit = 1073742004, // 0x400000B4
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.CurrencySubunit)</summary>
    CurrenCySubunit = 1073742005, // 0x400000B5
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpLeftParen)</summary>
    KpLeftParen = 1073742006, // 0x400000B6
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpRightParen)</summary>
    KpRightParen = 1073742007, // 0x400000B7
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpLeftBrace)</summary>
    KpLeftBrace = 1073742008, // 0x400000B8
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpRightBrace)</summary>
    KpRightBrace = 1073742009, // 0x400000B9
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpTab)</summary>
    KpTab = 1073742010, // 0x400000BA
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpBackspace)</summary>
    KpBackspace = 1073742011, // 0x400000BB
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpA)</summary>
    KpA = 1073742012, // 0x400000BC
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpB)</summary>
    KpB = 1073742013, // 0x400000BD
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpC)</summary>
    KpC = 1073742014, // 0x400000BE
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpD)</summary>
    KpD = 1073742015, // 0x400000BF
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpE)</summary>
    KpE = 1073742016, // 0x400000C0
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpF)</summary>
    KpF = 1073742017, // 0x400000C1
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpXor)</summary>
    KpXor = 1073742018, // 0x400000C2
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpPower)</summary>
    KpPower = 1073742019, // 0x400000C3
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpPercent)</summary>
    KpPercent = 1073742020, // 0x400000C4
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpLess)</summary>
    KpLess = 1073742021, // 0x400000C5
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpGreater)</summary>
    KpGreater = 1073742022, // 0x400000C6
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpAmpersand)</summary>
    KpAmpersand = 1073742023, // 0x400000C7
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpDblAmpersand)</summary>
    KpDblAmpersand = 1073742024, // 0x400000C8
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpVerticalBar)</summary>
    KpVerticalBar = 1073742025, // 0x400000C9
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpDBLVERTICALBAR)</summary>
    KpDblVerticalBar = 1073742026, // 0x400000CA
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpDblVerticalBar)</summary>
    KpColon = 1073742027, // 0x400000CB
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpHash)</summary>
    KpHash = 1073742028, // 0x400000CC
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpSpace)</summary>
    KpSpace = 1073742029, // 0x400000CD
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpAt)</summary>
    KpAt = 1073742030, // 0x400000CE
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpExClam)</summary>
    KpExClam = 1073742031, // 0x400000CF
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpMemStore)</summary>
    KpMemStore = 1073742032, // 0x400000D0
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpMemRecall)</summary>
    KpMemRecall = 1073742033, // 0x400000D1
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpMemClear)</summary>
    KpMemClear = 1073742034, // 0x400000D2
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpMemAdd)</summary>
    KpMemAdd = 1073742035, // 0x400000D3
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpMemSubtract)</summary>
    KpMemSubtract = 1073742036, // 0x400000D4
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpMemMultiply)</summary>
    KpMemMultiply = 1073742037, // 0x400000D5
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpMemDivide)</summary>
    KpMemDivide = 1073742038, // 0x400000D6
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpPlusMinus)</summary>
    KpPlusMinus = 1073742039, // 0x400000D7
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpClear)</summary>
    KpClear = 1073742040, // 0x400000D8
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpClearEntry)</summary>
    KpClearEntry = 1073742041, // 0x400000D9
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpBinary)</summary>
    KpBinary = 1073742042, // 0x400000DA
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpOctal)</summary>
    KpOctal = 1073742043, // 0x400000DB
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpDecimal)</summary>
    KpDecimal = 1073742044, // 0x400000DC
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.KpHexadecimal)</summary>
    KpHexadecimal = 1073742045, // 0x400000DD
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.LCtrl)</summary>
    LCtrl = 1073742048, // 0x400000E0
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.LShift)</summary>
    LShift = 1073742049, // 0x400000E1
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.LAlt)</summary>
    LAlt = 1073742050, // 0x400000E2
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.LGUI)</summary>
    LGUI = 1073742051, // 0x400000E3
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.RCtrl)</summary>
    RCtrl = 1073742052, // 0x400000E4
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.RShift)</summary>
    RShift = 1073742053, // 0x400000E5
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.RAlt)</summary>
    RAlt = 1073742054, // 0x400000E6
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.RGui)</summary>
    RGUI = 1073742055, // 0x400000E7
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Mode)</summary>
    Mode = 1073742081, // 0x40000101
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Sleep)</summary>
    Sleep = 1073742082, // 0x40000102
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Wake)</summary>
    Wake = 1073742083, // 0x40000103
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.ChannelIncrement)</summary>
    ChannelIncrement = 1073742084, // 0x40000104
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.ChannelDecrement)</summary>
    ChannelDecrement = 1073742085, // 0x40000105
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.MediaPlay)</summary>
    MediaPlay = 1073742086, // 0x40000106
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.MediaPause)</summary>
    MediaPause = 1073742087, // 0x40000107
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.MediaRecord)</summary>
    MediaRecord = 1073742088, // 0x40000108
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.MediaFastForward)</summary>
    MediaFastForward = 1073742089, // 0x40000109
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.MediaRewind)</summary>
    MediaRewind = 1073742090, // 0x4000010A
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.MediaNextTrack)</summary>
    MediaNextTrack = 1073742091, // 0x4000010B
    /// <summary>
    /// SDL.ScancodeToKeycode(SDL.Scancode.MediaPreviousTrack)
    /// </summary>
    MediaPreviousTrack = 1073742092, // 0x4000010C
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.MediaStop)</summary>
    MediaStop = 1073742093, // 0x4000010D
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.MediaEject)</summary>
    MediaEject = 1073742094, // 0x4000010E
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.MediaPlayPause)</summary>
    MediaPlayPause = 1073742095, // 0x4000010F
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.MediaSelect)</summary>
    MediaSelect = 1073742096, // 0x40000110
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcNew)</summary>
    AcNew = 1073742097, // 0x40000111
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcOpen)</summary>
    AcOpen = 1073742098, // 0x40000112
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcClose)</summary>
    AcClose = 1073742099, // 0x40000113
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcExit)</summary>
    AcExit = 1073742100, // 0x40000114
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcSave)</summary>
    AcSave = 1073742101, // 0x40000115
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcPrint)</summary>
    AcPrint = 1073742102, // 0x40000116
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcProperties)</summary>
    AcProperties = 1073742103, // 0x40000117
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcSearch)</summary>
    AcSearch = 1073742104, // 0x40000118
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcHome)</summary>
    AcHome = 1073742105, // 0x40000119
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcBack)</summary>
    AcBack = 1073742106, // 0x4000011A
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcForward)</summary>
    AcForward = 1073742107, // 0x4000011B
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcStop)</summary>
    AcStop = 1073742108, // 0x4000011C
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcRefresh)</summary>
    AcRefresh = 1073742109, // 0x4000011D
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.AcBookmarks)</summary>
    AcBookmarks = 1073742110, // 0x4000011E
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.SoftLeft)</summary>
    SoftLeft = 1073742111, // 0x4000011F
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.SoftRight)</summary>
    SoftRight = 1073742112, // 0x40000120
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.Call)</summary>
    Call = 1073742113, // 0x40000121
    /// <summary>SDL.ScancodeToKeycode(SDL.Scancode.EndCall)</summary>
    EndCall = 1073742114, // 0x40000122
}