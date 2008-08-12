using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;

namespace Common {
    /// <summary>
    /// Summary description for General.
    /// </summary>
    public static class General {
        /// <summary>
        /// Converts a string in RGB format to a .NET Color object.
        /// </summary>
        /// <param name="rgb">String in RGB format (i.e. 0,0,0).</param>
        /// <returns>A Color object that corresponds to the RGB value.</returns>
        public static Color ConvertStringToColor(string rgb) {
            try {
                string[] colors = rgb.Split(',');

                int r = int.Parse(colors[0].Trim());
                int g = int.Parse(colors[1].Trim());
                int b = int.Parse(colors[2].Trim());

                return Color.FromArgb(r, g, b);
            } catch {
                throw new Exception(rgb + " is not a valid color.");
            }
        }

        /// <summary>
        /// Returns the value with the the correct unit attached to it.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <example>Pass in "1025", return "1.1 KB"</example>
        /// <returns></returns>
        public static string GetHumanReadableSize(string value) {
            double size;
            string result = value;

            if (double.TryParse(value, out size)) {
                result = getHumanReadableSize(size);
            } else {
                // If the string has digits, then grab just what 
                // matches the regex and convert that to human readable form.
                Regex regex = new Regex("[0-9]+");
                int previousMatchDifference = 0;

                foreach (Match match in regex.Matches(value)) {
                    value = match.Value;

                    if (double.TryParse(value, out size)) {
                        int originalValueLength = value.Length;
                        value = getHumanReadableSize(size);
                        previousMatchDifference += (originalValueLength - value.Length);
                        int startAt = (match.Index - previousMatchDifference);

                        if (startAt > 0 && startAt < result.Length) {
                            result = regex.Replace(result, value, 1,
                                startAt);
                        }
                    }
                }
            }

            return result;
        }

        private static string getHumanReadableSize(double size) {
            string unit;
            string result;

            // Assume that size is in bytes
            if (size >= 1073741824) {
                size = size / 1024 / 1024 / 1024;
                unit = "GB";
            } else if (size >= 1048576) {
                size = size / 1024 / 1024;
                unit = "MB";
            } else if (size >= 1024) {
                size = size / 1024;
                unit = "KB";
            } else {
                unit = "B";
            }

            if (size == 0) {
                result = string.Format("{0} {1}",
                    size,
                    unit);
            } else {
                result = string.Format("{0:0,0.0}",
                        size);

                result = result.Trim('0');
                result = result.TrimEnd('.');

                result = string.Format("{0} {1}",
                    result,
                    unit);
            }

            return result;
        }

        /// <summary>
        /// Get the inner exception for the current exception.
        /// </summary>
        /// <param name="ex">Exception to get the inner exception from.</param>
        /// <returns>The lowest-level exception.</returns>
        public static Exception GetInnerException(Exception ex) {
            if (ex.InnerException == null) {
                return ex;
            }

            return GetInnerException(ex.InnerException);
        }

        /// <summary>
        /// Get the extension of the path passed in.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <returns>Extension.</returns>
        public static string GetExtension(string path) {
            int extensionIndex = path.LastIndexOf('.');
            string extension = string.Empty;

            if (extensionIndex > 0) {
                extension = path.Substring(extensionIndex, path.Length - extensionIndex);
            }

            return extension;
        }

        /// <summary>
        /// Gets the DateTime short date string from a string DateTime.
        /// </summary>
        /// <param name="dateTime">String datetime.</param>
        /// <returns>String equivalent of the DateTime short date.</returns>
        public static string GetDateTimeShortDateString(string dateTime) {
            string val = dateTime;
            DateTime parsedDateTime;

            if (DateTime.TryParse(dateTime, out parsedDateTime)) {
                val = parsedDateTime.ToShortDateString();
            }

            return val;
        }

        /// <summary>
        /// Gets the date time short time string from a string DateTime.
        /// </summary>
        /// <param name="dateTime">String datetime.</param>
        /// <returns>String equivalent of the DateTime short time.</returns>
        public static string GetDateTimeShortTimeString(string dateTime) {
            string val = dateTime;
            DateTime parsedDateTime;

            if (DateTime.TryParse(dateTime, out parsedDateTime)) {
                val = parsedDateTime.ToShortTimeString();
            }

            return val;
        }

        public static string CleanupPath(string path) {
            string directorySeperator = Path.DirectorySeparatorChar.ToString();

            if (path.Contains(":") &&
                !path.Contains(directorySeperator)) {
                path += directorySeperator;
            } else if (!path.Contains(@":\")) {
                path += @":\";
            } else if (path.EndsWith("/")) {
                path = path.Replace("/", directorySeperator);
            }

            return path;
        }
    }
}