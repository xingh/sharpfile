﻿using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure.SettingsSection {
    public sealed class Retrievers {
        private List<View> views = new List<View>();
        private List<ParentResourceRetriever> parentResourceRetrievers = new List<ParentResourceRetriever>();
        private List<ChildResourceRetriever> childResourceRetrievers = new List<ChildResourceRetriever>();

        [XmlArray("ParentResourceRetrievers")]
        [XmlArrayItem("ParentResourceRetriever")]
        public List<ParentResourceRetriever> ParentResourceRetrievers {
            get {
                return parentResourceRetrievers;
            }
            set {
                parentResourceRetrievers = value;
            }
        }

        [XmlArray("ChildResourceRetrievers")]
        [XmlArrayItem("ChildResourceRetriever")]
        public List<ChildResourceRetriever> ChildResourceRetrievers {
            get {
                return childResourceRetrievers;
            }
            set {
                childResourceRetrievers = value;
            }
        }

        [XmlArray("Views")]
        [XmlArrayItem("View")]
        public List<View> Views {
            get {
                return views;
            }
            set {
                views = value;
            }
        }

        public static List<View> GenerateDefaultViews() {
            List<View> views = new List<View>();
            FullyQualifiedType viewType = new FullyQualifiedType("SharpFile", "SharpFile.UI.ListView");
            FullyQualifiedType comparerType = new FullyQualifiedType("SharpFile", "SharpFile.UI.ListViewItemComparer");
            views.Add(new SettingsSection.View("ListView", viewType, comparerType));
            return views;
        }

        public static List<ParentResourceRetriever> GenerateDefaultParentResourceRetrievers() {
            List<ParentResourceRetriever> parentResourceRetrievers = new List<ParentResourceRetriever>();
            FullyQualifiedType type = new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.Retrievers.DriveRetriever");
            ParentResourceRetriever retriever = new ParentResourceRetriever(
                "DriveRetriever", type, "CompressedRetriever", "DefaultRetriever");
            parentResourceRetrievers.Add(retriever);
            return parentResourceRetrievers;
        }

        public static List<ChildResourceRetriever> GenerateDefaultChildResourceRetrievers() {
            List<ChildResourceRetriever> childResourceRetrieverSettings = new List<ChildResourceRetriever>();

            // Default retriever.
            FullyQualifiedType type = new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.Retrievers.DefaultRetriever");
            FullyQualifiedMethod method = new FullyQualifiedMethod("TrueFilterMethod",
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.ChildResourceRetrievers"));

            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("Filename", "DisplayName", SortOrder.Ascending, true));
            columnInfos.Add(new ColumnInfo("Size", "Size", SortOrder.None, false,
                new FullyQualifiedMethod("GetHumanReadableSize",
                    new FullyQualifiedType("Common", "Common.General")),
                    new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.DirectoryInfo"),
                    new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ParentResources.DriveInfo")));
            columnInfos.Add(new ColumnInfo("Date", "LastWriteTime", SortOrder.None, false,
                new FullyQualifiedMethod("GetDateTimeShortDateString",
                    new FullyQualifiedType("Common", "Common.General")),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.ParentDirectoryInfo"),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.RootDirectoryInfo")));
            columnInfos.Add(new ColumnInfo("Time", "LastWriteTime", SortOrder.None, false,
                new FullyQualifiedMethod("GetDateTimeShortTimeString",
                    new FullyQualifiedType("Common", "Common.General")),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.ParentDirectoryInfo"),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.RootDirectoryInfo")));

            SettingsSection.ChildResourceRetriever retriever = new SettingsSection.ChildResourceRetriever(
                "DefaultRetriever", type, method, "ListView", columnInfos);
            childResourceRetrieverSettings.Add(retriever);

            // Compressed retriever.
            type = new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.Retrievers.CompressedRetrievers.ReadOnlyCompressedRetriever");
            method = new FullyQualifiedMethod("IsFileWithExtension",
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.ChildResourceRetrievers"),
                ".zip");

            columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("Filename", "DisplayName", SortOrder.Ascending, true));
            columnInfos.Add(new ColumnInfo("Size", "Size", SortOrder.None, false,
                new FullyQualifiedMethod("GetHumanReadableSize",
                    new FullyQualifiedType("Common", "Common.General")),
                    new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.DirectoryInfo"),
                    new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ParentResources.DriveInfo")));
            columnInfos.Add(new ColumnInfo("Compressed Size", "CompressedSize", SortOrder.None, false,
                new FullyQualifiedMethod("GetHumanReadableSize",
                    new FullyQualifiedType("Common", "Common.General")),
                    new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.DirectoryInfo"),
                    new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.DriveInfo")));
            columnInfos.Add(new ColumnInfo("Date", "LastWriteTime", SortOrder.None, false,
                new FullyQualifiedMethod("GetDateTimeShortDateString",
                    new FullyQualifiedType("Common", "Common.General")),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.ParentDirectoryInfo"),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.RootDirectoryInfo")));
            columnInfos.Add(new ColumnInfo("Time", "LastWriteTime", SortOrder.None, false,
                new FullyQualifiedMethod("GetDateTimeShortTimeString",
                    new FullyQualifiedType("Common", "Common.General")),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.ParentDirectoryInfo"),
                new FullyQualifiedType("SharpFile.Infrastructure", "SharpFile.Infrastructure.IO.ChildResources.RootDirectoryInfo")));

            retriever = new SettingsSection.ChildResourceRetriever(
                "CompressedRetriever", type, method, "ListView", columnInfos);
            childResourceRetrieverSettings.Add(retriever);

            return childResourceRetrieverSettings;
        }
    }
}