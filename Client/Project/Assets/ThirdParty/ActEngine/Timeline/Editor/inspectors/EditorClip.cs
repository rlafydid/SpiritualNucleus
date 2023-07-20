using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    [ExcludeFromPreset]
    [TimelineHelpURL(typeof(TimelineClip))]
    class EditorClip : ScriptableObject
    {
        [SerializeField, SerializeReference] TimelineClip m_Clip;

        public TimelineClip clip
        {
            get { return m_Clip; }
            set { m_Clip = value; }
        }

        public Act.ActEditor actEditor;

        public int lastHash { get; set; }

        public override int GetHashCode()
        {
            return clip.Hash();
        }
    }
}
