namespace UnityEngine.ActTimeline
{
    interface ICurvesOwner
    {
        AnimationClip curves { get; }
        bool hasCurves { get; }
        double duration { get; }
        void CreateCurves(string curvesClipName);

        Object asset { get; }
        Object assetOwner { get; }
        TrackAsset targetTrack { get; }
    }
}
