// #define P_INVOKE_1
using System;
using System.Linq.Expressions;
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
namespace journaling_app_cs_wpf_proj {
    internal static partial class Native { // P/Invoke signatures for the C++ DLL
        private const string LibraryName = "whisper-cpp_clr_cpp_proj";

        [ DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]  // Adjust the calling convention as needed 
        public static extern IntPtr junk_create_instance();
        [ DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int junk_add(IntPtr junk, int a);
        [ DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void junk_destroy(IntPtr junk);
    }
    public sealed class Junk_cs : IDisposable // A C# wrapper around the native Junk class, implementing IDisposable for proper cleanup
    {  // 2nd Approach: C# wrapper class that manages the native instance and provides a more C#-friendly API
        private const    int    my_int = 33;  // Example of a managed field that can be used in the wrapper class to add functionality on top of the native instance name of type is: value type
        private          IntPtr _handle;
        private readonly object _disposeLock = new(); // Lock object to ensure thread safety during disposal
        public Junk_cs() { // Constructor creates the native instance
            _handle = Native.junk_create_instance(); // Call the native function to create the instance and store the handle
            if (_handle == IntPtr.Zero) throw new InvalidOperationException("Failed to create native junk, ptr is zero.");
        }
        private void ThrowIfDisposed(){if (_handle == IntPtr.Zero) throw new ObjectDisposedException(nameof(Junk_cs));}
        public int Add(int a) {
            ThrowIfDisposed(); // Check if the instance has been disposed before calling native methods, finalizer will call Dispose(false) which will set _handle to zero, so this check will prevent using a disposed instance
            return Native.junk_add(_handle, a + my_int); // dispose will set _handle to zero, so this check will prevent using a disposed instance, and we can use the managed field my_int to add extra functionality on top of the native method
        }
        // Core cleanup. When disposing == true, free managed + unmanaged.
        private void Dispose(bool disposing){// Core cleanup method that handles both managed and unmanaged resource cleanup based on the disposing parameter. When disposing == true, it means the method was called from the Dispose() method, so we should free both managed and unmanaged resources. When disposing == false, it means the method was called from the finalizer, so we should only free unmanaged resources to avoid potential issues with the order of finalization of managed objects.
            lock (_disposeLock) {
                if (_handle != IntPtr.Zero){Native.junk_destroy(_handle);_handle = IntPtr.Zero;}
            }
            // if disposing == true, dispose managed resources here (none currently)
        }
        public void Dispose(){ // Public Dispose method for IDisposable, calls the core cleanup with disposing == true to free both managed and unmanaged resources, and suppresses finalization since we've already cleaned up
            Dispose(disposing: true); // Call the core cleanup method with disposing == true to free both managed and unmanaged resources
            GC.SuppressFinalize(this);
        }
        ~Junk_cs(){ // Finalizer to ensure unmanaged resources are freed if Dispose is not called, only free unmanaged resources from the finalizer path
            // Only free unmanaged resources from the finalizer path to avoid potential issues with the order of finalization of managed objects. The finalizer will call Dispose(false) which will only free unmanaged resources and set _handle to zero, so the ThrowIfDisposed check will prevent using a disposed instance.
            Dispose(disposing: false); // Call the core cleanup method with disposing == false to only free unmanaged resources
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() { InitializeComponent(); } // Initialize the WPF components 
#if P_INVOKE_1
        private void Button_Click(object sender, RoutedEventArgs e) {
            Console.WriteLine("Debug mode enabled");
            //IntPtr junk = nint.Zero;  // TODO??: handle memory management with nint??
            IntPtr junk_handle = IntPtr.Zero;// Use IntPtr to store the handle to the native instance, which is a pointer, and we will manage its lifecycle properly.
            try {
                junk_handle = Native.junk_create_instance();   // Create the native instance from C ABI
                int i = Native.junk_add(junk_handle, 5);       // Call the add method on the native instance
            }
            catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            finally {
                if (junk_handle != IntPtr.Zero) {
                    Native.junk_destroy(junk_handle);             // Clean up the native instance
                }
                MessageBox.Show("Done" + junk_handle.ToString(), "GR Title Bar");
            }
        }
#else
        private void Button_Click(object sender, RoutedEventArgs e) {
            Console.WriteLine("Release mode");
        }
#endif
    }
}