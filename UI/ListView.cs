using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using Common;
using Common.Logger;
using Common.Win32;
using Common.Win32.Messages;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.Interfaces;
using SharpFile.Infrastructure.IO;
using SharpFile.Infrastructure.IO.ChildResources;
using SharpFile.Infrastructure.IO.ParentResources;
using SharpFile.Infrastructure.SettingsSection;
using View = SharpFile.Infrastructure.Interfaces.View;

namespace SharpFile.UI {
    public class ListView : System.Windows.Forms.ListView, IView {
        private static readonly object lockObject = new object();
        private static readonly string sortOrderAscendingKey = SortOrder.Ascending.GetType().FullName + ".Ascending";
        private static readonly string sortOrderDescendingKey = SortOrder.Descending.GetType().FullName + ".Descending";

        private ResourceContainer selectedResources = new ResourceContainer();
        private Dictionary<string, ListViewItem> itemDictionary = new Dictionary<string, ListViewItem>();
        private IViewComparer comparer = new ListViewItemComparer();
        private List<ColumnInfo> columnInfos;
        private long fileCount = 0;
        private long folderCount = 0;
        private int lastSelectedItemIndex = 0;
        private Dictionary<string, int> previousTopIndexes = new Dictionary<string, int>();

        public event View.UpdateStatusDelegate UpdateStatus;
        public event View.UpdateProgressDelegate UpdateProgress;
        public event View.GetImageIndexDelegate GetImageIndex;
        public event View.UpdatePathDelegate UpdatePath;
        public event View.UpdatePluginPanesDelegate UpdatePluginPanes;

        // TODO: This empty ListView ctor shouldn't be necessary, but the view can't be instantiated without it.
        public ListView()
            : this("Instantiated") {
        }

		public ListView(string name) {
			// This prevents flicker in the listview. 
			// It is a protected property, so it is only available if we derive from ListView.
			this.DoubleBuffered = true;
			this.Name = name;

			initializeComponent();
		}

        private void initializeComponent() {
            this.DoubleClick += listView_DoubleClick;
            this.KeyDown += listView_KeyDown;
            this.MouseUp += listView_MouseUp;
            this.ItemDrag += listView_ItemDrag;
            this.DragOver += listView_DragOver;
            this.DragDrop += listView_DragDrop;
            this.KeyUp += listView_KeyUp;
            this.BeforeLabelEdit += listView_BeforeLabelEdit;
            this.AfterLabelEdit += listView_AfterLabelEdit;
            this.GotFocus += listView_GotFocus;
            this.ColumnClick += listView_ColumnClick;
            this.ItemSelectionChanged += listView_ItemSelectionChanged;

            // Set some options on the listview.
            this.Dock = DockStyle.Fill;
            this.AllowColumnReorder = true;
            this.AllowDrop = true;
            this.FullRowSelect = true;
            this.LabelEdit = true;
            this.UseCompatibleStateImageBehavior = false;
            this.Sorting = SortOrder.Ascending;
            this.ListViewItemSorter = comparer;
            this.Font = Settings.Instance.Font;
            this.View = Settings.Instance.View;
        }

        #region Protected methods.
        /// <summary>
        /// Executes the currently selected item.
        /// </summary>
        protected virtual void execute() {
            if (this.SelectedItems.Count > 0) {
                IResource resource = this.SelectedItems[0].Tag as IResource;

                if (resource != null) {
                    setPreviousTopIndex(resource);
                    resource.Execute(this);
                }
            }
        }

        #region Overrides.
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m) {
			// Validate the rename textbox when editing a filename.
            if (m.Msg == (int)WM.COMMAND) {
				// Translate the WParam to a HWordWParam.
                uint hWordWParam = ((uint)m.WParam.ToInt32() & 0xFFFF0000) >> 16;

                if (hWordWParam == (int)WM.CUT) {
                    try {
                        if (LabelEditHandle != null && LabelEditHandle != IntPtr.Zero) {
                            validateEditLabel(LabelEditHandle);
                        }
                    } catch (Exception ex) {
                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, "Error while validating label.");
                    }
				}
			}

