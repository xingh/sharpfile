using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Common.Win32;

namespace Common.UI {
    

	/// <summary>
	/// Provides static methods to read system icons for both folders and files.
	/// </summary>
	/// <example>
	/// <code>IconReader.GetFileIcon("c:\\general.xls");</code>
	/// </example>
    public class IconReader {
        /// <summary>
        /// Options to specify the size of icons to return.
        /// </summary>
        public enum Size {
            /// <summary>
            /// Specify large icon - 32 pixels by 32 pixels.
            /// </summary>
            Large = 0,
            /// <summary>
            /// Specify small icon - 16 pixels by 16 pixels.
            /// </summary>
            Small = 1
        }

        /// <summary>
        /// Options to specify whether folders should be in the open or closed state.
        /// </summary>
        public enum FolderType {
            /// <summary>
            /// Specify open folder.
            /// </summary>
            Open = 0,
            /// <summary>
            /// Specify closed folder.
            /// </summary>
            Closed = 1
        }

        /// <summary>
        /// Returns an icon for a given file - indicated by the name parameter.
        /// </summary>
        /// <param name="path">Pathname for file.</param>
        /// <param name="size">Large or small</param>
        /// <param name="showOverlay">Whether to include the link icon</param>
        /// <returns>System.Drawing.Icon</returns>
        public static Icon GetIcon(Size size, string fullName, bool isFile, bool isIntensiveSearch, bool showOverlay, bool isLink) {
            uint dwAttrs = 0;
            uint flags = 0;

            if (isLink) {
                flags += (uint)SHGFI.LINKOVERLAY;
            }

            // Show overlay for files.
            if (showOverlay) {
                flags += (uint)SHGFI.ADDOVERLAYS;
            }

            if (!showOverlay && !isIntensiveSearch) {
                flags += (uint)SHGFI.USEFILEATTRIBUTES;
            }

            if (isFile) {
                dwAttrs += (uint)FILE_ATTRIBUTE.NORMAL;                
            } else {
                dwAttrs += (uint)FILE_ATTRIBUTE.DIRECTORY;
            }

            // Get the folder icon.
            return getIcon(fullName, size, dwAttrs, flags);
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <param name="path">Path to retrieve the icon.</param>
        /// <param name="dwAttrs">Attributes of the icon.</param>
        /// <param name="size">Size of the icon.</param>
        /// <param name="flags">Flags used when retrieving the icon.</param>
        /// <returns>Icon.</returns>
        private static Icon getIcon(string path, Size iconSize, uint dwAttrs, uint flags) {
            flags += (uint)SHGFI.SYSICONINDEX | (uint)SHGFI.ICON;

            /* Check the size specified for return. */
            if (Size.Small == iconSize) {
                flags += (uint)SHGFI.SMALLICON;
            } else {
                flags += (uint)SHGFI.LARGEICON;
            }

            Icon icon = null;
            SHFILEINFO shfi = new SHFILEINFO();

            try {
                Shell32.SHGetFileInfo(path,
                    dwAttrs,
                    ref shfi,
                    (uint)Marshal.SizeOf(shfi),
                    flags);

                // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly.
                icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
            } catch (ArgumentException ex) {
                string flagInfo = Enum.Format(typeof(SHGFI), flags, "F");

                ex.Data["flags"] = flagInfo;
                throw;
            } finally {
                // Cleanup.
                User32.DestroyIcon(shfi.hIcon);
            }

            return icon;
        }
    }
}