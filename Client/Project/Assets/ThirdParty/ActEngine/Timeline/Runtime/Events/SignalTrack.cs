using System;

namespace UnityEngine.ActTimeline
{
    /// <summary>
    /// Use this track to emit signals to a bound SignalReceiver.
    /// </summary>
    /// <remarks>
    /// This track cannot contain clips.
    /// </remarks>
    /// <seealso cref="UnityEngine.ActTimeline.SignalEmitter"/>
    /// <seealso cref="UnityEngine.ActTimeline.SignalReceiver"/>
    /// <seealso cref="UnityEngine.ActTimeline.SignalAsset"/>
    [Serializable]
    [TrackBindingType(typeof(SignalReceiver))]
    [TrackColor(0.25f, 0.25f, 0.25f)]
    [ExcludeFromPreset]
    [TimelineHelpURL(typeof(SignalTrack))]
    public class SignalTrack : MarkerTrack { }
}
