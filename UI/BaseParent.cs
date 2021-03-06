using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Common.Logger;
using SharpFile.Infrastructure;
using SharpFile.Infrastructure.Interfaces;
using SharpFile.Infrastructure.SettingsSection;
using WeifenLuo.WinFormsUI.Docking;

namespace SharpFile.UI {
	public class BaseParent : Form {
		protected const string formName = "SharpFile";

		private static readonly object lockObject = new object();
        private DriveDetector driveDetector;
		private int progressCount = 0;
        private bool isAdvancedLayout = false;

		protected ToolTip toolTip = new ToolTip();
		protected Timer timer = new Timer();
		protected ProgressDisk progressDisk = new ProgressDisk();
		protected StatusStrip statusStrip = new StatusStrip();
		protected ToolStripStatusLabel toolStripStatus = new ToolStripStatusLabel();
		protected MenuStrip menuStrip = new MenuStrip();
        protected NotifyIcon notifyIcon = new NotifyIcon();
        
        protected DockPanel dockPanel = new DockPanel();
        protected ContextMenu dockPanelContextMenu = new ContextMenu();

		protected ToolStripMenuItem fileMenu = new ToolStripMenuItem();
		protected ToolStripMenuItem exitToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem reloadSettingsStripMenuItem = new ToolStripMenuItem();

		protected ToolStripSeparator toolStripSeparator6 = new ToolStripSeparator();
		protected ToolStripSeparator toolStripSeparator7 = new ToolStripSeparator();

		protected ToolStripMenuItem viewMenu = new ToolStripMenuItem();
		protected ToolStripMenuItem statusBarToolStripMenuItem = new ToolStripMenuItem();
        protected ToolStripMenuItem pluginPaneToolStripMenuItem = new ToolStripMenuItem();

		protected ToolStripMenuItem toolsMenu = new ToolStripMenuItem();

		protected ToolStripMenuItem helpMenu = new ToolStripMenuItem();
		protected ToolStripMenuItem aboutToolStripMenuItem = new ToolStripMenuItem();

		public BaseParent() {
			initializeComponents();
			this.DoubleBuffered = true;

            driveDetector = new DriveDetector();

			SetStyle(ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.OptimizedDoubleBuffer, true);

            this.FormClosing += BaseParent_FormClosing;
            this.Load += BaseParent_Load;

            this.Resize += delegate {
				this.progressDisk.Location = new Point(base.ClientSize.Width - 35,
													   base.ClientSize.Height - 18);

                if (Settings.Instance.MinimizeToSystray) {
                    if (FormWindowState.Minimized == WindowState) {
                        Hide();
                        this.notifyIcon.Visible = true;
                    }
                }
			};

			timer.Enabled = true;
			timer.Tick += delegate {
				progressDisk.Value = (progressDisk.Value + 1) % 12;
			};
		}

        protected void updateProgress(int value) {
            lock (lockObject) {
                if (value < 100) {
                    progressCount++;
                } else if (value == 100) {
                    if (progressCount > 0) {
                        progressCount--;
                    }
                }

                if (progressCount > 0) {
                    if (!timer.Enabled) {
                        progressDisk.Value = 4;
                        progressDisk.Visible = true;
                        timer.Enabled = true;
                    }
                } else {
                    if (timer.Enabled) {
                        progressDisk.Visible = false;
                        timer.Enabled = false;
                    }
                }
            }
        }

        private void BaseParent_Load(object sender, EventArgs e) {
            this.Width = Settings.Instance.Width;
            this.Height = Settings.Instance.Height;
            this.Location = Settings.Instance.Location;
            this.WindowState = Settings.Instance.WindowState;

            try {
                onFormLoad();
            } catch (ReflectionTypeLoadException ex) {
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                    "Reflection type error while trying to generate settings when the form was opening.");

                Settings.Instance.Logger.LogNumerous(LogLevelType.ErrorsOnly, ex.LoaderExceptions,
                        "Settings loader exception.");

                throw;
            } catch (Exception ex) {
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                    "Error while trying to generate settings when the form was opening.");

