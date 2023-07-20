namespace UnityEditor.ActTimeline
{
    enum PlacementValidity
    {
        Valid,
        InvalidContains,
        InvalidIsWithin,
        InvalidStartsInBlend,
        InvalidEndsInBlend,
        InvalidContainsBlend,
        InvalidOverlapWithNonBlendableClip
    }
}
