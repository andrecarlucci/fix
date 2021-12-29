using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using System.Threading;

namespace Mischel.ConsoleDotNet
{
    #region Key Event

    /// <summary>
    /// Holds data for the ConsoleKey event.
    /// </summary>
    public class ConsoleKeyEventArgs : EventArgs
    {
        private bool bKeyDown;
        private int repeatCount;
        private ConsoleKey virtualKeyCode;
        private int virtualScanCode;
        private char uChar;
        private byte asciiChar;

        private ConsoleControlKeyState keyState;

        public ConsoleKeyEventArgs()
        {
        }

        internal ConsoleKeyEventArgs(ref ConsoleKeyEventInfo keyEvent)
        {
            bKeyDown = keyEvent.KeyDown;
            repeatCount = keyEvent.RepeatCount;
            virtualKeyCode = keyEvent.VirtualKeyCode;
            virtualScanCode = keyEvent.VirtualScanCode;
            uChar = keyEvent.UnicodeChar;
            asciiChar = keyEvent.AsciiChar;
            keyState = keyEvent.ControlKeyState;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a key down or key up event.
        /// </summary>
        public bool KeyDown
        {
            get { return bKeyDown; }
            set { bKeyDown = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that a key is being held down.
        /// </summary>
        public int RepeatCount
        {
            get { return repeatCount; }
            set { repeatCount = value; }
        }

        /// <summary>
        /// Gets or sets a value that identifies the given key in a device-independent manner.
        /// </summary>
        public ConsoleKey Key
        {
            get { return virtualKeyCode; }
            set { virtualKeyCode = value; }
        }

        /// <summary>
        /// Gets or sets the hardware-dependent virtual scan code.
        /// </summary>
        public int VirtualScanCode
        {
            get { return virtualScanCode; }
            set { virtualScanCode = value; }
        }

        /// <summary>
        /// Gets or sets the Unicode character for this key event.
        /// </summary>
        public char KeyChar
        {
            get { return uChar; }
            set { uChar = value; }
        }

        /// <summary>
        /// Gets or sets the ASCII character for this key event.
        /// </summary>
        public byte AsciiChar
        {
            get { return asciiChar; }
            set { asciiChar = value; }
        }

        /// <summary>
        /// Gets or sets a value specifying the control key state for this key event.
        /// </summary>
        public ConsoleControlKeyState ControlKeyState
        {
            get { return keyState; }
            set { keyState = value; }
        }
    }

    /// <summary>
    /// Occurs when a key is pressed or released on the console.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="ConsoleKeyEventArgs"/> that contains the event data.</param>
    public delegate void ConsoleKeyEventHandler(object sender, ConsoleKeyEventArgs e);

    #endregion

    #region Mouse Event

    public class ConsoleMouseEventArgs : EventArgs
    {
        private int x;
        private int y;
        private ConsoleMouseButtonState buttonState;
        private ConsoleControlKeyState keyState;
        private ConsoleMouseEventType eventFlags;

        public ConsoleMouseEventArgs(ConsoleMouseEventType eventFlags, int x, int y,
            ConsoleMouseButtonState buttonState, ConsoleControlKeyState keyState)
        {
            this.eventFlags = eventFlags;
            this.x = x;
            this.y = y;
            this.buttonState = buttonState;
            this.keyState = keyState;
        }

        internal ConsoleMouseEventArgs(ref ConsoleMouseEventInfo info)
        {
            x = info.MousePosition.X;
            y = info.MousePosition.Y;
            buttonState = info.ButtonState;
            keyState = info.ControlKeyState;
            eventFlags = info.EventFlags;
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public ConsoleMouseButtonState ButtonState
        {
            get { return buttonState; }
            set { buttonState = value; }
        }

        public ConsoleControlKeyState KeyState
        {
            get { return keyState; }
            set { keyState = value; }
        }

        public ConsoleMouseEventType EventFlags
        {
            get { return eventFlags; }
            set { eventFlags = value; }
        }
    }

    public delegate void ConsoleMouseEventHandler(object sender, ConsoleMouseEventArgs e);
    #endregion

    #region Window Buffer Size Event

    public class ConsoleWindowBufferSizeEventArgs : EventArgs
    {
        private int x;
        private int y;

        public ConsoleWindowBufferSizeEventArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public ConsoleWindowBufferSizeEventArgs(ref ConsoleWindowBufferSizeEventInfo info)
        {
            this.x = info.Size.X;
            this.y = info.Size.Y;
        }

        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
    }

    public delegate void ConsoleBufferSizeEventHandler(object sender, ConsoleWindowBufferSizeEventArgs e);

    #endregion

    public class ConsoleFocusEventArgs : EventArgs
    {
        private bool bFocus;

        public ConsoleFocusEventArgs(bool bFocus)
        {
            this.bFocus = bFocus;
        }

        public bool SetFocus
        {
            get { return bFocus; }
            set { bFocus = value; }
        }
    }

    public delegate void ConsoleFocusEventHandler(object sender, ConsoleFocusEventArgs e);

    public class ConsoleMenuEventArgs : EventArgs
    {
        private int commandId;

        public ConsoleMenuEventArgs(int commandId)
        {
            this.commandId = commandId;
        }

        public int CommandId
        {
            get { return commandId; }
            set { commandId = value; }
        }
    }

    public delegate void ConsoleMenuEventHandler(object sender, ConsoleMenuEventArgs e);

    /// <summary>
	/// Provides full access to Windows Console input functionality.
	/// </summary>
	public sealed class ConsoleInputBuffer : IDisposable
    {
        private IntPtr handle = IntPtr.Zero;
        internal bool ownsHandle = false;

        #region Construction and destruction
        // Constructor is marked internal because external programs
        // shouldn't ever need to create a ConsoleInputBuffer.
        internal ConsoleInputBuffer(IntPtr aHandle)
        {
            handle = aHandle;
            ownsHandle = false;
        }

        #region IDisposable Members


        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    if (handle != IntPtr.Zero && ownsHandle)
                    {
                        WinApi.CloseHandle(handle);
                        handle = IntPtr.Zero;
                    }
                }
            }
            disposed = true;
        }

        #endregion

        ~ConsoleInputBuffer()
        {
            Dispose(false);
        }
        #endregion

        public IntPtr Handle
        {
            get { return handle; }
        }

        #region Input Mode

        private bool GetModeFlag(ConsoleInputModeFlags flag)
        {
            return (InputMode & flag) != 0;
        }

        private void SetModeFlag(ConsoleInputModeFlags flag, bool val)
        {
            if (val)
                InputMode = InputMode | flag;
            else
                InputMode = InputMode & ~flag;
        }

        /// <summary>
        /// Control keys are processed by the system.  Ctrl+C generates a
        /// console control event.  If LineInput is also enabled, backspace,
        /// carriage return, and linefeed characters are processed by the system.
        /// </summary>
        public bool ProcessedInput
        {
            get { return GetModeFlag(ConsoleInputModeFlags.ProcessedInput); }
            set { SetModeFlag(ConsoleInputModeFlags.ProcessedInput, value); }
        }

        /// <summary>
        /// Read operations (ReadFile or ReadConsole) return only when a carriage
        /// return character is detected.  If this mode is disabled, the read
        /// functions return when one or more characters are available.
        /// </summary>
		public bool LineInput
        {
            get { return GetModeFlag(ConsoleInputModeFlags.LineInput); }
            set { SetModeFlag(ConsoleInputModeFlags.LineInput, value); }
        }

        /// <summary>
        /// Characters read by the ReadFile or ReadConsole function are written to
        /// the active screen buffer as they are read. This mode can be used only
        /// if the LineInput mode is also enabled.
        /// </summary>
		public bool EchoInput
        {
            get { return GetModeFlag(ConsoleInputModeFlags.EchoInput); }
            set { SetModeFlag(ConsoleInputModeFlags.EchoInput, value); }
        }

        /// <summary>
        /// User interactions that change the size of the console screen buffer
        /// are reported in the console's input buffer. 
        /// </summary>
		public bool WindowInput
        {
            get { return GetModeFlag(ConsoleInputModeFlags.WindowInput); }
            set { SetModeFlag(ConsoleInputModeFlags.WindowInput, value); }
        }

        /// <summary>
        /// When enabled, if the mouse pointer is within the borders of the console
        /// window and the window has the keyboard focus, mouse events generated by
        /// mouse movement and button presses are placed in the input buffer.
        /// </summary>
		public bool MouseInput
        {
            get { return GetModeFlag(ConsoleInputModeFlags.MouseInput); }
            set { SetModeFlag(ConsoleInputModeFlags.MouseInput, value); }
        }

        // To set InsertMode or QuickEditMode, be sure
        // to combine with ExtendedFlags
        /// <summary>
        /// When enabled, text entered in a console window will be inserted at the current
        /// cursor location and all text following that location will not be overwritten.
        /// When disabled, all following text will be overwritten. The ExtendedFlags
        /// mode must be included in order to enable this flag.
        /// </summary>
        public bool InsertMode
        {
            get { return GetModeFlag(ConsoleInputModeFlags.InsertMode | ConsoleInputModeFlags.ExtendedFlags); }
            set
            {
                // Must set ExtendedFlags.  But don't reset that flag.
                if (value)
                    SetModeFlag(ConsoleInputModeFlags.InsertMode | ConsoleInputModeFlags.ExtendedFlags, value);
                else
                    SetModeFlag(ConsoleInputModeFlags.InsertMode, value);
            }
        }

        /// <summary>
        /// This flag enables the user to use the mouse to select and edit text. To enable
        /// this option, you must also set the ExtendedFlags flag.
        /// </summary>
		public bool QuickEditMode
        {
            get { return GetModeFlag(ConsoleInputModeFlags.QuickEditMode | ConsoleInputModeFlags.ExtendedFlags); }
            set
            {
                // Must set ExtendedFlags.  But don't reset that flag.
                if (value)
                    SetModeFlag(ConsoleInputModeFlags.QuickEditMode | ConsoleInputModeFlags.ExtendedFlags, value);
                else
                    SetModeFlag(ConsoleInputModeFlags.QuickEditMode, value);
            }
        }

        // Unknown flag
        // AutoPosition = 256

        public ConsoleInputModeFlags InputMode
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
                    throw new IOException("Unable to get console mode.", Marshal.GetLastWin32Error());
                }
                return (ConsoleInputModeFlags)mode;
            }
            set
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                if (!WinCon.SetConsoleMode(handle, (int)value))
                {
                    int err = Marshal.GetLastWin32Error();
                    throw new IOException("Unable to set console mode.", err);
                }
            }
        }

        #endregion

        public void Flush()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }

            if (!WinCon.FlushConsoleInputBuffer(Handle))
            {
                throw new System.IO.IOException("Error flushing buffer", Marshal.GetLastWin32Error());
            }
        }

        public int NumMouseButtons
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }

                int numMouseButtons = 0;
                if (!WinCon.GetNumberOfConsoleMouseButtons(ref numMouseButtons))
                {
                    throw new System.IO.IOException("Unable to get number of mouse buttons.", Marshal.GetLastWin32Error());
                }
                return numMouseButtons;
            }
        }

        #region Selection
        private ConsoleSelectionInfo GetSelectionInfo()
        {
            ConsoleSelectionInfo csi = new ConsoleSelectionInfo();
            if (!WinCon.GetConsoleSelectionInfo(csi))
            {
                throw new System.IO.IOException("Unable to get selection info.", Marshal.GetLastWin32Error());
            }
            return csi;
        }

        public ConsoleSelectionFlags SelectionFlags
        {
            get
            {
                ConsoleSelectionInfo csi = GetSelectionInfo();
                return (ConsoleSelectionFlags)(csi.dwFlags);
            }
        }

        public Coord SelectionAnchor
        {
            get { return GetSelectionInfo().dwSelectionAnchor; }
        }

        public SmallRect SelectionRect
        {
            get { return GetSelectionInfo().srSelection; }
        }

        #endregion

        #region KeyAvailable and ReadKey
        /// <summary>
        /// Get a value indicating whether a key is available at the console.
        /// </summary>
        public bool KeyAvailable
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                // Peek at the input buffer, getting a copy of all events.
                int nEvents = NumInputEvents;
                ConsoleInputEventInfo[] buff = new ConsoleInputEventInfo[nEvents];
                int numRead = 0;
                if (!WinCon.PeekConsoleInput(handle, buff, nEvents, ref numRead))
                {
                    throw new IOException("Erorr reading input events.", Marshal.GetLastWin32Error());
                }

                // Go through the available events looking for a key event.
                // This uses the same logic as ReadKey.
                foreach (ConsoleInputEventInfo inpEvent in buff)
                {
                    if (inpEvent.EventType == ConsoleInputEventType.KeyEvent)
                    {
                        ConsoleKeyEventInfo keyEvent = inpEvent.KeyEvent;
                        bool isGood = keyEvent.KeyDown && (Enum.GetName(typeof(ConsoleKey), (ConsoleKey)keyEvent.VirtualKeyCode) != null);
                        if (isGood)
                            return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Obtains the next character or function key pressed by the user.
        /// The pressed key is displayed in the console window. 
        /// </summary>
        /// <returns>A ConsoleKeyInfo object that describes the ConsoleKey constant and Unicode
        /// character, if any, that correspond to the pressed console key.></returns>
        public ConsoleKeyInfo ReadKey()
        {
            return ReadKey(false);
        }

        /// <summary>
        /// Obtains the next character or function key pressed by the user.
        /// The pressed key is optionally displayed in the console window. 
        /// </summary>
        /// <param name="intercept">Determines whether to display the pressed key in the console window. 
        /// true to not display the pressed key; otherwise, false.</param>
        /// <returns>A ConsoleKeyInfo object that describes the ConsoleKey constant and Unicode
        /// character, if any, that correspond to the pressed console key.></returns>
        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            // @TODO: How to I echo the key?
            ConsoleInputEventInfo[] buff = new ConsoleInputEventInfo[1];
            int numEvents = 0;
            bool isGood = false;
            ConsoleKeyEventInfo keyEvent = new ConsoleKeyEventInfo();
            do
            {
                if (!WinCon.ReadConsoleInput(Handle, buff, 1, ref numEvents))
                {
                    throw new IOException("Error reading console.", Marshal.GetLastWin32Error());
                }
                // Make sure it's a key down event and that the key is
                // one of the virtual key codes....
                if (buff[0].EventType == ConsoleInputEventType.KeyEvent)
                {
                    keyEvent = buff[0].KeyEvent;
                    isGood = keyEvent.KeyDown && (Enum.GetName(typeof(ConsoleKey), (ConsoleKey)keyEvent.VirtualKeyCode) != null);
                }
            } while (!isGood);

            // Create ConsoleKeyInfo from key event
            return new ConsoleKeyInfo(keyEvent.UnicodeChar,
                (ConsoleKey)keyEvent.VirtualKeyCode,
                (keyEvent.ControlKeyState & ConsoleControlKeyState.ShiftPressed) != 0,
                (keyEvent.ControlKeyState & (ConsoleControlKeyState.RightAltPressed | ConsoleControlKeyState.LeftAltPressed)) != 0,
                (keyEvent.ControlKeyState & (ConsoleControlKeyState.RightCtrlPressed | ConsoleControlKeyState.LeftCtrlPressed)) != 0);
        }
        #endregion

        #region Read and ReadLine

        // Buffer for Read and ReadLine input
        private StringBuilder sBuffer = new StringBuilder();
        private int iString = 0;

        private void ReadLineFromConsole()
        {
            // Magic number of 26608 was arrived at experimentally.
            // ReadConsole fails if you try to read more than that many characters.
            char[] buff = new char[26608];
            int charsToRead = buff.Length;
            int charsRead = 0;
            if (!WinCon.ReadConsole(handle, buff, charsToRead, ref charsRead, IntPtr.Zero))
            {
                int err = Marshal.GetLastWin32Error();
                throw new IOException("Error reading console input buffer", err);
            }
            sBuffer.Length = 0;
            sBuffer.Append(buff, 0, charsRead);
        }

        public string ReadLine()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }

            if (iString >= sBuffer.Length)
            {
                ReadLineFromConsole();
            }
            // strip the carriage return/line feed (if it exists) from the string
            int i = sBuffer.Length;
            if (i > 0 && sBuffer[i - 1] == '\n')
            {
                i--;
                if (i > 0 && sBuffer[i - 1] == '\r')
                    i--;
                sBuffer.Length = i;
            }
            string sRet = string.Empty;
            if (iString < sBuffer.Length)
            {
                sRet = sBuffer.ToString(iString, sBuffer.Length - iString);
            }
            iString = 0;
            sBuffer.Length = 0;
            return sRet;
        }

        public int Read()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (iString >= sBuffer.Length)
            {
                ReadLineFromConsole();
            }
            int ret = Convert.ToInt32(sBuffer[iString]);
            iString++;
            return ret;
        }
        #endregion

        #region Peek, Read, Write

        /// <summary>
        /// Get the number of events currently waiting in the input buffer.
        /// </summary>
        public int NumInputEvents
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(this.ToString());
                }
                int numEvents = 0;
                if (!WinCon.GetNumberOfConsoleInputEvents(Handle, ref numEvents))
                {
                    throw new IOException("Unable to get input events count.", Marshal.GetLastWin32Error());
                }
                return numEvents;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nEvents"></param>
        /// <returns></returns>
        public ConsoleInputEventInfo[] PeekEvents(int nEvents)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }

            ConsoleInputEventInfo[] events = new ConsoleInputEventInfo[nEvents];
            int eventsRead = 0;
            if (!WinCon.PeekConsoleInput(Handle, events, nEvents, ref eventsRead))
            {
                throw new System.IO.IOException("Unable to peek events.", Marshal.GetLastWin32Error());
            }
            if (eventsRead < nEvents)
            {
                // create a new array that contains just the events that were read
                ConsoleInputEventInfo[] newBuff = new ConsoleInputEventInfo[eventsRead];
                Array.Copy(events, newBuff, eventsRead);
                events = newBuff;
            }

            return events;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nEvents"></param>
        /// <returns></returns>
		public ConsoleInputEventInfo[] ReadEvents(int nEvents)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            ConsoleInputEventInfo[] events = new ConsoleInputEventInfo[nEvents];
            int eventsRead = 0;
            if (!WinCon.ReadConsoleInput(Handle, events, nEvents, ref eventsRead))
            {
                throw new System.IO.IOException("Unable to read events.", Marshal.GetLastWin32Error());
            }
            if (eventsRead < nEvents)
            {
                // create a new array that contains just the events that were read
                ConsoleInputEventInfo[] newBuff = new ConsoleInputEventInfo[eventsRead];
                Array.Copy(events, newBuff, eventsRead);
                events = newBuff;
            }

            return events;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Events"></param>
        /// <param name="nEvents"></param>
        /// <returns></returns>
		private int WriteEvents(ConsoleInputEventInfo[] Events, int nEvents)
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.ToString());
            }
            if (nEvents > Events.Length)
            {
                throw new ArgumentException("Count cannot be larger than array size.", "nEvents");
            }
            int eventsWritten = 0;
            if (!WinCon.WriteConsoleInput(Handle, Events, nEvents, ref eventsWritten))
            {
                throw new IOException("Unable to write events.", Marshal.GetLastWin32Error());
            }
            return eventsWritten;
        }

        public void WriteEvents(EventArgs[] events, int nEvents)
        {
            // convert EventArgs to ConsoleInputEventInfo structures.
            ConsoleInputEventInfo[] consoleEvents = new ConsoleInputEventInfo[nEvents];
            for (int i = 0; i < nEvents; i++)
            {
                EventArgs e = events[i];
                ConsoleInputEventInfo ce = new ConsoleInputEventInfo();

                if (e is ConsoleKeyEventArgs)
                {
                    ConsoleKeyEventArgs eKey = e as ConsoleKeyEventArgs;
                    ce.EventType = ConsoleInputEventType.KeyEvent;
                    ce.KeyEvent.UnicodeChar = eKey.KeyChar;
                    ce.KeyEvent.ControlKeyState = eKey.ControlKeyState;
                    ce.KeyEvent.KeyDown = eKey.KeyDown;
                    ce.KeyEvent.RepeatCount = (short)eKey.RepeatCount;
                    ce.KeyEvent.VirtualKeyCode = eKey.Key;
                    ce.KeyEvent.VirtualScanCode = (short)eKey.VirtualScanCode;
                }
                else if (e is ConsoleMouseEventArgs)
                {
                    ConsoleMouseEventArgs eMouse = e as ConsoleMouseEventArgs;
                    ce.EventType = ConsoleInputEventType.MouseEvent;
                    ce.MouseEvent.ButtonState = eMouse.ButtonState;
                    ce.MouseEvent.ControlKeyState = eMouse.KeyState;
                    ce.MouseEvent.EventFlags = eMouse.EventFlags;
                    ce.MouseEvent.MousePosition = new Coord((short)eMouse.X, (short)eMouse.Y);
                }
                else if (e is ConsoleFocusEventArgs)
                {
                    ConsoleFocusEventArgs eFocus = e as ConsoleFocusEventArgs;
                    ce.EventType = ConsoleInputEventType.FocusEvent;
                    ce.FocusEvent.SetFocus = eFocus.SetFocus;
                }
                else if (e is ConsoleMenuEventArgs)
                {
                    ConsoleMenuEventArgs eMenu = e as ConsoleMenuEventArgs;
                    ce.EventType = ConsoleInputEventType.MenuEvent;
                    ce.MenuEvent.CommandId = eMenu.CommandId;
                }
                else if (e is ConsoleWindowBufferSizeEventArgs)
                {
                    ConsoleWindowBufferSizeEventArgs eWindow = e as ConsoleWindowBufferSizeEventArgs;
                    ce.EventType = ConsoleInputEventType.WindowBufferSizeEvent;
                    ce.WindowBufferSizeEvent.Size = new Coord((short)eWindow.X, (short)eWindow.Y);
                }
                else
                {
                    throw new ApplicationException("Unknown event type.");
                }
                consoleEvents[i] = ce;
            }
            WriteEvents(consoleEvents, consoleEvents.Length);
        }

        public void WriteEvents(EventArgs[] events)
        {
            WriteEvents(events, events.Length);
        }

        #endregion

        #region Events

        public event ConsoleKeyEventHandler KeyDown;
        public event ConsoleKeyEventHandler KeyUp;
        public event ConsoleMouseEventHandler MouseButton;
        public event ConsoleMouseEventHandler MouseMove;
        public event ConsoleMouseEventHandler MouseDoubleClick;
        public event ConsoleMouseEventHandler MouseScroll;
        public event ConsoleBufferSizeEventHandler BufferSizeChange;
        public event ConsoleFocusEventHandler Focus;
        public event ConsoleMenuEventHandler Menu;

        private void OnMouseButton(ConsoleMouseEventArgs e)
        {
            if (MouseButton != null)
            {
                MouseButton(this, e);
            }
        }

        private void OnMouseMove(ConsoleMouseEventArgs e)
        {
            if (MouseMove != null)
            {
                MouseMove(this, e);
            }
        }

        private void OnMouseDoubleClick(ConsoleMouseEventArgs e)
        {
            if (MouseDoubleClick != null)
            {
                MouseDoubleClick(this, e);
            }
        }

        private void OnMouseScroll(ConsoleMouseEventArgs e)
        {
            if (MouseScroll != null)
            {
                MouseScroll(this, e);
            }
        }

        private void OnKeyDown(ConsoleKeyEventArgs e)
        {
            if (KeyDown != null)
            {
                KeyDown(this, e);
            }
        }

        private void OnKeyUp(ConsoleKeyEventArgs e)
        {
            if (KeyUp != null)
            {
                KeyUp(this, e);
            }
        }

        private void OnBufferSizeChange(ConsoleWindowBufferSizeEventArgs e)
        {
            if (BufferSizeChange != null)
            {
                BufferSizeChange(this, e);
            }
        }

        private void OnFocus(ConsoleFocusEventArgs e)
        {
            if (Focus != null)
            {
                Focus(this, e);
            }
        }

        private void OnMenu(ConsoleMenuEventArgs e)
        {
            if (Menu != null)
            {
                Menu(this, e);
            }
        }

        public void ProcessEvents()
        {
            int nEvents = this.NumInputEvents;
            if (nEvents == 0)
                return;
            ConsoleInputEventInfo[] events = ReadEvents(nEvents);
            for (int i = 0; i < events.Length; i++)
            {
                switch (events[i].EventType)
                {
                    case ConsoleInputEventType.KeyEvent:
                        ConsoleKeyEventArgs eKey = new ConsoleKeyEventArgs(ref events[i].KeyEvent);
                        if (eKey.KeyDown)
                            OnKeyDown(eKey);
                        else
                            OnKeyUp(eKey);
                        break;

                    case ConsoleInputEventType.MouseEvent:
                        ConsoleMouseEventArgs eMouse = new ConsoleMouseEventArgs(ref events[i].MouseEvent);
                        if ((((int)eMouse.EventFlags) & 0xfffff) == 0)
                        {
                            eMouse.EventFlags = ConsoleMouseEventType.MouseButton;
                            OnMouseButton(eMouse);
                        }
                        else if ((eMouse.EventFlags & ConsoleMouseEventType.DoubleClick) != 0)
                        {
                            eMouse.EventFlags = ConsoleMouseEventType.DoubleClick;
                            OnMouseDoubleClick(eMouse);
                        }
                        else if ((eMouse.EventFlags & (ConsoleMouseEventType.MouseHWheeled | ConsoleMouseEventType.MouseWheeled)) != 0)
                        {
                            eMouse.EventFlags = eMouse.EventFlags & (ConsoleMouseEventType.MouseHWheeled | ConsoleMouseEventType.MouseWheeled);
                            OnMouseScroll(eMouse);
                        }
                        else if ((eMouse.EventFlags & ConsoleMouseEventType.MouseMoved) != 0)
                        {
                            eMouse.EventFlags = ConsoleMouseEventType.MouseMoved;
                            OnMouseMove(eMouse);
                        }
                        break;

                    case ConsoleInputEventType.WindowBufferSizeEvent:
                        ConsoleWindowBufferSizeEventArgs eBuff = new ConsoleWindowBufferSizeEventArgs(ref events[i].WindowBufferSizeEvent);
                        OnBufferSizeChange(eBuff);
                        break;

                    case ConsoleInputEventType.FocusEvent:
                        ConsoleFocusEventArgs eFocus = new ConsoleFocusEventArgs(events[i].FocusEvent.SetFocus);
                        OnFocus(eFocus);
                        break;

                    case ConsoleInputEventType.MenuEvent:
                        ConsoleMenuEventArgs eMenu = new ConsoleMenuEventArgs(events[i].MenuEvent.CommandId);
                        OnMenu(eMenu);
                        break;
                }
            }
        }

        #endregion

    }
}
