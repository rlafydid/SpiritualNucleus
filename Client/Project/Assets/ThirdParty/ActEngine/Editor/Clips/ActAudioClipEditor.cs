using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Act
{
    [ActCustomEditor(typeof(ActAudioClip))]
    public class ActAudioClipEditor : ActBaseClipEditor<ActAudioClip>
    {
        public override void OnEnable()
        {
            base.OnEnable();
        }
        public override void OnInspectorGUI()
        {
            DrawTriggerTime();
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.FloatField("持续时间", targetNode.Duration);
            }
            //DrawCustomToggle();
            //if (Facade.Externals.GetAudioTime != null && !string.IsNullOrEmpty(targetNode.assetId))
            //{
            //    if (Application.isPlaying)
            //        targetNode.Duration = Facade.Externals.GetAudioTime(targetNode.assetId);
            //}
            targetNode.configId = EditorGUILayout.TextField("表Id:", targetNode.configId);
            EditorGUI.BeginChangeCheck();
            targetNode.cueSheet = EditorGUILayout.TextField("音效表Sheet:", targetNode.cueSheet);
            targetNode.cueName = EditorGUILayout.TextField("音效名Name:", targetNode.cueName);

            if (EditorGUI.EndChangeCheck())
                RefreshDuration();

            if (GUILayout.Button("自适应"))
            {
                RefreshDuration();
            }
        }

        void RefreshDuration()
        {
            if (!string.IsNullOrEmpty(targetNode.cueSheet) || !string.IsNullOrEmpty(targetNode.cueName))
            {
                if (Facade.Externals.GetAudioTime != null)
                    targetNode.Duration = Facade.Externals.GetAudioTime.Invoke(targetNode.cueSheet, targetNode.cueName);
            }
            else
            {
                targetNode.Duration = 0;
            }

        }
    }
}

