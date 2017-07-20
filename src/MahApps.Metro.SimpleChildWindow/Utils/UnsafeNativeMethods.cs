using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace MahApps.Metro.SimpleChildWindow.Utils
{
	/// <devdoc>http://msdn.microsoft.com/en-us/library/ms182161.aspx</devdoc>
	[SuppressUnmanagedCodeSecurity]
	internal static class UnsafeNativeMethods
	{
		/// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms647486%28v=vs.85%29.aspx</devdoc>
		[DllImport("user32", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadStringW", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		internal static extern int LoadString([In] [Optional] SafeLibraryHandle hInstance, [In] uint uID, [Out] StringBuilder lpBuffer, [In] int nBufferMax);

		/// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms684175%28v=vs.85%29.aspx</devdoc>
		[DllImport("kernel32", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadLibraryW", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		internal static extern SafeLibraryHandle LoadLibrary([In] [MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

		/// <devdoc>http://msdn.microsoft.com/en-us/library/windows/desktop/ms683152%28v=vs.85%29.aspx</devdoc>
		[DllImport("kernel32", CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FreeLibrary([In] IntPtr hModule);
	}
}
