using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    partial class MoveItemHandler : IAttractable, IAttractionHandler
    {
        public void ActGrab(IEnumerable<ITimelineItem> items, Vector2 mousePosition)
        {
            if (items == null) return;

            items = items.ToArray(); // Cache enumeration result

            if (!items.Any()) return;

            m_GrabbedModalUndoGroup = Undo.GetCurrentGroup();

            var trackItems = items.GroupBy(d => d.gui.row).ToArray();
            var trackItemsCount = trackItems.Length;
            var tracks = items.Where(x => x != null).Distinct();

            movingItems = new MovingItems[trackItemsCount];

            Debug.Log($"trackItemsCount {trackItemsCount} itemsCount {items.Count()}");
            allowTrackSwitch = trackItemsCount == 1;

            for (var i = 0; i < trackItemsCount; ++i)
            {
                //var track = trackItems[i].Key;
                var grabbedItems = new MovingItems(m_State, null, trackItems[i].ToArray(), null, mousePosition, allowTrackSwitch);
                movingItems[i] = grabbedItems;
            }

            m_LeftMostMovingItems = null;
            m_RightMostMovingItems = null;

            foreach (var grabbedTrackItems in movingItems)
            {
                if (m_LeftMostMovingItems == null || m_LeftMostMovingItems.start > grabbedTrackItems.start)
                    m_LeftMostMovingItems = grabbedTrackItems;

                if (m_RightMostMovingItems == null || m_RightMostMovingItems.end < grabbedTrackItems.end)
                    m_RightMostMovingItems = grabbedTrackItems;
            }

            m_ItemGUIs = new HashSet<TimelineItemGUI>();
            m_ItemsGroup = new ItemsGroup(items);

            foreach (var item in items)
                m_ItemGUIs.Add(item.gui);

            EditMode.BeginMove(this);
            m_Grabbing = true;
        }

        public void UpdateRow(int row)
        {
            if (!EditMode.AllowTrackSwitch())
                return;

            var targetTracksChanged = false;

            foreach (var grabbedItem in movingItems)
            {
                if (grabbedItem.AllowTrackSwitch)
                {
                    targetTracksChanged = true;

                }
            }

            if (targetTracksChanged)
            {
                foreach (var item in movingItems)
                {
                    foreach (var clip in item.clips)
                    {
                        clip.row = row;
                    }
                }
            }

            RefreshPreviewItems();

            m_State.rebuildGraph |= targetTracksChanged;
        }

        void ActCancel()
        {
            if (!m_Grabbing)
                return;

            // TODO fix undo reselection persistency
            // identify the clips by their playable asset, since that reference will survive the undo
            // This is a workaround, until a more persistent fix for selection of clips across Undo can be found
            //var assets = movingItems.SelectMany(x => x.actClips).Select(x => x.asset);

            //Undo.RevertAllDownToGroup(m_GrabbedModalUndoGroup);

            //// reselect the clips from the original clip
            //var clipsToSelect = movingItems.Select(x => x.originalTrack).SelectMany(x => x.GetClips()).Where(x => assets.Contains(x.asset)).ToArray();
            //SelectionManager.RemoveTimelineSelection();

            //foreach (var c in clipsToSelect)
            //    SelectionManager.Add(c);

            Done();
        }
    }
}
