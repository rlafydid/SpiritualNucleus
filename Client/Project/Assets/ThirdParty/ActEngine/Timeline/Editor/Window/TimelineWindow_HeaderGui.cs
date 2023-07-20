using System.Linq;
using UnityEngine;
using UnityEngine.ActTimeline;

namespace UnityEditor.ActTimeline
{
    partial class TimelineWindow
    {
        static readonly GUIContent[] k_TimeReferenceGUIContents =
        {
            L10n.TextContent("Local", "Display time based on the current timeline."),
            L10n.TextContent("Global", "Display time based on the master timeline.")
        };

        void DrawTransportToolbar()
        {
            using (new EditorGUI.DisabledScope(false))
            {
                PreviewModeButtonGUI();
                ReloadButtonGUI();
            }

            using (new EditorGUI.DisabledScope(false))
            {
                GotoBeginingSequenceGUI();
                PreviousEventButtonGUI();
                PlayButtonGUI();
                NextEventButtonGUI();
                GotoEndSequenceGUI();
                PlayRangeButtonGUI();
                TimeCodeGUI();
                ReferenceTimeGUI();
            }
        }

        void ReloadButtonGUI()
        {
            if (GUILayout.Button("刷新"))
            {
                Reload();
            }
        }

        void DrawLogicFrameCount()
        {
            //using (new EditorGUI.DisabledScope(false))
            //{
            //    GUILayout.Label($"事件帧率: {Act.ActUtility.EventFPS}");
            //}
        }

        void DrawLifeTime()
        {
            GUILayout.Label($"生命: ");
            EditorGUI.BeginChangeCheck();
            actData.LifeTime = EditorGUILayout.FloatField(actData.LifeTime, GUILayout.Width(30));
            if (EditorGUI.EndChangeCheck())
            {
                actData.CheckLifeTime();
            }
            //EditorGUILayout.Label("生命周期: ", GUILayout.Width(200));
        }

        Object model;
        void DrawModel()
        {
            GUILayout.Label("绑定角色:");
            EditorGUI.BeginChangeCheck();
            model = EditorGUILayout.ObjectField(model, typeof(GameObject), false, GUILayout.Width(100));
            if (EditorGUI.EndChangeCheck())
            {
                actData.Model = model.name;
            }
            //EditorGUILayout.Label("生命周期: ", GUILayout.Width(200));
        }

        void Reload()
        {
            Close();
            StartDebugger();
            LoadModel();
            RefreshMaxTime();
        }

        void LoadModel()
        {
            if (model == null || (model != null && model.name != actData.Model))
                model = Act.ActUtility.GetAsset<GameObject>(actData.Model);
        }

        void Close()
        {
            state.editSequence.time = 0;
            //Init(actData);
            //SetCurrentTimeline(actData);
            Act.ActTimelineDebugger.Destroy();
        }

        void PreviewModeButtonGUI()
        {
            //if (state.ignorePreview && !Application.isPlaying)
            //{
            //    GUILayout.Label(DirectorStyles.previewDisabledContent, DirectorStyles.Instance.previewButtonDisabled);
            //    return;
            //}
            if (!Application.isPlaying)
            {
                GUILayout.Label(DirectorStyles.previewDisabledContent, DirectorStyles.Instance.previewButtonDisabled);
                return;
            }

            EditorGUI.BeginChangeCheck();
            var enabled = state.previewMode;
            enabled = GUILayout.Toggle(enabled, DirectorStyles.previewContent, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                // turn off auto play as well, so it doesn't auto reenable
                if (!enabled)
                {
                    state.SetPlaying(false);
                    state.recording = false;
                    Close();
                }
                else
                {
                    Reload();
                }

                state.previewMode = enabled;

                // if we are successfully enabled, rebuild the graph so initial states work correctly
                // Note: testing both values because previewMode setter can "fail"
                if (enabled && state.previewMode)
                    state.rebuildGraph = true;
            }
        }

