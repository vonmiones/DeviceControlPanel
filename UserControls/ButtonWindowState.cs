using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DeviceControlPanel.UserControls
{
    class ButtonWindowState : Button
    {
        public enum WindowStateEnum
        {
            Standby,
            Tray,
            Minimize,
            Maximize,
            Normal,
            Close
        }


        public static readonly DependencyProperty WindowStateProperty =
        DependencyProperty.Register("WindowState", typeof(WindowStateEnum), typeof(ButtonWindowState),
            new FrameworkPropertyMetadata(WindowStateEnum.Normal, FrameworkPropertyMetadataOptions.AffectsRender));

        public WindowStateEnum WindowState
        {
            get { return (WindowStateEnum)GetValue(WindowStateProperty); }
            set { SetValue(WindowStateProperty, value); }
        }

        public ButtonWindowState()
        {
            Loaded += WindowStateButton_Loaded;
        }

        private void WindowStateButton_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window != null)
                window.StateChanged += Window_StateChanged;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            UpdateWindowState();
        }

        protected override void OnClick()
        {
            base.OnClick();

            switch (WindowState)
            {
                case WindowStateEnum.Minimize:
                    Window.GetWindow(this).WindowState = System.Windows.WindowState.Minimized;
                    break;
                case WindowStateEnum.Maximize:
                    Window.GetWindow(this).WindowState = System.Windows.WindowState.Normal;
                    WindowState = WindowStateEnum.Normal;
                    break;
                case WindowStateEnum.Normal:
                    Window.GetWindow(this).WindowState = System.Windows.WindowState.Maximized;
                    WindowState = WindowStateEnum.Maximize;
                    break;
                case WindowStateEnum.Close:
                    Window.GetWindow(this).Close();
                    break;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == WindowStateProperty)
            {
                UpdateContent();
            }
        }

        private void UpdateContent()
        {
            FontFamily = new FontFamily("Webdings");

            switch (WindowState)
            {
                case WindowStateEnum.Minimize:
                    Content = "0";
                    break;
                case WindowStateEnum.Maximize:
                    Content = "2";
                    break;
                case WindowStateEnum.Normal:
                    Content = "1";
                    break;
                case WindowStateEnum.Close:
                    Content = "r";
                    break;
            }
        }

        private void UpdateWindowState()
        {
            Window window = Window.GetWindow(this);

            if (window != null)
            {
                WindowStateEnum newState;

                if (window.WindowState == System.Windows.WindowState.Maximized)
                {
                    newState = WindowStateEnum.Maximize;

                }
                else
                {
                    newState = WindowStateEnum.Normal;
                }

                if (WindowState == WindowStateEnum.Maximize || WindowState == WindowStateEnum.Normal)
                {
                    WindowState = newState;
                }
            }
        }

    }
}
