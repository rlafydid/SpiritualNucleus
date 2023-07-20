using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ActTimeline;
using UnityObject = UnityEngine.Object;

namespace UnityEditor.ActTimeline
{
    class TimelineMarkerGUI : TimelineItemGUI, ISnappable, IAttractable
    {
        public event System.Action onStartDrag;

        int m_ProjectedClipHash = -1;
        int m_MarkerHash;
        bool m_Selectable;

        MarkerDrawOptions m_MarkerDrawOptions;
        MarkerEditor m_Editor;

        ActTimelineEvent marker { get; }

        bool selectable
        {
            get { return m_Selectable; }
        }

        public double time
        {
            get { return marker.start; }
        }

        public override double start
        {
            get { return time; }
        }

        public override double end
        {
            get { return time; }
        }

        public override void Select()
        {
            //zOrder = zOrderProvider.Next();
            SelectionManager.Add(marker);
            //if(TimelineWindow.instance.state.previewMode)
            //                ActGizmosViewer.Instance.Add(DrawPos);
            //TimelineWindowViewPrefs.GetTrackViewModelData(parent.asset).markerTimeStamps[m_MarkerHash] = DateTime.UtcNow.Ticks;
        }

        public override bool IsSelected()
        {
            return SelectionManager.Contains(marker);
        }

        public override void Deselect()
        {
            Debug.Log("取消选中");
            SelectionManager.Remove(marker);
            //ActGizmosViewer.Instance.Remove(DrawPos);
        }

        void DrawPos()
        {
            if (marker.GetTarget is Act.ActBulletEvent bulletEvent && bulletEvent.BulletData != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(bulletEvent.BulletData.launchOffset, 0.1f);
            }
        }

        public override ITimelineItem item
        {
            get { return ItemsUtils.ToItem(marker); }
        }

        IZOrderProvider zOrderProvider { get; }

        public TimelineMarkerGUI(ActTimelineEvent actEvent) : base()
        {
            this._clip = actEvent;
            m_Selectable = false;
            marker = actEvent;
            //m_MarkerHash = 0;
            //var o = marker as object;
            ////if (!o.Equals(null))
            //m_MarkerHash = o.GetHashCode();

            //zOrder = zOrderProvider.Next();
            ItemToItemGui.Add(marker, this);
            //m_Editor = CustomTimelineEditorCache.GetMarkerEditor(actEvent);
        }

        int ComputeDirtyHash()
        {
            return time.GetHashCode();
        }

        static void DrawMarker(Rect drawRect, Type type, bool isSelected, bool isCollapsed, MarkerDrawOptions options)
        {
            if (Event.current.type == EventType.Repaint)
            {
                bool hasError = !string.IsNullOrEmpty(options.errorText);

                var style = StyleManager.UssStyleForType(type);
                style.Draw(drawRect, GUIContent.none, false, false, !isCollapsed, isSelected);

                // case1141836: Use of GUI.Box instead of GUI.Label causes desync in UI controlID
                if (hasError)
                    GUI.Label(drawRect, String.Empty, DirectorStyles.Instance.markerWarning);

                var tooltip = hasError ? options.errorText : options.tooltip;
                if (!string.IsNullOrEmpty(tooltip) && drawRect.Contains(Event.current.mousePosition))
                {
                    GUIStyle.SetMouseTooltip(tooltip, drawRect);
                }
            }
        }

        void UpdateDrawData()
        {
            if (Event.current.type == EventType.Layout)
            {
                m_MarkerDrawOptions = new MarkerDrawOptions()
                {
                    tooltip = string.Empty,
                    errorText = string.Empty,
                };
            }
        }

        public override void Draw(Rect trackRect, bool trackRectChanged, WindowState state)
        {
            UpdateDrawData();

            // compute marker hash
            var currentMarkerHash = ComputeDirtyHash();

            // update the clip projected rectangle on the timeline
            CalculateClipRectangle(trackRect, state, currentMarkerHash, trackRectChanged);

            var isSelected = SelectionManager.Contains(marker);
            var showMarkers = true;

            QueueOverlay(treeViewRect, isSelected, !showMarkers);
            DrawMarker(treeViewRect, clip.GetTarget is Act.ActBulletEvent ? typeof(MarkerItem) : typeof(SignalEmitter), isSelected, !showMarkers, m_MarkerDrawOptions);

            if (Event.current.type == EventType.Repaint && showMarkers && !parent.locked)
                state.spacePartitioner.AddBounds(this, rect);
        }

        public void QueueOverlay(Rect rect, bool isSelected, bool isCollapsed)
        {
            //if (Event.current.type == EventType.Repaint && m_Editor.supportsDrawOverlay)
            //{
            //    rect = GUIClip.Unclip(rect);
            //    //TimelineWindow.instance.AddUserOverlay(marker, rect, m_Editor, isCollapsed, isSelected);
            //}
            if (Event.current.type == EventType.Repaint)
            {
                rect = GUIClip.Unclip(rect);
                //TimelineWindow.instance.AddUserOverlay(marker, rect, m_Editor, isCollapsed, isSelected);
            }
        }

        public override void StartDrag()
        {
            if (onStartDrag != null)
                onStartDrag.Invoke();
        }

        void CalculateClipRectangle(Rect trackRect, WindowState state, int projectedClipHash, bool trackRectChanged)
        {
            //if (m_ProjectedClipHash == projectedClipHash && !trackRectChanged)
            //    return;

            m_ProjectedClipHash = projectedClipHash;
            treeViewRect = RectToTimeline(trackRect, state);
        }

        public override Rect RectToTimeline(Rect trackRect, WindowState state)
        {
            var style = StyleManager.UssStyleForType(typeof(SignalEmitter));
            var width = style.fixedWidth;
            var height = style.fixedHeight;
            var x = ((float)time * state.timeAreaScale.x) + state.timeAreaTranslation.x + trackRect.xMin;
            x -= 0.5f * width;
            return new Rect(x, trackRect.y, width, height);
        }

        public IEnumerable<Edge> SnappableEdgesFor(IAttractable attractable, ManipulateEdges manipulateEdges)
        {
            var edges = new List<Edge>();
            var attractableGUI = attractable as TimelineMarkerGUI;
            var canAddEdges = !(attractableGUI != null && attractableGUI.parent == parent);
            if (canAddEdges)
                edges.Add(new Edge(time));
            return edges;
        }

        public bool ShouldSnapTo(ISnappable snappable)
        {
            return snappable != this;
        }
    }
}
