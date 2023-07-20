using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public enum DirectionType
{
		Top,
		Bottom,
		Right,
		Left
}
public abstract class BaseUIPanel : MonoBehaviour {
	[HideInInspector]
	public string path;
	protected EventManager eventManager = EventManager.Instance;
	
	public float animation_Time = 0.5f;
	public GameObject animation_Top;
	public GameObject animation_Bottom;
	public GameObject animation_Left;
	public GameObject animation_Right;
	
	private struct UIAnimationStruct
	{
				public Vector3 fromPos;
				public Vector3 targetPos;
				public GameObject obj;
	}
		private Dictionary<DirectionType, UIAnimationStruct> animObjDict = new Dictionary<DirectionType, UIAnimationStruct> ();

	private IDictionary<string, Action<object>> eventDict = new Dictionary<string, Action<object>>();
	private IDictionary<string, Action> noParamEventDict = new Dictionary<string, Action>();
	
	[HideInInspector]
	public GameObject panelObj;


	public abstract void OnInit();
	public abstract void OnEnter(object obj = null);
	
		public void OnInitUI()
	{
				InitUIAnimation ();
				OnInit ();
	}

		public void OnEnterUI(object obj = null)
	{
				PlayBeginUIAnimation ();
				OnEnter (obj);
	}

	private void InitUIAnimation()
	{
				if (animation_Top != null) {
						UIAnimationStruct anim = new UIAnimationStruct ();
						anim.obj = animation_Top;
						animObjDict.Add (DirectionType.Top, anim);
				}
				if (animation_Bottom != null) {
						UIAnimationStruct anim = new UIAnimationStruct ();
						anim.obj = animation_Bottom;
						animObjDict.Add (DirectionType.Bottom, anim);
				}
				if (animation_Left != null) {
						UIAnimationStruct anim = new UIAnimationStruct ();
						anim.obj = animation_Left;
						animObjDict.Add (DirectionType.Left, anim);
				}
				if (animation_Right != null) {
						UIAnimationStruct anim = new UIAnimationStruct ();
						anim.obj = animation_Right;
						animObjDict.Add (DirectionType.Right, anim);
				}

				if (animObjDict.Count == 0)
						return;

				//List<DirectionType> keys = new List<DirectionType> (animObjDict.Keys);
				List<DirectionType> keys = new List<DirectionType> (animObjDict.Keys);
				int keyCount = keys.Count;
				for (int i = 0; i < keyCount; i++) {
						DirectionType key = keys [i];
						UIAnimationStruct animInfo = animObjDict [key];
						animInfo.fromPos = animInfo.obj.GetComponent<RectTransform> ().anchoredPosition;
						Vector3 targetPos = Vector3.zero;
						switch(key)
						{
						case DirectionType.Top:
								targetPos = animInfo.fromPos + new Vector3 (0, 1000f, 0);
								break;
						case DirectionType.Bottom:
								targetPos = animInfo.fromPos + new Vector3 (0, -1000f, 0);
								break;
						case DirectionType.Right:
								targetPos = animInfo.fromPos + new Vector3 (1000, 0, 0);
								break;
						case DirectionType.Left:
								targetPos = animInfo.fromPos + new Vector3 (-1000, 0, 0);
								break;
						}
						animInfo.targetPos = targetPos;

						animObjDict [key] = animInfo;
				}
				/*
				foreach(var item in animObjDict)
				{
						UIAnimationStruct animInfo = item.Value;
						animInfo.fromPos = animInfo.obj.GetComponent<RectTransform> ().anchoredPosition;
						Vector3 targetPos = Vector3.zero;
						switch(item.Key)
						{
						case DirectionType.Top:
								targetPos = animInfo.fromPos + new Vector3 (0, 1000f, 0);
								break;
						case DirectionType.Bottom:
								targetPos = animInfo.fromPos + new Vector3 (0, -1000f, 0);
								break;
						case DirectionType.Right:
								targetPos = animInfo.fromPos + new Vector3 (1000, 0, 0);
								break;
						case DirectionType.Left:
								targetPos = animInfo.fromPos + new Vector3 (-1000, 0, 0);
								break;
						}
						animInfo.targetPos = targetPos;

						//animObjDict [item.Key] = animInfo;
				}
				*/
	}

	private void PlayBeginUIAnimation()
	{
		//foreach(KeyValuePair<DirectionType, UIAnimationStruct> item in animObjDict)
		//{
		//		UIAnimationStruct animInfo = item.Value;
		//		RectTransform rectTrans = animInfo.obj.GetComponent<RectTransform> ();
		//		rectTrans.anchoredPosition = animInfo.targetPos;
		//				rectTrans.DOAnchorPos3D(animInfo.fromPos, animation_Time);
		//}
	}
	private void PlayEndUIAnimation()
	{
		//foreach(KeyValuePair<DirectionType, UIAnimationStruct> item in animObjDict)
		//{
		//		UIAnimationStruct animInfo = item.Value;
		//		RectTransform rectTrans = animInfo.obj.GetComponent<RectTransform> ();
		//		rectTrans.anchoredPosition = animInfo.fromPos;
		//				rectTrans.DOAnchorPos3D (animInfo.targetPos, animation_Time);
		//}
		
	}

	IEnumerator WaitPlayTime(float waitTime, Action action)
	{
			yield return new WaitForSeconds (waitTime);
			action ();
	}
	
	private void PlayEndCallBack()
	{
				UIManager.Instance.CloseUI (path);
	}

	public void OnRelease()
	{ 
		OnRemoveAllEvent ();
		OnExit ();
	}

	public virtual void OnExit()
	{
		
	}

	public virtual void OnUpdate()
	{

	}
	
	public void Close()
	{
		if (animObjDict.Count > 0) {
				PlayEndUIAnimation ();
				StartCoroutine (WaitPlayTime (animation_Time, PlayEndCallBack));
		} else {
				PlayEndCallBack ();
		}
	}

	public void AddEventListener(string key, Action<object> call)
	{
		if (eventDict.ContainsKey (key))
				return;
		
		eventManager.AddEventListener (key, call);
		eventDict.Add (key, call);
	}

	public void AddEventListener(string key, Action call)
	{
		if (noParamEventDict.ContainsKey (key))
				return;

		eventManager.AddEventListener (key, call);
		noParamEventDict.Add (key, call);
	}

	public void AddClickListener(Button btn, UnityAction clickFunc)
	{
		if (btn == null)
			return;
				
		btn.onClick.AddListener (clickFunc);
	}

	private void OnRemoveAllEvent()
	{
		foreach (KeyValuePair<string, Action<object>> item in eventDict) {
			eventManager.RemoveEventListener (item.Key, item.Value);
		}
		foreach (KeyValuePair<string, Action> item in noParamEventDict) {
			eventManager.RemoveEventListener (item.Key, item.Value);
		}
		eventDict.Clear ();
		noParamEventDict.Clear ();
	}
}
