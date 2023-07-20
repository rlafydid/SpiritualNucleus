using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    class TrackPropertyCurvesDataSource : BasePropertyKeyDataSource
    {
        protected override AnimationClip animationClip { get; }

        public TrackPropertyCurvesDataSource(TrackAsset track)
        {
            animationClip = track != null ? track.curves : null;
        }
    }
}
