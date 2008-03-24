﻿using System.Collections.Generic;

namespace SharpFile.Infrastructure {
    public interface IResourceGetter {
        IEnumerable<IChildResource> GetFiles();
        IEnumerable<IChildResource> GetFiles(string filter);

        IEnumerable<IChildResource> GetDirectories();
    }
}