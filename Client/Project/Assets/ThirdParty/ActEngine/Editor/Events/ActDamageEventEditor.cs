using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Act
{
    //[ActCustomEditor(typeof(ActDamageEvent))]
    public class ActDamageEventEditor : ActBaseEventEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var actEvent = target as ActDamageEvent;
            //EditorGUILayout.ObjectField(actEvent.EventName, actEvent.GetType(), new GUIContent(""));
        }
    }
}
