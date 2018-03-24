#if NET452
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Plisky.Win32 {
#pragma warning disable CS3001
#pragma warning disable CS3002
#pragma warning disable CS3003
#pragma warning disable CS3009

    [GeneratedCode("Used To SuppressCA", "Me")]
    [ExcludeFromCodeCoverage]
    public partial class NativeMethods {

        public delegate bool EnumWindowsCallbackDelegate(IntPtr hwnd, int lParam);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct LV_ITEM {
            public UInt32 mask;
            public Int32 iItem;
            public Int32 iSubItem;
            public UInt32 state;
            public UInt32 stateMask;
            public IntPtr pszText;
            public Int32 cchTextMax;
            public Int32 iImage;
            public IntPtr lParam;
        }

        public const int LVM_FIRST = 0x1000;
        public const int LVM_GETITEMCOUNT = LVM_FIRST + 4;

        //user object flags consts
        public const int FLAGSVALUE_WSF_VISIBLE = 1;

        public const int FLAGSVALUE_DF_ALLOWOTHERACCOUNTHOOK = 1;

        public const int UOI_FLAGS = 1;
        public const int UOI_NAME = 2;
        public const int UOI_TYPE = 3;
        public const int UOI_USER_SID = 4;

        public const int WHEELDELTA = 120;

#region Open Process

        [Flags]
        public enum ProcessAccessFlags : uint {
            All = 0x001F0FFF,
            PROCESS_ALL_ACCESS = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        public const int MEM_COMMIT = 0x1000;
        public const int MEM_RESERVE = 0x2000;
        public const int MEM_RESET = 0x80000;
        public const int MEM_LARGE_PAGES = 0x20000000;
        public const int MEM_PHYSICAL = 0x400000;
        public const int MEM_TOP_DOWN = 0x100000;
        public const int MEM_RELEASE = 0x8000;
        public const int MEM_DECOMMIT = 0x4000;

        public const int PAGE_EXECUTE = 0x10;
        public const int PAGE_EXECUTE_READ = 0x20;
        public const int PAGE_EXECUTE_READWRITE = 0x40;
        public const int PAGE_EXECUTE_WRITECOPY = 0x80;
        public const int PAGE_NOACCESS = 0x01;
        public const int PAGE_READONLY = 0x02;
        public const int PAGE_READWRITE = 0x04;
        public const int PAGE_WRITECOPY = 0x08;

        public const int LVIF_TEXT = 0x0001;
        public const int LVIF_IMAGE = 0x0002;
        public const int LVIF_PARAM = 0x0004;
        public const int LVIF_STATE = 0x0008;
        public const int LVIF_INDENT = 0x0010;
        public const int LVIF_NORECOMPUTE = 0x0800;
        public const int LVIS_FOCUSED = 0x0001;
        public const int LVIS_SELECTED = 0x0002;
        public const int LVIS_CUT = 0x0004;
        public const int LVIS_DROPHILITED = 0x0008;
        public const int LVIS_OVERLAYMASK = 0x0F00;
        public const int LVIS_STATEIMAGEMASK = 0xF000;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle,
           uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           byte[] lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, UIntPtr nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           UIntPtr dwSize, AllocationType dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           [Out] byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesRead);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool RevertToSelf();

        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        [Flags]
        public enum AllocationType {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            MEM_DECOMMIT = 0x4000,
            Release = 0x8000,
            MEM_RELEASE = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000
        }

#endregion

        [StructLayout(LayoutKind.Sequential)]
        public class tagCOPYDATASTRUCT {

            [MarshalAs(UnmanagedType.U4)]
            public Int32 dwDataAdditionalMisc; // Unsigned 32Bit Misc Data

            [MarshalAs(UnmanagedType.U4)]
            public Int32 cbDataSizeOfString;   // Unsigned 32Bit Size of LP

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpDataStringData;     // LP String Data
        };

        [StructLayout(LayoutKind.Sequential)]
        public class USEROBJECTFLAGS {

            [MarshalAs(UnmanagedType.Bool)]
            public bool fInherit;

            [MarshalAs(UnmanagedType.Bool)]
            public bool fReserved;

            [MarshalAs(UnmanagedType.U4)]
            public int dwFlags;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, int type);

#region OutputDebugString / FormatMessage

        public const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

        [SuppressMessage("Microsoft.Usage", "CA2205:UseManagedEquivalentsOfWin32Api", Justification = "Want to send to ODS not to a debugger")]
        [DllImport("kernel32.dll", EntryPoint = "OutputDebugStringA", SetLastError = false)]
        public static extern void OutputDebugString(String s);

        [DllImport("Kernel32.dll")]
        public static extern int FormatMessage(int flags, IntPtr source, int messageId, int languageId, StringBuilder buffer, int size, IntPtr arguments);

#endregion

#region SendMessage / PostMessage and SendMessage(WM_COPYDATA)

        /* Special HWND value for use with PostMessage() and SendMessage() */
        public IntPtr HWND_BROADCAST = (IntPtr)0xffff;

        // Import the SendMessage method of the User32 DLL.
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern System.IntPtr WMCD_SendMessage(int hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.AsAny)]object lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern System.IntPtr SendMessage(IntPtr hWnd, [MarshalAs(UnmanagedType.U4)]WindowMessages msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern System.IntPtr Data_SendMessage(IntPtr hWnd, WindowMessages msg, [MarshalAs(UnmanagedType.AsAny)]object wParam, [MarshalAs(UnmanagedType.AsAny)]object lParam);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(int hWnd, int Msg, int wParam, int lParam);

#endregion

        public const uint GR_GDIOBJECTS = 0; /* Count of GDI objects */
        public const uint GR_USEROBJECTS = 1; /* Count of USER objects */

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern uint GetGuiResources(IntPtr hProcess, uint uiFlags);

#region GetClientRect / GetWindowRect / ClientToScreen

        public struct W32RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public static System.Drawing.Rectangle GetDrawingRectangle(W32RECT input) {
                return new System.Drawing.Rectangle(input.left, input.top, input.right - input.left, input.bottom - input.top);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct W32POINT {
            public int x;
            public int y;

            public static System.Drawing.Point GetDrawingPoint(W32POINT input) {
                return new System.Drawing.Point(input.x, input.y);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, ref W32RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref W32RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref W32POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowDC")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

#endregion

#region BringWindowToTop / ShowWindow / SetActiveWindow / SetForeGroundWindow / Enable Window

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommands nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

#endregion

#region SetCapture, GetCapture, WindowFromPoint

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")]
        public static extern IntPtr WindowFromPoint(W32POINT Point);

        [DllImport("user32.dll", EntryPoint = "SetCapture")]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetCapture")]
        public static extern IntPtr GetCapture();

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        public static extern IntPtr ReleaseCapture();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT {

            /// <summary>
            /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
            /// <para>
            /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
            /// </para>
            /// </summary>
            public int Length;

            /// <summary>
            /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
            /// </summary>
            public int Flags;

            /// <summary>
            /// The current show state of the window.
            /// </summary>
            public ShowWindowCommands ShowCmd;

            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is minimized.
            /// </summary>
            public W32POINT MinPosition;

            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is maximized.
            /// </summary>
            public W32POINT MaxPosition;

            /// <summary>
            /// The window's coordinates when the window is in the restored position.
            /// </summary>
            public W32RECT NormalPosition;

            /// <summary>
            /// Gets the default (empty) value.
            /// </summary>
            public static WINDOWPLACEMENT GetWindowPlacement() {
                WINDOWPLACEMENT result = new WINDOWPLACEMENT();
                result.Length = Marshal.SizeOf(result);
                return result;
            }

            public static WINDOWPLACEMENT GetWindowPlacement(IntPtr HWnd) {
                WINDOWPLACEMENT result = new WINDOWPLACEMENT();
                result.Length = Marshal.SizeOf(result);
                if (!NativeMethods.GetWindowPlacement(HWnd, ref result)) {
                    throw new Win32Exception();
                }
                return result;
            }
        }

#endregion

#region FindWindow  / GetWindowText / IsWindow  / EnumWindows / FlashWindow

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string className, string windowTitle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        public static extern void GetWindowText(IntPtr handle, StringBuilder theText, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetClassName", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr handle, StringBuilder className, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetParent")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "IsWindowVisible")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32", EntryPoint = "EnumWindows")]
        public static extern int EnumWindows(EnumWindowsCallbackDelegate callback, int lParam);

        [DllImport("user32", EntryPoint = "EnumChildWindows")]
        public static extern int EnumChildWindows(IntPtr hWnd, EnumWindowsCallbackDelegate callback, int lParam);

        [DllImport("user32", EntryPoint = "FlashWindow")]
        public static extern int FlashWindow(IntPtr hWnd, bool onOff);

#endregion

#region CreateNamedPipe

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateNamedPipe(string lpName, uint dwOpenMode, uint dwPipeMode, uint nMaxInstances, uint nOutBufferSize, uint nInBufferSize, uint nDefaultTimeOut, IntPtr lpSecurityAttributes);

#endregion

#region GetProcessWindowStation / GetUserObjectInformation / GetCurrentOSThreadId

        [DllImport("user32.dll", EntryPoint = "GetProcessWindowStation", SetLastError = true)]
        public static extern IntPtr GetProcessWindowStation();

        [DllImport("user32.dll", EntryPoint = "GetUserObjectInformation", SetLastError = true)]
        //public static extern bool GetUserObjectInformation ( int hObj, int nIndex,[MarshalAs(UnmanagedType.LPStruct)]out USEROBJECTFLAGS uof,int  nLength ,[MarshalAs(UnmanagedType.I4)]out int nNeeded);
        public static extern bool GetUserObjectInformation(IntPtr hObj, int nIndex, [Out] byte[] pvInfo, uint nLength, out uint lpnLengthNeeded);

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentThreadId", SetLastError = true)]
        public static extern UInt32 GetCurrentOSThreadID();

#endregion

#region FindFirst / Next File ripped from MSDN

        /*
    typedef struct _WIN32_FIND_DATA
    {
      DWORD    dwFileAttributes;
      FILETIME ftCreationTime;
      FILETIME ftLastAccessTime;
      FILETIME ftLastWriteTime;
      DWORD    nFileSizeHigh;
      DWORD    nFileSizeLow;
      DWORD    dwReserved0;
      DWORD    dwReserved1;
      TCHAR    cFileName[ MAX_PATH ];
      TCHAR    cAlternateFileName[ 14 ];
    } WIN32_FIND_DATA, *PWIN32_FIND_DATA;
    */

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class FindData {
            public int fileAttributes = 0;

            // creationTime was embedded FILETIME structure
            public int creationTime_lowDateTime = 0;

            public int creationTime_highDateTime = 0;

            // lastAccessTime was embedded FILETIME structure
            public int lastAccessTime_lowDateTime = 0;

            public int lastAccessTime_highDateTime = 0;

            // lastWriteTime was embedded FILETIME structure
            public int lastWriteTime_lowDateTime = 0;

            public int lastWriteTime_highDateTime = 0;

            public int nFileSizeHigh = 0;
            public int nFileSizeLow = 0;
            public int dwReserved0 = 0;
            public int dwReserved1 = 0;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String fileName = null;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public String alternateFileName = null;
        }

        //HANDLE FindFirstFile(LPCTSTR lpFileName, LPWIN32_FIND_DATA lpFindFileData);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindFirstFile(String fileName, [In, Out] FindData findFileData);

        // BOOL FindNextFile(HANDLE hFindFile, LPWIN32_FIND_DATA lpFindFileData  );
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool FindNextFile(IntPtr hFindFile, [In, Out] FindData findFileData);

        // BOOL FindClose(HANDLE hFindFile );
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool FindClose(IntPtr hFindFile);

#endregion

#region performance interface - QueryPerformanceCounter / QueryPerformanceFrequency

        // Returns the number of ticks / second of the high res counter in the out parameter.  If retval is 0 then highres counter is not supported by hardware
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern int QueryPerformanceFrequency(out ulong frequency);

        // Returns the number of ticks elapsed for the performance counter, returns nonzero if it worked.  retval=0 is an error
        [DllImport("kernel32.dll")]
        protected static extern int QueryPerformanceCounter(out ulong performanceCount);

#endregion

#region ODS Import stuff

#region w32intf Constants

        public const int SECURITY_DESCRIPTOR_MIN_LENGTH = 20;
        public const int SECURITY_DESCRIPTOR_REVISION = 1;

        public const int SECTION_QUERY = 1;
        public const int SECTION_MAP_WRITE = 2;
        public const int SECTION_MAP_READ = 4;
        public const int SECTION_MAP_EXECUTE = 8;
        public const int SECTION_EXTEND_SIZE = 0x10;

        public const int FILE_MAP_COPY = SECTION_QUERY;
        public const int FILE_MAP_WRITE = SECTION_MAP_WRITE;
        public const int FILE_MAP_READ = SECTION_MAP_READ;

        public const int STATUS_WAIT_0 = 0x00000000;
        public const int STATUS_ABANDONED_WAIT_0 = 0x00000080;

        public const int STATUS_USER_APC = 0x000000C0;
        public const int STATUS_TIMEOUT = 0x00000102;
        public const uint WAIT_FAILED = 0xFFFFFFFF;
        public const int WAIT_OBJECT_0 = ((STATUS_WAIT_0) + 0);
        public const int WAIT_ABANDONED = ((STATUS_ABANDONED_WAIT_0) + 0);
        public const int WAIT_ABANDONED_0 = ((STATUS_ABANDONED_WAIT_0) + 0);
        public const int WAIT_TIMEOUT = STATUS_TIMEOUT;

#endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES {

            [MarshalAs(UnmanagedType.U4)]
            public uint nLength;

            [MarshalAs(UnmanagedType.LPStruct)]
            public IntPtr lpSecurityDescriptor;

            [MarshalAs(UnmanagedType.Bool)]
            public bool bIneritHandle;
        }

#region Win32 DLL Import Statements

        [DllImport("AdvApi32.dll", EntryPoint = "InitializeSecurityDescriptor", SetLastError = true)]
        public static extern bool InitializeSecurityDescriptor(IntPtr pSecurityDescriptor, uint dwRevision);

        [DllImport("AdvApi32.dll", EntryPoint = "InitializeSecurityDescriptor", SetLastError = true)]
        public static extern bool SetSecurityDescriptorDacl(IntPtr pSecurityDescriptor, bool bDaclPresent, IntPtr pDacl, bool bDaclDefaulted);

        [DllImport("Kernel32.dll", EntryPoint = "CreateFileMapping", SetLastError = true)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect,
          uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        [DllImport("Kernel32.dll", EntryPoint = "CreateEvent", SetLastError = true)]
        public static extern IntPtr CreateEvent(IntPtr lpEventAttributes, bool bManualReset, bool bInitialState, string lpName);

        [DllImport("Kernel32.dll", EntryPoint = "MapViewOfFile", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh,
          uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("Kernel32.dll", EntryPoint = "SetEvent", SetLastError = true)]
        public static extern bool SetEvent(IntPtr hEvent);

        [DllImport("Kernel32.dll", EntryPoint = "WaitForSingleObject", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hEvent, uint dwMilliseconds);

        [DllImport("Kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

#if false
        [DllImport("Kernel32.dll", EntryPoint = "UnmapViewOfFile", SetLastError = true)]
        public unsafe static extern bool UnmapViewOfFile(void* lpBaseAddress);
#endif

#endregion

#endregion

        [DllImport("user32.dll")]
        public static extern bool TranslateMessage([In] ref MSG lpMsg);

        public struct MSG {
            public IntPtr hwnd;
            /* public uint message;*/

            [MarshalAs(UnmanagedType.U4)]
            public WindowMessages message;

            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public W32POINT pt;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short VkKeyScan(char key);

        [DllImport("user32.dll")]
        public static extern int OemKeyScan(short wAsciiVal);

#region SendInput and supporting types.

        [Flags]
        public enum KeyboadInputEventFlags : int {
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002,
            KEYEVENTF_UNICODE = 0x0004,
            KEYEVENTF_SCANCODE = 0x0008
        }

        public struct HARDWAREINPUT {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        public struct KEYBDINPUT {

            [MarshalAs(UnmanagedType.U2)]
            public VK wVk;

            /* public ushort wVk; */
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        public struct MOUSEINPUT {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        // This enum supplies the values of the type field for the INPUT structure.
        public enum SendInputType : int {
            INPUT_MOUSE = 0x00,
            INPUT_KEYBOARD = 0x01,
            INPUT_HARDWARE = 0x02
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION {

            // Fields
            [FieldOffset(0)]
            public HARDWAREINPUT hi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT {
            public SendInputType type;
            public INPUTUNION inputUnion;
        }

        // CBSize is the sizeof an input structure and must always be so.  sizeof(INPUT)
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        // This retrieves additional info associated with the current threads input que.  It should be used to populate the extra info data.
        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr GetMessageExtraInfo();

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool BlockInput([In, MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        [DllImport("user32.dll", EntryPoint = "GetKeyState", SetLastError = true)]
        public static extern short GetKeyState(VK nVirtKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetKeyboardState(byte[] keystate);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int SetKeyboardState(byte[] keystate);

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void mouse_event(MouseActivity dwFlags, Int32 dx, Int32 dy, UInt32 dwData, IntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

#endregion
    }

#pragma warning restore CS3001
#pragma warning restore CS3002
#pragma warning restore CS3003
#pragma warning restore CS3009
}

#ENDIF