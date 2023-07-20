using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EventManager : SingletonTamplate<EventManager> {

		private IDictionary<string, System.Action<object>> eventHandleList = new Dictionary<string, System.Action<object>> ();
		private IDictionary<string, System.Action> noParamEventHandleList = new Dictionary<string, System.Action> ();

		//带参事件
		public void AddEventListener(string key, Action<object> callBack)
		{
				if (eventHandleList.ContainsKey (key)) {
						eventHandleList [key] += callBack;
				} else {
						eventHandleList.Add (key, callBack);
				}
		}

		//无参事件
		public void AddEventListener(string key, Action callBack)
		{
				if (noParamEventHandleList.ContainsKey (key)) {
						noParamEventHandleList [key] += callBack;
				} else {
						noParamEventHandleList.Add (key, callBack);
				}
		}

		//带参数事件
		public void RemoveEventListener(string key, Action<object> callBack)
		{
				if (eventHandleList.ContainsKey (key)) {
						eventHandleList [key] -= callBack;
						if (eventHandleList [key] == null) {
								eventHandleList.Remove (key);
						}
				}
		}

		//无参数事件
		public void RemoveEventListener(string key, Action callBack)
		{
				if (noParamEventHandleList.ContainsKey (key)) {
						noParamEventHandleList [key] -= callBack;
						if (noParamEventHandleList [key] == null) {
								noParamEventHandleList.Remove (key);
						}
				}
		}


		public void DispachEvent(string key, object data = null)
		{
			//带参
			if (eventHandleList.ContainsKey (key)) {
				eventHandleList [key] (data);
			}
			//无参
			if (noParamEventHandleList.ContainsKey (key)) {
				noParamEventHandleList [key] ();
			}
		}
}
