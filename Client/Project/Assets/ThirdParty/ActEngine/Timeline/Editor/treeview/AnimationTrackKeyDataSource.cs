using UnityEngine;
using UnityEngine.ActTimeline;

// Data sources for key overlays
namespace UnityEditor.ActTimeline
{
    // Used for key overlays manipulators
    class AnimationTrackKeyDataSource : BasePropertyKeyDataSource
    {
        readonly float m_TrackOffset;

        protected override AnimationClip animationClip { get; }

        public AnimationTrackKeyDataSource(AnimationTrack track)
        {
            animationClip = track != null ? track.infiniteClip : null;
            m_TrackOffset = track != null ? (float)track.infiniteClipTimeOffset : 0.0f;
        }

        protected override float TransformKeyTime(float keyTime)
        {
            return keyTime + m_TrackOffset;
        }
    }
}
