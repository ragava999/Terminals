using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Gecko
{
	
	[StructLayout(LayoutKind.Sequential)]
	public class gfxRect
	{
		public double X;
		public double Y;
		public double Width;
		public double Height;
	}
}
