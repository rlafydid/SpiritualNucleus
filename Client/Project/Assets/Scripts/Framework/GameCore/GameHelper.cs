using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameHelper : MonoBehaviour {
	private static GameHelper _instance;
	public static GameHelper Instance
	{
			get {
					return _instance;
			}
	}

	void Awake()
	{
			_instance = this;
	}

	public static GameObject propPanel
	{
		get{
			return GameObject.Find ("Prop").gameObject;
		}
	}

	public static GameObject canvasPanel
	{
		get{
			return GameObject.Find ("Canvas").gameObject;
		}
	}

	public static Vector2 StartPosition
	{
		get{
			return new Vector2(-23f, 23f);
		}
	}

	
}
