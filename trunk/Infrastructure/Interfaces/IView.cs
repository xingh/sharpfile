using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SharpFile.Infrastructure {
	public interface IView {
		void AddItemRange(IList<IChildResource> resources);
        void AddItem(IResource resource);
		void RemoveItem(string path);
		void Clear();
		void BeginUpdate();
		void EndUpdate();
        void Invoke(Delegate method);
		string Path { get; }
		string Filter { get; }
        FileSystemWatcher FileSystemWatcher { get; }
		Control Control { get; }
        void ShowMessageBox(string text);
        IViewComparer Comparer { get; set; }
        IEnumerable<ColumnInfo> ColumnInfos { get; set; }
        string Name { get; set; }
        bool Enabled { get; set; }

        void OnUpdatePath(string path);
        void OnUpdateProgress(int progress);
        int OnGetImageIndex(IResource resource);
        void OnUpdateStatus(string status);
        void OnUpdatePreviewPanel(IResource resource);

		event View.GetImageIndexDelegate GetImageIndex;
		event View.UpdateProgressDelegate UpdateProgress;
		event View.UpdateStatusDelegate UpdateStatus;
		event View.UpdatePathDelegate UpdatePath;
        event View.UpdatePreviewPanelDelegate UpdatePreviewPanel;
	}

	public static class View {
        public delegate int GetImageIndexDelegate(IResource fsi);
		public delegate void UpdateProgressDelegate(int value);
		public delegate void UpdatePathDelegate(string path);
		public delegate void UpdateStatusDelegate(string status);
        public delegate void UpdatePreviewPanelDelegate(IResource resource);
	}
}