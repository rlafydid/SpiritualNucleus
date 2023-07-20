using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Act
{
    [ActCustomEditor(typeof(ActBaseClip))]
    public class ActBaseClipEditor<T> : ActEditor where T : ActBaseClip
    {
        protected bool customDuration = false;
        protected T targetNode { get => target as T; }

        public override void OnInspectorGUI()
        {
            DrawTriggerTime();
            DrawDuration();
        }

        protected void DrawTriggerTime()
        {
            targetNode.TriggerTime = EditorGUILayout.FloatField("触发时间", targetNode.TriggerTime);
        }
        protected void DrawDuration(bool disabled = false)
        {
            using (new EditorGUI.DisabledScope(!customDuration && disabled))
            {
                targetNode.Duration = EditorGUILayout.FloatField("持续时间", targetNode.Duration);
            }
        }

        protected void DrawCustomToggle()
        {
            customDuration = EditorGUILayout.Toggle("自定义持续时间", customDuration);
        }
    }
}
