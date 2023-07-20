using Act;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    class TimelineSwitch
    {
        public const bool isUseNew = true;
        public const bool isNormalScrollScale = false;
    }
    partial class TimelineTreeView
    {

        public void NewCalculateRowRects()
        {
            if (m_TreeView.isSearching)
                return;

            const float startY = 6.0f;
            IList<TreeViewItem> rows = m_TreeView.data.GetRows();
            m_RowRects = new List<Rect>(rows.Count);
            m_ExpandedRowRects = new List<Rect>(rows.Count);

            float curY = startY;
            m_MaxWidthOfRows = 1f;

            // first pass compute the row rects
            for (int i = 0; i < rows.Count; ++i)
            {
                var item = rows[i];

                if (i != 0)
                    curY += 3;

                //Vector2 rowSize = new Vector2(m_TreeView.GetTotalRect().width, kMinTrackHeight);
                Vector2 rowSize = GetSizeOfRow(item);

                m_RowRects.Add(new Rect(0, curY, rowSize.x, rowSize.y));
                m_ExpandedRowRects.Add(m_RowRects[i]);

                curY += rowSize.y;

                if (rowSize.x > m_MaxWidthOfRows)
                    m_MaxWidthOfRows = rowSize.x;
            }
        }

        void NewOnRowGUI(Rect rowRect, TreeViewItem item, int row, bool selected, bool focused)
        {
            if (item.displayName == "root")
                return;

            //using (new EditorGUI.DisabledScope(TimelineWindow.instance.currentMode.TrackState(TimelineWindow.instance.state) == TimelineModeGUIState.Disabled))
            using (new EditorGUI.DisabledScope(false))
            {
                if (item.GetType() != typeof(ActTimelineTrackGUI))
                {
                    Debug.LogError($"TreeViewItem {item.GetType().Name} displayName : {item.displayName}");
                    IList<TreeViewItem> rows = m_TreeView.data.GetRows();
                    foreach (var testItem in rows)
                    {
                        Debug.LogError($"TreeViewItem error item {item.GetType().Name} displayName : {item.displayName} id : {item.id}");
                    }

                }
                else
                {
                    var track = (ActTimelineTrackGUI)item;
                    track.treeViewToWindowTransformation = m_TreeView.GetTotalRect().position - m_TreeView.state.scrollPos;
                    track.Draw(rowRect, TimelineWindow.instance.state, item.id, selectIndex == item.id);

                    Rect r = new Rect(track.treeViewToWindowTransformation.x, track.treeViewToWindowTransformation.y, 300f, 10f);
                    if (Event.current.type == EventType.Repaint)
                    {
                        m_State.spacePartitioner.AddBounds(track);
                    }
                }

            }

        }

        int selectIndex;

        void NewSelectionChangedCallback(int[] ids)
        {
            if (Event.current.button == 1 && PickerUtils.TopmostPickedItem() is ISelectable)
                return;
            selectIndex = ids[0];

            if (Event.current.command || Event.current.control || Event.current.shift)
                SelectionManager.UnSelectTracks();
            else
                SelectionManager.Clear();

            //foreach (var id in ids)
            //{
            //    var trackGUI = (TimelineTrackBaseGUI)m_TreeView.FindItem(id);
            //    SelectionManager.Add(trackGUI.track);
            //}

            m_State.GetWindow().Repaint();
        }

        void NewContextClickItemCallback(int id)
        {
            Debug.Log($"ContextClickItemCallback {id}");
            // may not occur if another menu is active
            if (!m_TreeView.IsSelected(id))
                NewSelectionChangedCallback(new[] { id });
            Debug.Log("ShowTrackContextMenu");

            var mapping = Act.ActUtility.GetClipDisplayNameMapping;

            GenericMenu menu = new GenericMenu();

            foreach (var item in mapping)
            {
                menu.AddItem(new GUIContent(item.Value.MenuName), false,
                () =>
                {
                    ClipModifier.CreateClip(id, item.Key);
                });
            }

            var eventMapping = Act.ActUtility.GetEventDisplayNameMapping;

            foreach (var item in eventMapping)
            {
                menu.AddItem(new GUIContent(item.Value.MenuName), false,
                () =>
                {
                    ClipModifier.CreateEvent(id, item.Key);
                });
            }

            menu.ShowAsContext();
            //SequencerContextMenu.ShowTrackContextMenu(Event.current.mousePosition);

            Event.current.Use();
        }



    }
}