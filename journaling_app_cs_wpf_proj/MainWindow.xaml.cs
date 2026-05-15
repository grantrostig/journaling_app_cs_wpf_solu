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
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e){
            Junk junk = junk_create_instance();
            junk_add(junk, 5);
            junk_destroy(junk);

        }
    }
    internal static partial class Native
    {
        private const string LibraryName = "native";

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr junk_create();

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int junk_add(IntPtr junk, int a, int b);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void junk_destroy(IntPtr junk);
    }

    public sealed class Junk : IDisposable
    {
        private IntPtr _handle;

        public Junk()
        {
            _handle = Native.junk_Create();

            if (_handle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to create native junk.");
        }

        public int Add(int a, int b)
        {
            ThrowIfDisposed();
            return Native.junk_Add(_handle, a, b);
        }

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                Native.junk_Destroy(_handle);
                _handle = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }
        ~junk() { Dispose(); }
        private void ThrowIfDisposed() { if (_handle == IntPtr.Zero) throw new ObjectDisposedException(nameof(junk)); }
}

//public static class Program {
    public static void Main()
        {
            using var junk = new junk();
            int result = junk.add(2, 3);
            Console.WriteLine(result); // 5
        }


    }