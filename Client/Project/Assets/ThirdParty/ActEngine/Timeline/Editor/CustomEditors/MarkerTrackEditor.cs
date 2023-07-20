using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    [CustomTimelineEditor(typeof(MarkerTrack))]
    class MarkerTrackEditor : TrackEditor
    {
        public static readonly float DefaultMarkerTrackHeight = 24;

        public override TrackDrawOptions GetTrackOptions(TrackAsset track, Object binding)
        {
            var options = base.GetTrackOptions(track, binding);
            options.minimumHeight = DefaultMarkerTrackHeight;
            return options;
        }
    }
}
