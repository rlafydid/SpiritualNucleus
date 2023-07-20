using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    class ActTimelineTrackGUI : TreeViewItem, IRowGUI, IBounds
    {
        readonly TimelineTreeViewGUI m_TreeViewGUI;

        TreeViewController treeView { get; }

        public TimelineWindow TimelineWindow
        {
            get
            {
                if (m_TreeViewGUI == null)
                    return null;

                return m_TreeViewGUI.TimelineWindow;
            }
        }
        public Vector2 treeViewToWindowTransformation { get; set; }

        public TrackAsset asset => null;

        public Rect boundingRect
        {
            get { return ToWindowSpace(trackRect); }
        }

        public bool locked => false;

        public bool showMarkers => false;

        public bool muted => false;

        Rect trackRect;

        public List<TimelineClipGUI> clipList
        {
            get
            {
                return ActTimelineUtility.GetClipGUIList(this.id);
            }
        }

        public List<TimelineMarkerGUI> eventList
        {
            get
            {
                return ActTimelineUtility.GetEventGUIList(this.id);
            }
        }

        public ActTimelineTrackGUI(int id, int depth, string displayName) : base(id, depth, displayName)
        {
            this.displayName = $"child {id}";
        }

        public void Draw(Rect trackRect, WindowState state, int row, bool isSelect)
        {
            this.trackRect = trackRect;

            var trackContentRect = trackRect;

            float inlineCurveHeight = trackRect.height - GetTrackContentHeight(state);
            bool hasInlineCurve = inlineCurveHeight > 0.0f;

            if (hasInlineCurve)
            {
                trackContentRect.height -= inlineCurveHeight;
            }

            DrawBackground(trackContentRect, state, row, isSelect);

            Rect r = trackRect;
            r.x += 200;
            //EditorGUI.LabelField(r, $"row={row} id={this.id} clipCount = {clipList.Count} rect {trackContentRect.ToString()}");

            for (int i = 0; i < clipList.Count; i++)
            {
                clipList[i].parent = this;
                clipList[i].Draw(trackContentRect, false, state);
            }

            for (int i = 0; i < eventList.Count; i++)
            {
                eventList[i].parent = this;
                eventList[i].Draw(trackContentRect, false, state);
            }
        }
        float minimumHeight => TrackEditor.DefaultTrackHeight;

        float GetTrackContentHeight(WindowState state)
        {
            var defaultHeight = Mathf.Min(minimumHeight, TrackEditor.MaximumTrackHeight);

            return (defaultHeight) * state.trackScale;
        }

        void DrawBackground(Rect trackRect, WindowState state, int row, bool isSelect)
        {
            Color trackBackgroundColor;

            if (isSelect)
            {
                trackBackgroundColor = state.IsEditingASubTimeline() ?
                    DirectorStyles.Instance.customSkin.colorTrackSubSequenceBackgroundSelected :
                    DirectorStyles.Instance.customSkin.colorTrackBackgroundSelected;
            }
            else
            {
                trackBackgroundColor = state.IsEditingASubTimeline() ?
                DirectorStyles.Instance.customSkin.colorTrackSubSequenceBackground :
                DirectorStyles.Instance.customSkin.colorTrackBackground;
            }

            EditorGUI.DrawRect(trackRect, trackBackgroundColor);
            trackRect.x += 200;
        }

        public Rect ToWindowSpace(Rect localRect)
        {
            localRect.position += treeViewToWindowTransformation;
            return localRect;
        }
    }
}
