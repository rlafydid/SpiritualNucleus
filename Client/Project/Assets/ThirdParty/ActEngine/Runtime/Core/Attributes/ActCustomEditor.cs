using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActCustomEditorAttribute : Attribute
    {
        public Type Type;
        public ActCustomEditorAttribute(Type type)
        {
            Type = type;
        }
    }
}

