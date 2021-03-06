﻿using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using BExplorer.Shell;
using System.Linq;
using System;
using BExplorer.Shell._Plugin_Interfaces;

namespace BetterExplorerControls
{

    /// <summary> Interaction logic for PreviewPane.xaml </summary>
    public partial class DetailsPane : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public ShellView Browser;

        private IListItemEx SelectedItem => this.Browser != null && this.Browser.GetSelectedCount() > 0 ? this.Browser.SelectedItems[0] : null;

        public DetailsPane()
        {
            InitializeComponent();
            DataContext = this;
            this.Loaded += (sender, e) => this.SizeChanged += PreviewPane_SizeChanged;
        }

        private void Setup_PreviewPane()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)(() =>
            {
                if (SelectedItem != null)
                {
                    if (!Browser.SelectedItems.Any()) return;
                    if (this.SelectedItem.IsFolder) return;

                    //http://www.codeproject.com/Articles/7987/Retrieve-detailed-information-of-a-File
                    var sh = new Shell32.ShellClass();
                    Shell32.Folder dir = sh.NameSpace(System.IO.Path.GetDirectoryName(SelectedItem.ParsingName));
                    Shell32.FolderItem item = dir.ParseName(System.IO.Path.GetFileName(SelectedItem.ParsingName));

                    // loop through the Folder Items
                    for (int i = 0; i < 30; i++)
                    {
                        // read the current detail Info from the FolderItem Object
                        //(Retrieves details about an item in a folder. For example, its size, type, or the time of its last modification.)
                        // some examples:
                        // 0 Retrieves the name of the item. 
                        // 1 Retrieves the size of the item. 
                        // 2 Retrieves the type of the item. 
                        // 3 Retrieves the date and time that the item was last modified. 
                        // 4 Retrieves the attributes of the item. 
                        // -1 Retrieves the info tip information for the item. 
                        string colName = dir.GetDetailsOf(String.Empty, i);
                        string value = dir.GetDetailsOf(item, i);
                        // Create a helper Object for holding the current Information
                        // an put it into a ArrayList
                    }


                    //this.SelectedItem.Thumbnail.CurrentSize = new System.Windows.Size(this.ActualHeight - 20, this.ActualHeight - 20);
                    //this.SelectedItem.Thumbnail.FormatOption = BExplorer.Shell.Interop.ShellThumbnailFormatOption.Default;
                    //this.SelectedItem.Thumbnail.RetrievalOption = BExplorer.Shell.Interop.ShellThumbnailRetrievalOption.Default;
                    //icon.Source = this.SelectedItem.Thumbnail.BitmapSource;

                    //txtDisplayName.Text = SelectedItem.DisplayName;
                    //txtFileType.Text = "Extension: " + SelectedItem.Extension;
                    //txtPath.Text = "Location : " + SelectedItem.FileSystemPath;

                    //var OpenWirgList = SelectedItem.GetAssocList();
                    //if (OpenWirgList.Any()) {
                    //	txtOpenWith.Text = "Opens With: " + OpenWirgList.First().DisplayName;
                    //}

                    var File = new System.IO.FileInfo(Browser.SelectedItems[0].ParsingName);
                    txtFileSize.Text = "Size: " + File.Length.ToString();
                    txtFileCreated.Text = "Created: " + File.CreationTime.ToLongDateString();
                    txtFileModified.Text = "Modified: " + File.LastWriteTime.ToLongDateString();
                }
            }));
        }

        private void PreviewPane_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualHeight == 0)
                return;

            Setup_PreviewPane();
        }

        public void FillPreviewPane(ShellView browser)
        {
            if (this.Browser == null)
                this.Browser = browser;

            Setup_PreviewPane();
        }
    }
}