            base.WndProc(ref m);
        }
        #endregion
        #endregion

        #region Delegate methods
        /// <summary>
        /// Passes the status to any listening events.
        /// </summary>
        /// <param name="status">Status to show.</param>
        public void OnUpdateStatus(IView view) {
            if (UpdateStatus != null) {
                UpdateStatus(view);
            }
        }

        /// <summary>
        /// Passes the value to any listening events.
        /// </summary>
        /// <param name="value">Percentage value for status.</param>
        public void OnUpdateProgress(int value) {
            if (UpdateProgress != null) {
                UpdateProgress(value);
            }
        }

        /// <summary>
        /// Passes the path to any listening events.
        /// </summary>
        /// <param name="path">Path to update.</param>
        public void OnUpdatePath(IResource path) {
			if (path != null) {
				if (UpdatePath != null) {
					UpdatePath(path);
				}

				Settings.Instance.DualParent.SelectedPath = path.FullName;
			}
        }

        /// <summary>
        /// Passes the filesystem info to any listening events.
        /// </summary>
        /// <param name="fsi"></param>
        /// <returns></returns>
        public int OnGetImageIndex(IResource fsi, bool useFileAttributes) {
            if (GetImageIndex != null) {
                return GetImageIndex(fsi, useFileAttributes);
            }

            return -1;
        }

        /// <summary>
        /// Updates any listening plugin panes with the current view.
        /// </summary>
        /// <param name="view">Current view.</param>
        public void OnUpdatePluginPanes(IView view) {
            if (UpdatePluginPanes != null) {
                UpdatePluginPanes(this);
            }
        }
        #endregion

        #region Events.
        void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
			if (SelectedItems.Count > 0) {
				lastSelectedItemIndex = e.ItemIndex;
                OnUpdatePluginPanes(this);
			} else {
				// Make sure that the item's icon doesn't disappear.
				this.BeginInvoke((MethodInvoker)delegate {
					if (lastSelectedItemIndex > 0 && lastSelectedItemIndex < Items.Count) {
						ListViewItem item = Items[lastSelectedItemIndex];
						item.Selected = true;
						item.ImageIndex = OnGetImageIndex((IResource)item.Tag, false);
					}
				});
			}
        }

        void listView_ColumnClick(object sender, ColumnClickEventArgs e) {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == comparer.ColumnIndex) {
                // Reverse the current sort direction for this column.
                if (comparer.SortOrder == SortOrder.Ascending) {
                    comparer.SortOrder = SortOrder.Descending;
                } else {
                    comparer.SortOrder = SortOrder.Ascending;
                }
            } else {
                // A new column is being sorted, so default to ascending.
                comparer.ColumnIndex = e.Column;
                comparer.SortOrder = SortOrder.Ascending;
            }

            setColumnSort(comparer.ColumnIndex, comparer.SortOrder);

            // Perform the sort with these new sort options.
            this.Sort();
        }

        /// <summary>
        /// Fires when the list view gets focus.
        /// </summary>
        private void listView_GotFocus(object sender, EventArgs e) {
            OnUpdatePath(Path);
            OnUpdateStatus(this);
            OnUpdatePluginPanes(this);
        }

        /// <summary>
        /// Refreshes the listview when a file/directory is double-clicked in the listview.
        /// </summary>
        private void listView_DoubleClick(object sender, EventArgs e) {
            execute();
        }

        /// <summary>
        /// Selects an item in the listview when the Space bar is hit.
        /// </summary>
        private void listView_KeyDown(object sender, KeyEventArgs e) {
            // TODO: Make the keycodes configurable.
            switch (e.KeyCode) {
                case Keys.Space:
                    calculateSize();
                    break;
                case Keys.Escape:
                    OnUpdateProgress(100);
                    break;
                case Keys.Enter:
                    execute();
                    break;
            }
        }

        /// <summary>
        /// Displays the right-click context menu.
        /// </summary>
        private void listView_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                ShellContextMenu menu = new ShellContextMenu();

                if (this.SelectedItems.Count > 1) {
                    List<string> paths = getSelectedPaths();
                    menu.PopupMenu(paths, this.Handle);
                } else if (this.SelectedItems.Count == 1) {
                    menu.PopupMenu(this.SelectedItems[0].Name, this.Handle);
                } else {
                    menu.PopupMenu(Path.FullName, this.Handle);
                }
            }
        }

        /// <summary>
        /// Performs the necessary action when a file is dropped on the form.
        /// </summary>
        private void listView_DragDrop(object sender, DragEventArgs e) {
            // Can only drop files.
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
                return;
            }

            string[] fileDrops = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileDrop in fileDrops) {
                IResource resource = FileSystemInfoFactory.GetFileSystemInfo(fileDrop);

                if (resource != null && resource is IChildResource) {
                    string destination = string.Format(@"{0}{1}",
                                                       Path.FullName,
                                                       resource.Name);

                    if (!System.IO.File.Exists(destination)) {
                        try {
                            switch (e.Effect) {
                                case DragDropEffects.Copy:
                                    ((IChildResource)resource).Copy(destination, false);
                                    break;
                                case DragDropEffects.Move:
                                    ((IChildResource)resource).Move(destination);
                                    break;
                                case DragDropEffects.Link:
                                    // TODO: Need to handle links.
                                    break;
                            }
                        } catch (System.IO.IOException ex) {
                            Settings.Instance.Logger.LogAndInvoke(LogLevelType.ErrorsOnly, ShowMessageBox, ex,
                                "Failed to perform the specified operation for {0}", destination);
                        } catch (Exception ex) {
                            Settings.Instance.Logger.LogAndInvoke(LogLevelType.ErrorsOnly, ShowMessageBox, ex,
                                "The selected operation could not be completed for {0}.", destination);
                        }
                    } else {
                        Settings.Instance.Logger.LogAndInvoke(LogLevelType.MildlyVerbose, ShowMessageBox,
                            "The file, {0}, already exists.", destination);
                    }
                }
            }
        }

        /// <summary>
        /// Performs action neccessary to allow drags from listview.
        /// </summary>
        private void listView_DragOver(object sender, DragEventArgs e) {
            const int ALT = 32;
            const int CTRL = 8;
            const int SHIFT = 4;

            // Determine whether file data exists in the drop data. If not, then
            // the drop effect reflects that the drop cannot occur.
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.None;
                return;
            }

            // Set the effect based upon the KeyState.
            // Can't get links to work - Use of Ole1 services requiring DDE windows is disabled.
            /*
            if ((e.KeyState & (CTRL | ALT)) == (CTRL | ALT) &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) {
                e.Effect = DragDropEffects.Link;
            } else if ((e.KeyState & ALT) == ALT &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) {
                e.Effect = DragDropEffects.Link;
            } else*/
            if ((e.KeyState & SHIFT) == SHIFT &&
                (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) {
                e.Effect = DragDropEffects.Move;
            } else if ((e.KeyState & CTRL) == CTRL &&
                     (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
                e.Effect = DragDropEffects.Copy;
            } else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move) {
                // By default, the drop action should be move, if allowed.
                e.Effect = DragDropEffects.Move;

                // If the disk is different, then default to a COPY operation.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0) {
                    FileInfo fileInfo = new FileInfo(files[0]);
                    string originalPath = fileInfo.FullName.Substring(0, fileInfo.FullName.IndexOf(':'));
                    string currentPath = Path.FullName.Substring(0, Path.FullName.IndexOf(':'));

                    if (!originalPath.Equals(currentPath, StringComparison.OrdinalIgnoreCase)) {
                        if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy) {
                            e.Effect = DragDropEffects.Copy;
                        }
                    } else if (fileInfo.DirectoryName.Equals(Path.FullName, StringComparison.OrdinalIgnoreCase)) {
                        e.Effect = DragDropEffects.None;
                    }
                }
            } else {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Performs a drag operation.
        /// </summary>
        private void listView_ItemDrag(object sender, ItemDragEventArgs e) {
            List<string> paths = getSelectedPaths();

            if (paths.Count > 0) {
                DoDragDrop(new DataObject(DataFormats.FileDrop, paths.ToArray()),
                           DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
            }
        }

        /// <summary>
        /// Performs actions based on the key pressed.
        /// </summary>
        private void listView_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F2) {
                if (this.SelectedItems.Count > 0) {
                    ListViewItem item = this.SelectedItems[0];

                    if (!(item.Tag is ParentDirectoryInfo) && !(item.Tag is RootDirectoryInfo)) {
                        item.BeginEdit();
                    }
                }
            }
        }

        /// <summary>
        /// Make sure that the item being edited is valid.
        /// </summary>
        private void listView_BeforeLabelEdit(object sender, LabelEditEventArgs e) {
            ListViewItem item = this.Items[e.Item];
            IResource resource = (IResource)item.Tag;

            if (item.Tag is ParentDirectoryInfo ||
                item.Tag is RootDirectoryInfo ||
                item.Tag is DriveInfo) {
                e.CancelEdit = true;
            }

            int index = e.Item;

            // BeginInvoke will ensure that this message will get executed after the Framework's messages to select all of the text.
            this.BeginInvoke((MethodInvoker)delegate {
                // Only select the name in the textbox, not the extension.
                int selectedLength = Items[index].Text.Length;

                if (Items[index].Text.LastIndexOf('.') > -1) {
                    selectedLength = Items[index].Text.LastIndexOf('.');
                }

				User32.SendMessage(LabelEditHandle, (int)EM.SETSEL, 0, new IntPtr(selectedLength));
            });
        }

        /// <summary>
        /// Renames the file/directory that was being edited.
        /// </summary>
        private void listView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            if (string.IsNullOrEmpty(e.Label)) {
                e.CancelEdit = true;
            } else {
                if (!validateEditLabel(LabelEditHandle)) {
                    Items[e.Item].BeginEdit();
                    e.CancelEdit = true;
                }
            }

            if (!e.CancelEdit) {
                ListViewItem item = this.Items[e.Item];
                IChildResource resource = (IChildResource)item.Tag;

                if (!(item.Tag is ParentDirectoryInfo) && !(item.Tag is RootDirectoryInfo)) {
                    string destination = string.Format("{0}{1}",
                                                       Path.FullName,
                                                       e.Label);

                    try {
                        resource.Move(destination);
                    } catch (Exception ex) {
                        e.CancelEdit = true;

                        Settings.Instance.Logger.LogAndInvoke(LogLevelType.ErrorsOnly, ShowMessageBox, ex,
                            "Renaming to {0} failed.", destination);
                    }
                }
            }
        }
        #endregion

        #region Public methods.
        public new void Invoke(Delegate method) {
            if (this.InvokeRequired) {
                base.Invoke(method);
            } else {
                method.DynamicInvoke(null);
            }
        }

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="text">Text to show.</param>
        public void ShowMessageBox(string text) {
            Invoke((MethodInvoker)delegate {
                MessageBox.Show(this, text);
            });
        }

        /// <summary>
        /// Clears the listview.
        /// </summary>
        public new void Clear() {
            lock (lockObject) {
                this.Invoke((MethodInvoker)delegate {
                    this.Items.Clear();
                    this.itemDictionary.Clear();
                });

                lastSelectedItemIndex = 0;
                fileCount = 0;
                folderCount = 0;
            }
        }

        public new void BeginUpdate() {
            this.Invoke((MethodInvoker)delegate {
                base.BeginUpdate();
            });
        }

        public new void EndUpdate() {
            this.Invoke((MethodInvoker)delegate {
                base.EndUpdate();
            });
        }

        /// <summary>
        /// Removes the items.
        /// </summary>
        /// <param name="path"></param>
        public void RemoveItem(string path) {
            lock (lockObject) {
                this.Items.RemoveByKey(path);
                this.itemDictionary.Remove(path);

				IResource resource = FileSystemInfoFactory.GetFileSystemInfo(path);

				if (resource != null) {
					if (resource is FileInfo) {
						fileCount--;
					} else if (resource is DirectoryInfo) {
						folderCount--;
					}
				}
            }
        }

        /// <summary>
        /// Parses the file/directory information and updates the listview.
        /// </summary>
        public void AddItemRange(IList<IResource> resources) {
            StringBuilder sb = new StringBuilder();
            Stopwatch sw = new Stopwatch();

            this.Invoke((MethodInvoker)delegate {
                if (this.SmallImageList == null) {
                    this.SmallImageList = Forms.GetPropertyInParent<ImageList>(this.Parent, "ImageList");
                }
            });

            sw.Start();

            // Create new listview items from the resources.
            ListViewItemsCreator listViewItemsCreator = new ListViewItemsCreator(this, resources);
            List<ListViewItem> listViewItems = listViewItemsCreator.Get(sb);

            fileCount = listViewItemsCreator.FileCount;
            folderCount = listViewItemsCreator.FolderCount;

            this.Invoke((MethodInvoker)delegate {
                this.Items.AddRange(listViewItems.ToArray());
            });

            Settings.Instance.Logger.Log(LogLevelType.Verbose, "Add resources took {0} ms.",
                sw.ElapsedMilliseconds.ToString());
            sw.Reset();

            if (sb.Length > 0) {
                Settings.Instance.Logger.LogAndInvoke(LogLevelType.ErrorsOnly, ShowMessageBox, sb.ToString());
            }

            this.Invoke((MethodInvoker)delegate {
                for (int index = 0; index < this.Columns.Count; index++) {
                    ColumnInfo column = (ColumnInfo)this.Columns[index].Tag;

                    if (column.SortOrder != SortOrder.None) {
                        comparer.ColumnIndex = index;
                        comparer.SortOrder = column.SortOrder;
                    }
                }

                if (comparer.SortOrder == SortOrder.None) {
                    comparer.SortOrder = SortOrder.Ascending;
                }

                setColumnSort(comparer.ColumnIndex, comparer.SortOrder);
                this.Sort();

                // Resize the columns based on the column content.
                this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            });

            OnUpdateStatus(this);

            string path = string.Empty;

            foreach (IResource resource in resources) {
                if (!(resource is FileInfo) && !(resource is ParentDirectoryInfo) && !(resource is RootDirectoryInfo)) {
                    path = resource.Path.ToLower();
                    break;
                }
            }

            if (previousTopIndexes.ContainsKey(path)) {
                int previousTopIndex = previousTopIndexes[path];
                
                if (previousTopIndex > 0 && previousTopIndex < Items.Count) {
                    this.Invoke((MethodInvoker)delegate {
                        if (previousTopIndex < Items.Count) {
                            this.EnsureVisible(previousTopIndex);
                        } else {
                            this.EnsureVisible(Items.Count - 1);
                        }

                        previousTopIndexes.Remove(path);
                    });
                }
            }
        }

        public void UpdateImageIndexes(bool useFileAttributes) {
            // Lock this in case the listview items are changed while icons are being generated.
            lock (lockObject) {
                ListViewItemsImageIndexer imageIndexer = new ListViewItemsImageIndexer(this);
                imageIndexer.Update(useFileAttributes);
            }
        }

        public void ClearPreviousTopIndexes() {
            previousTopIndexes.Clear();
        }

        /// <summary>
        /// Parses the file/directory information and inserts the file info into the listview.
        /// </summary>
        public void AddItem(IChildResource resource) {
            if (resource != null) {
                // Create a new listview item.
                StringBuilder sb = new StringBuilder();
                ListViewItemsCreator listViewItemsCreator = new ListViewItemsCreator(this, resource);
                List<ListViewItem> items = listViewItemsCreator.Get(sb);

                if (items.Count > 0) {
                    ListViewItem item = items[0];
                    fileCount += listViewItemsCreator.FileCount;
                    folderCount += listViewItemsCreator.FolderCount;

                    if (sb.Length > 0) {
                        Settings.Instance.Logger.LogAndInvoke(LogLevelType.ErrorsOnly, ShowMessageBox, 
                            sb.ToString());
                    }

                    this.Items.Add(item);

                    // Basic stuff that should happen everytime files are shown.
                    this.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    OnUpdateStatus(this);
                    this.Sort();

                    int imageIndex = OnGetImageIndex(resource, false);

                    if (imageIndex > -1) {
                        item.ImageIndex = imageIndex;
                    }
                }
            }
        }
		#endregion

        #region Private methods.
        /// <summary>
        /// Sets the previous top index.
        /// </summary>
        /// <param name="resource">Resource that is being executed.</param>
        private void setPreviousTopIndex(IResource resource) {
            if (!(resource is FileInfo) && !(resource is RootDirectoryInfo) && !(resource is ParentDirectoryInfo)) {
                string path = ((IResource)this.TopItem.Tag).Path.ToLower();

                // Get the top of the bottom scrollbar.
                int bottomBound = this.ClientRectangle.Bottom;
                // Get the client height and divide by the top item's rectangle height;
                // this gives the number of visible items (plus one for the headerbar)
                int itemsVisible =
                    (this.ClientRectangle.Height / this.TopItem.Bounds.Height) - 2;

                // Add one item if the horizontal scrollbar is visible.
                if (false) {
                    itemsVisible++;
                }

                int lastVisibleIndex = this.TopItem.Index + itemsVisible;

                if (previousTopIndexes.ContainsKey(path)) {
                    previousTopIndexes[path] = lastVisibleIndex;
                } else {
                    previousTopIndexes.Add(path, lastVisibleIndex);
                }
            }
        }

        /// <summary>
        /// Show the correct sort string graphic.
        /// </summary>
        /// <param name="index">Index of the column to sort.</param>
        /// <param name="sortOrder">Sort order for the column.</param>
        private void setColumnSort(int index, SortOrder sortOrder) {
            const string descendingSortString = "\u25BC ";
            const string ascendingSortString = "\u25B2 ";

            string sortString = ascendingSortString;

            // Set the correct sor
            if (sortOrder == SortOrder.Descending) {
                sortString = descendingSortString;
            } else if (sortOrder == SortOrder.None) {
                sortString = string.Empty;
            }

            // Clear the sort graphic for all columns.
            foreach (ColumnHeader column in this.Columns) {
                column.Text = column.Text.Replace(descendingSortString, string.Empty);
                column.Text = column.Text.Replace(ascendingSortString, string.Empty);
                this.ColumnInfos[column.Index].SortOrder = SortOrder.None;
            }

            // Show the correct sort for the column.
            this.Columns[index].Text = sortString + this.Columns[index].Text;
            this.ColumnInfos[index].SortOrder = sortOrder;
        }

        /// <summary>
        /// Validates the edit label and displays a balloon tip if it is not.
        /// </summary>
        /// <param name="handle">Handle of the label.</param>
        /// <returns>Whether the label is valid or not.</returns>
        private bool validateEditLabel(IntPtr handle) {
            StringBuilder sb = new StringBuilder(256);
			User32.GetWindowText(handle.ToInt32(), sb, 255);
            string label = sb.ToString();

            foreach (char ch in System.IO.Path.GetInvalidFileNameChars()) {
                if (label.IndexOf(ch) > -1) {
                    const string text = "Filenames cannot have any illegal characters including: \", |, <, >, *, ?, :, /, \\";

                    EditBalloon editBalloon = new EditBalloon(handle);
                    editBalloon.Title = "Illegal Characters";
                    editBalloon.TitleIcon = TooltipIcon.Warning;
                    editBalloon.Text = text;
                    editBalloon.Show();

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Calculates the size of the currently selected item.
        /// </summary>
        private void calculateSize() {
            if (this.SelectedItems != null
                && this.SelectedItems.Count > 0) {
                int maxIndex = 0;
                int sizeIndex = getSizeColumnIndex();
                ColumnInfo columnInfo = ColumnInfos[sizeIndex];

                foreach (ListViewItem item in this.SelectedItems) {
                    // Get the last item that was selected. Used to determine the next item to focus on.
                    if (item.Index > maxIndex) {
                        maxIndex = item.Index;
                    }

                    IChildResource childResource = (IChildResource)item.Tag;
                    Settings.Instance.Logger.Log(LogLevelType.Verbose, "Attempting to calculate the size of {0}",
                        childResource.FullName);

                    lock (lockObject) {
                        if (!selectedResources.Contains(childResource)) {
                            item.ForeColor = Color.Red;
                            selectedResources.Add(childResource);

                            if (string.IsNullOrEmpty(item.SubItems[sizeIndex].Text)
                                || childResource is DirectoryInfo) {
                                // Calculates the resource size in a background thread. 
                                // Necessary for directories because it is labor-intensive.
                                getResourceSize(columnInfo, item, sizeIndex);
                            }                            
                        } else {
                            item.ForeColor = Color.Black;
                            selectedResources.Remove(childResource);
                        }

                        // Update the status and resize columns as appropriate.                    
                        this.AutoResizeColumn(sizeIndex, ColumnHeaderAutoResizeStyle.ColumnContent);
                        OnUpdateStatus(this);
                    }

                    // Unfocus and unselect the selected item.
                    item.Focused = false;
                    item.Selected = false;
                }

                // Focus on the next item that hasn't been selected.
                int nextIndex = maxIndex + 1;
                if (this.Items.Count > nextIndex) {
                    this.Items[nextIndex].Focused = true;
                    this.Items[nextIndex].Selected = true;
                }
            }
        }

        /// <summary>
        /// Gets the column index of the size property for a ListViewItem.
        /// </summary>
        /// <returns>The index of the column that uses the size property for resources.</returns>
        private int getSizeColumnIndex() {
            int sizeIndex = -1;

            // Look for the size property within the column infos.
            foreach (ColumnInfo columnInfo in ColumnInfos) {
                if (columnInfo.Property.Equals("Size")) {
                    // Look for the column header that has the same text as the column info.
                    foreach (ColumnHeader columnHeader in Columns) {
                        if (columnHeader.Text.Equals(columnInfo.Text)) {
                            sizeIndex = columnHeader.Index;
                            break;
                        }
                    }
                }

                // Break out of the loop if the correct index has been found.
                if (sizeIndex > -1) {
                    break;
                }
            }

            return sizeIndex;
        }

        /// <summary>
        /// Gets the size of a resource in a background worker thread.
        /// </summary>
        /// <param name="columnInfo">Column info of the size property.</param>
        /// <param name="item">ListViewItem to retrieve the size from.</param>
        /// <param name="sizeIndex">Index of the columns that has the size.</param>
        private void getResourceSize(ColumnInfo columnInfo, ListViewItem item, int sizeIndex) {
            const string ellipsis = "...";
            IChildResource childResource = (IChildResource)item.Tag;

            using (BackgroundWorker backgroundWorker = new BackgroundWorker()) {
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.DoWork += delegate(object anonymousSender, DoWorkEventArgs eventArgs) {
                    backgroundWorker.ReportProgress(50);

                    // Update the list view through BeginInvoke so that the correct thread is used.
                    this.BeginInvoke((MethodInvoker)delegate {
                        item.SubItems[sizeIndex].Text = ellipsis;
                        this.AutoResizeColumn(sizeIndex, ColumnHeaderAutoResizeStyle.ColumnContent);
                    });

                    eventArgs.Result = childResource.Size;
                    backgroundWorker.ReportProgress(100);
                };

                backgroundWorker.ProgressChanged += delegate(object anonymousSender, ProgressChangedEventArgs eventArgs) {
                    OnUpdateProgress(eventArgs.ProgressPercentage);
                };

                backgroundWorker.RunWorkerCompleted +=
                    delegate(object anonymousSender, RunWorkerCompletedEventArgs eventArgs) {
                        if (eventArgs.Error == null &&
                            eventArgs.Result != null &&
                            eventArgs.Result is long) {
                            string size = ((long)eventArgs.Result).ToString();

                            // Update the list view through BeginInvoke so that the correct thread is used.
                            this.BeginInvoke((MethodInvoker)delegate {
                                if (columnInfo.MethodDelegate != null) {
                                    item.SubItems[sizeIndex].Text = columnInfo.MethodDelegate.Invoke(size);
                                } else {
                                    item.SubItems[sizeIndex].Text = size;
                                }
                            });
                        }
                    };

                backgroundWorker.RunWorkerAsync(childResource);
            }
        }

        /// <summary>
        /// Gets the selected paths.
        /// </summary>
        private List<string> getSelectedPaths() {
            if (this.SelectedItems.Count == 0) {
                return new List<string>(0);
            }

            // Get an array of the listview items.
            ListViewItem[] itemArray = new ListViewItem[this.SelectedItems.Count];
            this.SelectedItems.CopyTo(itemArray, 0);

            // Convert the listviewitem array into a string array of the paths.
            string[] nameArray = Array.ConvertAll<ListViewItem, string>(itemArray, delegate(ListViewItem item) {
                return item.Name;
            });

            return new List<string>(nameArray);
        }
        #endregion

        #region Properties.
        /// <summary>
        /// File count.
        /// </summary>
        public long FileCount {
            get {
                return fileCount;
            }
        }

        /// <summary>
        /// Folder count.
        /// </summary>
        public long FolderCount {
            get {
                return folderCount;
            }
        }

        /// <summary>
        /// Current path.
        /// </summary>
        public IResource Path {
            get {
                string path = Forms.GetPropertyInParent<string>(this.Parent, "Path");
                return FileSystemInfoFactory.GetFileSystemInfo(path);
            }
        }

        /// <summary>
        /// Current filter.
        /// </summary>
        public string Filter {
            get {
                return Forms.GetPropertyInParent<string>(this.Parent, "Filter");
            }
        }

        /// <summary>
        /// Current FileSystemWatcher.
        /// </summary>
        public FileSystemWatcher FileSystemWatcher {
            get {
                return Forms.GetPropertyInParent<FileSystemWatcher>(this.Parent, "FileSystemWatcher");
            }
        }

        public Control Control {
            get {
                return this;
            }
        }

        public IViewComparer Comparer {
            get {
                return comparer;
            }
            set {
                comparer = value;
            }
        }

        public List<ColumnInfo> ColumnInfos {
            get {
                return columnInfos;
            }
            set {
                columnInfos = value;

                this.Invoke((MethodInvoker)delegate {
                    this.Columns.Clear();

                    foreach (ColumnInfo columnInfo in columnInfos) {
                        ColumnHeader columnHeader = new ColumnHeader();
                        columnHeader.Text = columnInfo.Text;
                        columnHeader.Tag = columnInfo;
                        this.Columns.Add(columnHeader);
                    }
                });
            }
        }

        public IntPtr LabelEditHandle {
            get {
				return User32.SendMessage(Handle,
                    (int)LVM.GETEDITCONTROL, 0, IntPtr.Zero);
            }
        }

        public IResource SelectedResource {
            get {
                if (SelectedItems.Count == 0) {
                    return Path;
                } else {
                    return (IResource)SelectedItems[0].Tag;
                }
            }
        }

        public ResourceContainer SelectedResources {
            get {
                return selectedResources;
            }
        }

        public Dictionary<string, ListViewItem> ItemDictionary {
            get {
                return itemDictionary;
            }
        }
        #endregion
    }
}