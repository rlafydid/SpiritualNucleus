using System;
using UnityEngine.Playables;

namespace UnityEngine.ActTimeline
{
    [Serializable]
    [NotKeyable]
    class AudioClipProperties
    {
        [Range(0.0f, 1.0f)]
        public float volume = 1.0f;
    }
}
