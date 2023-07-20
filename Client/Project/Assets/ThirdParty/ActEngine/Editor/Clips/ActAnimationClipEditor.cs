using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Act
{
    [ActCustomEditor(typeof(ActAnimationClip))]
    public class ActAnimationClipEditor : ActBaseClipEditor<ActAnimationClip>
    {
        GameObject model = null;

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
            EditorGUILayout.BeginHorizontal();
            targetNode.animName = EditorGUILayout.TextField("动画名字:", targetNode.animName);
            if (EditorGUILayout.DropdownButton(L10n.IconContent("icon dropdown", "Animation Selector"), FocusType.Passive, EditorStyles.FromUSS("sequenceSwitcher"), GUILayout.Width(23)))
            {
                ShowAnimationSelector();
            }
            EditorGUILayout.EndHorizontal();
            targetNode.dontStopOnDisable = EditorGUILayout.Toggle("保持结束姿势:", targetNode.dontStopOnDisable);
            //targetNode.speed = EditorGUILayout.FloatField("速度:", targetNode.speed);

            if (GUILayout.Button("自适应"))
            {
                if (getModel != null && !string.IsNullOrEmpty(targetNode.animName))
                {
                    targetNode.Duration = GetAnimationDuration(getModel, targetNode.animName);
                }
            }
        }

        void ShowAnimationSelector()
        {
            GameObject model = getModel;
            var sequenceMenu = new GenericMenu();
            if (model == null || model.GetComponentInChildren<SimpleAnimation>() == null)
            {
                sequenceMenu.AddDisabledItem(L10n.TextContent("没有绑定模型或没有添加SimpleAnimation"));
            }
            else
            {
                SimpleAnimation animaiton = model.GetComponentInChildren<SimpleAnimation>();

                var states = animaiton.GetEditorStates;

                if (states == null)
                    sequenceMenu.AddDisabledItem(L10n.TextContent("没有动画"));
                else
                {
                    foreach (var item in states)
                    {
                        sequenceMenu.AddItem(new GUIContent(item.name), item.name == targetNode.animName, Selected, item.name);
                    }
                }
            }

            sequenceMenu.ShowAsContext();
        }

        void Selected(object arg)
        {
            targetNode.animName = (string)arg;
            targetNode.Duration = GetAnimationDuration(getModel, targetNode.animName);
        }

        float GetAnimationDuration(GameObject obj, string stateName)
        {
            if (obj != null)
            {
                var anim = obj.GetComponentInChildren<SimpleAnimation>();
                if (anim != null)
                {
                    float duration = anim.GetClipDuration(stateName);
                    if (duration > 0)
                        return duration;
                }
            }
            Debug.LogError($"没有在模型{model}上找到{stateName}动画, 请再SimpleAnimation组件添加{stateName}对应的动画.");
            return -1;
        }
    }
}