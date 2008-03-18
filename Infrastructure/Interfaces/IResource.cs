﻿namespace SharpFile.Infrastructure {
    public interface IResource {
        string DisplayName { get; }
        string FullName { get; }
        string Name { get; }
        long Size { get; }

        void Execute(IView view);
        ChildResourceRetrievers GetChildResourceRetrievers();
        ChildResourceRetrievers GetChildResourceRetrievers(bool filterByFsi);
    }
}