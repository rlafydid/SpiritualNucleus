using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace UnityEditor.ActTimeline
{
    partial class TimelineDataSource
    {
        void NewFetchData()
        {
            m_RootItem = new ActTimelineTrackGUI(-1, 0, "root");
            m_RootItem.children = new List<TreeViewItem>();
            m_RootItem.displayName = "root";
            for (int i = 0; i < 20; i++)
            {
                ActTimelineTrackGUI newItem;
                newItem = new ActTimelineTrackGUI(i, 1, "child" + i.ToString());

                m_RootItem.children.Add(newItem);
            }
            SetExpanded(m_RootItem, true);
        }

    }
}


