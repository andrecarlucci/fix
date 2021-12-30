// JConsole.cs - Extended character-mode application support.
// Jim Mischel - 2006/08/25
// http://www.mischel.com/diary/2006/09/01.htm
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Mischel.ConsoleDotNet
{
    #region ConsoleControlEvent

    /// <summary>
    /// Provides data for the ConsoleControlEvent event.
    /// </summary>
    public class ConsoleControlEventArgs : System.EventArgs
    {
        private ConsoleControlEventType evType;
        private bool cancel = false;

        /// <summary>
        /// Creates a new instance of the ConsoleControlEventArgs class.
        /// </summary>
        /// <param name="evType">Type of control event.</param>
        public ConsoleControlEventArgs(ConsoleControlEventType evType)
        {
            this.evType = evType;
        }

        /// <summary>
        /// Gets the type of control event.
        /// </summary>
        public ConsoleControlEventType EventType
        {
            get { return evType; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event should be canceled.
        /// If true, the event is canceled.  If false, the event is processed
        /// and the application will terminate.
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
    }

    /// <summary>
    /// Occurs when a console control event is sent.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="ConsoleControlEventArgs"/> that contains the event data.</param>
    public delegate void ConsoleControlEventHandler(object sender, ConsoleControlEventArgs e);

    #endregion

    #region JConsole class
    /// <summary>
	/// Provides an enhanced interface to the Windows console.  Intended to be used
    /// in conjunction with the System.Console class.
	/// </summary>
	public sealed class JConsole
    {
        #region Constructors

        private JConsole()
        {
            // private constructor prevents instantiation
        }

        /// <summary>
        /// Static constructor initializes static fields.
        /// </summary>
        static JConsole()
        {
            // initialize the control handler if a console window exists.
            IntPtr hwnd = WindowHandle;
            if (hwnd != IntPtr.Zero)
            {
                SetupControlHandler();
            }
        }

        #endregion

        #region Attaching and detaching

        /// <summary>
        /// Allocate a new console for the calling process.
        /// </summary>
		public static void AllocConsole()
        {
            if (!WinCon.AllocConsole())
            {
                throw new IOException("Unable to allocate console", Marshal.GetLastWin32Error());
            }
            SetupControlHandler();
        }

        /// <summary>
        /// Process ID to pass to AttachConsole in order to attach to parent console.
        /// </summary>
        public const int AttachParent = -1;

        /// <summary>
        /// Attach the calling process to the console of the specified process.
        /// </summary>
        /// <param name="processId">Identifier of the process whose console will be attached.</param>
		public static void AttachConsole(int processId)
        {
            if (!WinCon.AttachConsole(processId))
            {
                int err = Marshal.GetLastWin32Error();
                throw new IOException(String.Format("Unable to attach console 0x{0,X}", processId), err);
            }
            SetupControlHandler();
        }

        /// <summary>
        /// Detaches the calling process from its console.
        /// </summary>
		public static void FreeConsole()
        {
            ReleaseControlHandler();
            if (!WinCon.FreeConsole())
            {
                throw new IOException("Unable to free console", Marshal.GetLastWin32Error());
            }
        }

        // Delegate used to install the control handler.
        private static ConsoleCtrlHandlerDelegate HandlerRoutine = null;

        // Install the control handler.
        private static void SetupControlHandler()
        {
            HandlerRoutine = new ConsoleCtrlHandlerDelegate(ControlHandler);
            if (!WinCon.SetConsoleCtrlHandler(HandlerRoutine, true))
            {
                throw new IOException("Unable to set handler routine.", Marshal.GetLastWin32Error());
            }
        }

        // Uninstall the control handler.
        private static void ReleaseControlHandler()
        {
            if (HandlerRoutine != null)
            {
                if (!WinCon.SetConsoleCtrlHandler(HandlerRoutine, false))
                {
                    throw new IOException("Unable to clear handler routine.", Marshal.GetLastWin32Error());
                }
                HandlerRoutine = null;
            }
        }

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Gets the window handle of the attached console
        /// </summary>
        /// <returns>The window handle of the attached console window.  If no console is
        /// attached, the function returns IntPtr.Zero.</returns>
        public static IntPtr WindowHandle
        {
            get { return WinCon.GetConsoleWindow(); }
        }

        /// <summary>
        /// Gets the list of processes that are attached to the console.
        /// </summary>
        /// <returns>Returns an array of integer process ids.</returns>
		public static int[] GetProcessList()
        {
            int[] processes = new int[10];
            do
            {
                int rslt = WinCon.GetConsoleProcessList(processes, processes.Length);
                if (rslt == 0)
                {
                    throw new IOException("Unable to get process list", Marshal.GetLastWin32Error());
                }
                if (rslt <= processes.Length)
                {
                    // if the array is exactly the right size, return it
                    if (rslt == processes.Length)
                    {
                        return processes;
                    }

                    // otherwise create a new array of the required length
                    int[] newProcesses = new int[rslt];
                    Array.Copy(processes, newProcesses, rslt);
                    return newProcesses;
                }
                else
                {
                    // The initial array was too small.
                    // Allocate more space and try again.
                    processes = new int[rslt];
                }
            } while (true);
        }

        #endregion

        #region Aliases

        /// <summary>
        /// Add a console alias string and target text.
        /// </summary>
        /// <param name="Source">The source text.</param>
        /// <param name="Target">The target (replacement) text.</param>
        /// <param name="ExeName">The name of the executable file for which the alias is
        /// to be defined.</param>
        public static void AddAlias(string Source, string Target, string ExeName)
        {
            if (!WinCon.AddConsoleAlias(Source, Target, ExeName))
            {
                throw new IOException("Unable to add alias", Marshal.GetLastWin32Error());
            }
        }

        private static int GetAliasesLength(string ExeName)
        {
            return WinCon.GetConsoleAliasesLength(ExeName);
        }

        /// <summary>
        /// Retrieves the text for the specified console alias and executable.
        /// </summary>
        /// <param name="Source">The console alias whose text is to be retrieved.</param>
        /// <param name="ExeName">The name of the executable file.</param>
        /// <returns>On success, the return value is a non-null, non-empty string.
        /// If the Source and ExeName do not specify a valid alias, null is returned.</returns>
		public static string GetAlias(string Source, string ExeName)
        {
            // The only reliable way to allocate enough space is to the
            // combined length for all aliases.
            int length = GetAliasesLength(ExeName);
            if (length == 0)
            {
                // no aliases defined, so just return null
                return null;
            }

            char[] buff = new char[length];
            int rslt = WinCon.GetConsoleAlias(Source, buff, buff.Length, ExeName);
            if (rslt == 0)
            {
                int err = Marshal.GetLastWin32Error();
                // GetConsoleAlias fails if it can't find the alias.
                // GetLastError returns 31 in that case.
                if (err == 31) // ERROR_GEN_FAILURE
                {
                    return null;
                }
                throw new IOException("Unable to get alias", err);
            }

            // Documentation for GetConsoleAlias says that the return value is non-zero
            // on success.  It appears that the return value is the number of characters
            // copied to the buffer (including the 0 terminator), but I can't be
            // sure that's reliable.
            // go look for the terminator.
            int i;
            for (i = 0; i < buff.Length; i++)
            {
                if (buff[i] == '\0')
                    break;
            }

            return new string(buff, 0, i);
        }

        /// <summary>
        /// Retrieves all defined aliases for the specified executable.
        /// </summary>
        /// <param name="ExeName">The executable file whose aliases are to be retrieved.</param>
        /// <returns>Returns a NameValueCollection that contains the source and target
        /// (replacement) strings for all defined aliases.</returns>
		public static NameValueCollection GetAliases(string ExeName)
        {
            int length = GetAliasesLength(ExeName);
            NameValueCollection aliases = new NameValueCollection();
            if (length > 0)
            {
                char[] buff = new char[length];
                if (WinCon.GetConsoleAliases(buff, length, ExeName) == 0)
                {
                    throw new IOException("Unable to retrieve alias strings", Marshal.GetLastWin32Error());
                }
                // The returned buffer contains a series of nul-terminated strings
                // of the form source=target\0source=target\0
                // Parse the strings and add them to the aliases collection.
                int startIndex = 0;
                string source = "";
                string target = "";
                bool bSource = true;
                for (int i = 0; i < length; i++)
                {
                    if (bSource)
                    {
                        // searching for source string
                        if (buff[i] == '=')
                        {
                            source = new string(buff, startIndex, i - startIndex);
                            startIndex = i + 1;
                            bSource = false;
                        }
                    }
                    else
                    {
                        // searching for target string
                        if (buff[i] == '\0')
                        {
                            target = new string(buff, startIndex, i - startIndex);
                            startIndex = i + 1;
                            aliases.Add(source, target);
                            bSource = true;
                        }
                    }
                }
            }
            return aliases;
        }

        /// <summary>
        /// Retrieves the names of all executable files that have defined console aliases.
        /// </summary>
        /// <returns>A StringCollection that contains one entry for each executable
        /// file that has console aliases.</returns>
		public static StringCollection GetAliasExes()
        {
            StringCollection strings = new StringCollection();

            int length = GetAliasExesLength();
            if (length > 0)
            {
                char[] buff = new char[length];
                if (WinCon.GetConsoleAliasExes(buff, length) == 0)
                {
                    throw new IOException("Unable to get alias exes", Marshal.GetLastWin32Error());
                }
                // we have a buffer of nul-terminated strings
                // create individual strings
                int startIndex = 0;
                for (int i = 0; i < length; i++)
                {
                    if (buff[i] == '\0')
                    {
                        string s = new string(buff, startIndex, i - startIndex);
                        strings.Add(s);
                        startIndex = i + 1;
                    }
                }
            }
            return strings;
        }

        private static int GetAliasExesLength()
        {
            return WinCon.GetConsoleAliasExesLength();
        }
        #endregion

        #region Control events


        /// <summary>
        /// Occurs when a console control event is generated.
        /// </summary>
        public static event ConsoleControlEventHandler ControlEvent;

        // Console control event handler raises the ConsoleControlEvent.
        // This method is called by the console API when a control event occurs.
        private static bool ControlHandler(ConsoleControlEventType ctrlType)
        {
            switch (ctrlType)
            {
                case ConsoleControlEventType.CtrlC:
                case ConsoleControlEventType.CtrlBreak:
                case ConsoleControlEventType.CtrlClose:
                case ConsoleControlEventType.CtrlLogoff:
                case ConsoleControlEventType.CtrlShutdown:
                    if (ControlEvent != null)
                    {
                        ConsoleControlEventArgs e = new ConsoleControlEventArgs(ctrlType);
                        ControlEvent(null, e);
                        return e.Cancel;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Generate a console control event.
        /// </summary>
        /// <param name="eventId">The event to generate.  This must be CtrlC or CtrlBreak.</param>
        /// <param name="processGroupId">In most cases, set to 0 to send the event to
        /// all attached processes.</param>
        public static void GenerateCtrlEvent(ConsoleControlEventType eventId, int processGroupId)
        {
            if (!WinCon.GenerateConsoleCtrlEvent((int)eventId, processGroupId))
            {
                throw new IOException("Error generating event.", Marshal.GetLastWin32Error());
            }
        }
        #endregion

        #region Screen buffers

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
            ConsoleScreenBuffer sb = new ConsoleScreenBuffer(outHandle);
            sb.ownsHandle = true;
            return sb;
        }

        /// <summary>
        /// Sets the active console screen buffer to the buffer referrenced by the sb parameter
        /// </summary>
        /// <param name="sb">The screen buffer that will become the active screen buffer.</param>
        public static void SetActiveScreenBuffer(ConsoleScreenBuffer sb)
        {
            if (!WinCon.SetConsoleActiveScreenBuffer(sb.Handle))
            {
                throw new IOException("Error setting active screen buffer.", Marshal.GetLastWin32Error());
            }
        }
        #endregion

        #region Input buffer

        /// <summary>
        /// Opens the screen buffer.
        /// </summary>
        /// <returns>Returns a new <see cref="ConsoleInputBuffer"/> instance that references the
        /// console's input buffer.</returns>
        /// <remarks>This method allocates a new ConsoleInputBuffer instance.  Callers should
        /// call Dispose on the returned instance when they're done with it.</remarks>
        static public ConsoleInputBuffer GetInputBuffer()
        {
            IntPtr inHandle = WinApi.CreateFile("CONIN$",
                WinApi.GENERIC_READ | WinApi.GENERIC_WRITE,
                WinApi.FILE_SHARE_READ | WinApi.FILE_SHARE_WRITE,
                null,
                WinApi.OPEN_EXISTING,
                0,
                IntPtr.Zero);
            if (inHandle.ToInt32() == WinApi.INVALID_HANDLE_VALUE)
            {
                throw new IOException("Unable to open CONIN$", Marshal.GetLastWin32Error());
            }
            ConsoleInputBuffer inputBuffer = new ConsoleInputBuffer(inHandle);
            inputBuffer.ownsHandle = true;

            return inputBuffer;
        }
        #endregion

        #region Standard handles

        /// <summary>
        /// Gets a value indicating whether the standard input handle is redirected.
        /// </summary>
        /// <returns>Returns True if the standard input handle is redirected to a file.
        /// Returns False if the standard input handle references the console input buffer.</returns>
        public static bool IsStdInRedirected()
        {
            return !IsCharHandle(StdInHandle);
        }

        /// <summary>
        /// Gets a value indicating whether the standard output handle is redirected.
        /// </summary>
        /// <returns>Returns True if the standard output handle is redirected to a file.
        /// Returns False if the standard output handle references a console screen buffer.</returns>
        public static bool IsStdOutRedirected()
        {
            return !IsCharHandle(StdOutHandle);
        }

        /// <summary>
        /// Gets a value indicating whether the standard error handle is redirected.
        /// </summary>
        /// <returns>Returns True if the standard error handle is redirected to a file.
        /// Returns False if the standard error handle references a console screen buffer.</returns>
        public static bool IsStdErrRedirected()
        {
            return !IsCharHandle(StdErrHandle);
        }

        /// <summary>
        /// Gets or sets the standard input handle.
        /// </summary>
        public static IntPtr StdInHandle
        {
            get { return WinCon.GetStdHandle(WinCon.STD_INPUT_HANDLE); }
            set
            {
                if (!WinCon.SetStdHandle(WinCon.STD_INPUT_HANDLE, value))
                {
                    throw new IOException("Error setting standard input handle.", Marshal.GetLastWin32Error());
                }
            }
        }

        /// <summary>
        /// Gets or sets the standard output handle.
        /// </summary>
        public static IntPtr StdOutHandle
        {
            get { return WinCon.GetStdHandle(WinCon.STD_OUTPUT_HANDLE); }
            set
            {
                if (!WinCon.SetStdHandle(WinCon.STD_OUTPUT_HANDLE, value))
                {
                    throw new IOException("Error setting standard output handle.", Marshal.GetLastWin32Error());
                }
            }
        }

        /// <summary>
        /// Gets or sets the standard error handle
        /// </summary>
        public static IntPtr StdErrHandle
        {
            get { return WinCon.GetStdHandle(WinCon.STD_ERROR_HANDLE); }
            set
            {
                if (!WinCon.SetStdHandle(WinCon.STD_ERROR_HANDLE, value))
                {
                    throw new IOException("Error setting standard error handle.", Marshal.GetLastWin32Error());
                }
            }
        }

        private static bool IsCharHandle(IntPtr handle)
        {
            return (WinApi.GetFileType(handle) == WinApi.FILE_TYPE_CHAR);
        }

        #endregion
    }

    #endregion
}
