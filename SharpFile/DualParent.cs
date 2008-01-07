using System;
using System.Drawing;
using System.Windows.Forms;
using Common;
using SharpFile.Infrastructure;
using SharpFile.UI;

namespace SharpFile {
	public class DualParent : BaseParent {
		private SplitContainer splitContainer = new SplitContainer();
        private ToolStripMenuItem swapPanelItem = new ToolStripMenuItem();
		private ToolStripMenuItem displayPanelItem = new ToolStripMenuItem();
		private ToolStripMenuItem displayLeftPanelItem = new ToolStripMenuItem();
		private ToolStripMenuItem displayRightPanelItem = new ToolStripMenuItem();
		private int splitterPercentage;

		public DualParent() {
			Child child1 = new Child("view1");
			child1.TabControl.Appearance = TabAppearance.FlatButtons;
			child1.TabControl.IsVisible = true;
			child1.Dock = DockStyle.Fill;
			child1.OnGetImageIndex += delegate(IResource fsi) {
				return IconManager.GetImageIndex(fsi, ImageList);
			};

			child1.OnUpdateStatus += delegate(string status) {
				toolStripStatus.Text = status;
			};

			child1.OnUpdateProgress += delegate(int value) {
                updateProgress(value);
			};

			child1.OnUpdatePath += delegate(string path) {
				this.Text = string.Format("{0} - {1}",
										  formName,
										  path);
			};

			Child child2 = new Child("view2");
			child2.Dock = DockStyle.Fill;
			child2.TabControl.IsVisible = true;
			child2.TabControl.Appearance = TabAppearance.FlatButtons;
			child2.OnGetImageIndex += delegate(IResource fsi) {
				return IconManager.GetImageIndex(fsi, ImageList);
			};

			child2.OnUpdateStatus += delegate(string status) {
				toolStripStatus.Text = status;
			};

			child2.OnUpdateProgress += delegate(int value) {
                updateProgress(value);
			};

			child2.OnUpdatePath += delegate(string path) {
				this.Text = string.Format("{0} - {1}",
										  formName,
										  path);
			};

            splitContainer.SplitterWidth = 1;
			splitContainer.Panel1.Controls.Add(child1);
			splitContainer.Panel2.Controls.Add(child2);

			splitContainer.SplitterMoving += delegate(object sender, SplitterCancelEventArgs e) {
				decimal percent = Convert.ToDecimal(e.SplitX) / Convert.ToDecimal(this.Width);
				splitterPercentage = Convert.ToInt32(percent * 100);

				string tip = string.Format("{0}%",
					splitterPercentage);

				toolTip.Show(tip, this, e.MouseCursorX - 10, e.MouseCursorY);
			};

			splitContainer.SplitterMoved += delegate {
				toolTip.RemoveAll();
			};

			MenuItem twentyFivePercentMenuItem = new MenuItem("25%", splitterContextMenuOnClick);
			MenuItem fiftyPercentMenuItem = new MenuItem("50%", splitterContextMenuOnClick);
			MenuItem seventyFivePercentMenuItem = new MenuItem("75%", splitterContextMenuOnClick);
			ContextMenu splitterContextMenu = new ContextMenu(new MenuItem[] {
				twentyFivePercentMenuItem,
				fiftyPercentMenuItem,
				seventyFivePercentMenuItem
			});

			splitContainer.ContextMenu = splitterContextMenu;
		}

		private void splitterContextMenuOnClick(object sender, EventArgs e) {
			MenuItem menuItem = (MenuItem)sender;
            splitterPercentage = Convert.ToInt32(menuItem.Text.Replace("%", string.Empty));
            double percent = Convert.ToDouble(splitterPercentage) * 0.01;
			splitContainer.SplitterDistance = Convert.ToInt32(this.Width * percent);
		}

		protected override void addMenuStripItems() {
			this.menuStrip.Items.AddRange(new ToolStripItem[]
			                              	{
			                              		this.fileMenu,
			                              		this.editMenu,
			                              		this.viewMenu,
			                              		this.toolsMenu,
			                              		this.helpMenu
			                              	});

			this.viewMenu.DropDownItems.Add(displayPanelItem);
            this.viewMenu.DropDownItems.Add(swapPanelItem);

            this.swapPanelItem.Text = "Swap Panels";
            this.swapPanelItem.MouseUp += delegate(object sender, MouseEventArgs e) {
                Control panel1Control = this.splitContainer.Panel1.Controls[0];
                Control panel2Control = this.splitContainer.Panel2.Controls[0];

                this.splitContainer.Panel2.Controls.Clear();
                this.splitContainer.Panel2.Controls.Add(panel1Control);

                this.splitContainer.Panel1.Controls.Clear();
                this.splitContainer.Panel1.Controls.Add(panel2Control);
            };

			this.displayPanelItem.Text = "Display Panel";
			this.displayPanelItem.DropDownItems.AddRange(new ToolStripItem[] {
				this.displayLeftPanelItem,
				this.displayRightPanelItem
			});

			this.displayLeftPanelItem.Text = "Left";
			this.displayLeftPanelItem.Checked = true;
			this.displayLeftPanelItem.CheckOnClick = true;
			this.displayLeftPanelItem.CheckStateChanged += delegate(object sender, EventArgs e) {
				if (!this.splitContainer.Panel2Collapsed) {
					this.splitContainer.Panel1Collapsed = !this.displayLeftPanelItem.Checked;
				} else {
					this.displayLeftPanelItem.Checked = true;
				}
			};

			this.displayRightPanelItem.Text = "Right";
			this.displayRightPanelItem.Checked = true;
			this.displayRightPanelItem.CheckOnClick = true;
			this.displayRightPanelItem.CheckStateChanged += delegate(object sender, EventArgs e) {
				if (!this.splitContainer.Panel1Collapsed) {
					this.splitContainer.Panel2Collapsed = !this.displayRightPanelItem.Checked;
				} else {
					this.displayRightPanelItem.Checked = true;
				}
			};
		}

		protected override void addControls() {
			this.splitContainer.Dock = DockStyle.Fill;
			this.splitContainer.Size = new Size(641, 364);
			this.splitContainer.SplitterDistance = 318;

			this.Controls.Add(this.splitContainer);
			base.addControls();
		}

		protected override void onFormClosing() {
			Settings.Instance.DualParent.Panel1Path = Forms.GetPropertyInChild<string>(this.splitContainer.Panel1, "Path");
            Settings.Instance.DualParent.Panel2Path = Forms.GetPropertyInChild<string>(this.splitContainer.Panel2, "Path");
            Settings.Instance.DualParent.SplitterPercentage = splitterPercentage;

			base.onFormClosing();
		}

		protected override void onFormLoad() {
            Forms.SetPropertyInChild<string>(this.splitContainer.Panel1, "Path", Settings.Instance.DualParent.Panel1Path);
            Forms.SetPropertyInChild<string>(this.splitContainer.Panel2, "Path", Settings.Instance.DualParent.Panel2Path);

            splitterPercentage = Settings.Instance.DualParent.SplitterPercentage;
			decimal percent = Convert.ToDecimal(splitterPercentage * 0.01);
			int splitterDistance = Convert.ToInt32(percent * this.Width);
			splitContainer.SplitterDistance = splitterDistance;

			base.onFormLoad();
		}
	}
}