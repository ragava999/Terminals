using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Kohl.Framework.WinForms
{
    public class PersistWindowState : Component
    {
        private bool m_allowSaveMinimized;

        private int m_normalHeight;

        private int m_normalLeft;

        private int m_normalTop;

        private int m_normalWidth;

        private Form m_parent;

        private string m_regPath;

        private FormWindowState m_windowState;

        public bool AllowSaveMinimized
        {
            set
            {
                this.m_allowSaveMinimized = value;
            }
        }

        public Form Parent
        {
            get
            {
                return this.m_parent;
            }
            set
            {
                this.m_parent = value;
                this.m_parent.Closing += new CancelEventHandler(this.OnClosing);
                this.m_parent.Resize += new EventHandler(this.OnResize);
                this.m_parent.Move += new EventHandler(this.OnMove);
                this.m_parent.Load += new EventHandler(this.OnLoad);
                this.m_normalWidth = this.m_parent.Width;
                this.m_normalHeight = this.m_parent.Height;
            }
        }

        public string RegistryPath
        {
            get
            {
                return this.m_regPath;
            }
            set
            {
                this.m_regPath = value;
            }
        }

        public PersistWindowState()
        {
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(this.m_regPath);
            registryKey.SetValue("Left", this.m_normalLeft);
            registryKey.SetValue("Top", this.m_normalTop);
            registryKey.SetValue("Width", this.m_normalWidth);
            registryKey.SetValue("Height", this.m_normalHeight);
            if (!this.m_allowSaveMinimized && this.m_windowState == FormWindowState.Minimized)
            {
                this.m_windowState = FormWindowState.Normal;
            }
            registryKey.SetValue("WindowState", (int)this.m_windowState);
            if (this.SaveStateEvent != null)
            {
                this.SaveStateEvent(this, new RegistryKeyEventArgs(registryKey));
            }
        }

        private void OnLoad(object sender, EventArgs e)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(this.m_regPath);
            if (registryKey != null)
            {
                int value = (int)registryKey.GetValue("Left", this.m_parent.Left);
                int num = (int)registryKey.GetValue("Top", this.m_parent.Top);
                int value1 = (int)registryKey.GetValue("Width", this.m_parent.Width);
                int num1 = (int)registryKey.GetValue("Height", this.m_parent.Height);
                FormWindowState formWindowState = (FormWindowState)registryKey.GetValue("WindowState", (int)this.m_parent.WindowState);
                this.m_parent.Location = new Point(value, num);
                this.m_parent.Size = new Size(value1, num1);
                this.m_parent.WindowState = formWindowState;
                if (this.LoadStateEvent != null)
                {
                    this.LoadStateEvent(this, new RegistryKeyEventArgs(registryKey));
                }
            }
        }

        private void OnMove(object sender, EventArgs e)
        {
            if (this.m_parent.WindowState == FormWindowState.Normal)
            {
                this.m_normalLeft = this.m_parent.Left;
                this.m_normalTop = this.m_parent.Top;
            }
            this.m_windowState = this.m_parent.WindowState;
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (this.m_parent.WindowState == FormWindowState.Normal)
            {
                this.m_normalWidth = this.m_parent.Width;
                this.m_normalHeight = this.m_parent.Height;
            }
        }

        public event WindowStateDelegate LoadStateEvent;

        public event WindowStateDelegate SaveStateEvent;

        public delegate void WindowStateDelegate(object sender, RegistryKeyEventArgs e);

        public class RegistryKeyEventArgs : EventArgs
        {
            public RegistryKeyEventArgs(RegistryKey key)
            {
                this.Key = key;
            }

            public RegistryKey Key { get; set; }
        }
    }
}