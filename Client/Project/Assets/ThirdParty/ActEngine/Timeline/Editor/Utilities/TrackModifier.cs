using UnityEngine;
using UnityEditor;
using UnityEngine.ActTimeline;
using UnityEngine.Playables;

namespace UnityEditor.ActTimeline
{
    static class TrackModifier
    {
        public static bool DeleteTrack(TimelineAsset timeline, TrackAsset track)
        {
            if (TimelineEditor.inspectedDirector != null)
            {
                TimelineUndo.PushUndo(TimelineEditor.inspectedDirector, L10n.Tr("Delete Track"));
                TimelineEditor.inspectedDirector.ClearGenericBinding(track);
            }

            return timeline.DeleteTrack(track);
        }
    }
}
