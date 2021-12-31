// JConsole.cs - Extended character-mode application support.
// Jim Mischel - 2006/08/25
// http://www.mischel.com/diary/2006/09/01.htm
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Mischel.ConsoleDotNet
{

    /// <summary>
	/// Provides an enhanced interface to the Windows console.  Intended to be used
    /// in conjunction with the System.Console class.
	/// </summary>
	public sealed class JConsole
    {
        private JConsole()
        {
        }


        /// <summary>
        /// Opens the currently active screen buffer.
        /// </summary>
        /// <returns>A new <see cref="ConsoleScreenBuffer" /> instance that references the currently active
        /// console screen buffer.</returns>
        /// <remarks>This method allocates a new ConsoleScreenBuffer instance.  Callers should
        /// call Dispose on the returned instance when they're done with it.</remarks>
        public static ConsoleScreenBuffer GetActiveScreenBuffer()
        {
            // CONOUT$ always references the current active screen buffer.
            // NOTE:  *MUST* specify GENERIC_READ | GENERIC_WRITE.  Otherwise
            // the console API calls will fail with Win32 error INVALID_HANDLE_VALUE.
            // Also must include the file sharing flags or CreateFile will fail.
            IntPtr outHandle = WinApi.CreateFile("CONOUT$",
                WinApi.GENERIC_READ | WinApi.GENERIC_WRITE,
                WinApi.FILE_SHARE_READ | WinApi.FILE_SHARE_WRITE,
                null,
                WinApi.OPEN_EXISTING,
                0,
                IntPtr.Zero);
            if (outHandle.ToInt32() == WinApi.INVALID_HANDLE_VALUE)
            {
                throw new IOException("Unable to open CONOUT$", Marshal.GetLastWin32Error());
            }
            var sb = new ConsoleScreenBuffer(outHandle)
            {
                ownsHandle = true
            };
            return sb;
        }
    }
}