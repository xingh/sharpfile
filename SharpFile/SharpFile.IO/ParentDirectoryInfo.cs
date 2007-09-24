namespace SharpFile.IO {
	public class ParentDirectoryInfo : DirectoryInfo {
		public ParentDirectoryInfo(System.IO.DirectoryInfo directoryInfo)
			: base(directoryInfo) {
			this.displayName = "..";
		}
	}
}