namespace SharpFile.IO {
	public class RootDirectoryInfo : DirectoryInfo {
		public RootDirectoryInfo(System.IO.DirectoryInfo directoryInfo)
			: base(directoryInfo) {
			this.displayName = ".";
		}
	}
}