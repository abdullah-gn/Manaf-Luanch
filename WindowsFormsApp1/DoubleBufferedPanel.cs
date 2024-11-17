using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public class DoubleBufferedPanel : Panel
	{
		public DoubleBufferedPanel()
		{
			this.DoubleBuffered = true;
			this.SetStyle(
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint, true);
		}
	}
}
