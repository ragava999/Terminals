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
// Generated by IDLImporter from file nsIB2GKeyboard.idl
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
    ///This Source Code Form is subject to the terms of the Mozilla Public
    /// License, v. 2.0. If a copy of the MPL was not distributed with this file,
    /// You can obtain one at http://mozilla.org/MPL/2.0/. </summary>
	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("40ad96b2-9efa-41fb-84c7-fbcec9b153f0")]
	public interface nsIB2GKeyboard
	{
		
		/// <summary>
        ///This Source Code Form is subject to the terms of the Mozilla Public
        /// License, v. 2.0. If a copy of the MPL was not distributed with this file,
        /// You can obtain one at http://mozilla.org/MPL/2.0/. </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void SendKey(int keyCode, int charCode);
		
		/// <summary>
        /// selection the previous element will be unselected.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void SetSelectedOption(Gecko.JsVal index);
		
		/// <summary>
        /// selection, then the last index specified in indexes will be selected.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void SetSelectedOptions(Gecko.JsVal indexes);
		
		/// <summary>
        /// Forms Validation), the value will simply be ignored by the element.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void SetValue(Gecko.JsVal value);
		
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void RemoveFocus();
		
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsIDOMEventListener GetOnfocuschangeAttribute();
		
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void SetOnfocuschangeAttribute([MarshalAs(UnmanagedType.Interface)] nsIDOMEventListener aOnfocuschange);
		
		/// <summary>
        /// composing text length
        /// </summary>
		[return: MarshalAs(UnmanagedType.Interface)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		nsIDOMEventListener GetOnselectionchangeAttribute();
		
		/// <summary>
        /// composing text length
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void SetOnselectionchangeAttribute([MarshalAs(UnmanagedType.Interface)] nsIDOMEventListener aOnselectionchange);
		
		/// <summary>
        /// The start position of the selection.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		int GetSelectionStartAttribute();
		
		/// <summary>
        /// The stop position of the selection.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		int GetSelectionEndAttribute();
		
		/// <summary>
        /// Set the selection range of the the editable text.
        ///
        /// @param start The beginning of the selected text.
        /// @param end The end of the selected text.
        ///
        /// Note that the start position should be less or equal to the end position.
        /// To move the cursor, set the start and end position to the same value.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void SetSelectionRange(int start, int end);
		
		/// <summary>
        /// Replace text around the beginning of the current selection range of the
        /// editable text.
        ///
        /// @param text The string to be replaced with.
        /// @param beforeLength The number of characters to be deleted before the
        /// beginning of the current selection range. Defaults to 0.
        /// @param afterLength The number of characters to be deleted after the
        /// beginning of the current selection range. Defaults to 0.
        /// </summary>
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
		void ReplaceSurroundingText([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Gecko.CustomMarshalers.AStringMarshaler")] nsAStringBase text, int beforeLength, int afterLength);
	}
}
