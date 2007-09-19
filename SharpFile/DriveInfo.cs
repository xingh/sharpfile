using System;

namespace SharpFile
{
    public class DriveInfo
    {
        private string description;
        private string providerName;
        private long freeSpace;
        private long size;
        private string name;
        private int driveType;

        public DriveInfo(string name, string providerName, int driveType, string description,
            long size, long freeSpace)
        {
            this.name = name;
            this.providerName = providerName;
            this.driveType = driveType;
            this.description = description;
            this.size = size;
            this.freeSpace = freeSpace;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public string ProviderName
        {
            get
            {
                return providerName;
            }
        }

        // TODO: This should be an enum.
        public int DriveType
        {
            get
            {
                return driveType;
            }
        }

        public long FreeSpace
        {
            get
            {
                return freeSpace;
            }
        }

        public long Size
        {
            get
            {
                return size;
            }
        }
    }
}