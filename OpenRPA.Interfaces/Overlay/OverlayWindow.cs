﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenRPA.Interfaces.Overlay
{
    public partial class OverlayWindow : Form
    {
        private bool ClickThrough = true;
        public OverlayWindow(bool ClickThrough)
        {
            this.ClickThrough = ClickThrough;
            InitializeComponent();
            AllowTransparency = true;
            Opacity = 0.5;
        }
        public void SetTimeout(TimeSpan closeAfter)
        {
            tmr = new System.Windows.Forms.Timer();
            tmr.Tick += Tmr_Tick; ;
            tmr.Interval = (int)closeAfter.TotalMilliseconds;
            tmr.Start();
        }
        public void setLocation(System.Drawing.Rectangle rect)
        {
            GenericTools.RunUI(this, () => {
                this.Bounds = rect;
                this.TopMost = true;
                // this.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);
            });
        }
        private void Tmr_Tick(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                var result = base.CreateParams;
                if (!ClickThrough) return result;
                 result.ExStyle |= (int)WindowStylesEx.WS_EX_TOOLWINDOW;
                result.ExStyle |= (int)WindowStylesEx.WS_EX_TRANSPARENT;
                result.ExStyle |= (int)WindowStylesEx.WS_EX_NOACTIVATE;
                result.ExStyle |= (int)WindowStylesEx.WS_EX_LAYERED;
                return result;
            }
        }
        protected override void CreateHandle()
        {
            try
            {
                base.CreateHandle();
            }
            catch (Exception)
            {
                return;
            }
            // Note: We need this because the Form.TopMost property does not respect
            // the "ShowWithoutActivation" flag, meaning the window will steal the
            // focus every time it is made visible.
            SetTopMost(new HandleRef(this, Handle), true);
        }
        private System.Windows.Forms.Timer tmr;
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public static HandleRef HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));
        public static HandleRef HWND_NOTOPMOST = new HandleRef(null, new IntPtr(-2));
        public const int SWP_NOSIZE = 1;
        public const int SWP_NOMOVE = 2;
        public const int SWP_NOZORDER = 4;
        public const int SWP_NOACTIVATE = 16;
        public static void SetTopMost(HandleRef handleRef, bool value)
        {
            var key = value ? HWND_TOPMOST : HWND_NOTOPMOST;
            var result = SetWindowPos(handleRef, key, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            if (!result)
            {
                throw NativeMethods.GetLastWin32Error("SetTopMost:SetWindowPos");
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);
    }

    [Flags]
    public enum WindowStylesEx : uint
    {
        /// <summary>Specifies a window that accepts drag-drop files.</summary>
        WS_EX_ACCEPTFILES = 0x00000010,

        /// <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
        WS_EX_APPWINDOW = 0x00040000,

        /// <summary>Specifies a window that has a border with a sunken edge.</summary>
        WS_EX_CLIENTEDGE = 0x00000200,

        /// <summary>
        /// Specifies a window that paints all descendants in bottom-to-top painting order using double-buffering.
        /// This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. This style is not supported in Windows 2000.
        /// </summary>
        /// <remarks>
        /// With WS_EX_COMPOSITED set, all descendants of a window get bottom-to-top painting order using double-buffering.
        /// Bottom-to-top painting order allows a descendent window to have translucency (alpha) and transparency (color-key) effects,
        /// but only if the descendent window also has the WS_EX_TRANSPARENT bit set.
        /// Double-buffering allows the window and its descendents to be painted without flicker.
        /// </remarks>
        WS_EX_COMPOSITED = 0x02000000,

        /// <summary>
        /// Specifies a window that includes a question mark in the title bar. When the user clicks the question mark,
        /// the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message.
        /// The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command.
        /// The Help application displays a pop-up window that typically contains help for the child window.
        /// WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
        /// </summary>
        WS_EX_CONTEXTHELP = 0x00000400,

        /// <summary>
        /// Specifies a window which contains child windows that should take part in dialog box navigation.
        /// If this style is specified, the dialog manager recurses into children of this window when performing navigation operations
        /// such as handling the TAB key, an arrow key, or a keyboard mnemonic.
        /// </summary>
        WS_EX_CONTROLPARENT = 0x00010000,

        /// <summary>Specifies a window that has a double border.</summary>
        WS_EX_DLGMODALFRAME = 0x00000001,

        /// <summary>
        /// Specifies a window that is a layered window.
        /// This cannot be used for child windows or if the window has a class style of either CS_OWNDC or CS_CLASSDC.
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/ms632599(v=vs.85).aspx#layered
        /// Using a layered window can significantly improve performance and
        /// visual effects for a window that has a complex shape, animates its
        /// shape, or wishes to use alpha blending effects. The system
        /// automatically composes and repaints layered windows and the windows of
        /// underlying applications. As a result, layered windows are rendered
        /// smoothly, without the flickering typical of complex window regions. In
        /// addition, layered windows can be partially translucent, that is,
        /// alpha-blended.
        /// </summary>
        WS_EX_LAYERED = 0x00080000,

        /// <summary>
        /// Specifies a window with the horizontal origin on the right edge. Increasing horizontal values advance to the left.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        WS_EX_LAYOUTRTL = 0x00400000,

        /// <summary>Specifies a window that has generic left-aligned properties. This is the default.</summary>
        WS_EX_LEFT = 0x00000000,

        /// <summary>
        /// Specifies a window with the vertical scroll bar (if present) to the left of the client area.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        WS_EX_LEFTSCROLLBAR = 0x00004000,

        /// <summary>
        /// Specifies a window that displays text using left-to-right reading-order properties. This is the default.
        /// </summary>
        WS_EX_LTRREADING = 0x00000000,

        /// <summary>
        /// Specifies a multiple-document interface (MDI) child window.
        /// </summary>
        WS_EX_MDICHILD = 0x00000040,

        /// <summary>
        /// Specifies a top-level window created with this style does not become the foreground window when the user clicks it.
        /// The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
        /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.
        /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
        /// </summary>
        WS_EX_NOACTIVATE = 0x08000000,

        /// <summary>
        /// Specifies a window which does not pass its window layout to its child windows.
        /// </summary>
        WS_EX_NOINHERITLAYOUT = 0x00100000,

        /// <summary>
        /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
        /// </summary>
        WS_EX_NOPARENTNOTIFY = 0x00000004,

        /// <summary>Specifies an overlapped window.</summary>
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,

        /// <summary>Specifies a palette window, which is a modeless dialog box that presents an array of commands.</summary>
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,

        /// <summary>
        /// Specifies a window that has generic "right-aligned" properties. This depends on the window class.
        /// The shell language must support reading-order alignment for this to take effect.
        /// Using the WS_EX_RIGHT style has the same effect as using the SS_RIGHT (static), ES_RIGHT (edit), and BS_RIGHT/BS_RIGHTBUTTON (button) control styles.
        /// </summary>
        WS_EX_RIGHT = 0x00001000,

        /// <summary>Specifies a window with the vertical scroll bar (if present) to the right of the client area. This is the default.</summary>
        WS_EX_RIGHTSCROLLBAR = 0x00000000,

        /// <summary>
        /// Specifies a window that displays text using right-to-left reading-order properties.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        WS_EX_RTLREADING = 0x00002000,

        /// <summary>Specifies a window with a three-dimensional border style intended to be used for items that do not accept user input.</summary>
        WS_EX_STATICEDGE = 0x00020000,

        /// <summary>
        /// Specifies a window that is intended to be used as a floating toolbar.
        /// A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font.
        /// A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB.
        /// If a tool window has a system menu, its icon is not displayed on the title bar.
        /// However, you can display the system menu by right-clicking or by typing ALT+SPACE. 
        /// </summary>
        WS_EX_TOOLWINDOW = 0x00000080,

        /// <summary>
        /// Specifies a window that should be placed above all non-topmost windows and should stay above them, even when the window is deactivated.
        /// To add or remove this style, use the SetWindowPos function.
        /// </summary>
        WS_EX_TOPMOST = 0x00000008,

        /// <summary>
        /// Specifies a window that should not be painted until siblings beneath the window (that were created by the same thread) have been painted.
        /// The window appears transparent because the bits of underlying sibling windows have already been painted.
        /// To achieve transparency without these restrictions, use the SetWindowRgn function.
        /// </summary>
        WS_EX_TRANSPARENT = 0x00000020,

        /// <summary>Specifies a window that has a border with a raised edge.</summary>
        WS_EX_WINDOWEDGE = 0x00000100
    }
}
