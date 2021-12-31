// WinCon.cs - Interface module for Windows Console API
// Jim Mischel, 2006/08/25
//
// This module provides the managed-to-unmanaged interface to communicate with
// the Windows Console API.  Full API documentation is available in the Windows SDK
// documentation topic, "Character-Mode Applications," at
// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dllproc/base/character_mode_applications.asp
using System;
using System.Runtime.InteropServices;

namespace Mischel.ConsoleDotNet
{

    /// <summary>
    /// Defines the coordinates of a character cell in a console window.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
	public struct Coord
	{
		[FieldOffset(0)] private short x;
		[FieldOffset(2)] private short y;
		/// <summary>
		/// Creates a new instance of the Coord structure.
		/// </summary>
		/// <param name="mx">The column position in the window.</param>
		/// <param name="my">The row position in the window.</param>
		public Coord(short mx, short my)
		{
			x = mx;
			y = my;
		}

		/// <summary>
		/// Gets or sets the column position.
		/// </summary>
		public short X
		{
			get { return x; }
			set { x = value; }
		}

		/// <summary>
		/// Gets or sets the row position.
		/// </summary>
		public short Y
		{
			get { return y; }
			set { y = value; }
		}
	}

	/// <summary>
	/// Defines the coordinates of the upper left and lower right corners of
	/// a rectangle.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class SmallRect
	{
		public short left;
		public short top;
		public short right;
		public short bottom;

		/// <summary>
		/// Creates a new instance of the SmallRect structure.
		/// </summary>
		/// <param name="mLeft">Column position of top left corner.</param>
		/// <param name="mTop">Row position of the top left corner.</param>
		/// <param name="mRight">Column position of the bottom right corner.</param>
		/// <param name="mBottom">Row position of the bottom right corner.</param>
		public SmallRect(short mLeft, short mTop, short mRight, short mBottom)
		{
			left = mLeft;
			top = mTop;
			right = mRight;
			bottom = mBottom;
		}

		/// <summary>
		/// Gets or sets the column position of the top left corner of a rectangle.
		/// </summary>
		public short Left
		{
			get { return left; }
			set { left = value; }
		}

		/// <summary>
		/// Gets or sets the row position of the top left corner of a rectangle.
		/// </summary>
		public short Top
		{
			get { return top; }
			set { top = value; }
		}

		/// <summary>
		/// Gets or sets the column position of the bottom right corner of a rectangle.
		/// </summary>
		public short Right
		{
			get { return right; }
			set { right = value; }
		}

		/// <summary>
		/// Gets or sets the row position of the bottom right corner of a rectangle.
		/// </summary>
		public short Bottom
		{
			get { return bottom; }
			set { bottom = value; }
		}

		/// <summary>
		/// Gets or sets the width of a rectangle.  When setting the width, the
		/// column position of the bottom right corner is adjusted.
		/// </summary>
		public short Width
		{
			get { return (short)(right - left + 1); }
			set { right = (short)(left + value - 1); }
		}

		/// <summary>
		/// Gets or sets the height of a rectangle.  When setting the height, the
		/// row position of the bottom right corner is adjusted.
		/// </summary>
		public short Height
		{
			get { return (short)(bottom - top + 1); }
			set { bottom = (short)(top + value - 1); }
		}
	}

	/// <summary>
	/// Contains information about a console screen buffer.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public class ConsoleScreenBufferInfo
	{
		public Coord dwSize;
		public Coord dwCursorPosition;
		public short wAttributes;
		[MarshalAs(UnmanagedType.Struct)] public SmallRect srWindow;
		public Coord dwMaximumWindowSize;
	}

	/// <summary>
	/// Windows Console API definitions.
	/// </summary>
	public sealed class WinCon
	{
		public const int CONSOLE_TEXTMODE_BUFFER = 1;

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateConsoleScreenBuffer(
			int dwDesiredAccess,
			int dwShareMode,
			[In, Out][MarshalAs(UnmanagedType.LPStruct)] SecurityAttributes lpSecurityAttributes,
			int dwFlags,
			IntPtr lpScreenBufferData);


		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetConsoleScreenBufferInfo(
			IntPtr hConsoleOutput,
			[In, Out][MarshalAs(UnmanagedType.LPStruct)] ConsoleScreenBufferInfo lpConsoleScreenBufferInfo);

		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern Coord GetLargestConsoleWindowSize(IntPtr hConsoleOutput);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadConsoleOutputCharacter(
			IntPtr hConsoleOutput,
			[Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] lpCharacter,
			int nLength,
			Coord dwReadCoord,
			ref int lpNumberOfCharsRead);


		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleCursorPosition(
			IntPtr hConsoleOutput,
			Coord dwCursorPosition);


		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleScreenBufferSize(
			IntPtr hConsoleOutput,
			Coord dwSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleWindowInfo(
			IntPtr hConsoleOutput,
			bool bAbsolute,
			[In][MarshalAs(UnmanagedType.LPStruct)] SmallRect lpConsoleWindow);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteConsole(
			IntPtr hConsoleOutput,
			[In][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] lpBuffer,
			int NumberOfCharsToWrite,
			ref int NumberOfCharsWritten,
			IntPtr reserved);

		private WinCon()
		{
			// private constructor prevents instantiation
		}
	}
}
