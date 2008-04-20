﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpFile.Infrastructure {
    public class ParentResourceRetrieverInfo {
        private FullyQualifiedType fullyQualifiedType;
        private string name;
        private List<string> childResourceRetrievers;

        public ParentResourceRetrieverInfo() {
        }

        [XmlAttribute("Name")]
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        [XmlArray("ChildResourceRetrievers")]
        [XmlArrayItem("ChildResourceRetriever")]
        public List<string> ChildResourceRetrievers {
            get {
                return childResourceRetrievers;
            }
            set {
                childResourceRetrievers = value;
            }
        }

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
        }
    }
}