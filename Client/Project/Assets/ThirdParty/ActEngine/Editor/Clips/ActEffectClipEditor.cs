using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Act
{
    [ActCustomEditor(typeof(ActEffectClip))]
    public class ActEffectClipEditor : ActBaseClipEditor<ActEffectClip>
    {
        bool isLoopParticle = false;
        public override void OnEnable()
        {
            base.OnEnable();
        }
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawTriggerTime();
            DrawDuration(!targetNode.isLoop && !isLoopParticle);

            bool isRefresh = false;
            EditorGUI.BeginChangeCheck();
            targetNode.assetPrefab = EditorGUILayout.ObjectField("特效资源:", targetNode.assetPrefab, typeof(GameObject), false) as GameObject;
            isRefresh = EditorGUI.EndChangeCheck();

            targetNode.generateMode = (EGeneratePoint)EditorGUILayout.EnumPopup("生成位置:", targetNode.generateMode);
            targetNode.direction = (EDirection)EditorGUILayout.EnumPopup("朝向:", targetNode.direction);
            targetNode.space = (ESpace)EditorGUILayout.EnumPopup("位于空间:", targetNode.space);
            if (targetNode.space == ESpace.World)
            {
                targetNode.actionMode = (EActionMode)EditorGUILayout.EnumPopup("行为模式:", targetNode.actionMode);
                if (targetNode.actionMode != EActionMode.None)
                    targetNode.followType = (EFollowType)EditorGUILayout.EnumPopup("跟随类型:", targetNode.followType);
            }

            targetNode.bindPoint = EditorGUILayout.TextField("绑点或骨骼:", targetNode.bindPoint);
            //targetNode.worldOffset = EditorGUILayout.Vector3Field("偏移:", targetNode.worldOffset);

            EditorGUI.BeginChangeCheck();
            targetNode.localOffset = EditorGUILayout.Vector3Field("偏移:", targetNode.localOffset);
            targetNode.eulerAngles = EditorGUILayout.Vector3Field("旋转:", targetNode.eulerAngles);
            targetNode.scale = EditorGUILayout.FloatField("缩放:", targetNode.scale);
            if (EditorGUI.EndChangeCheck())
            {
                targetNode.RefreshByInitialData();
            }
            // ""
            targetNode.speed = EditorGUILayout.FloatField("速度:", targetNode.speed);

            using (new EditorGUI.DisabledScope(isLoopParticle))
            {
                targetNode.isLoop = EditorGUILayout.Toggle("循环", targetNode.isLoop);
            }

            targetNode.dontDestroyWithAct = EditorGUILayout.Toggle(new GUIContent("不随着Act销毁", "播放完特效才销毁"), targetNode.dontDestroyWithAct);

            if (GUILayout.Button("刷新数据") || isRefresh)
            {
                Debug.Log("EndChangeCheck");
                //if (!string.IsNullOrEmpty(targetNode.assetName))
                //{
                SetDuration();
                //}
            }
        }

        void SetDuration()
        {
            float duration = ActEditorUtility.GetParticleDuration(targetNode.assetPrefab);
            isLoopParticle = false;
            if (duration == 0)
            {
                isLoopParticle = true;
                targetNode.isLoop = true;
            }
            else
            {
                targetNode.Duration = duration;
                targetNode.isLoop = false;
            }
        }
    }
}

