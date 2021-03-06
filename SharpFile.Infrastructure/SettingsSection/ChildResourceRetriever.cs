﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Common;
using Common.Logger;

namespace SharpFile.Infrastructure.SettingsSection {
    public class ChildResourceRetriever {
        private string name;
        private FullyQualifiedType fullyQualifiedType;
        private List<ColumnInfo> columnInfos;
        private Delegate filterMethod;
        private FullyQualifiedMethod fullyQualifiedMethod;
        private string view;
        private List<string> filterMethodArguments;

        /// <summary>
        /// Empty ctor for xml serialization.
        /// </summary>
        public ChildResourceRetriever() {
        }

        public ChildResourceRetriever(string name, FullyQualifiedType fullyQualifiedType, FullyQualifiedMethod fullyQualifiedMethod,
            string view, List<ColumnInfo> columnInfos) {
            this.name = name;
            this.fullyQualifiedType = fullyQualifiedType;
            this.fullyQualifiedMethod = fullyQualifiedMethod;
            this.view = view;
            this.columnInfos = columnInfos;
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

        public FullyQualifiedType FullyQualifiedType {
            get {
                return fullyQualifiedType;
            }
            set {
                fullyQualifiedType = value;
            }
        }

        public FullyQualifiedMethod FullyQualifiedMethod {
            get {
                return fullyQualifiedMethod;
            }
            set {
                fullyQualifiedMethod = value;
            }
        }

        public string View {
            get {
                return view;
            }
            set {
                view = value;
            }
        }

        [XmlArray("Columns")]
        [XmlArrayItem("Column")]
        public List<ColumnInfo> ColumnInfos {
            get {
                return columnInfos;
            }
            set {
                columnInfos = value;
            }
        }

        [XmlIgnore]
        public List<string> FilterMethodArguments {
            get {
                return filterMethodArguments;
            }
            set {
                filterMethodArguments = value;
            }
        }

        [XmlIgnore]
        public Delegate FilterMethod {
            get {
                if (filterMethod == null && fullyQualifiedMethod != null) {
                    try {
                        // Create the appropriate method delegate based on whether there were arguments passed in.
                        if (fullyQualifiedMethod.Arguments != null && fullyQualifiedMethod.Arguments.Count > 0) {
                            filterMethod = Reflection.CreateDelegate<Interfaces.ChildResourceRetriever.FilterMethodWithArgumentsDelegate>(
                               fullyQualifiedMethod.FullyQualifiedType.Assembly,
                               fullyQualifiedMethod.FullyQualifiedType.Type,
                               fullyQualifiedMethod.Name);

                            filterMethodArguments = fullyQualifiedMethod.Arguments;
                        } else {
                            filterMethod = Reflection.CreateDelegate<Interfaces.ChildResourceRetriever.FilterMethodDelegate>(
                                fullyQualifiedMethod.FullyQualifiedType.Assembly,
                                fullyQualifiedMethod.FullyQualifiedType.Type,
                                fullyQualifiedMethod.Name);
                        }
                    } catch (Exception ex) {
                        string message = "Creating the FilterMethod, {0}, for the {1} ChildResourceRetriever failed.";

                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, message,
                                fullyQualifiedMethod.FullyQualifiedType.Type, name);
                    }
                }

                // If the filterMethod is still null, then there was an error creating the delegate, 
                // or the method delegate type was null. Either way, set a default.
                if (filterMethod == null) {
                    filterMethod = Reflection.CreateDelegate<Interfaces.ChildResourceRetriever.FilterMethodDelegate>(
                                "SharpFile.Infrastructure",
                                "SharpFile.Infrastructure.ChildResourceRetrievers",
                                "FalseFilterMethod");
                }

                return filterMethod;
            }
        }
    }
}