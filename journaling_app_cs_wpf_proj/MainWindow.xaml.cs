using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace journaling_app_cs_wpf_proj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var junk = Native.junk_create_instance();
            IntPtr junk2 = Native.junk_create_instance();
            int i = Native.junk_add(junk, 5);
            Native.junk_destroy(junk);
            Native.junk_destroy(junk2);
        }
    }
    internal static partial class Native
    {
        private const string LibraryName = "whisper-cpp_clr_cpp_proj";
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]  // Adjust the calling convention as needed 
        public static extern IntPtr junk_create_instance();
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int junk_add(IntPtr junk, int a);
        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void junk_destroy(IntPtr junk);
    }

    public sealed class Junk : IDisposable
    {
        private IntPtr _handle;
        public Junk()
        {
            _handle = Native.junk_create_instance();
            if (_handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create native junk.");
        }

        public int Add(int a, int b)
        {
            ThrowIfDisposed();
            return Native.junk_add(_handle, a);
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                Native.junk_destroy(_handle);
                _handle = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }
        ~Junk() { Dispose(); }
        private void ThrowIfDisposed() { if (_handle == IntPtr.Zero) throw new ObjectDisposedException(nameof(Junk)); }
    }
}