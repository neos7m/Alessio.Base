using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Alessio.Base.Extensions
{
	public static class Deconstructors
	{
		public static void Deconstruct(this Point point, out int x, out int y)
		{
			x = point.X;
			y = point.Y;
		}
	}
}
