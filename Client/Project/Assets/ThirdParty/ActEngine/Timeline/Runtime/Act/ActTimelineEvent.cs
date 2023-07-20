using Act;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.ActTimeline
{
    public class ActTimelineEvent : TimelineClip
    {
        public override ClipType GetClipType => ClipType.Event;

        protected override double m_Duration
        {
            get
            {
                //if (actClipData.Duration > 0)
                return 0;
                //else
                //    return actClipData.Asset.LifeTime - actClipData.TriggerTime;
            }
        }
        public override int row
        {
            get
            {
                return actEventData.Row;
            }
            set
            {
                actEventData.Row = value;
            }
        }

        protected override double m_Start
        {
            get
            {
                return actEventData.TriggerTime;
            }
            set
            {
                actEventData.TriggerTime = (float)value;
            }
        }

        public override double end
        {
            get
            {
                return m_Start;
            }
        }

        public override bool supportResize => false;

        [SerializeField]
        ActBaseEvent actEventData;

        public override object GetTarget => actEventData;

        internal ActTimelineEvent(Act.ActBaseEvent actEvent)
        {
            // parent clip into track
            this.actEventData = actEvent;
            this.displayName = Act.ActUtility.GetDisplayName(actEvent.GetType());
        }
    }
}
