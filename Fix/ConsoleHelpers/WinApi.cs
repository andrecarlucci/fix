// WinApi.cs - Some helpful Windows API functions used by the ConsoleDotNet classes.
// Jim Mischel - 2006/08/25
//
using System;
using System.Runtime.InteropServices;

namespace Mischel.ConsoleDotNet
{
	[StructLayout(LayoutKind.Sequential)]
	public class SecurityAttributes
	{
		public int nLength;
		public IntPtr lpSecurityDescriptor;
		public bool bInheritHandle;
	}

	public sealed class WinApi
	{

		// Some useful constants
		public const int GENERIC_READ = unchecked((int)0x80000000);
		public const int GENERIC_WRITE = 0x40000000;
		public const int FILE_SHARE_READ = 1;
		public const int FILE_SHARE_WRITE = 2;
		public const int INVALID_HANDLE_VALUE = -1;

		private WinApi()
		{
		}

		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr handle);

		public const int FILE_TYPE_UNKNOWN = 0x0000;
		public const int FILE_TYPE_DISK = 0x0001;
		public const int FILE_TYPE_CHAR = 0x0002;
		public const int FILE_TYPE_PIPE = 0x0003;
		public const int FILE_TYPE_REMOTE = 0x8000;

        public const int CREATE_NEW = 1;
        public const int CREATE_ALWAYS = 2;
        public const int OPEN_EXISTING = 3;
        public const int OPEN_ALWAYS = 4;
        public const int TRUNCATE_EXISTING = 5;

        [DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateFile(
			string Filename,
			int DesiredAccess,
			int ShareMode,
			[In][MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes SecAttr,
			int CreationDisposition,
			int FlagsAndAttributes,
			IntPtr TemplateFile);
	}
}
