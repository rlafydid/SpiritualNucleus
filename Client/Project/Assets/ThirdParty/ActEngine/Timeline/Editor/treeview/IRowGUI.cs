using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    interface IRowGUI
    {
        TrackAsset asset { get; }
        Rect boundingRect { get; }
        bool locked { get; }
        bool showMarkers { get; }
        bool muted { get; }

        Rect ToWindowSpace(Rect treeViewRect);
    }
}
