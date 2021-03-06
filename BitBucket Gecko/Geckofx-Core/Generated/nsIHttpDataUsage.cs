// --------------------------------------------------------------------------------------------
// Version: MPL 1.1/GPL 2.0/LGPL 2.1
// 
// The contents of this file are subject to the Mozilla Public License Version
// 1.1 (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
// http://www.mozilla.org/MPL/
// 
// Software distributed under the License is distributed on an "AS IS" basis,
// WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
// for the specific language governing rights and limitations under the
// License.
// 
// <remarks>
// Generated by IDLImporter from file nsIHttpDataUsage.idl
// 
// You should use these interfaces when you access the COM objects defined in the mentioned
// IDL/IDH file.
// </remarks>
// --------------------------------------------------------------------------------------------
namespace Gecko
{
	using System;
	using System.Runtime.InteropServices;
	using System.Runtime.InteropServices.ComTypes;
	using System.Runtime.CompilerServices;
	
	
	/// <summary>
    /// nsIHttpDataUsage contains counters for the amount of HTTP data transferred
    /// in and out of this session since the last time it was reset with the
    /// resetHttpDataUsage() method. These counters are normally reset on each
    /// telemetry ping.
    ///
    /// Data is split into ethernet and cell. ethernet includes wifi.
    ///
    /// </summary>
	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("79dee3eb-9323-4d5c-b0a8-1baa18934d9e")]
	public interface nsIHttpDataUsage
	{
		
		/// <summary>
        /// nsIHttpDataUsage contains counters for the amount of HTTP data transferred
        /// in and out of this session since the last time it was reset with the
        /// resetHttpDataUsage() method. These counters are normally reset on each
        /// telemetry ping.
        ///
        /// Data is split into ethernet and cell. ethernet includes wifi.
        ///
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		ulong GetEthernetBytesReadAttribute();
		
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		ulong GetEthernetBytesWrittenAttribute();
		
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		ulong GetCellBytesReadAttribute();
		
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		ulong GetCellBytesWrittenAttribute();
		
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void ResetHttpDataUsage();
	}
}
