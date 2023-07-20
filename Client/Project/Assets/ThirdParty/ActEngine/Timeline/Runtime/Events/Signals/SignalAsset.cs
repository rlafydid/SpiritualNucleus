using System;
using UnityEngine;

namespace UnityEngine.ActTimeline
{
    /// <summary>
    /// An asset representing an emitted signal. A SignalAsset connects a SignalEmitter with a SignalReceiver.
    /// </summary>
    /// <seealso cref="UnityEngine.ActTimeline.SignalEmitter"/>
    /// <seealso cref="UnityEngine.ActTimeline.SignalReceiver"/>
    //[AssetFileNameExtension("signal")]
    [TimelineHelpURL(typeof(SignalAsset))]
    public class SignalAsset : ScriptableObject
    {
        internal static event Action<SignalAsset> OnEnableCallback;

        void OnEnable()
        {
            if (OnEnableCallback != null)
                OnEnableCallback(this);
        }
    }
}
