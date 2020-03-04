﻿using Flare_Sharp.ClientBase.Categories;
using Flare_Sharp.ClientBase.Modules;
using Flare_Sharp.Memory;
using Flare_Sharp.UI;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Flare_Sharp.ClientBase.UI
{
    /// <summary>
    /// Interaction logic for OverlayHost.xaml
    /// </summary>
    public partial class OverlayHost : Window
    {
        public static OverlayHost ui;
        Win32.WinEventDelegate overDel;
        public delegate void RepaintDel();
        public int x
        {
            get
            {
                return MCM.getMinecraftRect().Left+8;
            }
        }
        public int y
        {
            get
            {
                Win32.WINDOWPLACEMENT place = new Win32.WINDOWPLACEMENT();
                Win32.GetWindowPlacement(MCM.mcWinHandle, ref place);
                return MCM.getMinecraftRect().Top + 33 + (Convert.ToInt32(place.showCmd == Win32.SW_SHOWMAXIMIZED) * 7);
            }
        }
        public int width
        {
            get
            {
                return MCM.getMinecraftRect().Right-x-8;
            }
        }
        public int height
        {
            get
            {
                return MCM.getMinecraftRect().Bottom-y - 8;
            }
        }

        IntPtr thisHandle;

        public static event EventHandler postOverlayLoad;
        bool loaded = false;

        Grid panel = new Grid();

        public static SolidColorBrush primary = new SolidColorBrush(Colors.Black) { Opacity = 0.5 };
        public static SolidColorBrush secondary = new SolidColorBrush(Colors.White);
        public static SolidColorBrush tertiary = new SolidColorBrush(Colors.White) { Opacity = 0.5 };
        public static SolidColorBrush quaternary = new SolidColorBrush(Colors.Gray) { Opacity = 0.5 };
        public static SolidColorBrush quinary = new SolidColorBrush(Colors.Black);
        public static SolidColorBrush rainbow = new SolidColorBrush(Colors.White);

        public OverlayHost()
        {
            ui = this;
            panel.Background = new SolidColorBrush(Colors.Transparent);
            this.Content = panel;
            InitializeComponent();
            RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;
            Loaded += windowLoaded;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            overDel = new Win32.WinEventDelegate(LocationChangeCallback);
            Win32.SetWinEventHook((uint)Win32.SWEH_Events.EVENT_OBJECT_LOCATIONCHANGE, (uint)Win32.SWEH_Events.EVENT_OBJECT_LOCATIONCHANGE, IntPtr.Zero, overDel, MCM.mcWinProcId, Win32.GetWindowThreadProcessId(MCM.mcWinHandle, IntPtr.Zero), (uint)Win32.SWEH_dwFlags.WINEVENT_OUTOFCONTEXT | (uint)Win32.SWEH_dwFlags.WINEVENT_SKIPOWNPROCESS | (uint)Win32.SWEH_dwFlags.WINEVENT_SKIPOWNTHREAD);
            loaded = true;
            if (postOverlayLoad != null)
            {
                postOverlayLoad.Invoke(this, new EventArgs());
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            thisHandle = new WindowInteropHelper(this).Handle;
            UInt64 initialStyle = Win32.GetWindowLong(thisHandle, -20);
            Win32.SetWindowLong(thisHandle, -20, initialStyle | 0x20);
        }

        //protected override void OnRender(DrawingContext context)
        //{
        //    foreach (Category cat in CategoryHandler.registry.categories)
        //    {
        //        foreach (Module mod in cat.modules)
        //        {
        //            if (mod.enabled) {
        //                if (mod is VisualModule)
        //                {
        //                    VisualModule vmod = (VisualModule)mod;
        //                    vmod.onDraw(context);
        //                }
        //            }
        //        }
        //    }
        //}

        float rainbowProg = 0f;
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            vHost.children.Clear();
            foreach (Category cat in CategoryHandler.registry.categories)
            {
                foreach (Module mod in cat.modules)
                {
                    if (mod.enabled)
                    {
                        if (mod is VisualModule)
                        {
                            VisualModule vmod = (VisualModule)mod;
                            vmod.onRender();
                        }
                    }
                }
            }
            vHost.InvalidateVisual();
        }

        public void LocationChangeCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            Win32.SetWindowPos(thisHandle, (IntPtr)(-1), x, y, width, height, 0);
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
        }

        public static Color Rainbow(float progress)
        {
            float div = (Math.Abs(progress % 1) * 6);
            int ascending = (int)((div % 1) * 255);
            int descending = 255 - ascending;

            switch ((int)div)
            {
                case 0:
                    return Color.FromArgb(255, 255, (byte)ascending, 0);
                case 1:
                    return Color.FromArgb(255, (byte)descending, 255, 0);
                case 2:
                    return Color.FromArgb(255, 0, 255, (byte)ascending);
                case 3:
                    return Color.FromArgb(255, 0, (byte)descending, 255);
                case 4:
                    return Color.FromArgb(255, (byte)ascending, 0, 255);
                default: // case 5:
                    return Color.FromArgb(255, 255, 0, (byte)descending);
            }
        }

        public void addChildObj(Visual element)
        {
            vHost.children.Add(element);
        }

        public void repaint()
        {
            if (loaded)
            {
                RepaintDel repaintDel = new RepaintDel(ui.InvalidateVisual);
                ui.Dispatcher.Invoke(repaintDel);
            }
        }
    }
}