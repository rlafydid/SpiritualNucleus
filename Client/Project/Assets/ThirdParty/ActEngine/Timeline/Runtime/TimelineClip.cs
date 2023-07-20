using System;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace UnityEngine.ActTimeline
{
    /// <summary>
    /// Implement this interface to support advanced features of timeline clips.
    /// </summary>
    public interface ITimelineClipAsset
    {
        /// <summary> 
        /// Returns a description of the features supported by clips with PlayableAssets implementing this interface.
        /// </summary>
        ClipCaps clipCaps { get; }
    }

    public enum ClipType
    {
        Clip,
        Event
    }

    /// <summary>
    /// Represents a clip on the timeline.
    /// </summary>
    [Serializable]
    public partial class TimelineClip : ICurvesOwner
    {
        /// <summary>
        /// The default capabilities for a clip
        /// </summary>
        public static readonly ClipCaps kDefaultClipCaps = ClipCaps.None;

        /// <summary>
        /// The default length of a clip in seconds.
        /// </summary>
        public static readonly float kDefaultClipDurationInSeconds = 5;

        /// <summary>
        /// The minimum timescale allowed on a clip
        /// </summary>
        public static readonly double kTimeScaleMin = 1.0 / 1000;

        /// <summary>
        /// The maximum timescale allowed on a clip
        /// </summary>
        public static readonly double kTimeScaleMax = 1000;

        internal static readonly string kDefaultCurvesName = "Clip Parameters";

        internal static readonly double kMinDuration = 1 / 60.0;

        // constant representing the longest possible sequence duration
        internal static readonly double kMaxTimeValue = 1000000; // more than a week's time, and within numerical precision boundaries

        /// <summary>
        /// How the clip handles time outside its start and end range.
        /// </summary>
        public enum ClipExtrapolation
        {
            /// <summary>
            /// No extrapolation is applied.
            /// </summary>
            None,

            /// <summary>
            /// Hold the time at the end value of the clip.
            /// </summary>
            Hold,

            /// <summary>
            /// Repeat time values outside the start/end range.
            /// </summary>
            Loop,

            /// <summary>
            /// Repeat time values outside the start/end range, reversing direction at each loop
            /// </summary>
            PingPong,

            /// <summary>
            /// Time values are passed in without modification, extending beyond the clips range
            /// </summary>
            Continue
        };


        internal TimelineClip()
        {
            // parent clip into track
        }

        public virtual ClipType GetClipType { get; }

        protected virtual double m_Start { get; set; }
        Object m_Asset;
        protected virtual double m_Duration { get; set; }

        string m_DisplayName;

        public virtual int row { get; set; }

        public virtual bool supportResize { get; }

        public virtual Color GetColor { get; }

        public virtual object GetTarget { get; }
        /// <summary>
        /// A speed multiplier for the clip;
        /// </summary>
        public double timeScale
        {
            get { return 1.0; }
            set
            {
            }
        }

        /// <summary>
        /// The start time, in seconds, of the clip
        /// </summary>
        public double start
        {
            get { return m_Start; }
            set
            {
                UpdateDirty(value, m_Start);
                var newValue = Math.Max(SanitizeTimeValue(value, m_Start), 0);
                //if (m_ParentTrack != null && m_Start != newValue)
                //{
                //    m_ParentTrack.OnClipMove();
                //}
                m_Start = newValue;
            }
        }

        /// <summary>
        /// The length, in seconds, of the clip
        /// </summary>
        public double duration
        {
            get { return m_Duration; }
            set
            {
                UpdateDirty(m_Duration, value);
                m_Duration = Math.Max(SanitizeTimeValue(value, m_Duration), double.Epsilon);
            }
        }

        /// <summary>
        /// The end time, in seconds of the clip
        /// </summary>
        public virtual double end
        {
            get
            {
                return m_Start + m_Duration;
            }
        }

        /// <summary>
        /// Local offset time of the clip.
        /// </summary>
        public double clipIn
        {
            get { return 0; }
            set
            {
            }
        }

        /// <summary>
        /// The name displayed on the clip
        /// </summary>
        public string displayName
        {
            get { return m_DisplayName; }
            set { m_DisplayName = value; }
        }


        /// <summary>
        /// The length, in seconds, of the PlayableAsset attached to the clip.
        /// </summary>
        public double clipAssetDuration
        {
            get
            {
                return duration;
            }
        }

        /// <summary>
        /// An animation clip containing animated properties of the attached PlayableAsset
        /// </summary>
        /// <remarks>
        /// This is where animated clip properties are stored.
        /// </remarks>
        public AnimationClip curves
        {
            get { return null; }
            internal set { }
        }

        /// <summary>
        /// Whether this clip contains animated properties for the attached PlayableAsset.
        /// </summary>
        /// <remarks>
        /// This property is false if the curves property is null or if it contains no information.
        /// </remarks>
        public bool hasCurves
        {
            get { return false; }
        }

        /// <summary>
        /// The PlayableAsset attached to the clip.
        /// </summary>
        public Object asset
        {
            get { return m_Asset; }
            set { m_Asset = value; }
        }

        Object ICurvesOwner.assetOwner
        {
            get { return GetParentTrack(); }
        }

        TrackAsset ICurvesOwner.targetTrack
        {
            get { return GetParentTrack(); }
        }

        /// <summary>
        /// underlyingAsset property is obsolete. Use asset property instead
        /// </summary>
        [Obsolete("underlyingAsset property is obsolete. Use asset property instead", true)]
        public Object underlyingAsset
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Returns the TrackAsset to which this clip is attached.
        /// </summary>
        [Obsolete("parentTrack is deprecated and will be removed in a future release. Use " + nameof(GetParentTrack) + "() and " + nameof(TimelineClipExtensions) + "::" + nameof(TimelineClipExtensions.MoveToTrack) + "() or " + nameof(TimelineClipExtensions) + "::" + nameof(TimelineClipExtensions.TryMoveToTrack) + "() instead.", false)]
        public TrackAsset parentTrack
        {
            get { return null; }
            set { SetParentTrack_Internal(value); }
        }

        /// <summary>
        /// Get the TrackAsset to which this clip is attached.
        /// </summary>
        /// <returns>the parent TrackAsset</returns>
        public TrackAsset GetParentTrack()
        {
            return null;
        }

        /// <summary>
        /// Sets the parent track without performing any validation. To ensure a valid change use TimelineClipExtensions.TrySetParentTrack(TrackAsset) instead.
        /// </summary>
        /// <param name="newParentTrack"></param>
        internal void SetParentTrack_Internal(TrackAsset newParentTrack)
        {
            return;
        }

        /// <summary>
        /// The ease in duration of the timeline clip in seconds. This only applies if the start of the clip is not overlapping.
        /// </summary>
        public double easeInDuration
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }

        /// <summary>
        /// The ease out duration of the timeline clip in seconds. This only applies if the end of the clip is not overlapping.
        /// </summary>
        public double easeOutDuration
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        /// <summary>
        /// The amount of overlap in seconds on the start of a clip.
        /// </summary>
        public double blendInDuration
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// The amount of overlap in seconds at the end of a clip.
        /// </summary>
        public double blendOutDuration
        {
            get { return 0; }
            set { }
        }


        /// <summary>
        /// Returns whether the clip is blending in
        /// </summary>
        public bool hasBlendIn { get { return false; } }

        /// <summary>
        /// Returns whether the clip is blending out
        /// </summary>
        public bool hasBlendOut { get { return false; } }

        /// <summary>
        /// The amount of the clip being used for ease or blend in as a percentage
        /// </summary>
        public float mixInPercentage
        {
            get { return 0; }
        }

        /// <summary>
        /// The amount of the clip blending or easing in, in seconds
        /// </summary>
        public double mixInDuration
        {
            get { return 0; }
        }

        /// <summary>
        /// The time in seconds that an ease out or blend out starts
        /// </summary>
        public double mixOutTime
        {
            get { return 0; }
        }

        /// <summary>
        /// The amount of the clip blending or easing out, in seconds
        /// </summary>
        public double mixOutDuration
        {
            get { return 0; }
        }

        /// <summary>
        /// The amount of the clip being used for ease or blend out as a percentage
        /// </summary>
        public float mixOutPercentage
        {
            get { return 0; }
        }

        /// <summary>
        /// Returns whether this clip is recordable in editor
        /// </summary>
        public bool recordable
        {
            get { return false; }
            internal set { }
        }

        /// <summary>
        /// exposedParameter is deprecated and will be removed in a future release
        /// </summary>
        [Obsolete("exposedParameter is deprecated and will be removed in a future release", true)]
        public List<string> exposedParameters
        {
            get { return null; }
        }

        /// <summary>
        /// Returns the capabilities supported by this clip.
        /// </summary>
        public ClipCaps clipCaps
        {
            get
            {
                var clipAsset = asset as ITimelineClipAsset;
                return (clipAsset != null) ? clipAsset.clipCaps : kDefaultClipCaps;
            }
        }

        internal int Hash()
        {
            return HashUtility.CombineHash(m_Start.GetHashCode(),
                m_Duration.GetHashCode());
        }

        /// <summary>
        /// Given a time, returns the weight from the mix out
        /// </summary>
        /// <param name="time">Time (relative to the timeline)</param>
        /// <returns></returns>
        public float EvaluateMixOut(double time)
        {
            if (!clipCaps.HasAny(ClipCaps.Blending))
                return 1.0f;

            if (mixOutDuration > Mathf.Epsilon)
            {
                var perc = (float)(time - mixOutTime) / (float)mixOutDuration;
                //perc = Mathf.Clamp01(mixOutCurve.Evaluate(perc));
                return perc;
            }
            return 1.0f;
        }

        /// <summary>
        /// Given a time, returns the weight from the mix in
        /// </summary>
        /// <param name="time">Time (relative to the timeline)</param>
        /// <returns></returns>
        public float EvaluateMixIn(double time)
        {
            if (!clipCaps.HasAny(ClipCaps.Blending))
                return 1.0f;

            if (mixInDuration > Mathf.Epsilon)
            {
                var perc = (float)(time - m_Start) / (float)mixInDuration;
                //perc = Mathf.Clamp01(mixInCurve.Evaluate(perc));
                return perc;
            }
            return 1.0f;
        }

        static AnimationCurve GetDefaultMixInCurve()
        {
            return AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        static AnimationCurve GetDefaultMixOutCurve()
        {
            return AnimationCurve.EaseInOut(0, 1, 1, 0);
        }

        /// <summary>
        /// Converts from global time to a clips local time.
        /// </summary>
        /// <param name="time">time relative to the timeline</param>
        /// <returns>
        /// The local time with extrapolation applied
        /// </returns>
        public double ToLocalTime(double time)
        {
            if (time < 0)
                return time;

            return 0;
        }

        /// <summary>
        /// Converts from global time to local time of the clip
        /// </summary>
        /// <param name="time">The time relative to the timeline</param>
        /// <returns>The local time, ignoring any extrapolation or bounds</returns>
        public double ToLocalTimeUnbound(double time)
        {
            return (time - m_Start) * timeScale + clipIn;
        }

        /// <summary>
        /// Converts from local time of the clip to global time
        /// </summary>
        /// <param name="time">Time relative to the clip</param>
        /// <returns>The time relative to the timeline</returns>
        internal double FromLocalTimeUnbound(double time)
        {
            return (time - clipIn) / timeScale + m_Start;
        }

        /// <summary>
        /// If this contains an animation asset, returns the animation clip attached. Otherwise returns null.
        /// </summary>
        public AnimationClip animationClip
        {
            get
            {
                if (m_Asset == null)
                    return null;

                var playableAsset = m_Asset as AnimationPlayableAsset;
                return playableAsset != null ? playableAsset.clip : null;
            }
        }

        static double SanitizeTimeValue(double value, double defaultValue)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                Debug.LogError("Invalid time value assigned");
                return defaultValue;
            }

            return Math.Max(-kMaxTimeValue, Math.Min(kMaxTimeValue, value));
        }

        /// <summary>
        /// Returns whether the clip is being extrapolated past the end time.
        /// </summary>
        public ClipExtrapolation postExtrapolationMode
        {
            get { return ClipExtrapolation.None; }
            internal set { }
        }

        /// <summary>
        /// Returns whether the clip is being extrapolated before the start time.
        /// </summary>
        public ClipExtrapolation preExtrapolationMode
        {
            get { return ClipExtrapolation.None; }
            internal set { }
        }

        internal void SetPostExtrapolationTime(double time)
        {
        }

        internal void SetPreExtrapolationTime(double time)
        {
        }

        /// <summary>
        /// Given a time, returns whether it falls within the clip's extrapolation
        /// </summary>
        /// <param name="sequenceTime">The time relative to the timeline</param>
        /// <returns>True if <paramref name="sequenceTime"/> is within the clip extrapolation</returns>
        public bool IsExtrapolatedTime(double sequenceTime)
        {
            return IsPreExtrapolatedTime(sequenceTime) || IsPostExtrapolatedTime(sequenceTime);
        }

        /// <summary>
        /// Given a time, returns whether it falls within the clip's pre-extrapolation
        /// </summary>
        /// <param name="sequenceTime">The time relative to the timeline</param>
        /// <returns>True if <paramref name="sequenceTime"/> is within the clip pre-extrapolation</returns>
        public bool IsPreExtrapolatedTime(double sequenceTime)
        {
            return preExtrapolationMode != ClipExtrapolation.None &&
                sequenceTime < m_Start && sequenceTime >= m_Start - 0;
        }

        /// <summary>
        /// Given a time, returns whether it falls within the clip's post-extrapolation
        /// </summary>
        /// <param name="sequenceTime">The time relative to the timeline</param>
        /// <returns>True if <paramref name="sequenceTime"/> is within the clip post-extrapolation</returns>
        public bool IsPostExtrapolatedTime(double sequenceTime)
        {
            return postExtrapolationMode != ClipExtrapolation.None &&
                (sequenceTime > end) && (sequenceTime - end < 0);
        }

        /// <summary>
        /// Creates an AnimationClip to store animated properties for the attached PlayableAsset.
        /// </summary>
        /// <remarks>
        /// If curves already exists for this clip, this method produces no result regardless of the
        /// value specified for curvesClipName.
        /// </remarks>
        /// <remarks>
        /// When used from the editor, this method attempts to save the created curves clip to the TimelineAsset.
        /// The TimelineAsset must already exist in the AssetDatabase to save the curves clip. If the TimelineAsset
        /// does not exist, the curves clip is still created but it is not saved.
        /// </remarks>
        /// <param name="curvesClipName">
        /// The name of the AnimationClip to create.
        /// This method does not ensure unique names. If you want a unique clip name, you must provide one.
        /// See ObjectNames.GetUniqueName for information on a method that creates unique names.
        /// </param>
        public void CreateCurves(string curvesClipName)
        {
        }

        /// <summary>
        /// Outputs a more readable representation of the timeline clip as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} ({1:F2}, {2:F2}):{3:F2} | {4}", displayName, start, end, clipIn, GetParentTrack());
        }

        /// <summary>
        /// Use this method to adjust ease in and ease out values to avoid overlapping.
        /// </summary>
        /// <remarks>
        /// Ease values will be adjusted to respect the ratio between ease in and ease out.
        /// </remarks>
        public void ConformEaseValues()
        {
        }

        static double CalculateEasingRatio(double easeIn, double easeOut)
        {
            if (Math.Abs(easeIn - easeOut) < TimeUtility.kTimeEpsilon)
                return 0.5;

            if (easeIn == 0.0)
                return 0.0;

            if (easeOut == 0.0)
                return 1.0;

            return easeIn / (easeIn + easeOut);
        }

#if UNITY_EDITOR
        internal int DirtyIndex { get; private set; }

        public string defaultCurvesName => throw new NotImplementedException();

        internal void MarkDirty()
        {
            DirtyIndex++;
        }

        void UpdateDirty(double oldValue, double newValue)
        {
            if (oldValue != newValue)
                DirtyIndex++;
        }

#else
        void UpdateDirty(double oldValue, double newValue) { }
#endif
    }
}
