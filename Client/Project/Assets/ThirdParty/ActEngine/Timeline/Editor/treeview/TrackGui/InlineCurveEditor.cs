using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    interface IClipCurveEditorOwner
    {
        ClipCurveEditor clipCurveEditor { get; }
        bool inlineCurvesSelected { get; }
        bool showLoops { get; }
        TrackAsset owner { get; }
        void SelectCurves();
        void ValidateCurvesSelection();
    }

    class InlineCurveResizeHandle : IBounds
    {
        public Rect boundingRect { get; private set; }

        public TimelineTrackGUI trackGUI { get; }

        public InlineCurveResizeHandle(TimelineTrackGUI trackGUI)
        {
            this.trackGUI = trackGUI;
        }

        public void Draw(Rect headerRect, WindowState state)
        {
            const float resizeHandleHeight = WindowConstants.trackResizeHandleHeight;
            var rect = new Rect(headerRect.xMin, headerRect.yMax - resizeHandleHeight + 1f, headerRect.width, resizeHandleHeight);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SplitResizeUpDown);

            boundingRect = trackGUI.ToWindowSpace(rect);

            if (Event.current.type == EventType.Repaint)
            {
                state.headerSpacePartitioner.AddBounds(this);
                EditorGUI.DrawRect(rect, DirectorStyles.Instance.customSkin.colorAnimEditorBinding);
                var dragStyle = DirectorStyles.Instance.inlineCurveHandle;
                dragStyle.Draw(rect, GUIContent.none, false, false, false, false);
            }
        }
    }

    class InlineCurveEditor : IBounds
    {
        Rect m_TrackRect;
        Rect m_HeaderRect;
        readonly TimelineTrackGUI m_TrackGUI;
        readonly InlineCurveResizeHandle m_ResizeHandle;

        bool m_LastSelectionWasClip;
        TimelineClipGUI m_LastSelectedClipGUI;

        Rect IBounds.boundingRect { get { return m_TrackGUI.ToWindowSpace(m_TrackRect); } }

        [UsedImplicitly] // Used in tests
        public TimelineClipGUI currentClipGui
        {
            get { return m_LastSelectedClipGUI; }
        }

        public IClipCurveEditorOwner currentCurveEditor
        {
            get { return m_LastSelectionWasClip ? (IClipCurveEditorOwner)m_LastSelectedClipGUI : (IClipCurveEditorOwner)m_TrackGUI; }
        }

        public InlineCurveEditor(TimelineTrackGUI trackGUI)
        {
            m_TrackGUI = trackGUI;
            m_ResizeHandle = new InlineCurveResizeHandle(trackGUI);
        }

        static bool MouseOverTrackArea(Rect curveRect, Rect trackRect)
        {
            curveRect.y = trackRect.y;
            curveRect.height = trackRect.height;

            // clamp the curve editor to the track. this allows the menu to scroll properly
            curveRect.xMin = Mathf.Max(curveRect.xMin, trackRect.xMin);
            curveRect.xMax = trackRect.xMax;

            return curveRect.Contains(Event.current.mousePosition);
        }

        static bool MouseOverHeaderArea(Rect headerRect, Rect trackRect)
        {
            headerRect.y = trackRect.y;
            headerRect.height = trackRect.height;

            return headerRect.Contains(Event.current.mousePosition);
        }

        static void OnMouseClick(IClipCurveEditorOwner clipCurveEditorOwner, Event currentEvent)
        {
            if (currentEvent.modifiers == ManipulatorsUtils.actionModifier)
            {
                if (clipCurveEditorOwner.inlineCurvesSelected)
                    SelectionManager.Clear();
                else
                    clipCurveEditorOwner.SelectCurves();
            }
            else
            {
                clipCurveEditorOwner.SelectCurves();
            }

            HandleCurrentEvent();
        }

        void UpdateViewModel()
        {
            var curveEditor = currentCurveEditor.clipCurveEditor;
            if (curveEditor == null || curveEditor.bindingHierarchy.treeViewController == null)
                return;

            var vm = TimelineWindowViewPrefs.GetTrackViewModelData(m_TrackGUI.track);
            vm.inlineCurvesState = curveEditor.bindingHierarchy.treeViewController.state;
            vm.inlineCurvesShownAreaInsideMargins = curveEditor.shownAreaInsideMargins;
            vm.lastInlineCurveDataID = curveEditor.dataSource.id;
        }

        static void HandleCurrentEvent()
        {
#if UNITY_EDITOR_OSX
            Event.current.type = EventType.Ignore;
#else
            Event.current.Use();
#endif
        }

        static void SelectFromCurveOwner(IClipCurveEditorOwner curveOwner)
        {
            if (curveOwner.clipCurveEditor == null)
            {
                SelectionManager.SelectInlineCurveEditor(null);
            }
            else if (!curveOwner.inlineCurvesSelected && SelectionManager.Count() == 1)
            {
                SelectionManager.SelectInlineCurveEditor(curveOwner);
            }
        }
    }
}
