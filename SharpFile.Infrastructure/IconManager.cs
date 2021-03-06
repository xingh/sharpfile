using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Common.Logger;
using Common.UI;
using SharpFile.Infrastructure.Interfaces;
using SharpFile.Infrastructure.IO.ChildResources;
using SharpFile.Infrastructure.IO.ParentResources;

namespace SharpFile.Infrastructure {
	public static class IconManager {
        private static readonly object lockObject = new object();
        private static Dictionary<string, int> iconHash = new Dictionary<string, int>();
        private const string folderKey = ":FOLDER_KEY:";

        /// <summary>
        /// Get the image index for the file system object.
        /// </summary>
        /// <param name="fsi">File system object.</param>
        /// <param name="imageList">ImageList.</param>
        /// <returns>Image index.</returns>
        public static int GetImageIndex(IResource resource, bool useFileAttributes, ImageList imageList) {
            int imageIndex = -1;

            if (Settings.Instance.IconSettings.ShowIcons && resource != null && imageList != null) {
                try {
                    // Prevent more than one thread from updating the ImageList at a time.
                    lock (lockObject) {
                        const IconReader.Size iconSize = IconReader.Size.Small;
                        string fullPath = resource.FullName.ToLower();
                        bool showOverlay = false;
                        bool isLink = false;
                        bool isIntensiveSearch = false;

                        // Set the image index correctly.
                        imageIndex = iconHash.Count;

                        // When icons are to be retrieved by their file attributes (grabbing an icon by its
                        // file extension, then don't attempt to get overlays or search intensively.
                        if (!useFileAttributes) {
                            // Specifies whether overlays are turned on for all files, or if they have 
                            // been turned on specifically for some paths.
                            if (Settings.Instance.IconSettings.ShowOverlaysForAllPaths ||
                                Settings.Instance.IconSettings.ShowOverlayPaths.Find(delegate(string s) {
                                return (fullPath.Contains(s));
                            }) != null) {
                                showOverlay = true;
                            }

                            string driveTypeFullyQualifiedEnum = string.Empty;

                            try {
                                // HACK: Derive a fully qualified type from the full name of the assembly, plus the enum name.
                                driveTypeFullyQualifiedEnum = string.Format("{0}.{1}",
                                    resource.Root.DriveType.GetType().FullName,
                                    resource.Root.DriveType.ToString());

                                if (Settings.Instance.IconSettings.IntensiveSearchDriveTypeEnums.Find(delegate(FullyQualifiedEnum f) {
                                    return f.ToString().Equals(driveTypeFullyQualifiedEnum);
                                }) != null) {
                                    isIntensiveSearch = true;
                                }
                            } catch (ArgumentException ex) {
                                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                    "ArgumentException when determining the drive type's fully qualified enumeration.");
                            }
                        }

                        if (resource is FileInfo) {
                            string extension = ((FileInfo)resource).Extension;

                            // Lower-case the extension if it is available.
                            if (!string.IsNullOrEmpty(extension)) {
                                extension = extension.ToLower();
                            }

                            if (showOverlay ||
                                (string.IsNullOrEmpty(extension) ||
                                Settings.Instance.IconSettings.RetrieveIconExtensions.Contains(extension))) {
                                // Add the full name of the file if it is an executable into the ImageList.
                                if (!iconHash.ContainsKey(fullPath)) {
                                    Icon icon = IconReader.GetIcon(iconSize, fullPath, true, isIntensiveSearch, 
                                        showOverlay, isLink);
                                    imageList.Images.Add(fullPath, icon);
                                    iconHash.Add(fullPath, imageIndex);
                                }

                                imageIndex = iconHash[fullPath];
                            } else {
                                // Add the extension into the ImageList.
                                if (!iconHash.ContainsKey(extension)) {
                                    Icon icon = IconReader.GetIcon(iconSize, fullPath, true, isIntensiveSearch, false, false);
                                    imageList.Images.Add(extension, icon);
                                    iconHash.Add(extension, imageIndex);
                                }

                                imageIndex = iconHash[extension];
                            }
                        } else if (resource is DriveInfo || resource is ParentDirectoryInfo || resource is RootDirectoryInfo) {
                            if (!iconHash.ContainsKey(fullPath)) {
                                    Icon icon = IconReader.GetIcon(iconSize, fullPath, false, true, showOverlay, 
                                        isLink);
                                    imageList.Images.Add(fullPath, icon);
                                    iconHash.Add(fullPath, imageIndex);
                                }

                                imageIndex = iconHash[fullPath];
                        } else if (resource is DirectoryInfo) {
                            if (isIntensiveSearch) {
                                if (!iconHash.ContainsKey(fullPath)) {
                                    Icon icon = IconReader.GetIcon(iconSize, fullPath, false, true, showOverlay, isLink);
                                    imageList.Images.Add(fullPath, icon);
                                    iconHash.Add(fullPath, imageIndex);
                                }

                                imageIndex = iconHash[fullPath];
                            } else {
                                // 
                                if (!iconHash.ContainsKey(folderKey)) {
                                    Icon icon = IconReader.GetIcon(iconSize, fullPath, false, isIntensiveSearch,
                                        showOverlay, isLink);
                                    imageList.Images.Add(folderKey, icon);
                                    iconHash.Add(folderKey, imageIndex);
                                }

                                imageIndex = iconHash[folderKey];
                            }
                        } else {
                            throw new Exception("Unable to retrieve an icon for a " + resource.GetType());
                        }
                    }
                } catch (ArgumentException ex) {
                    imageIndex = -1;
                    string flags = (string)ex.Data["flags"];

                    Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                        "Icon image could not be retrieved for {0} with flags {1}.",
                        resource.FullName,
                        flags);
                }
            }

            return imageIndex;
        }

		/*
		public static int GetImageIndex(string text, Font font, ImageList imageList) {
			//lock (lockObject) {
				int imageIndex = imageList.Images.Count;

				// ImageList is buggy, need to ensure we do this:
				IntPtr ilsHandle = imageList.Handle;

				// Create the bitmap to hold the drag image:
				Bitmap bitmap = new Bitmap(imageList.ImageSize.Width, imageList.ImageSize.Height);

				// Get a graphics object from it:
				Graphics gfx = Graphics.FromImage(bitmap);

				// Default fill the bitmap with black:
				gfx.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);

				// Draw text in highlighted form:
				StringFormat fmt = new StringFormat(StringFormatFlags.LineLimit);
				fmt.Alignment = StringAlignment.Center;

				SizeF size = gfx.MeasureString(text, font, bitmap.Width, fmt);

				float left = 0F;
				if (size.Height > bitmap.Height) {
					size.Height = bitmap.Height;
				}
				if (size.Width < bitmap.Width) {
					left = (bitmap.Width - size.Width) / 2F;
				}

				RectangleF textRect = new RectangleF(
					left, 0F, size.Width, size.Height);

				gfx.FillRectangle(SystemBrushes.Highlight, textRect);
				gfx.DrawString(text, font, SystemBrushes.HighlightText,
					textRect, fmt);
				fmt.Dispose();

				// Add the image to the ImageList:
				//int index = GetBitmapIndex(bitmap);
				//ils.Images.Add(bitmap, Color.Black);

				imageList.Images.Add(bitmap, Color.Black);

				// Clear up the graphics object:
				gfx.Dispose();
				// Clear up the bitmap:
				bitmap.Dispose();

				return imageIndex;
			//}
		}
		*/
	}
}