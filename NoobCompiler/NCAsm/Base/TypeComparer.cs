﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm {
    public class TypeComparer : IComparer<Type> {
        public int Compare(Type x,
                           Type y) {
            return x.AssemblyQualifiedName.CompareTo(y.AssemblyQualifiedName);
        }
    }
}
