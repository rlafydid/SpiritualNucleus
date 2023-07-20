using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActGizmosViewer : MonoBehaviour {

        static ActGizmosViewer _instance;
        public static ActGizmosViewer Instance {
                get {
                        _instance = GameObject.FindObjectOfType<ActGizmosViewer>();
                        if (_instance == null) {
                                GameObject gizmosObj = new GameObject("GizmosView");

                                _instance = gizmosObj.AddComponent<ActGizmosViewer>();
                        }
                        return _instance;
                }
        }


        Action gizmosEvents;

        public void Add(Action action) {
                gizmosEvents += action;

        }

        public void Remove(Action action) {
                gizmosEvents -= action;
        }

        public void Clear() {
                gizmosEvents = null;
        }

        private void OnDrawGizmos() {
                gizmosEvents?.Invoke();
        }
}


