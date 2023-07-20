using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.ActTimeline
{
    [Serializable]
    public class ActTimelineClip : TimelineClip
    {
        public override ClipType GetClipType => ClipType.Clip;

        protected override double m_Duration
        {
            get
            {
                //if (actClipData.Duration > 0)
                return Math.Max(actClipData.Duration, 0.1f);
                //else
                //    return actClipData.Asset.LifeTime - actClipData.TriggerTime;
            }
            set
            {
                actClipData.Duration = (float)Math.Max(value, 0.1);
            }
        }
        public override int row
        {
            get
            {
                return actClipData.Row;
            }
            set
            {
                actClipData.Row = value;
            }
        }

        protected override double m_Start
        {
            get
            {
                return actClipData.TriggerTime;
            }
            set
            {
                actClipData.TriggerTime = (float)value;
            }
        }

        public override double end
        {
            get
            {
                if (actClipData is Act.ActAnimationClip animClip)
                {
                    float duration = animClip.Duration;
                    if (animClip.Duration <= 0)
                        duration = 0.2f;
                    return m_Start + duration;
                }
                else
                {
                    return m_Start + m_Duration;
                }
            }
        }

        public override bool supportResize => actClipData.SupportResize;

        [SerializeField]
        Act.ActBaseClip actClipData;

        public override object GetTarget => actClipData;

        internal ActTimelineClip(Act.ActBaseClip clip)
        {
            // parent clip into track
            this.actClipData = clip;
            this.displayName = Act.ActUtility.GetDisplayName(clip.GetType());
        }
    }
}