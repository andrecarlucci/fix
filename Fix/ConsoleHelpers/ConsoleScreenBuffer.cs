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

	}
}
