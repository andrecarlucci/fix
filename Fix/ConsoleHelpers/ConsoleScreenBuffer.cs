// ScreenBuffer.cs - Definition of the ConsoleScreenBuffer class.
// Jim Mischel - 2006/08/25
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Mischel.ConsoleDotNet
{
	/// <summary>
	/// Provides an interface to a console screen buffer to extend the
	/// .NET support for character-mode applications.
	/// </summary>
	public class ConsoleScreenBuffer : IDisposable
	{
		// Windows resource handle.
		private IntPtr handle;

		// Flag indicates whether the object owns the handle.
		// If true, the handle is closed when the object is disposed.
		internal bool ownsHandle = false;

		#region Construction and destruction
		/// <summary>
		/// Create a new instance of the ConsoleScreenBuffer class by creating a new
		/// console screen buffer handle.
		/// </summary>
		public ConsoleScreenBuffer()
		{
			handle = WinCon.CreateConsoleScreenBuffer(
				WinApi.GENERIC_READ | WinApi.GENERIC_WRITE,
				WinApi.FILE_SHARE_READ | WinApi.FILE_SHARE_WRITE,
				null,
				WinCon.CONSOLE_TEXTMODE_BUFFER,
				IntPtr.Zero);
			if (handle.ToInt32() == WinApi.INVALID_HANDLE_VALUE)
			{
				throw new IOException("Unable to create screen buffer", Marshal.GetLastWin32Error());
			}
			ownsHandle = true;
		}

		/// <summary>
		/// Create a new instance of the ConsoleScreenBuffer class from a passed handle.
		/// </summary>
		/// <param name="handle">Handle to an existing Windows console screen buffer.</param>
		internal ConsoleScreenBuffer(IntPtr handle)
		{
			this.handle = handle;
			ownsHandle = false;
		}

		~ConsoleScreenBuffer()
		{
			Dispose(false);
		}

		#region IDisposable Members

		private bool disposed = false;
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called.
			if (!this.disposed)
			{
				if (disposing)
				{
				}
				if (ownsHandle && handle != IntPtr.Zero)
				{
					WinApi.CloseHandle(handle);
					handle = IntPtr.Zero;
				}
			}
			disposed = true;
		}

		#endregion

		#endregion

		#region Miscellaneous

		/// <summary>
		/// Gets the Windows screen buffer handle.
		/// </summary>
		public IntPtr Handle
		{
			get { return handle; }
		}

		private ConsoleScreenBufferInfo GetScreenBufferInfo()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			ConsoleScreenBufferInfo csbi = new ConsoleScreenBufferInfo();
			if (!WinCon.GetConsoleScreenBufferInfo(handle, csbi))
			{
				int err = Marshal.GetLastWin32Error();
				Console.WriteLine("err = {0}", err);
				throw new IOException("Error getting screen buffer info", err);
			}
			return csbi;
		}

		#endregion

		#region Cursor

		/// <summary>
		/// Gets or sets the height of the console cursor, expressed as a percentage of
		/// the character cell.  From 1 to 100 percent.
		/// </summary>
		public int CursorSize
		{
			get { return GetCursorInfo().Size; }
			set { SetCursorInfo(GetCursorInfo().Visible, value); }
		}

		/// <summary>
		/// Gets or sets the cursor visibility.
		/// </summary>
		public bool CursorVisible
		{
			get { return GetCursorInfo().Visible; }
			set { SetCursorInfo(value, GetCursorInfo().Size); }
		}

		/// <summary>
		/// Gets or sets the column position of the cursor in the screen buffer.
		/// </summary>
		public int CursorLeft
		{
			get { return GetScreenBufferInfo().dwCursorPosition.X; }
			set { SetCursorPosition(value, CursorTop); }
		}

		/// <summary>
		/// Gets or sets the row position of the cursor in the screen buffer.
		/// </summary>
		public int CursorTop
		{
			get { return GetScreenBufferInfo().dwCursorPosition.Y; }
			set { SetCursorPosition(CursorLeft, value); }
		}

		/// <summary>
		/// Sets the cursor row and column position.
		/// </summary>
		/// <param name="x">The new column position of the cursor.</param>
		/// <param name="y">The new row position of the cursor.</param>
		public void SetCursorPosition(int x, int y)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}

			Coord cursorPos = new Coord((short)x, (short)y);
			if (!WinCon.SetConsoleCursorPosition(handle, cursorPos))
			{
				throw new ApplicationException("Error setting cursor position");
			}
		}

		private ConsoleCursorInfo GetCursorInfo()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			ConsoleCursorInfo cci = new ConsoleCursorInfo();
			if (!WinCon.GetConsoleCursorInfo(handle, cci))
			{
				throw new ApplicationException("Error getting cursor information.");
			}
			return cci;
		}

		private void SetCursorInfo(bool visible, int size)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}

			ConsoleCursorInfo cci = new ConsoleCursorInfo(visible, size);
			if (!WinCon.SetConsoleCursorInfo(handle, cci))
			{
				throw new ApplicationException("Error setting cursor information.");
			}
		}

		#endregion

		#region Screen buffer size

		/// <summary>
		/// Gets or sets the height (in character rows) of the console screen buffer.
		/// </summary>
		public int Height
		{
			get { return GetScreenBufferInfo().dwSize.Y; }
			set { SetBufferSize(Width, (short)value); }
		}

		/// <summary>
		/// Gets or sets the width (in character columns) of the console screen buffer.
		/// </summary>
		public int Width
		{
			get { return GetScreenBufferInfo().dwSize.X; }
			set { SetBufferSize((short)value, Height); }
		}

		/// <summary>
		/// Sets the screen buffer size in character columns and rows.
		/// </summary>
		/// <param name="width">Desired width, in character columns, of screen buffer.</param>
		/// <param name="height">Desired height, in character rows, of screen buffer.</param>
		public void SetBufferSize(int width, int height)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}

			Coord sz = new Coord((short)width, (short)height);

			if (!WinCon.SetConsoleScreenBufferSize(handle, sz))
			{
				throw new IOException("Unable to set screen buffer size", Marshal.GetLastWin32Error());
			}
		}
		#endregion

		#region Screen buffer window

		/// <summary>
		/// Gets or sets the screen buffer window height in character rows.
		/// </summary>
		public int WindowHeight
		{
			get { return GetWindowRect().Height; }
			set { SetWindowSize(WindowWidth, value); }
		}

		/// <summary>
		/// Gets or sets the screen buffer window width in character columns.
		/// </summary>
		public int WindowWidth
		{
			get { return GetWindowRect().Width; }
			set { SetWindowSize(value, WindowHeight); }
		}

		/// <summary>
		/// Sets the screen buffer window size.
		/// </summary>
		/// <param name="width">Desired window width in character columns.</param>
		/// <param name="height">Desired window height in character rows.</param>
		public void SetWindowSize(int width, int height)
		{
			SmallRect sr = GetWindowRect();
			sr.Width = (short)width;
			sr.Height = (short)height;
			SetWindowRect(sr, true);
		}

		/// <summary>
		/// Gets or sets the row position of the window in the screen buffer.
		/// </summary>
		public int WindowTop
		{
			get { return GetWindowRect().Top; }
			set { SetWindowPosition(WindowLeft, value); }
		}

		/// <summary>
		/// Gets or sets the column position of the window in the screen buffer.
		/// </summary>
		public int WindowLeft
		{
			get { return GetWindowRect().Left; }
			set { SetWindowPosition(value, WindowTop); }
		}

		/// <summary>
		/// Sets the position of the window within the screen buffer.
		/// </summary>
		/// <param name="left">Column position of the top-left corner of the screen buffer window.</param>
		/// <param name="top">Row position of the top-left corner of the screen buffer window.</param>
		public void SetWindowPosition(int left, int top)
		{
			SmallRect sr = GetWindowRect();
			int width = sr.Width;
			int height = sr.Height;
			sr.left = (short)left;
			sr.top = (short)top;
			sr.Width = (short)width;
			sr.Height = (short)height;
			SetWindowRect(sr, true);
		}

		private SmallRect GetWindowRect()
		{
			return GetScreenBufferInfo().srWindow;
		}

		private void SetWindowRect(SmallRect sr, bool bAbsolute)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			if (!WinCon.SetConsoleWindowInfo(handle, bAbsolute, sr))
			{
				int err = Marshal.GetLastWin32Error();
				throw new ApplicationException(String.Format("Unable to set window rect: {0}", err));
			}
		}


		// LargestWindowSize takes into account only the current font and physical
		// console window size.
		/// <summary>
		/// Get the largest possible window height, expressed in character rows.
		/// </summary>
		public int LargestWindowHeight
		{
			get { return GetLargestWindowSize().Y; }
		}

		/// <summary>
		/// Get the largest possible window width, expressed in character rows.
		/// </summary>
		public int LargestWindowWidth
		{
			get { return GetLargestWindowSize().X; }
		}

		private Coord GetLargestWindowSize()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			Coord size = WinCon.GetLargestConsoleWindowSize(handle);
			if (size.X == 0 && size.Y == 0)
			{
				throw new ApplicationException("Error getting largest window size");
			}
			return size;
		}

		/// <summary>
		/// Gets the maximum window width, based on the display size, current font, and screen buffer size.
		/// </summary>
		public int MaximumWindowWidth
		{
			get { return MaximumWindowSize.X; }
		}

		/// <summary>
		/// Gets the maximum window height, based on the display size, current font, and screen buffer size.
		/// </summary>
		public int MaximumWindowHeight
		{
			get { return MaximumWindowSize.Y; }
		}

		// MaximumWindowSize takes into account the size of the console screen buffer.
		private Coord MaximumWindowSize
		{
			get { return GetScreenBufferInfo().dwMaximumWindowSize; }
		}
		#endregion

		#region Scrolling

		/// <summary>
		/// Copies a specified source area of the screen buffer to a specified destination area.
		/// Vacated character cells are filled with spaces and the current color attribute.
		/// </summary>
		/// <param name="sourceLeft">Column position of the source area's top-left corner.</param>
		/// <param name="sourceTop">Row position of the source arean't top-left corner.</param>
		/// <param name="sourceWidth">Width, in character columns, of the source area.</param>
		/// <param name="sourceHeight">Height, in character rows, of the source area.</param>
		/// <param name="targetLeft">Column position of the target's top-left corner.</param>
		/// <param name="targetTop">Row position of the target's top-left corner.</param>
		public void MoveBufferArea(
			int sourceLeft,
			int sourceTop,
			int sourceWidth,
			int sourceHeight,
			int targetLeft,
			int targetTop
		)
		{
			MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop,
				' ', ForegroundColor, BackgroundColor);
		}

		/// <summary>
		/// Copies a specified source area of the screen buffer to a specified destination area.
		/// Vacated character cells are filled with the specified character and color attributes.
		/// </summary>
		/// <param name="sourceLeft">Column position of the source area's top-left corner.</param>
		/// <param name="sourceTop">Row position of the source arean't top-left corner.</param>
		/// <param name="sourceWidth">Width, in character columns, of the source area.</param>
		/// <param name="sourceHeight">Height, in character rows, of the source area.</param>
		/// <param name="targetLeft">Column position of the target's top-left corner.</param>
		/// <param name="targetTop">Row position of the target's top-left corner.</param>
		/// <param name="sourceChar">Character with which to fill vacated character positions.</param>
		/// <param name="sourceForeColor">Foreground color to use for filling.</param>
		/// <param name="sourceBackColor">Background color to use for filling.</param>
		public void MoveBufferArea(
			int sourceLeft,
			int sourceTop,
			int sourceWidth,
			int sourceHeight,
			int targetLeft,
			int targetTop,
			char sourceChar,
			ConsoleColor sourceForeColor,
			ConsoleColor sourceBackColor
		)
		{
			SmallRect sourceRect = new SmallRect((short)sourceLeft, (short)sourceTop,
				(short)(sourceLeft + sourceWidth - 1), (short)(sourceTop + sourceHeight - 1));
			Coord dest = new Coord((short)targetLeft, (short)targetTop);
			ConsoleCharInfo cci = new ConsoleCharInfo(sourceChar, new ConsoleCharAttribute(sourceForeColor, sourceBackColor));
			if (!WinCon.ScrollConsoleScreenBuffer(handle, sourceRect, null, dest, ref cci))
			{
				throw new IOException("Error scrolling screen buffer", Marshal.GetLastWin32Error());
			}
		}

		#endregion

		#region Console Mode and Display Mode

		private bool GetModeFlag(ConsoleOutputModeFlags flag)
		{
			return (OutputMode & flag) != 0;
		}

		private void SetModeFlag(ConsoleOutputModeFlags flag, bool val)
		{
			if (val)
				OutputMode = OutputMode | flag;
			else
				OutputMode = OutputMode & ~flag;
		}

		/// <summary>
		/// Gets or sets the ProcessedOutput mode flag.
		/// </summary>
		/// <remarks>
		/// If enabled, characters written to the console or echoed to the console on read
		/// are examined for ASCII control sequences and the correct action is
		/// performed.  Backspace, tab, bell, carriage return, and linefeed
		/// characters are processed.
		/// </remarks>
		public bool ProcessedOutput
		{
			get { return GetModeFlag(ConsoleOutputModeFlags.Processed); }
			set { SetModeFlag(ConsoleOutputModeFlags.Processed, value); }
		}

		/// <summary>
		/// Gets or sets the WrapAtEol output mode flag.
		/// </summary>
		/// <remarks>
		/// If enabled, when writing characters or echoing input, the cursor moves to the
		/// beginning of the row when it reaches the end of the current row.
		/// </remarks>
		public bool WrapAtEol
		{
			get { return GetModeFlag(ConsoleOutputModeFlags.WrapAtEol); }
			set { SetModeFlag(ConsoleOutputModeFlags.WrapAtEol, value); }
		}

		/// <summary>
		/// Gets or sets the console output mode bits.
		/// </summary>
		public ConsoleOutputModeFlags OutputMode
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException(this.ToString());
				}
				int mode = 0;
				if (!WinCon.GetConsoleMode(handle, ref mode))
				{
					throw new ApplicationException("Unable to get console mode.");
				}
				return (ConsoleOutputModeFlags)mode;
			}
			set
			{
				if (disposed)
				{
					throw new ObjectDisposedException(this.ToString());
				}
				if (!WinCon.SetConsoleMode(handle, (int)value))
				{
					throw new ApplicationException("Unable to set console mode.");
				}
			}
		}

		/// <summary>
		/// Sets the console display mode.
		/// </summary>
		/// <param name="mode">The desired display mode:  Windowed or Fullscreen.</param>
		public void SetDisplayMode(ConsoleDisplayMode mode)
		{
			Coord newSize = new Coord(0, 0);
			if (!WinCon.SetConsoleDisplayMode(this.handle, (int)mode, ref newSize))
			{
				int err = Marshal.GetLastWin32Error();
				throw new IOException("Unable to set display mode.", err);
			}
		}

		/// <summary>
		/// Gets the current console display mode.
		/// </summary>
		public ConsoleDisplayMode DisplayMode
		{
			get
			{
				int dMode = 0;
				if (!WinCon.GetConsoleDisplayMode(ref dMode))
				{
					throw new IOException("Unable to get display mode", Marshal.GetLastWin32Error());
				}
				return (ConsoleDisplayMode)dMode;
			}
		}

		#endregion

		#region Foreground and Background Color

		/// <summary>
		/// Gets or sets the foreground color used to write characters.
		/// </summary>
		public ConsoleColor ForegroundColor
		{
			get { return new ConsoleCharAttribute(GetScreenBufferInfo().wAttributes).Foreground; }
			set
			{
				ConsoleCharAttribute attr = new ConsoleCharAttribute(GetScreenBufferInfo().wAttributes);
				attr.Foreground = value;
				SetTextAttribute(attr);
			}
		}

		/// <summary>
		/// Gets or sets the background color used to write characters.
		/// </summary>
		public ConsoleColor BackgroundColor
		{
			get { return new ConsoleCharAttribute(GetScreenBufferInfo().wAttributes).Background; }
			set
			{
				ConsoleCharAttribute attr = new ConsoleCharAttribute(GetScreenBufferInfo().wAttributes);
				attr.Background = value;
				SetTextAttribute(attr);
			}
		}

		/// <summary>
		/// Resets foreground and background colors to their defaults.
		/// </summary>
		public void ResetColor()
		{
			ConsoleCharAttribute attr = new ConsoleCharAttribute(ConsoleColor.Gray, ConsoleColor.Black);
			SetTextAttribute(attr);
		}

		/// <summary>
		/// Sets the current attribute (foreground and background colors) to be used when
		/// writing characters.
		/// </summary>
		/// <param name="attr">The desired output attribute.</param>
		public void SetTextAttribute(ConsoleCharAttribute attr)
		{
			if (!WinCon.SetConsoleTextAttribute(handle, attr))
			{
				throw new ApplicationException("Unable to set text attribute");
			}
		}

		#endregion

		#region Font

		// There are two font functions.
		// GetCurrentConsoleFont returns a ConsoleFontInfo class that
		// contains the number and size for the current font in a maximized or not window.
		// GetConsoleFontSize returns the size for a given font id.
		// I'm just going to implement GetFontSize to return the size of the current font.
		public Coord GetFontSize()
		{
			ConsoleDisplayMode cMode = DisplayMode;
			bool bMax = (cMode & ConsoleDisplayMode.Fullscreen) != 0;
			return GetFontSize(bMax);
		}

		public Coord GetFontSize(bool bMax)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			ConsoleFontInfo cfi = new ConsoleFontInfo();
			if (!WinCon.GetCurrentConsoleFont(handle, bMax, cfi))
			{
				throw new ApplicationException("Unable to get font information.");
			}
			return cfi.dwFontSize;
		}

		#endregion

		#region Fill and Clear

		/// <summary>
		/// Fills character attributes at a given cursor position.
		/// </summary>
		/// <param name="fgColor">The foreground color to use in the fill.</param>
		/// <param name="bgColor">The background color to use in the fill.</param>
		/// <param name="numAttrs">The number of character cells to be filled with the attribute.</param>
		/// <param name="x">The column position where the fill operation is to start.</param>
		/// <param name="y">The row position where the fill operation is to start.</param>
		/// <returns>The number of character cells in which the attribute was written.</returns>
		public int FillAttributeXY(ConsoleColor fgColor, ConsoleColor bgColor, int numAttrs, int x, int y)
		{
			Coord pos = new Coord((short)x, (short)y);
			ConsoleCharAttribute attr = new ConsoleCharAttribute(fgColor, bgColor);
			int attrsWritten = 0;
			if (!WinCon.FillConsoleOutputAttribute(handle, attr, numAttrs, pos, ref attrsWritten))
			{
				throw new ApplicationException("Error writing attributes");
			}
			return attrsWritten;
		}

		/// <summary>
		/// Fills characters at a given cursor position.
		/// </summary>
		/// <param name="c">The character to write.</param>
		/// <param name="numChars">The number of character cells to be filled with the character.</param>
		/// <param name="x">The column position where the fill operation is to start.</param>
		/// <param name="y">The row position where the fill operation is to start.</param>
		/// <returns>The number of character cells in which the character was written.</returns>
		public int FillCharXY(Char c, int numChars, int x, int y)
		{
			Coord pos = new Coord((short)x, (short)y);
			int charsWritten = 0;
			if (!WinCon.FillConsoleOutputCharacter(handle, c, numChars, pos, ref charsWritten))
			{
				throw new ApplicationException("Error writing attributes");
			}
			return charsWritten;
		}

		/// <summary>
		/// Fill characters and attributes at a given cursor position.
		/// </summary>
		/// <param name="c">The character to write.</param>
		/// <param name="numChars">The number of character cells to be filled with the character.</param>
		/// <param name="x">The column position where the fill operation is to start.</param>
		/// <param name="y">The row position where the fill operation is to start.</param>
		/// <param name="fgColor">The foreground attribute to use for new characters.</param>
		/// <param name="bgColor">The background attribute to use for new characters.</param>
		/// <returns>The number of character cells in which the character was written.</returns>
		public int FillCharXY(Char c, int numChars, int x, int y, ConsoleColor fgColor, ConsoleColor bgColor)
		{
			FillCharXY(c, numChars, x, y);
			return FillAttributeXY(fgColor, bgColor, numChars, x, y);
		}

		/// <summary>
		/// Clear the entire screen buffer.
		/// </summary>
		public void Clear()
		{
			Clear(' ');
		}

		/// <summary>
		/// Fill the entire screen buffer with the specified character.
		/// </summary>
		/// <param name="c">The character with which to fill the buffer.</param>
		public void Clear(Char c)
		{
			Clear(c, this.ForegroundColor, this.BackgroundColor);
		}

		/// <summary>
		/// Fill the entire screen buffer with the specified character and attributes.
		/// </summary>
		/// <param name="c">The character with which to fill the buffer.</param>
		/// <param name="fgColor">The foreground attribute to use for new characters.</param>
		/// <param name="bgColor">The background attribute to use for new characters.</param>
		public void Clear(Char c, ConsoleColor fgColor, ConsoleColor bgColor)
		{
			Coord sz = GetScreenBufferInfo().dwSize;
			int nChars = sz.X * sz.Y;
			FillCharXY(c, nChars, 0, 0, fgColor, bgColor);
			SetCursorPosition(0, 0);
		}

		#endregion

		#region Writing

		/// <summary>
		/// Writes a string at the current cursor position.
		/// </summary>
		/// <param name="text">The string to be written.</param>
		/// <returns>The number of characters written.</returns>
		public int Write(string text)
		{
			return Write(text.ToCharArray(), text.Length);
		}

		/// <summary>
		/// Writes a string at the current cursor position and moves to the next line.
		/// </summary>
		/// <param name="text">The string to be written.</param>
		/// <returns>The number of characters written, including the newline separators.</returns>
		public int WriteLine(string text)
		{
			return Write(text + "\r\n");
		}

		/// <summary>
		/// Writes an array of chars at the current cursor position.
		/// </summary>
		/// <param name="text">An array containing the characters to be written.</param>
		/// <param name="nChars">The number of characters to be written.</param>
		/// <returns>The number of characters written.</returns>
		public int Write(char[] text, int nChars)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			int charsWritten = 0;
			if (!WinCon.WriteConsole(handle, text, nChars, ref charsWritten, IntPtr.Zero))
			{
				throw new System.IO.IOException("Write error", Marshal.GetLastWin32Error());
			}
			return charsWritten;
		}

		/// <summary>
		/// Writes characters to the buffer at the given position.
		/// The cursor position is not updated.
		/// </summary>
		/// <param name="text">The string to be output.</param>
		/// <param name="x">Column position of the starting location.</param>
		/// <param name="y">Row position of the starting location.</param>
		/// <returns></returns>
		public int WriteXY(string text, int x, int y)
		{
			return WriteXY(text.ToCharArray(), text.Length, x, y);
		}

		/// <summary>
		/// Writes characters from a character array to the screen buffer at the given cursor position.
		/// </summary>
		/// <param name="text">An array containing the characters to be written.</param>
		/// <param name="nChars">The number of characters to be written.</param>
		/// <param name="x">Column position in which to write the first character.</param>
		/// <param name="y">Row position in which to write the first character.</param>
		/// <returns>Returns the number of characters written.</returns>
		public int WriteXY(char[] text, int nChars, int x, int y)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			if (nChars > text.Length)
			{
				throw new ArgumentException("nChars cannot be larger than the array length.");
			}
			int charsWritten = 0;
			Coord writePos = new Coord((short)x, (short)y);
			if (!WinCon.WriteConsoleOutputCharacter(handle, text, nChars, writePos, ref charsWritten))
			{
				throw new System.IO.IOException("Write error", Marshal.GetLastWin32Error());
			}
			return charsWritten;
		}

		/// <summary>
		/// Writes character attributes to the screen buffer at the given cursor position.
		/// </summary>
		/// <param name="attrs">An array of attributes to be written to the screen buffer.</param>
		/// <param name="nattrs">The number of attributes to be written.</param>
		/// <param name="x">Column position in which to write the first attribute.</param>
		/// <param name="y">Row position in which to write the first attribute.</param>
		/// <returns>Returns the number of attributes written.</returns>
		public int WriteAttributesXY(ConsoleCharAttribute[] attrs, int nattrs, int x, int y)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			if (nattrs > attrs.Length)
			{
				throw new ArgumentException("nattrs cannot be larger than the array length");
			}
			int attrsWritten = 0;
			Coord writePos = new Coord((short)x, (short)y);

			if (!WinCon.WriteConsoleOutputAttribute(handle, attrs, attrs.Length, writePos, ref attrsWritten))
			{
				throw new System.IO.IOException("Write error", Marshal.GetLastWin32Error());
			}
			return attrsWritten;
		}

		/// <summary>
		/// Writes character and attribute information to a rectangular portion of the screen buffer.
		/// </summary>
		/// <param name="buff">The array that contains characters and attributes to be written.</param>
		/// <param name="buffX">Column position of the first character to be written from the array.</param>
		/// <param name="buffY">Row position of the first character to be written from the array.</param>
		/// <param name="left">Column position of the top-left corner of the screen buffer area where characters are to be written.</param>
		/// <param name="top">Row position of the top-left corner of the screen buffer area where characters are to be written.</param>
		/// <param name="right">Column position of the bottom-right corner of the screen buffer area where characters are to be written.</param>
		/// <param name="bottom">Row position of the bottom-right corner of the screen buffer area where characters are to be written.</param>
		public void WriteBlock(ConsoleCharInfo[,] buff, int buffX, int buffY, int left, int top, int right, int bottom)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			Coord bufferSize = new Coord((short)buff.GetLength(1), (short)buff.GetLength(0));
			Coord bufferPos = new Coord((short)buffX, (short)buffY);
			SmallRect writeRegion = new SmallRect((short)left, (short)top, (short)right, (short)bottom);
			if (!WinCon.WriteConsoleOutput(handle, buff, bufferSize, bufferPos, writeRegion))
			{
				throw new IOException("Write error.", Marshal.GetLastWin32Error());
			}
		}

		#endregion

		#region Reading

		/// <summary>
		/// Reads characters from the screen buffer, starting at the given position.
		/// </summary>
		/// <param name="nChars">The number of characters to read.</param>
		/// <param name="x">Column position of the first character to read.</param>
		/// <param name="y">Row position of the first character to read.</param>
		/// <returns>A string containing the characters read from the screen buffer.</returns>
		public string ReadXY(int nChars, int x, int y)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			char[] buff = new char[nChars];
			int charsRead = 0;
			if (!WinCon.ReadConsoleOutputCharacter(handle, buff, nChars,
				new Coord((short)x, (short)y), ref charsRead))
			{
				throw new System.IO.IOException("Read error", Marshal.GetLastWin32Error());
			}
			return new string(buff, 0, charsRead);
		}

		/// <summary>
		/// Reads character attributes from the screen buffer, starting at the given position.
		/// </summary>
		/// <param name="nattrs">Number of attributes to read.</param>
		/// <param name="x">Column position of the first attribute to read.</param>
		/// <param name="y">Row position of the first attribute to read.</param>
		/// <returns>An array containing the attributes read from the screen buffer.</returns>
		public ConsoleCharAttribute[] ReadAtrributesXY(int nColors, int x, int y)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			ConsoleCharAttribute[] buff = new ConsoleCharAttribute[nColors];
			int colorsRead = 0;
			if (!WinCon.ReadConsoleOutputAttribute(handle, buff, nColors,
				new Coord((short)x, (short)y), ref colorsRead))
			{
				throw new System.IO.IOException("Read error", Marshal.GetLastWin32Error());
			}
			if (colorsRead < nColors)
			{
				ConsoleCharAttribute[] newBuff = new ConsoleCharAttribute[colorsRead];
				Array.Copy(buff, newBuff, colorsRead);
				return newBuff;
			}
			return buff;
		}

		/// <summary>
		/// Reads a rectangular block of character and attribute information from the screen buffer into the passed array.
		/// </summary>
		/// <param name="buff">The array into which character information is to be placed.</param>
		/// <param name="buffX">The column position in the array where the first character is to be placed.</param>
		/// <param name="buffY">The row position in the array where the first character is to be placed.</param>
		/// <param name="left">Column position of the top-left corner of the screen buffer area from which characters are to be read.</param>
		/// <param name="top">Row position of the top-left corner of the screen buffer area from which characters are to be read.</param>
		/// <param name="right">Column position of the bottom-right corner of the screen buffer area from which characters are to be read.</param>
		/// <param name="bottom">Row position of the bottom-right corner of the screen buffer area from which characters are to be read.</param>
		public void ReadBlock(ConsoleCharInfo[,] buff, int buffX, int buffY, int left, int top, int right, int bottom)
		{
			if (disposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
			// determine size of the buffer
			Coord bufferSize = new Coord((short)buff.GetLength(1), (short)buff.GetLength(0));
			Coord bufferPos = new Coord((short)buffX, (short)buffY);
			SmallRect readRegion = new SmallRect((short)left, (short)top, (short)right, (short)bottom);
			if (!WinCon.ReadConsoleOutput(handle, buff, bufferSize, bufferPos, readRegion))
			{
				throw new IOException("Read error.", Marshal.GetLastWin32Error());
			}
		}

		#endregion
	}
}
