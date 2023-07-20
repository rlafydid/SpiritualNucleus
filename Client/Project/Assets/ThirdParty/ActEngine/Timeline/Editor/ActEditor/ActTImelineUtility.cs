using Act;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ActTimeline;
using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    static class ActTimelineUtility
    {
        static Dictionary<Act.ActBaseClip, TimelineClipGUI> actClipMapping = new Dictionary<Act.ActBaseClip, TimelineClipGUI>();
        static Dictionary<Act.ActBaseEvent, TimelineMarkerGUI> actEventMapping = new Dictionary<Act.ActBaseEvent, TimelineMarkerGUI>();

        public static TimelineClip AddToTimeline(this Act.ActBaseClip clip)
        {
            if (!actClipMapping.ContainsKey(clip))
            {
                TimelineClip tClip = new ActTimelineClip(clip);

                var currentClipGUI = new TimelineClipGUI(tClip) { isInvalid = false };

                actClipMapping.Add(clip, currentClipGUI);
                return tClip;
            }
            return null;
        }

        public static TimelineClip AddToTimeline(this Act.ActBaseEvent actEvent)
        {
            if (!actEventMapping.ContainsKey(actEvent))
            {
                ActTimelineEvent tEvent = new ActTimelineEvent(actEvent);

                var currentEventGUI = new TimelineMarkerGUI(tEvent) { isInvalid = false };

                actEventMapping.Add(actEvent, currentEventGUI);

                return tEvent;
            }
            return null;
        }

        public static void CheckClipsGUI(ActAsset asset)
        {
            Dictionary<Act.ActBaseClip, TimelineClipGUI> lastMapping = new Dictionary<ActBaseClip, TimelineClipGUI>(actClipMapping);
            actClipMapping.Clear();
            foreach (var clip in asset.Clips)
            {
                if (lastMapping.TryGetValue(clip, out var gui))
                {
                    actClipMapping.Add(clip, gui);
                }
                else
                {
                    clip.AddToTimeline();
                }
            }

            Dictionary<ActBaseEvent, TimelineMarkerGUI> lastEventMapping = new Dictionary<ActBaseEvent, TimelineMarkerGUI>(actEventMapping);
            actEventMapping.Clear();
            foreach (var clip in asset.Events)
            {
                if (lastEventMapping.TryGetValue(clip, out var gui))
                {
                    actEventMapping.Add(clip, gui);
                }
                else
                {
                    clip.AddToTimeline();
                }
            }
        }

        public static List<TimelineClipGUI> GetClipGUIList(int row = -1)
        {
            List<TimelineClipGUI> guiList = new List<TimelineClipGUI>();
            foreach (var item in actClipMapping)
            {
                if (item.Key.Row == row || row == -1)
                {
                    guiList.Add(item.Value);
                }
            }
            return guiList;
        }

        public static List<TimelineMarkerGUI> GetEventGUIList(int row = -1)
        {
            List<TimelineMarkerGUI> guiList = new List<TimelineMarkerGUI>();
            foreach (var item in actEventMapping)
            {
                if (item.Key.Row == row || row == -1)
                {
                    guiList.Add(item.Value);
                }
            }
            return guiList;
        }

        public static List<ActBaseClip> GetActClipList(int row = -1)
        {
            List<ActBaseClip> clipList = new List<ActBaseClip>();
            foreach (var item in TimelineWindow.instance.actData.Clips)
            {
                if (item.Row == row || row == -1)
                {
                    clipList.Add(item);
                }
            }
            return clipList;
        }

    }
}
