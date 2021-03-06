using System;
using System.ComponentModel;
using System.IO;
using System.Timers;
using Common.Logger;

namespace SharpFile.Infrastructure {
	public class FileSystemWatcher {
		private System.IO.FileSystemWatcher fileSystemWatcher;
		private Timer timer;
		private ISynchronizeInvoke synchronizingObject;
        private bool isManualWatcher = false;

		private static object lockObject = new object();

        public delegate void ChangedDelegate(object sender, FileSystemEventArgs e);
        public event ChangedDelegate Changed;

		public FileSystemWatcher(ISynchronizeInvoke synchronizingObject, double interval) {
            timer = new Timer();
            fileSystemWatcher = new System.IO.FileSystemWatcher();
			this.synchronizingObject = synchronizingObject;

            if (isManualWatcher) {
                // Set up the timer.
                timer.SynchronizingObject = synchronizingObject;
                timer.Interval = interval;
                timer.Enabled = true;

                timer.Elapsed += delegate {
                    OnChanged(null, null);
                };
            } else {
                // Set up the watcher.
                fileSystemWatcher.SynchronizingObject = synchronizingObject;
                fileSystemWatcher.IncludeSubdirectories = false;
                fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName |
                    NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;

                fileSystemWatcher.Created += fileSystemWatcher_Event;
                fileSystemWatcher.Changed += fileSystemWatcher_Event;
                fileSystemWatcher.Deleted += fileSystemWatcher_Event;
                fileSystemWatcher.Renamed += fileSystemWatcher_Event;
            }
		}

		void fileSystemWatcher_Event(object sender, FileSystemEventArgs e) {
            OnChanged(sender, e);
		}

		public void OnChanged(object sender, FileSystemEventArgs e) {
			if (Changed != null) {
				Changed(sender, e);
			}
		}

        public bool EnableRaisingEvents {
            get {
                return fileSystemWatcher.EnableRaisingEvents;
            }
            set {
                try {
                    fileSystemWatcher.EnableRaisingEvents = value;
                } catch (ArgumentException) {
                    // Ignore an argument exception. It just means that the path is not valid to be watched.
                    // This can happen when using a custom ChildResourceRetriever, like the CompressedRetriever.
                } catch (Exception ex) {
                    Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "There was an error when enabling the file system watcher.");
                }
            }
        }

        public string Path {
            get {
                return fileSystemWatcher.Path;
            }
            set {
                try {
                    fileSystemWatcher.Path = value;
                } catch (ArgumentException) {
                    // Ignore an argument exception. It just means that the path is not valid to be watched.
                    // This can happen when using a custom ChildResourceRetriever, like the CompressedFileRetriever.
                } catch (Exception ex) {
                    Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                "There was an error when setting the file system watcher path to {0}.",
                                value);
                }
            }
        }

		public string Filter {
			get {
				return fileSystemWatcher.Filter;
			}
			set {
				fileSystemWatcher.Filter = value;
			}
		}

		public double Interval {
			get {
				return timer.Interval;
			}
			set {
				timer.Interval = value;
			}
		}
	}
}