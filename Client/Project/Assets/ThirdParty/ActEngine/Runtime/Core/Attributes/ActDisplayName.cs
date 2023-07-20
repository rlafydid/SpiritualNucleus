using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class ActDisplayNameAttribute : Attribute
    {
        public string DisplayName;
        public string MenuName;
        public int Order;
        public bool ShowNameOnly = false;

        public ActDisplayNameAttribute() { }

        public ActDisplayNameAttribute(string displayName, string menuName = null, int order = 0, bool showNameOnly = false)
        {
            this.DisplayName = displayName;
            this.MenuName = menuName != null ? menuName : displayName;
            this.Order = order;
            this.ShowNameOnly = showNameOnly;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class DependentObjectAttribute : Attribute
    {
        public Type DependentObjectType;
        public DependentObjectAttribute(Type dependentObjectType)
        {
            DependentObjectType = dependentObjectType;
        }
    }
}
