using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ActTimeline;
using Object = UnityEngine.Object;

namespace UnityEditor.ActTimeline
{
    static class MarkerModifier
    {
        public static void CreateMarker(int row, Type type)
        {
            var clips = ActTimelineUtility.GetClipGUIList(row);
            clips = clips.OrderBy(d => d.clip.start).ToList();

            float triggerTime = 0;
            if (clips.Count > 0 && clips[0].start < 1)
            {
                var lastClip = clips[clips.Count - 1];
                triggerTime = (float)lastClip.end;
            }

            var clip = (Act.ActBaseClip)Activator.CreateInstance(type);
            clip.TriggerTime = triggerTime;
            clip.Duration = 1;
            clip.Row = row;
            clip.Asset = TimelineWindow.instance.actData;
            clip.AddToTimeline();

            //Add(clip);
        }
        public static void DeleteMarker(IMarker marker)
        {
            var trackAsset = marker.parent;
            if (trackAsset != null)
            {
                SelectionManager.Remove(marker);
                trackAsset.DeleteMarker(marker);
            }
        }

        public static IEnumerable<IMarker> CloneMarkersToParent(IEnumerable<IMarker> markers, TrackAsset parent)
        {
            if (!markers.Any()) return Enumerable.Empty<IMarker>();
            var clonedMarkers = new List<IMarker>();
            foreach (var marker in markers)
                clonedMarkers.Add(CloneMarkerToParent(marker, parent));
            return clonedMarkers;
        }

        public static IMarker CloneMarkerToParent(IMarker marker, TrackAsset parent)
        {
            var markerObject = marker as ScriptableObject;
            if (markerObject == null) return null;

            var newMarkerObject = Object.Instantiate(markerObject);
            AddMarkerToParent(newMarkerObject, parent);

            newMarkerObject.name = markerObject.name;
            try
            {
                CustomTimelineEditorCache.GetMarkerEditor((IMarker)newMarkerObject).OnCreate((IMarker)newMarkerObject, marker);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }


            return (IMarker)newMarkerObject;
        }

        static void AddMarkerToParent(ScriptableObject marker, TrackAsset parent)
        {
            TimelineCreateUtilities.SaveAssetIntoObject(marker, parent);
            TimelineUndo.RegisterCreatedObjectUndo(marker, L10n.Tr("Duplicate Marker"));
            UndoExtensions.RegisterTrack(parent, L10n.Tr("Duplicate Marker"));

            if (parent != null)
            {
                parent.AddMarker(marker);
                ((IMarker)marker).Initialize(parent);
            }
        }
    }
}