        void GotoBeginingSequenceGUI()
        {
            if (GUILayout.Button(DirectorStyles.gotoBeginingContent, EditorStyles.toolbarButton))
            {
                state.editSequence.time = 0;
                state.EnsurePlayHeadIsVisible();
            }
        }

        // in the editor the play button starts/stops simulation
        void PlayButtonGUIEditor()
        {
            EditorGUI.BeginChangeCheck();
            var isPlaying = GUILayout.Toggle(state.playing, DirectorStyles.playContent, EditorStyles.toolbarButton);
            if (EditorGUI.EndChangeCheck())
            {
                state.SetPlaying(isPlaying);
            }
        }

        // in playmode the button reflects the playing state.
        //  needs to disabled if playing is not possible
        void PlayButtonGUIPlayMode()
        {
            bool buttonEnabled = true;
            using (new EditorGUI.DisabledScope(!buttonEnabled))
            {
                PlayButtonGUIEditor();
            }
        }

        void PlayButtonGUI()
        {
            if (!Application.isPlaying)
                PlayButtonGUIEditor();
            else
                PlayButtonGUIPlayMode();
        }

        void NextEventButtonGUI()
        {
            if (GUILayout.Button(DirectorStyles.nextFrameContent, EditorStyles.toolbarButton))
            {
                state.referenceSequence.frame += 1;
            }
        }

        void PreviousEventButtonGUI()
        {
            if (GUILayout.Button(DirectorStyles.previousFrameContent, EditorStyles.toolbarButton))
            {
                state.referenceSequence.frame -= 1;
            }
        }

        void GotoEndSequenceGUI()
        {
            if (GUILayout.Button(DirectorStyles.gotoEndContent, EditorStyles.toolbarButton))
            {
                state.editSequence.time = state.maxTime;
                state.EnsurePlayHeadIsVisible();
            }
        }

        void PlayRangeButtonGUI()
        {
            using (new EditorGUI.DisabledScope(state.ignorePreview || state.IsEditingASubTimeline()))
            {
                state.playRangeEnabled = GUILayout.Toggle(state.playRangeEnabled, DirectorStyles.Instance.playrangeContent, EditorStyles.toolbarButton);
            }
        }

        // Draws the box to enter the time field
        void TimeCodeGUI()
        {
            const string timeFieldHint = "TimelineWindow-TimeCodeGUI";

            EditorGUI.BeginChangeCheck();
            var currentTime = state.IsReadyAsset ? TimeReferenceUtility.ToTimeString(state.editSequence.time, "0.####") : "0";
            var r = EditorGUILayout.GetControlRect(false, EditorGUI.kSingleLineHeight, EditorStyles.toolbarTextField, GUILayout.Width(WindowConstants.timeCodeWidth));
            var id = GUIUtility.GetControlID(timeFieldHint.GetHashCode(), FocusType.Keyboard, r);
            var newCurrentTime = EditorGUI.DelayedTextFieldInternal(r, id, GUIContent.none, currentTime, null, EditorStyles.toolbarTextField);

            if (EditorGUI.EndChangeCheck())
                state.editSequence.time = TimeReferenceUtility.FromTimeString(newCurrentTime);
        }

        void ReferenceTimeGUI()
        {
            if (!state.IsEditingASubTimeline())
                return;

            EditorGUI.BeginChangeCheck();
            state.timeReferenceMode = (TimeReferenceMode)EditorGUILayout.CycleButton((int)state.timeReferenceMode, k_TimeReferenceGUIContents, DirectorStyles.Instance.timeReferenceButton);
            if (EditorGUI.EndChangeCheck())
                OnTimeReferenceModeChanged();
        }

        void OnTimeReferenceModeChanged()
        {
            m_TimeAreaDirty = true;
            InitTimeAreaFrameRate();
            SyncTimeAreaShownRange();

            foreach (var inspector in InspectorWindow.GetAllInspectorWindows())
            {
                inspector.Repaint();
            }
        }
    }
}
