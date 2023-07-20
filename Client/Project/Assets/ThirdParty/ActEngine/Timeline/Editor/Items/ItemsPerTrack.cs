using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    class ItemsPerTrack
    {
        public virtual TrackAsset targetTrack { get; }

        public IEnumerable<ITimelineItem> items
        {
            get { return m_ItemsGroup.items; }
        }

        public IEnumerable<TimelineClip>
            clips
        {
            get { return m_ItemsGroup.items.OfType<ClipItem>().Select(i => i.clip); }
        }

        public IEnumerable<IMarker> markers
        {
            get { return m_ItemsGroup.items.OfType<MarkerItem>().Select(i => i.marker); }
        }

        public ITimelineItem leftMostItem
        {
            get { return m_ItemsGroup.leftMostItem; }
        }

        public ITimelineItem rightMostItem
        {
            get { return m_ItemsGroup.rightMostItem; }
        }

        public int row { get => clips.First().row; }

        protected readonly ItemsGroup m_ItemsGroup;

        public ItemsPerTrack(TrackAsset targetTrack, IEnumerable<ITimelineItem> items)
        {
            Debug.Log("Create ItemsPoerTrack");
            this.targetTrack = targetTrack;
            m_ItemsGroup = new ItemsGroup(items);
        }
    }
}
