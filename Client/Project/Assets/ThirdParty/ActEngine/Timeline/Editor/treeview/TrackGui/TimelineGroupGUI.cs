using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    class TimelineGroupGUI : TimelineTrackBaseGUI
    {
        protected DirectorStyles m_Styles;
        protected Rect m_TreeViewRect = new Rect(0, 0, 0, 0);
        protected GUIContent m_ProblemIcon = new GUIContent();

        bool m_MustRecomputeUnions = true;
        int m_GroupDepth;
        readonly bool m_IsReferencedTrack;
        readonly List<TimelineClipUnion> m_Unions = new List<TimelineClipUnion>();

        public override Rect boundingRect
        {
            get { return ToWindowSpace(m_TreeViewRect); }
        }

        public Rect ToWindowSpace(Rect localRect)
        {
            localRect.position += treeViewToWindowTransformation;
            return localRect;
        }

        public override bool expandable
        {
            get { return !m_IsRoot; }
        }

        // The expanded rectangle (contains children) as calculated by the the tree gui
        public Rect expandedRect { get; set; }
        // The row rectangle (header only) as calculated by the tree gui
        public Rect rowRect { get; set; }
        // the drop rectangle as set by the tree gui when targetted by a drag and drop
        public Rect dropRect { get; set; }

        public TimelineGroupGUI(TreeViewController treeview, TimelineTreeViewGUI treeviewGUI, int id, int depth, TreeViewItem parent, string displayName, TrackAsset trackAsset, bool isRoot)
            : base(id, depth, parent, displayName, trackAsset, treeview, treeviewGUI)
        {
            m_Styles = DirectorStyles.Instance;
            m_IsRoot = isRoot;

            var trackPath = AssetDatabase.GetAssetPath(trackAsset);
            var sequencePath = AssetDatabase.GetAssetPath(treeviewGUI.TimelineWindow.state.editSequence.asset);
            if (trackPath != sequencePath)
                m_IsReferencedTrack = true;

            m_GroupDepth = CalculateGroupDepth(parent);
        }

        public virtual float GetHeight(WindowState state)
        {
            // group tracks don't scale in height
            return TrackEditor.DefaultTrackHeight;
        }

        public override void OnGraphRebuilt() { }

        static int CalculateGroupDepth(TreeViewItem parent)
        {
            int depth = 0;

            bool done = false;
            do
            {
                var gui = parent as TimelineGroupGUI;
                if (gui == null || gui.track == null)
                    done = true;
                else
                {
                    if (gui.track is GroupTrack)
                        depth++;

                    parent = parent.parent;
                }
            }
            while (!done);

            return depth;
        }

        protected bool IsSubTrack()
        {
            if (track == null)
                return false;

            var parentTrack = track.parent as TrackAsset;
            if (parentTrack == null)
                return false;

            return parentTrack.GetType() != typeof(GroupTrack);
        }

        protected TrackAsset ParentTrack()
        {
            if (IsSubTrack())
                return track.parent as TrackAsset;
            return null;
        }


        static bool AllChildrenMuted(TimelineGroupGUI groupGui)
        {
            if (!groupGui.track.muted)
                return false;
            if (groupGui.children == null)
                return true;
            return groupGui.children.OfType<TimelineGroupGUI>().All(AllChildrenMuted);
        }

        protected static float DrawButtonSuite(int numberOfButtons, ref Rect buttonRect)
        {
            var style = DirectorStyles.Instance.trackButtonSuite;
            var buttonWidth = WindowConstants.trackHeaderButtonSize * numberOfButtons + WindowConstants.trackHeaderButtonPadding * Math.Max(0, numberOfButtons - 1);
            var suiteWidth = buttonWidth + style.padding.right + style.padding.left;

            var rect = new Rect(buttonRect.xMax - style.margin.right - suiteWidth, buttonRect.y + style.margin.top, suiteWidth, buttonRect.height);
            if (Event.current.type == EventType.Repaint)
                style.Draw(rect, false, false, false, false);
            buttonRect.x -= style.margin.right + style.padding.right;
            return style.margin.left + style.padding.left;
        }

        public override void Draw(Rect headerRect, Rect contentRect, WindowState state)
        {
        }
    }
}
