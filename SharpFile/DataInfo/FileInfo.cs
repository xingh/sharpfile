using System;

namespace SharpFile
{
    public class FileInfo : DataInfo
    {
		private string extension;

        public FileInfo(System.IO.FileInfo fileInfo)
        {
            this.name = fileInfo.Name;
            this.size = fileInfo.Length;
            this.lastWriteTime = fileInfo.LastWriteTime;
			this.fullPath = fileInfo.FullName;
			this.extension = fileInfo.Extension;
        }

		public string Extension {
			get {
				return extension;
			}
		}
    }
}
