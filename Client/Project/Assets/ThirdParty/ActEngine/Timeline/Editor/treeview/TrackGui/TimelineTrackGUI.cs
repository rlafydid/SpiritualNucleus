using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.ActTimeline;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace UnityEditor.ActTimeline
{
    class TimelineTrackGUI : TimelineGroupGUI, IClipCurveEditorOwner, IRowGUI
    {
        struct TrackDrawData
        {
            public bool m_AllowsRecording;
            public bool m_ShowTrackBindings;
            public bool m_HasBinding;
            public bool m_IsSubTrack;
            public PlayableBinding m_Binding;
            public Object m_TrackBinding;
            public Texture m_TrackIcon;
            public bool m_HasMarkers;
        }

        static class Styles
        {
            public static readonly GUIContent trackCurvesBtnOnTooltip = DirectorStyles.TrTextContent(string.Empty, "Hide curves view");
            public static readonly GUIContent trackCurvesBtnOffTooltip = DirectorStyles.TrTextContent(string.Empty, "Show curves view");
            public static readonly GUIContent trackMarkerBtnOnTooltip = DirectorStyles.TrTextContent(string.Empty, "Collapse Track Markers");
            public static readonly GUIContent trackMarkerBtnOffTooltip = DirectorStyles.TrTextContent(string.Empty, "Expand Track Markers");

            public static readonly GUIContent kActiveRecordButtonTooltip = DirectorStyles.TrTextContent(string.Empty, "End recording");
            public static readonly GUIContent kInactiveRecordButtonTooltip = DirectorStyles.TrTextContent(string.Empty, "Start recording");
            public static readonly GUIContent kIgnorePreviewRecordButtonTooltip = DirectorStyles.TrTextContent(string.Empty, "Recording is disabled: scene preview is ignored for this TimelineAsset");
            public static readonly GUIContent kDisabledRecordButtonTooltip = DirectorStyles.TrTextContent(string.Empty,
                "Recording is not permitted when Track Offsets are set to Auto. Track Offset settings can be changed in the track menu of the base track.");
            public static Texture2D kProblemIcon = DirectorStyles.GetBackgroundImage(DirectorStyles.Instance.warning);
        }

        static GUIContent s_ArmForRecordContentOn;
        static GUIContent s_ArmForRecordContentOff;
        static GUIContent s_ArmForRecordDisabled;

        readonly InfiniteTrackDrawer m_InfiniteTrackDrawer;
        readonly TrackEditor m_TrackEditor;
        readonly GUIContent m_DefaultTrackIcon;
        readonly TrackResizeHandle m_ResizeHandle;

        TrackDrawData m_TrackDrawData;
        TrackDrawOptions m_TrackDrawOptions;

        bool m_InlineCurvesSkipped;
        int m_TrackHash = -1;
        int m_BlendHash = -1;
        int m_LastDirtyIndex = -1;

        bool? m_TrackHasAnimatableParameters;
        int m_HeightExtension;

        public override bool expandable
        {
            get { return hasChildren; }
        }

        internal InlineCurveEditor inlineCurveEditor { get; set; }

        public ClipCurveEditor clipCurveEditor { get; private set; }

        public bool inlineCurvesSelected => SelectionManager.IsCurveEditorFocused(this);

        bool IClipCurveEditorOwner.showLoops
        {
            get { return false; }
        }

        TrackAsset IClipCurveEditorOwner.owner
        {
            get { return track; }
        }

        static bool DoesTrackAllowsRecording(TrackAsset track)
        {
            // if the root animation track is in auto mode, recording is not allowed
            var animTrack = TimelineUtility.GetSceneReferenceTrack(track) as AnimationTrack;
            if (animTrack != null)
                return animTrack.trackOffset != TrackOffset.Auto;

            return false;
        }

        bool trackHasAnimatableParameters
        {
            get
            {
                // cache this value to avoid the recomputation
                if (!m_TrackHasAnimatableParameters.HasValue)
                    m_TrackHasAnimatableParameters = track.HasAnyAnimatableParameters() ||
                        track.clips.Any(c => c.HasAnyAnimatableParameters());

                return m_TrackHasAnimatableParameters.Value;
            }
        }

        public bool locked
        {
            get { return track.lockedInHierarchy; }
        }

        public bool showMarkers
        {
            get { return track.GetShowMarkers(); }
        }

        public bool muted
        {
            get { return track.muted; }
        }

        public List<TimelineClipGUI> clips
        {
            get
            {
                return null;
            }
        }

        TrackAsset IRowGUI.asset { get { return track; } }

        public int heightExtension
        {
            get => m_HeightExtension;
            set => m_HeightExtension = Math.Max(0, value);
        }

        float minimumHeight => m_TrackDrawOptions.minimumHeight <= 0.0f ? TrackEditor.DefaultTrackHeight : m_TrackDrawOptions.minimumHeight;

        public TimelineTrackGUI(TreeViewController tv, TimelineTreeViewGUI w, int id, int depth, TreeViewItem parent, string displayName, TrackAsset sequenceActor)
            : base(tv, w, id, depth, parent, displayName, sequenceActor, false)
        {

            UpdateInfiniteClipEditor(w.TimelineWindow);

            var bindings = track.outputs.ToArray();
            m_TrackDrawData.m_HasBinding = bindings.Length > 0;
            if (m_TrackDrawData.m_HasBinding)
                m_TrackDrawData.m_Binding = bindings[0];
            m_TrackDrawData.m_IsSubTrack = IsSubTrack();
            m_TrackDrawData.m_AllowsRecording = DoesTrackAllowsRecording(sequenceActor);
            m_TrackDrawData.m_HasMarkers = track.GetMarkerCount() > 0;
            m_DefaultTrackIcon = TrackResourceCache.GetTrackIcon(track);

            m_TrackEditor = CustomTimelineEditorCache.GetTrackEditor(sequenceActor);
            m_TrackDrawOptions = m_TrackEditor.GetTrackOptions_Safe(track, null);

            m_TrackDrawOptions.errorText = null; // explicitly setting to null for an uninitialized state
            m_ResizeHandle = new TrackResizeHandle(this);
            heightExtension = TimelineWindowViewPrefs.GetTrackHeightExtension(track);

            RebuildGUICacheIfNecessary();
        }

        public override float GetVerticalSpacingBetweenTracks()
        {
            if (track != null && track.isSubTrack)
                return 1.0f; // subtracks have less of a gap than tracks
            return base.GetVerticalSpacingBetweenTracks();
        }

        void UpdateInfiniteClipEditor(TimelineWindow window)
        {
            if (clipCurveEditor != null || track == null || !ShouldShowInfiniteClipEditor())
                return;

            var dataSource = CurveDataSource.Create(this);
            clipCurveEditor = new ClipCurveEditor(dataSource, window, track);
        }

        void DetectTrackChanged()
        {
            if (Event.current.type == EventType.Layout)
            {
                // incremented when a track or it's clips changed
                if (m_LastDirtyIndex != track.DirtyIndex)
                {
                    m_TrackEditor.OnTrackChanged_Safe(track);
                    m_LastDirtyIndex = track.DirtyIndex;
                }
                OnTrackChanged();
            }
        }

        // Called when the source track data, including it's clips have changed has changed.
        void OnTrackChanged()
        {
            // recompute blends if necessary
            int newBlendHash = BlendHash();
            if (m_BlendHash != newBlendHash)
            {
                UpdateClipOverlaps();
                m_BlendHash = newBlendHash;
            }

            RebuildGUICacheIfNecessary();
        }

        void UpdateDrawData(WindowState state)
        {
            if (Event.current.type == EventType.Layout)
            {
                m_TrackDrawData.m_ShowTrackBindings = false;
                m_TrackDrawData.m_TrackBinding = null;

                if (state.editSequence.director != null && showSceneReference)
                {
                    m_TrackDrawData.m_ShowTrackBindings = state.GetWindow().currentMode.ShouldShowTrackBindings(state);
                    m_TrackDrawData.m_TrackBinding = state.editSequence.director.GetGenericBinding(track);
                }

                var lastHeight = m_TrackDrawOptions.minimumHeight;
                m_TrackDrawOptions = m_TrackEditor.GetTrackOptions_Safe(track, m_TrackDrawData.m_TrackBinding);

                m_TrackDrawData.m_HasMarkers = track.GetMarkerCount() > 0;
                m_TrackDrawData.m_AllowsRecording = DoesTrackAllowsRecording(track);
                m_TrackDrawData.m_TrackIcon = m_TrackDrawOptions.icon;
                if (m_TrackDrawData.m_TrackIcon == null)
                    m_TrackDrawData.m_TrackIcon = m_DefaultTrackIcon.image;

                // track height has changed. need to update gui
                if (!Mathf.Approximately(lastHeight, m_TrackDrawOptions.minimumHeight))
                    state.Refresh();
            }
        }

        public override void Draw(Rect headerRect, Rect contentRect, WindowState state)
        {
            DetectTrackChanged();
            UpdateDrawData(state);

            UpdateInfiniteClipEditor(state.GetWindow());

            var trackContentRect = contentRect;

            float inlineCurveHeight = contentRect.height - GetTrackContentHeight(state);
            bool hasInlineCurve = inlineCurveHeight > 0.0f;

            if (hasInlineCurve)
            {
                trackContentRect.height -= inlineCurveHeight;
            }

            if (Event.current.type == EventType.Repaint)
            {
                m_TreeViewRect = trackContentRect;
            }

            RebuildGUICacheIfNecessary();

            // Prevents from drawing outside of bounds, but does not effect layout or markers
            bool isOwnerDrawSucceed = false;

            Vector2 visibleTime = state.timeAreaShownRange;

            if (drawer != null)
                isOwnerDrawSucceed = drawer.DrawTrack(trackContentRect, track, visibleTime, state);

            if (!isOwnerDrawSucceed)
            {
                using (new GUIViewportScope(trackContentRect))
                    DrawBackground(trackContentRect, track, visibleTime, state);

                if (m_InfiniteTrackDrawer != null)
                    m_InfiniteTrackDrawer.DrawTrack(trackContentRect, track, visibleTime, state);

                // draw after user customization so overlay text shows up
                //using (new GUIViewportScope(trackContentRect))
                //    m_ItemsDrawer.Draw(trackContentRect, state);
            }

            EditorGUI.LabelField(trackContentRect, $"id={this.id} rect {trackContentRect.ToString()}");

            //DrawTrackHeader(trackHeaderRect, state); //探注释

            //DrawTrackColorKind(headerRect); //探注释
            //DrawTrackState(contentRect, contentRect, track);

            //EditorGUI.Button(headerRect, new GUIContent($"headerRect"));
            //EditorGUI.Button(contentRect, new GUIContent($"contentRect"));
        }

        void DrawBackground(Rect trackRect, TrackAsset trackAsset, Vector2 visibleTime, WindowState state)
        {
            bool canDrawRecordBackground = IsRecording(state);
            if (canDrawRecordBackground)
            {
                DrawRecordingTrackBackground(trackRect, trackAsset, visibleTime, state);
            }
            else
            {
                Color trackBackgroundColor;

                if (SelectionManager.Contains(track))
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
            }
        }

        public override float GetHeight(WindowState state)
        {
            var height = GetTrackContentHeight(state);

            return height;
        }

        float GetTrackContentHeight(WindowState state)
        {
            var defaultHeight = Mathf.Min(minimumHeight, TrackEditor.MaximumTrackHeight);
            return (defaultHeight + heightExtension) * state.trackScale;
        }

        static bool CanDrawIcon(GUIContent icon)
        {
            return icon != null && icon != GUIContent.none && icon.image != null;
        }

        bool showSceneReference
        {
            get
            {
                return track != null &&
                    m_TrackDrawData.m_HasBinding &&
                    !m_TrackDrawData.m_IsSubTrack &&
                    m_TrackDrawData.m_Binding.sourceObject != null &&
                    m_TrackDrawData.m_Binding.outputTargetType != null &&
                    typeof(Object).IsAssignableFrom(m_TrackDrawData.m_Binding.outputTargetType);
            }
        }

        static void ObjectBindingField(Rect position, Object obj, PlayableBinding binding, int controlId)
        {
            var allowScene =
                typeof(GameObject).IsAssignableFrom(binding.outputTargetType) ||
                typeof(Component).IsAssignableFrom(binding.outputTargetType);

            var bindingFieldRect = EditorGUI.IndentedRect(position);
            using (new GUIViewportScope(bindingFieldRect))
            {
                EditorGUI.BeginChangeCheck();
                var newObject = UnityEditorInternals.DoObjectField(EditorGUI.IndentedRect(position), obj, binding.outputTargetType, controlId, allowScene, true);
                if (EditorGUI.EndChangeCheck())
                    BindingUtility.BindWithInteractiveEditorValidation(TimelineEditor.inspectedDirector, binding.sourceObject as TrackAsset, newObject);
            }
        }

        bool IsRecording(WindowState state)
        {
            return state.recording && state.IsArmedForRecord(track);
        }

        // background to draw during recording
        void DrawRecordingTrackBackground(Rect trackRect, TrackAsset trackAsset, Vector2 visibleTime, WindowState state)
        {
            if (drawer != null)
                drawer.DrawRecordingBackground(trackRect, trackAsset, visibleTime, state);
        }

        void UpdateClipOverlaps()
        {
            TrackExtensions.ComputeBlendsFromOverlaps(track.clips);
        }

        internal void RebuildGUICacheIfNecessary()
        {
            if (m_TrackHash == track.Hash())
                return;

            //m_ItemsDrawer = new TrackItemsDrawer(this);
            m_TrackHash = track.Hash();
        }

        int BlendHash()
        {
            var hash = 0;
            foreach (var clip in track.clips)
            {
                hash = HashUtility.CombineHash(hash,
                    (clip.duration - clip.start).GetHashCode());
            }
            return hash;
        }

        // callback when the corresponding graph is rebuilt. This can happen, but not have the GUI rebuilt.
        public override void OnGraphRebuilt()
        {
            RefreshCurveEditor();
        }

        void RefreshCurveEditor()
        {
            var window = TimelineWindow.instance;
            if (track != null && window != null && window.state != null)
            {
                bool hasEditor = clipCurveEditor != null;
                bool shouldHaveEditor = ShouldShowInfiniteClipEditor();
                if (hasEditor != shouldHaveEditor)
                    window.state.AddEndFrameDelegate((x, currentEvent) =>
                    {
                        x.Refresh();
                        return true;
                    });
            }
        }

        bool ShouldShowInfiniteClipEditor()
        {
            var animationTrack = track as AnimationTrack;
            if (animationTrack != null)
                return animationTrack.ShouldShowInfiniteClipEditor();

            return trackHasAnimatableParameters;
        }

        public void SelectCurves()
        {
            SelectionManager.RemoveTimelineSelection();
            SelectionManager.SelectInlineCurveEditor(this);
        }

        public void ValidateCurvesSelection() { }
    }
}
