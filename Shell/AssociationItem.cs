﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BExplorer.Shell
{
	public class AssociationItem
	{
		public String DisplayName { get; set; }
		public String ApplicationPath { get; set; }

		public IntPtr InvokePtr { get; set; }

		public BitmapSource Icon
		{
			get
			{
				var item = new ShellItem(this.ApplicationPath.ToShellParsingName());
				var bs = item.Thumbnail.SmallBitmapSource;
				return bs;
			}
		}
	}
}
