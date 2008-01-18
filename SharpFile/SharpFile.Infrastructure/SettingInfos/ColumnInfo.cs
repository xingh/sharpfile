﻿using System.Collections;
using System.Xml.Serialization;
using System;
using System.Reflection;
using Common;
using Common.Logger;

namespace SharpFile.Infrastructure {
    [Serializable]
    public class ColumnInfo {
        private string text;
        private string property;
        private IComparer comparer;
        private FullyQualifiedType comparerType;
        private bool primaryColumn;
        private CustomMethod customMethod;
        private FullyQualifiedMethod methodDelegateType;

        public delegate string CustomMethod(string val);

        public ColumnInfo() {
        }

        public ColumnInfo(string text, string property, IComparer comparer)
            : this(text, property, null, comparer, false) {
        }

        public ColumnInfo(string text, string property, IComparer comparer, bool primaryColumn)
            : this(text, property, null, comparer, primaryColumn) {
        }

        public ColumnInfo(string text, string property, CustomMethod customMethod, IComparer comparer)
            : this(text, property, customMethod, comparer, false) {
        }

        public ColumnInfo(string text, string property, CustomMethod customMethod, IComparer comparer, bool primaryColumn) {
            this.text = text;
            this.property = property;
            this.comparer = comparer;
            this.primaryColumn = primaryColumn;
            this.customMethod = customMethod;

            this.comparerType = new FullyQualifiedType(
                comparer.GetType().Namespace,
                comparer.GetType().FullName);

            if (customMethod != null) {
                this.methodDelegateType = new FullyQualifiedMethod(new FullyQualifiedType(
                    customMethod.Method.DeclaringType.Namespace,
                    customMethod.Method.DeclaringType.FullName),
                    customMethod.Method.Name);
            }
        }

        [XmlAttribute("Text")]
        public string Text {
            get {
                return text;
            }
            set {
                text = value;
            }
        }

        [XmlAttribute("Property")]
        public string Property {
            get {
                return property;
            }
            set {
                property = value;
            }
        }

        public FullyQualifiedType ComparerType {
            get {
                return comparerType;
            }
            set {
                comparerType = value;
            }
        }

        [XmlIgnore]
        public IComparer Comparer {
            get {
                if (comparer == null) {
                    if (comparerType != null) {
                        try {
                            object comparerObject = Reflection.InstantiateObject(comparerType.Assembly,
                                comparerType.Type);

                            if (comparerObject is IComparer) {
                                comparer = (IComparer)comparerObject;
                            } else {
                                Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly,
                                    "Comparer, {0}, does not inherit from IComparer.",
                                    comparerType.Type);
                            }
                        } catch (TypeLoadException ex) {
                            Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex,
                                    "Comparer, {0}, can not be instantiated.",
                                    comparerType.Type);
                        }
                    }
                }

                return comparer;
            }
        }

        [XmlAttribute("PrimaryColumn")]
        public bool PrimaryColumn {
            get {
                return primaryColumn;
            }
            set {
                primaryColumn = value;
            }
        }

        public FullyQualifiedMethod MethodDelegateType {
            get {
                return methodDelegateType;
            }
            set {
                methodDelegateType = value;
            }
        }

        [XmlIgnore]
        public CustomMethod MethodDelegate {
            get {
                if (customMethod == null && methodDelegateType != null) {
                    try {
                        customMethod = Common.Reflection.CreateDelegate<CustomMethod>(
                            methodDelegateType.FullyQualifiedType.Assembly,
                            methodDelegateType.FullyQualifiedType.Type,
                            methodDelegateType.Method);
                    } catch (Exception ex) {
                        string message = "Creating the CustomMethod, {0}, for the {1} ColumnInfo failed.";

                        Settings.Instance.Logger.Log(LogLevelType.ErrorsOnly, ex, message,
                                methodDelegateType.FullyQualifiedType.Type, text);

                    }
                }

                return customMethod;
            }
        }
    }
}