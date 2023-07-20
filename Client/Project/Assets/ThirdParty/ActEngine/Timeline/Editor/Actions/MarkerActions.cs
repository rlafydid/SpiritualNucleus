using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.ActTimeline.Actions;
using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    [UsedImplicitly]
    class CopyMarkersToClipboard : MarkerAction
    {
        public override ActionValidity Validate(IEnumerable<IMarker> markers) => ActionValidity.Valid;

        public override bool Execute(IEnumerable<IMarker> markers)
        {
            TimelineEditor.clipboard.CopyItems(markers.ToItems());
            return true;
        }
    }
}
