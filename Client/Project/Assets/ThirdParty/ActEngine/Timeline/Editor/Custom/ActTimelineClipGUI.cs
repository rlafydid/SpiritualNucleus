using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    class TimelineClipGUI : TimelineItemGUI, ISnappable, IAttractable
    {
        public Rect clippedRect { get; private set; }
        bool m_ClipViewDirty = true;
        Rect m_ClipCenterSection;

        Rect m_MixOutRect;
        Rect m_MixInRect;


        public override int row { get => clip.row; }

        public Rect mixOutRect
        {
            get
            {
                var percent = clip.mixOutPercentage;
                var x = Mathf.Round(treeViewRect.width * (1 - percent));
                var width = Mathf.Round(treeViewRect.width * percent);
                m_MixOutRect.Set(x, 0.0f, width, treeViewRect.height);
                return m_MixOutRect;
            }
        }
        public Rect mixInRect
        {
            get
            {
                var width = Mathf.Round(treeViewRect.width * clip.mixInPercentage);
                m_MixInRect.Set(0.0f, 0.0f, width, treeViewRect.height);
                return m_MixInRect;
            }
        }
        ClipDrawData m_ClipDrawData;

        public override ITimelineItem item
        {
            get { return ItemsUtils.ToItem(clip); }
        }

        public override double start => clip.start;

        public override double end => clip.end;
        static readonly IconData k_DiggableClipIcon = new IconData(DirectorStyles.LoadIcon("TimelineDigIn"));
        static readonly float k_MinMixWidth = 2;
        static readonly float k_MaxHandleWidth = 10f;
        static readonly float k_MinHandleWidth = 1f;

        public TimelineClipHandle actLeftHandle { get; }
        public TimelineClipHandle actRightHandle { get; }

        bool supportResize
        {
            get
            {
                return clip.supportResize;
            }
        }

        //ClipEditor m_ClipEditor;

        public TimelineClipGUI(TimelineClip clip) : base()
        {
            this._clip = clip;

            //m_ClipEditor = CustomTimelineEditorCache.GetClipEditor(clip);
            actLeftHandle = new TimelineClipHandle(this, TrimEdge.Start);
            actRightHandle = new TimelineClipHandle(this, TrimEdge.End);

            ItemToItemGui.Add(clip, this);
        }

        // Entry point to the Clip Drawing...
        public override void Draw(Rect trackRect, bool trackRectChanged, WindowState state)
        {
            //this.zOrder = new LayerZOrder(Layer.Clips, order);
            // update the clip projected rectangle on the timeline
            CalculateClipRectangle(trackRect, state);


            // 执行此方法才能选中,点击
            AddToSpacePartitioner(state);

            CalculateBlendRect();

            // update the loop rects (when clip loops)
            //CalculateLoopRects(trackRect, state);

            //DrawExtrapolation(trackRect, treeViewRect);

            DrawInto(treeViewRect, state);


            //DrawDebug.Draw(clippedRect, "TimelineClipGUI.clippedRect", Color.red);
        }

        void DrawInto(Rect drawRect, WindowState state)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            // create the inline curve editor if not already created
            //CreateInlineCurveEditor(state);

            // @todo optimization, most of the calculations (rect, offsets, colors, etc.) could be cached
            // and rebuilt when the hash of the clip changes.

            if (isInvalid)
            {
                DrawInvalidClip(treeViewRect);
                return;
            }

            GUI.BeginClip(drawRect);

            var originRect = new Rect(0.0f, 0.0f, drawRect.width, drawRect.height);
            string clipLabel = clip.displayName;
            var selected = IsSelected();

            //if (selected && 1.0 != clip.timeScale)
            //    clipLabel += " " + clip.timeScale.ToString("F2") + "x";

            UpdateDrawData(state, originRect, clipLabel, IsSelected(), false, drawRect.x);
            DrawClip(m_ClipDrawData);

            GUI.EndClip();

            if (selected && supportResize)
            {
                var cursorRect = rect;
                cursorRect.xMin += actLeftHandle.boundingRect.width;
                cursorRect.xMax -= actRightHandle.boundingRect.width;
                EditorGUIUtility.AddCursorRect(cursorRect, MouseCursor.MoveArrow);
            }

            if (supportResize)
            {
                var handleWidth = Mathf.Clamp(drawRect.width * 0.3f, k_MinHandleWidth, k_MaxHandleWidth);

                actLeftHandle.Draw(drawRect, handleWidth, state);
                actRightHandle.Draw(drawRect, handleWidth, state);
            }
        }
        public void DrawInvalidClip(Rect targetRect)
        {
            DrawSimpleClip(targetRect, ClipBorder.Selection(), DirectorStyles.Instance.customSkin.colorInvalidClipOverlay);
        }

        void DrawSimpleClip(Rect targetRect, ClipBorder border, Color overlay)
        {
            var drawOptions = UpdateClipDrawOptions(clip);
            ClipDrawer.DrawSimpleClip(clip, targetRect, border, overlay, drawOptions);
        }
        public void DrawGhostClip(Rect targetRect)
        {
            DrawSimpleClip(targetRect, ClipBorder.Selection(), new Color(1.0f, 1.0f, 1.0f, 0.5f));
        }
        void UpdateDrawData(WindowState state, Rect drawRect, string title, bool selected, bool previousClipSelected, float rectXOffset)
        {

            m_ClipDrawData.clip = clip;
            m_ClipDrawData.targetRect = drawRect;
            m_ClipDrawData.clipCenterSection = m_ClipCenterSection;
            m_ClipDrawData.unclippedRect = treeViewRect;
            m_ClipDrawData.title = title;
            m_ClipDrawData.selected = selected;
            m_ClipDrawData.previousClipSelected = previousClipSelected;

            Vector3 shownAreaTime = state.timeAreaShownRange;
            m_ClipDrawData.localVisibleStartTime = clip.ToLocalTimeUnbound(Math.Max(clip.start, shownAreaTime.x));
            m_ClipDrawData.localVisibleEndTime = clip.ToLocalTimeUnbound(Math.Min(clip.end, shownAreaTime.y));

            m_ClipDrawData.clippedRect = new Rect(clippedRect.x - rectXOffset, 0.0f, clippedRect.width, clippedRect.height);

            //m_ClipDrawData.minLoopIndex = minLoopIndex;
            //m_ClipDrawData.loopRects = m_LoopRects;
            //m_ClipDrawData.supportsLooping = supportsLooping;
            m_ClipDrawData.clipBlends = GetClipBlends();
            //m_ClipDrawData.clipEditor = m_ClipEditor;
            m_ClipDrawData.ClipDrawOptions = UpdateClipDrawOptions(clip);

            UpdateClipIcons(state);
        }
        public ClipBlends GetClipBlends()
        {
            var _mixInRect = mixInRect;
            var _mixOutRect = mixOutRect;

            var blendInKind = BlendKind.None;
            if (_mixInRect.width > k_MinMixWidth && clip.hasBlendIn)
                blendInKind = BlendKind.Mix;
            else if (_mixInRect.width > k_MinMixWidth)
                blendInKind = BlendKind.Ease;

            var blendOutKind = BlendKind.None;
            if (_mixOutRect.width > k_MinMixWidth && clip.hasBlendOut)
                blendOutKind = BlendKind.Mix;
            else if (_mixOutRect.width > k_MinMixWidth)
                blendOutKind = BlendKind.Ease;

            return new ClipBlends(blendInKind, _mixInRect, blendOutKind, _mixOutRect);
        }
        void UpdateClipIcons(WindowState state)
        {
            // Pass 1 - gather size
            int required = 0;
            //bool requiresDigIn = ShowDrillIcon(state.editSequence.director);
            //if (requiresDigIn)
            //    required++;

            var icons = m_ClipDrawData.ClipDrawOptions.icons;
            foreach (var icon in icons)
            {
                if (icon != null)
                    required++;
            }

            // Pass 2 - copy icon data
            if (required == 0)
            {
                m_ClipDrawData.rightIcons = null;
                return;
            }

            if (m_ClipDrawData.rightIcons == null || m_ClipDrawData.rightIcons.Length != required)
                m_ClipDrawData.rightIcons = new IconData[required];

            int index = 0;
            //if (requiresDigIn)
            //    m_ClipDrawData.rightIcons[index++] = k_DiggableClipIcon;

            foreach (var icon in icons)
            {
                if (icon != null)
                    m_ClipDrawData.rightIcons[index++] = new IconData(icon);
            }
        }

        ClipItem itemClip;
        public override void Select()
        {
            MoveToTop();
            //SelectionManager.Add(clip);
            //if (clipCurveEditor != null && SelectionManager.Count() == 1)
            //    SelectionManager.SelectInlineCurveEditor(this);
            SelectionManager.Add(clip);
        }

        void MoveToTop()
        {
        }

        public override bool IsSelected()
        {
            return SelectionManager.Contains(clip);
        }

        public override void Deselect()
        {
            Debug.Log("Deselect");
            SelectionManager.Remove(clip);
            //SelectionManager.Remove(clip);
            //if (inlineCurvesSelected)
            //    SelectionManager.SelectInlineCurveEditor(null);
        }

        public override bool CanSelect(Event evt)
        {
            ClipBlends clipBlends = GetClipBlends();
            Vector2 mousePos = evt.mousePosition - rect.position;
            return m_ClipCenterSection.Contains(mousePos) || IsPointLocatedInClipBlend(mousePos, clipBlends);
        }

        void AddToSpacePartitioner(WindowState state)
        {
            if (Event.current.type == EventType.Repaint)
            {
                state.spacePartitioner.AddBounds(this, rect);
            }
        }
        void CalculateClipRectangle(Rect trackRect, WindowState state)
        {
            if (m_ClipViewDirty)
            {
                var clipRect = RectToTimeline(trackRect, state);
                treeViewRect = clipRect;

                // calculate clipped rect
                clipRect.xMin = Mathf.Max(clipRect.xMin, trackRect.xMin);
                clipRect.xMax = Mathf.Min(clipRect.xMax, trackRect.xMax);

                if (clipRect.width > 0 && clipRect.width < 2)
                {
                    clipRect.width = 5.0f;
                }

                clippedRect = clipRect;
            }
        }
        void CalculateBlendRect()
        {
            m_ClipCenterSection = treeViewRect;
            m_ClipCenterSection.x = 0;
            m_ClipCenterSection.y = 0;

            m_ClipCenterSection.xMin = mixInRect.xMax;
            m_ClipCenterSection.width = Mathf.Round(treeViewRect.width - mixInRect.width - mixOutRect.width);
            m_ClipCenterSection.xMax = m_ClipCenterSection.xMin + m_ClipCenterSection.width;
        }

        static bool IsPointLocatedInClipBlend(Vector2 pt, ClipBlends blends)
        {
            if (blends.inRect.Contains(pt))
            {
                if (blends.inKind == BlendKind.Mix)
                    return Sign(pt, blends.inRect.min, blends.inRect.max) < 0;
                return true;
            }

            if (blends.outRect.Contains(pt))
            {
                if (blends.outKind == BlendKind.Mix)
                    return Sign(pt, blends.outRect.min, blends.outRect.max) >= 0;
                return true;
            }

            return false;
        }

        static float Sign(Vector2 point, Vector2 linePoint1, Vector2 linePoint2)
        {
            return (point.x - linePoint2.x) * (linePoint1.y - linePoint2.y) - (linePoint1.x - linePoint2.x) * (point.y - linePoint2.y);
        }

        public IEnumerable<Edge> SnappableEdgesFor(IAttractable attractable, ManipulateEdges manipulateEdges)
        {
            var edges = new List<Edge>();

            bool canAddEdges = !parent.muted;

            if (canAddEdges)
            {
                // Hack: Trim Start in Ripple mode should not have any snap point added
                if (EditMode.editType == EditMode.EditType.Ripple && manipulateEdges == ManipulateEdges.Left)
                    return edges;

                if (attractable != this)
                {
                    if (EditMode.editType == EditMode.EditType.Ripple)
                    {
                        bool skip = false;

                        // Hack: Since Trim End and Move in Ripple mode causes other snap point to move on the same track (which is not supported), disable snapping for this special cases...
                        // TODO Find a proper way to have different snap edges for each edit mode.
                        if (manipulateEdges == ManipulateEdges.Right)
                        {
                            var otherClipGUI = attractable as TimelineClipGUI;
                            skip = otherClipGUI != null && otherClipGUI.parent == parent;
                        }
                        else if (manipulateEdges == ManipulateEdges.Both)
                        {
                            var moveHandler = attractable as MoveItemHandler;
                            skip = moveHandler != null && moveHandler.movingItems.Any(clips => clips.targetTrack == clip.GetParentTrack() && clip.start >= clips.start);
                        }

                        if (skip)
                            return edges;
                    }

                    AddEdge(edges, clip.start);
                    AddEdge(edges, clip.end);
                }
                else
                {
                    if (manipulateEdges == ManipulateEdges.Right)
                    {
                        var d = clip.duration;

                        if (d < double.MaxValue)
                        {
                            //if (clip.SupportsLooping())
                            //{
                            //    var l = TimelineHelpers.GetLoopDuration(clip);

                            //    var shownTime = TimelineWindow.instance.state.timeAreaShownRange;
                            //    do
                            //    {
                            //        AddEdge(edges, d, false);
                            //        d += l;
                            //    }
                            //    while (d < shownTime.y);
                            //}
                            //else
                            //{
                            AddEdge(edges, d, false);
                            //}
                        }
                    }

                }
            }
            return edges;
        }
        static void AddEdge(List<Edge> edges, double time, bool showEdgeHint = true)
        {
            var shownTime = TimelineWindow.instance.state.timeAreaShownRange;
            if (time >= shownTime.x && time <= shownTime.y)
                edges.Add(new Edge(time, showEdgeHint));
        }
        public bool ShouldSnapTo(ISnappable snappable)
        {
            return true;
        }


        public override Rect RectToTimeline(Rect trackRect, WindowState state)
        {
            var offsetFromTimeSpaceToPixelSpace = state.timeAreaTranslation.x + trackRect.xMin; //

            var start = (float)(DiscreteTime)clip.start;
            var end = (float)(DiscreteTime)clip.end;

            return Rect.MinMaxRect(
                Mathf.Round(start * state.timeAreaScale.x + offsetFromTimeSpaceToPixelSpace), Mathf.Round(trackRect.yMin),
                Mathf.Round(end * state.timeAreaScale.x + offsetFromTimeSpaceToPixelSpace), Mathf.Round(trackRect.yMax)
            );
        }

        static void DrawClip(ClipDrawData drawData)
        {
            ClipDrawer.DrawDefaultClip(drawData);

            //if (drawData.clip.asset is AnimationPlayableAsset)
            //{
            //    var state = TimelineWindow.instance.state;
            //    if (state.recording && state.IsArmedForRecord(drawData.clip.GetParentTrack()))
            //    {
            //        ClipDrawer.DrawAnimationRecordBorder(drawData);
            //        ClipDrawer.DrawRecordProhibited(drawData);
            //    }
            //}
        }

        static ClipDrawOptions UpdateClipDrawOptions(TimelineClip clip)
        {
            try
            {
                //return CustomTimelineEditorCache.GetDefaultClipEditor().GetClipDefaultOptions();
                var options = new ClipDrawOptions()
                {
                    errorText = "",
                    tooltip = string.Empty,
                    icons = System.Linq.Enumerable.Empty<Texture2D>()
                };
                if (clip.GetClipType == ClipType.Clip)
                {
                    var actClip = clip.GetTarget as Act.ActBaseClip;
                    if (actClip.GetType() == typeof(Act.ActEffectClip))
                        options.highlightColor = DirectorStyles.Instance.customSkin.colorControl;
                    else if (actClip.GetType() == typeof(Act.ActAudioClip))
                        options.highlightColor = DirectorStyles.Instance.customSkin.colorAudio;
                    else if (actClip.GetType() == typeof(Act.ActAnimationClip))
                        options.highlightColor = DirectorStyles.Instance.customSkin.colorAnimation;
                    return options;
                }

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return CustomTimelineEditorCache.GetDefaultClipEditor().GetClipDefaultOptions();
        }

        //void DetectClipChanged(bool trackRectChanged)
        //{
        //    if (Event.current.type == EventType.Layout)
        //    {
        //        if (clip.DirtyIndex != m_LastDirtyIndex)
        //        {
        //            m_ClipViewDirty = true;

        //            try
        //            {
        //                m_ClipEditor.OnClipChanged(clip);
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.LogException(e);
        //            }

        //            m_LastDirtyIndex = clip.DirtyIndex;
        //        }
        //        m_ClipViewDirty |= trackRectChanged;
        //    }
        //}

    }
}

