﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BExplorer.Shell._Plugin_Interfaces;
using BExplorer.Shell.Interop;

namespace BExplorer.Shell
{
	/// <summary>
	/// Interaction logic for ToolTip.xaml
	/// </summary>
	public partial class ToolTip : Window, INotifyPropertyChanged
	{
		private IListItemEx _ShellItem;
		private DispatcherTimer _DelayTimer = new DispatcherTimer(DispatcherPriority.Background);
    private ShellView _View { get; set; }

		public int Type { get; set; }
		public IListItemEx CurrentItem
		{
			get
			{
				return _ShellItem;
			}

			set
			{
				_ShellItem = value;
				RaisePropertyChanged("CurrentItem");
			}
		}

		public Int32 ItemIndex { get; set; }
		public String Contents {get; set;}
		
		public ToolTip(ShellView view)
		{
			InitializeComponent();
			this.DataContext = this;
      this._View = view;
			_DelayTimer.Interval = TimeSpan.FromMilliseconds(700);
			_DelayTimer.Tick += DelayTimer_Tick;
		}

		void DelayTimer_Tick(object sender, EventArgs e)
		{
			_DelayTimer.Stop();

			var t = new Thread(() =>
			{
				var tooltip = CurrentItem.ToolTipText;
				if (String.IsNullOrEmpty(tooltip) && Type == 1)
				{
					Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (ThreadStart)(this.Hide));
					return;
				}
				Contents = Type == 0 ? $"{CurrentItem.DisplayName}\r\n{CurrentItem.ToolTipText}" : CurrentItem.ToolTipText;
				RaisePropertyChanged("Contents");
				Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (ThreadStart)(() =>
				{
          var lvi = new LVITEMINDEX();
          lvi.iItem = this.ItemIndex;
          lvi.iGroup = this._View.GetGroupIndex(this.ItemIndex);
          var bounds = new User32.RECT();
          User32.SendMessage(this._View.LVHandle, MSG.LVM_GETITEMINDEXRECT, ref lvi, ref bounds);
          var rect = new System.Drawing.Rectangle(bounds.Left, bounds.Top, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
					var posm = User32.GetCursorPosition();
					var mousePos = this._View.PointToClient(posm);
          var isInsideItem = rect.Contains(mousePos);

					if (isInsideItem)
						this.Show();
					else
						this.Hide();

				}));
			});
			t.SetApartmentState(ApartmentState.STA);
			t.Start();
		}

		public ToolTip(String contents)
		{
			InitializeComponent();
			this.DataContext = this;

		}

		public void ShowTooltip()
		{
			if (!_DelayTimer.IsEnabled)
				_DelayTimer.Start();
		}

		public void HideTooltip()
		{
			_DelayTimer.Stop();
			if (this.IsVisible) {
				Contents = String.Empty;
				RaisePropertyChanged("Contents");
				this.Hide();
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string propertyName)
		{
			var handlers = PropertyChanged;

			handlers(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var textBlock = sender as TextBlock;
			var xPos = System.Windows.Forms.Cursor.Position.X + textBlock.ActualWidth + 16 > Screen.GetWorkingArea(System.Windows.Forms.Cursor.Position).Width ? System.Windows.Forms.Cursor.Position.X - textBlock.ActualWidth + 16 : System.Windows.Forms.Cursor.Position.X + 16;
			var yPos = System.Windows.Forms.Cursor.Position.Y + textBlock.ActualHeight > Screen.GetWorkingArea(System.Windows.Forms.Cursor.Position).Height ? System.Windows.Forms.Cursor.Position.Y - textBlock.ActualHeight : System.Windows.Forms.Cursor.Position.Y;
			this.Left = xPos;
			this.Top = yPos;
		}
	}
}
