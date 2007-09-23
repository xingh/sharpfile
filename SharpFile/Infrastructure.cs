using System.Collections.Generic;
using System.IO;
using System.Management;
using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using Etier.IconHelper;
using System.Drawing;

namespace SharpFile {
	public static class Infrastructure {
		private const string allFilesPattern = "*.*";
		private const string openFolderKey = ":OPEN_FOLDER:";
		private const string closedFolderKey = ":CLOSED_FOLDER:";

		private static object lockObject = new object();

		public static IEnumerable<FileSystem.FileSystemInfo> GetFiles(System.IO.DirectoryInfo directoryInfo) {
			return GetFiles(directoryInfo, allFilesPattern);
		}

		public static IEnumerable<FileSystem.FileSystemInfo> GetFiles(System.IO.DirectoryInfo directoryInfo, string pattern) {
			List<FileSystem.FileSystemInfo> dataInfos = new List<FileSystem.FileSystemInfo>();

			if (directoryInfo.Root.Equals(directoryInfo.FullName)) {
				dataInfos.Add(new RootInfo(directoryInfo.Root));
			}

			if (directoryInfo.Parent != null) {
				dataInfos.Add(new ParentDirectoryInfo(directoryInfo.Parent));
			}

			System.IO.DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
			dataInfos.AddRange(
				Array.ConvertAll<System.IO.DirectoryInfo, FileSystem.FileSystemInfo>(directoryInfos, delegate(System.IO.DirectoryInfo di) {
				return new DirectoryInfo(di);
			})
			);

			System.IO.FileInfo[] fileInfos = getFilteredFiles(directoryInfo, pattern);
			dataInfos.AddRange(
				Array.ConvertAll<System.IO.FileInfo, FileSystem.FileSystemInfo>(fileInfos, delegate(System.IO.FileInfo fi) {
				return new FileInfo(fi);
			})
			);

			return dataInfos;
		}

		private static System.IO.FileInfo[] getFilteredFiles(System.IO.DirectoryInfo directoryInfo, string pattern) {
			if (pattern.Equals(allFilesPattern)) {
				return directoryInfo.GetFiles();
			} else {
				return directoryInfo.GetFiles(pattern, SearchOption.TopDirectoryOnly);
			}
		}

		/// <summary>
		/// Get files based on a regex. Not currently used.
		/// </summary>
		private static System.IO.FileInfo[] getFilteredFiles(System.IO.DirectoryInfo directoryInfo, Regex pattern) {
			return Array.FindAll<System.IO.FileInfo>(directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly),
				delegate(System.IO.FileInfo fi) {
					return pattern.IsMatch(fi.Name);
				});
		}

		public static IEnumerable<DriveInfo> GetDrives() {
			foreach (ManagementObject managementObject in GetManagementObjects("Win32_LogicalDisk")) {
				int driveType_i = managementObject["DriveType"] != null ?
					int.Parse(managementObject["DriveType"].ToString()) : 0;

				if (driveType_i >= 2 &&
					driveType_i <= 6) {
					string name = managementObject["Name"].ToString();
					DriveType driveType = (DriveType)Enum.Parse(typeof(DriveType), driveType_i.ToString());
					string description = managementObject["Description"] != null ?
						managementObject["Description"].ToString() : string.Empty;
					string providerName = managementObject["ProviderName"] != null ?
						managementObject["ProviderName"].ToString() : "";
					long freeSpace = managementObject["FreeSpace"] != null ?
						long.Parse(managementObject["FreeSpace"].ToString()) : 0;
					long size = managementObject["Size"] != null ?
						long.Parse(managementObject["Size"].ToString()) : 0;

					DriveInfo driveInfo = new DriveInfo(name, providerName, driveType, description, size, freeSpace);
					yield return driveInfo;
				}
			}
		}

		public static ManagementObjectCollection GetManagementObjects(string path) {
			ManagementClass managementClass = new ManagementClass(path);
			ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
			return managementObjectCollection;
		}

		public static int GetImageIndex(FileSystem.FileSystemInfo dataInfo, ImageList imageList) {
			lock (lockObject) {
				int imageIndex = imageList.Images.Count;
				string fullPath = dataInfo.FullPath;

				if (dataInfo is FileInfo) {
					string extension = ((FileInfo)dataInfo).Extension;

					// TODO: Specify the extensions to grab the images from in a config file.
					if (extension.Equals(".exe") ||
						extension.Equals(".lnk") ||
						extension.Equals(".dll") ||
						extension.Equals(".ps") ||
						extension.Equals(".scr") ||
						extension.Equals(".ico") ||
						extension.Equals(".icn") ||
						extension.Equals(string.Empty)) {
						// Add the full name of the file if it is an executable into the the ImageList.
						if (!imageList.Images.ContainsKey(fullPath)) {
							Icon icon = IconReader.GetFileIcon(fullPath, IconReader.IconSize.Small, false);
							imageList.Images.Add(fullPath, icon);
						}

						imageIndex = imageList.Images.IndexOfKey(fullPath);
					} else {
						// Add the extension into the ImageList.
						if (!imageList.Images.ContainsKey(extension)) {
							Icon icon = IconReader.GetFileIcon(fullPath, IconReader.IconSize.Small, false);
							imageList.Images.Add(extension, icon);
						}

						imageIndex = imageList.Images.IndexOfKey(extension);
					}
				} else if (dataInfo is DirectoryInfo) {
					// Add the directory information into the ImageList.
					if (!imageList.Images.ContainsKey(closedFolderKey)) {
						Icon icon = IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Closed);
						imageList.Images.Add(closedFolderKey, icon);
					}

					imageIndex = imageList.Images.IndexOfKey(closedFolderKey);
				} else {
					throw new ArgumentException("The object, " + dataInfo.GetType() + ", is not supported.");
				}

				return imageIndex;
			}
		}
	}
}