                throw;
            }
        }

		private void BaseParent_FormClosing(object sender, FormClosingEventArgs e) {
			Settings.Instance.Width = this.Width;
			Settings.Instance.Height = this.Height;
            Settings.Instance.Location = this.Location;
            Settings.Instance.WindowState = this.WindowState;

            try {
                onFormClosing();
            } catch (Exception ex) {
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                    "Error while trying to save settings when the form was closing.");
            }
		}

		private void initializeComponents() {
			ComponentResourceManager resources = new ComponentResourceManager(typeof(BaseParent));

            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.menuStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();

			addMenuStripItems();
			this.menuStrip.BackColor = SystemColors.Control;
            this.menuStrip.RenderMode = ToolStripRenderMode.System;
            this.menuStrip.TabIndex = 0;

            this.fileMenu.DropDownItems.AddRange(new ToolStripItem[]
			                                     	{
                                                        this.reloadSettingsStripMenuItem,
														this.exitToolStripMenuItem
			                                     	});
            this.fileMenu.ImageTransparentColor = SystemColors.ActiveBorder;
            this.fileMenu.Size = new Size(35, 20);
            this.fileMenu.Text = "&File";

            this.exitToolStripMenuItem.Size = new Size(145, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += this.exitToolsStripMenuItem_Click;

            this.reloadSettingsStripMenuItem.Text = "&Reload Settings";
            this.reloadSettingsStripMenuItem.Click += this.reloadSettingsStripMenuItem_Click;

			this.viewMenu.DropDownItems.AddRange(new ToolStripItem[]
			                                     	{
                                                        this.pluginPaneToolStripMenuItem,
			                                     		this.statusBarToolStripMenuItem                                                        
			                                     	});
			this.viewMenu.Size = new Size(41, 20);
			this.viewMenu.Text = "&View";

			this.statusBarToolStripMenuItem.Checked = true;
			this.statusBarToolStripMenuItem.CheckOnClick = true;
			this.statusBarToolStripMenuItem.CheckState = CheckState.Checked;
			this.statusBarToolStripMenuItem.Size = new Size(135, 22);
			this.statusBarToolStripMenuItem.Text = "&Status Bar";
			this.statusBarToolStripMenuItem.Click += statusBarToolStripMenuItem_Click;

            this.pluginPaneToolStripMenuItem.Size = new Size(135, 22);
            this.pluginPaneToolStripMenuItem.Text = "&Plugin Pane";
            this.pluginPaneToolStripMenuItem.Checked = !Settings.Instance.PluginPanes.IsHidden;
            this.pluginPaneToolStripMenuItem.Click += pluginPaneToolStripMenuItem_Click;

			this.toolsMenu.Text = "&Tools";

			this.helpMenu.DropDownItems.AddRange(new ToolStripItem[]
			                                     	{
			                                     		this.aboutToolStripMenuItem
			                                     	});
			this.helpMenu.Size = new Size(40, 20);
			this.helpMenu.Text = "&Help";

			this.aboutToolStripMenuItem.Size = new Size(173, 22);
			this.aboutToolStripMenuItem.Text = "&About ...";
            this.aboutToolStripMenuItem.Click += delegate {
                AboutBox aboutBox = new AboutBox();
                aboutBox.Show();
            };

			this.progressDisk.ActiveForeColor1 = Color.LightGray;
			this.progressDisk.ActiveForeColor2 = Color.White;
			this.progressDisk.BackGroundColor = Color.Transparent;
			this.progressDisk.BlockSize = BlockSize.Medium;
			this.progressDisk.InactiveForeColor1 = Color.DimGray;
			this.progressDisk.InactiveForeColor2 = Color.DarkGray;
			this.progressDisk.Location = new Point(597, 435);
			this.progressDisk.Size = new Size(16, 16);
			this.progressDisk.SquareSize = 16;
			this.progressDisk.TabIndex = 4;

			this.statusStrip.Items.AddRange(new ToolStripItem[]
			                                	{
			                                		this.toolStripStatus
			                                	});
			this.statusStrip.Location = new Point(0, 431);
			this.statusStrip.Size = new Size(632, 22);
			this.statusStrip.Dock = DockStyle.Bottom;
			this.statusStrip.Visible = true;

			this.toolStripStatus.Size = new Size(0, 10);
			this.toolStripStatus.Dock = DockStyle.Bottom;

            this.dockPanelContextMenu.Popup += new EventHandler(dockPanelContextMenu_Popup);

            this.dockPanel.Dock = DockStyle.Fill;
            this.dockPanel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((byte)(0)));
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.TabIndex = 0;
            this.dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            this.Controls.Add(this.dockPanel);

			addControls();

			this.MainMenuStrip = this.menuStrip;

			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();

            this.notifyIcon.Text = "SharpFile";
            this.notifyIcon.Visible = false;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            
            this.notifyIcon.DoubleClick += delegate(object sender, EventArgs e) {
                Show();
                this.WindowState = FormWindowState.Normal;
                this.notifyIcon.Visible = false;
            };

			this.ResumeLayout(false);
			this.PerformLayout();
		}

        protected virtual void dockPanelContextMenu_Popup(object sender, EventArgs e) {
            ContextMenu dockPanelContextMenu = (ContextMenu)sender;
            dockPanelContextMenu.MenuItems.Clear();

            MenuItem addTabMenuItem = new MenuItem("Add tab", dockPanelContextMenuOnClick);
            MenuItem closeTabMenuItem = new MenuItem("Close tab", dockPanelContextMenuOnClick);

            try {
                // Don't show the context menu unless the mouse cursor is inside the "tab" area.
                if (MousePosition.Y < PointToScreen(dockPanel.Location).Y + 30) {
                    if (dockPanel.ActivePane.Contents.Count == 1) {
                            dockPanelContextMenu.MenuItems.AddRange(new MenuItem[] {
                            addTabMenuItem
                        });
                    } else {
                        dockPanelContextMenu.MenuItems.AddRange(new MenuItem[] {
                            addTabMenuItem,
                            closeTabMenuItem
                        });
                    }
                }
            } catch (Exception ex) {
                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                    "Trying to determine which menu items to add to the dock panel's context menu.");
            }
        }

        private void addBrowser(string path) {
            Browser browser = getBrowser(path);
            browser.Show(dockPanel);
        }

        protected Browser getBrowser(string path) {
            Browser browser = new Browser("view1");
            browser.Path = path;
            browser.DockHandler.DockAreas = DockAreas.Document | DockAreas.DockBottom;

            if (isAdvancedLayout) {
                browser.AllowEndUserDocking = true;
            } else {
                browser.AllowEndUserDocking = false;
            }

            browser.UpdateStatus += delegate(IView view) {
				// TODO: Add Folder/file count and to the ResourceContainer to show something like 2/25 folders, 0/10 files.
                toolStripStatus.Text = string.Format("{0}/{1} folders, {2}/{3} files, {4} selected",
                    view.SelectedResources.FolderCount,
                    view.FolderCount,
                    view.SelectedResources.FileCount,
                    view.FileCount,
                    view.SelectedResources.HumanReadableTotalSize);
            };

            browser.UpdateProgress += delegate(int value) {
                updateProgress(value);
            };

            browser.GetImageIndex += delegate(IResource fsi, bool useFileAttributes) {
                return IconManager.GetImageIndex(fsi, useFileAttributes, ImageList);
            };

            browser.UpdatePath += delegate(IResource updatePath) {
                this.Text = string.Format("{0} - {1}",
                                          formName,
                                          updatePath.FullName);

                Settings.Instance.DualParent.SelectedPath = updatePath.FullName;
            };

            browser.UpdatePluginPanes += delegate(IView view) {
                foreach (IPluginPane panel in Settings.Instance.PluginPanes.Instances) {
                        panel.Update(view);

                        if (panel.DockHandler.IsActivated) {
                            panel.DockHandler.GiveUpFocus();
                        }
                }

                if (view.SelectedResource != null) {
                    Settings.Instance.DualParent.SelectedFile = view.SelectedResource.FullName;
                }
            };

            browser.KeyDown += delegate(object sender, KeyEventArgs e) {
                if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Tab) {
                    int nextContentIndex =
                        this.dockPanel.ActiveDocumentPane.Contents.IndexOf(this.dockPanel.ActiveDocument);
                    nextContentIndex++;

                    if (this.dockPanel.ActiveDocumentPane.Contents.Count <= nextContentIndex) {
                        nextContentIndex = 0;
                    }

                    this.dockPanel.ActivePane.Contents[nextContentIndex].DockHandler.Show();
                } else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.W) {
                    if (this.dockPanel.ActiveDocument.DockHandler.CloseButton) {
                        this.dockPanel.ActiveDocument.DockHandler.Close();
                    }
                } else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.E) {
                    ((Browser)this.dockPanel.ActiveDocument).FocusPath();
                } else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F) {
                    ((Browser)this.dockPanel.ActiveDocument).FocusFilter();
                }
            };

            return browser;
        }

        /// <summary>
        /// Override the ProcessCmdKey method to handle tab key presses.
        /// </summary>
        /// <param name="msg">Message.</param>
        /// <param name="keyData">Key data.</param>
        /// <returns>True if the key has been processed, otherwise returns the result from calling the base's ProcessCmdKey method.</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            switch (keyData) {
                case Keys.Tab:
                    // Calculate the next pane index and activate it.
                    int nextPaneIndex =
                        this.dockPanel.Panes.IndexOf(this.dockPanel.ActivePane);
                    nextPaneIndex++;

                    if (this.dockPanel.Panes.Count <= nextPaneIndex 
                        || this.dockPanel.Panes[nextPaneIndex].IsHidden
                        || !this.dockPanel.Panes[nextPaneIndex].Visible) {
                        nextPaneIndex = 0;
                    }

                    this.dockPanel.Panes[nextPaneIndex].Activate();

                    // Return true to indicate that the key has been processed.
                    // http://msdn.microsoft.com/en-us/library/system.windows.forms.control.processcmdkey.aspx
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }

        protected virtual void dockPanelContextMenuOnClick(object sender, EventArgs e) {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Text) {
                case "Add tab":
                    addBrowser(Infrastructure.SettingsSection.DualParent.DefaultDrive);
                    break;
                case "Close tab":
                    if (dockPanel.ActivePane.Contents.Count != 1 
                        && dockPanel.ActiveDocument.DockHandler.CloseButton) {
                        dockPanel.ActiveDocument.DockHandler.Close();
                    }

                    if (dockPanel.ActivePane.Contents.Count == 1) {
                        dockPanel.ActivePane.Contents[0].DockHandler.CloseButton = false;
                    }
                    break;
            }
        }

        private void reloadSettingsStripMenuItem_Click(object sender, EventArgs e) {
			Settings.Load(Settings.Instance);

            // Call a method called reload on every child control to reload data.
            // Refresh the screen.
        }

		private void exitToolsStripMenuItem_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void statusBarToolStripMenuItem_Click(object sender, EventArgs e) {
			statusStrip.Visible = statusBarToolStripMenuItem.Checked;
		}

        private void pluginPaneToolStripMenuItem_Click(object sender, EventArgs e) {
            bool pluginPaneToolStripMenuItemChecked = !pluginPaneToolStripMenuItem.Checked;
            togglePluginPaneToolStripMenu(pluginPaneToolStripMenuItemChecked);
        }

		protected virtual void addControls() {
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.progressDisk);
            this.Controls.Add(this.statusStrip);
		}

        protected void addPluginPanes() {
            // Load plugin panes.
            int paneIndex = dockPanel.Panes.Count;

            foreach (IPluginPane pane in Settings.Instance.PluginPanes.Instances) {
                // Need to get some settings before the pane is shown because Show() will wipe them clean.
                bool isHidden = pane.DockHandler.IsHidden;

                pane.Dock = DockStyle.None;
                pane.DockAreas = DockAreas.DockBottom;
                pane.AllowEndUserDocking = false;
                pane.DockHandler.HideOnClose = true;

                // For the first plugin, create a new pane on the dock panel. Otherwise, attach to first pane.
                if (dockPanel.Panes.Count == paneIndex) {
                    pane.Show(dockPanel, Settings.Instance.PluginPanes.VisibleState);
                } else {
                    pane.Show(dockPanel.Panes[paneIndex], null);
                }

                if (Settings.Instance.PluginPanes.IsHidden) {
                    pane.DockHandler.Hide();
                }

                if (isHidden) {
                    pane.DockHandler.Hide();
                }                

                // Update the check status if the DockStateChanged delegate fired.
                pane.DockHandler.DockStateChanged += delegate(object sender, EventArgs e) {
                    foreach (ToolStripMenuItem item in pluginPaneToolStripMenuItem.DropDownItems) {
                        IPluginPane p = (IPluginPane)item.Tag;
                        DockContentHandler handler = (DockContentHandler)sender;

                        if (handler.Form.Name.Equals(p.Name)) {
                            if (handler.IsHidden) {
                                item.Checked = false;
                            } else {
                                item.Checked = true;
                            }

                            updatePluginPaneToolStripMenuItemCheckedStatus();
                            break;
                        }
                    }
                };

                // Add menu item for the pane to control visibility.
                ToolStripMenuItem menuItem = new ToolStripMenuItem(pane.Name);
                menuItem.Tag = pane;
                menuItem.Checked = (pane.DockState != DockState.Hidden);

                // Show/hide the pane depending on its previous state.
                menuItem.Click += delegate(object sender, EventArgs e) {
                    IPluginPane p = (IPluginPane)menuItem.Tag;

                    if (p.DockHandler.IsHidden) {
                        p.DockHandler.Show();
                        p.DockHandler.GiveUpFocus();
                    } else {
                        p.DockHandler.Hide();
                    }

                    updatePluginPaneToolStripMenuItemCheckedStatus();
                };

                this.pluginPaneToolStripMenuItem.DropDownItems.Add(menuItem);
            }

            // Activate the correct pane.
            foreach (IPluginPane pane in Settings.Instance.PluginPanes.Instances) {
                if (pane.IsActivated) {
                    pane.DockHandler.Activate();
                }
            }

            if (dockPanel.Panes.Count > paneIndex) {
                DockPane pluginPane = dockPanel.Panes[paneIndex];
                pluginPane.Name = "pluginPane";

                if (pluginPane.DockState == DockState.DockBottom 
                    || pluginPane.DockState == DockState.DockBottomAutoHide) {
                    dockPanel.DockBottomPortion = Settings.Instance.PluginPanes.DockPortion;
                }
            }
        }

        protected virtual void onFormClosing() {
            if (Settings.Instance.PluginPanes.Instances.Count > 0) {
                IPluginPane pluginPane = Settings.Instance.PluginPanes.Instances[0];
                Settings.Instance.PluginPanes.VisibleState = pluginPane.VisibleState;

                if (pluginPane.DockState == DockState.DockBottom
                    || pluginPane.DockState == DockState.DockBottomAutoHide) {
                    Settings.Instance.PluginPanes.DockPortion = dockPanel.DockBottomPortion;
                }
            }

            Settings.Instance.PluginPanes.IsHidden = !pluginPaneToolStripMenuItem.Checked;

            foreach (IPluginPane pane in Settings.Instance.PluginPanes.Instances) {
                foreach (PluginPane pluginPaneSetting in Settings.Instance.PluginPanes.Panes) {
                    if (pane.Name.Equals(pluginPaneSetting.Name)) {
                        pluginPaneSetting.AutoHidePortion = pane.AutoHidePortion;
                        pluginPaneSetting.IsHidden = pane.DockHandler.IsHidden;
                        pluginPaneSetting.IsActivated = pane.DockHandler.IsActivated;
                    }
                }
            }
        }

        protected virtual void onFormLoad() {
            foreach (Tool tool in Settings.Instance.DualParent.Tools) {
                string menuItemName = tool.Name;
                ToolStripMenuItem menuItem = new ToolStripMenuItem(menuItemName);

                if (tool.Key.HasValue && tool.Key.Value.PrimaryKey != Keys.None) {
                    Keys keys = tool.Key.Value.PrimaryKey;

                    if (tool.Key.Value.ModifierKeys != null && tool.Key.Value.ModifierKeys.Count > 0) {
                        foreach (Keys key in tool.Key.Value.ModifierKeys) {
                            keys |= key;
                        }
                    }

                    menuItem.ShortcutKeys = keys;
                }
                
                menuItem.Tag = tool;
                menuItem.Click += (EventHandler)delegate {
                    Tool t = (Tool)menuItem.Tag;

                    if (!t.Name.Equals(Tool.SeperatorName, StringComparison.InvariantCultureIgnoreCase)) {
                        t.Execute();
                    }
                };

                if (tool.Name.Equals(Tool.SeperatorName, StringComparison.InvariantCultureIgnoreCase)) {
                    this.toolsMenu.DropDownItems.Insert(this.toolsMenu.DropDownItems.Count, new ToolStripSeparator());
                } else {
                    this.toolsMenu.DropDownItems.Insert(this.toolsMenu.DropDownItems.Count, menuItem);
                }
            }

            togglePluginPaneToolStripMenu(!Settings.Instance.PluginPanes.IsHidden);
        }

		protected virtual void addMenuStripItems() {
		}

        private void togglePluginPaneToolStripMenu(bool pluginPaneToolStripMenuItemChecked) {
            foreach (DockPane pane in dockPanel.Panes) {
                if (pane.Name.Equals("pluginPane")) {
                    if (dockPanel.Panes.Count > 0) {
                        pluginPaneToolStripMenuItem.Checked = pluginPaneToolStripMenuItemChecked;

                        // Show/hide each piece of content in the plugin pane.
                        foreach (IDockContent content in pane.Contents) {
                            if (pluginPaneToolStripMenuItemChecked) {
                                content.DockHandler.Show();
                                content.DockHandler.GiveUpFocus();
                            } else {
                                content.DockHandler.Hide();
                            }
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the plugin pane menu item depending on the status of its sub items.
        /// </summary>
        private void updatePluginPaneToolStripMenuItemCheckedStatus() {
            // Uncheck the main menu item if all of the items are also unchecked.
            bool pluginPaneToolStripMenuItemChecked = false;

            foreach (ToolStripMenuItem i in pluginPaneToolStripMenuItem.DropDownItems) {
                pluginPaneToolStripMenuItemChecked |= i.Checked;
            }

            pluginPaneToolStripMenuItem.Checked = pluginPaneToolStripMenuItemChecked;
        }

        public DriveDetector DriveDetector {
            get {
                return driveDetector;
            }
        }

		public ImageList ImageList {
			get {
				return Settings.Instance.ImageList;
			}
		}
	}
}