using System;

namespace UnityEditor.ActTimeline
{
    interface ISelectable : ILayerable
    {
        void Select();
        bool IsSelected();
        void Deselect();
        bool CanSelect(UnityEngine.Event evt);
    }
}
