﻿using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using SharpFile.Infrastructure.Interfaces;
using SharpFile.Infrastructure.IO.ChildResources;

namespace SharpFile.Infrastructure.IO.Retrievers.CompressedRetrievers {
    public class ReadOnlyCompressedRetriever : ChildResourceRetriever {
        /// <summary>
        /// Override the default Execute for ChildResourceRetrievers for files. Use the default for everything else.
        /// </summary>
        /// <param name="view">View to output the results to.</param>
        /// <param name="resource">Resource to grab output from.</param>
        public override void Execute(IView view, IResource resource) {
            if (resource is CompressedDirectoryInfo || resource is CompressedFileInfo) {
                return;
            } else {
                useFileAttributes = true;
                base.Execute(view, resource);
            }
        }

        protected override IList<IResource> getResources(IResource resource, string filter) {
            List<IResource> childResources = new List<IResource>();

            if (Settings.Instance.ShowRootDirectory) {
                childResources.Add(new RootDirectoryInfo(resource.Root.Name));
            }

            if (Settings.Instance.ShowParentDirectory) {
                if (!Settings.Instance.ShowRootDirectory ||
                    (Settings.Instance.ShowRootDirectory &&
                    !resource.Path.Equals(resource.Root.Name))) {
                    childResources.Add(new ParentDirectoryInfo(resource.Path));
                }
            }

            using (ZipFile zipFile = new ZipFile(resource.FullName)) {
                foreach (ZipEntry zipEntry in zipFile) {
                    string zipEntryName = zipEntry.Name.Replace("/", Common.Path.DirectorySeparator);

                    if (zipEntryName.EndsWith(Common.Path.DirectorySeparator)) {
                        zipEntryName = zipEntryName.Remove(zipEntryName.Length - 1, 1);
                    }

                    if (string.IsNullOrEmpty(filter) || filter.Equals("*.*") || zipEntryName.Contains(filter)) {
                        // Only show the current directories filesystem objects.
                        string fullName = string.Format(@"{0}{1}{2}",
                                resource.FullName,
                                Common.Path.DirectorySeparator,
                                zipEntryName);

                        if (zipEntry.IsFile) {
                            childResources.Add(new CompressedFileInfo(fullName, zipEntryName, zipEntry.Size,
                                zipEntry.CompressedSize, zipEntry.DateTime));
                        }
                        //} else if (zipEntry.IsDirectory) {
                        //    childResources.Add(new CompressedDirectoryInfo(fullName, zipEntryName,
                        //            zipEntry.DateTime));
                        //}
                    }
                }
            }

            return childResources;
        }
    }
}