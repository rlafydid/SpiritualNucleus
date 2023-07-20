using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    interface ICurvesOwnerInspectorWrapper
    {
        ICurvesOwner curvesOwner { get; }
        SerializedObject serializedPlayableAsset { get; }
        int lastCurveVersion { get; set; }
        double lastEvalTime { get; set; }

        double ToLocalTime(double time);
    }
}
