using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EffectManager : SingletonTamplate<EffectManager>{
	private static Transform effectPanel;
	public static Transform EffectPanel
	{
		get {
			if (effectPanel != null)
				return effectPanel;
			
			effectPanel =  new GameObject ("EffectPanel").transform;
			return effectPanel;
		}
	}

	private Dictionary<string, List<GameObject>> effectPoolsList = new Dictionary<string, List<GameObject>> ();

	public GameObject LoadEffect(string path)
	{
		if (effectPoolsList.ContainsKey(path)) {
			List<GameObject> effects = effectPoolsList [path];
			GameObject returnEffect = effects [0];
			returnEffect.SetActive (true);
			effects.RemoveAt (0);
			if (effects.Count == 0) {
				effectPoolsList.Remove (path);
			}
			return returnEffect;
		} else {
			GameObject effect = GameObject.Instantiate(Resources.Load (path) as GameObject);
			List<GameObject> effects = new List<GameObject>();
			effects.Add (effect);
			effectPoolsList [path] = effects;
			return effect;
		}
	}	

	public void UnLoadEffect(GameObject effect)
	{
		if (effect == null)
			return;
				
		effect.transform.SetParent (EffectPanel);
		effect.SetActive (false);
	}

	public void ClearEffect()
	{
		foreach(KeyValuePair<string, List<GameObject>> item in effectPoolsList)
		{
			int count = item.Value.Count;
			for (int i = 0; i < count; i++) {
				UnLoadEffect(item.Value[i]);
			}
		}
				effectPoolsList.Clear ();
	}

	public void InitEffect(GameObject effect)
	{
		Transform effectTrans = effect.transform;
		effectTrans.localScale = Vector3.one;
		effectTrans.localPosition = Vector3.zero;
		effectTrans.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
	}
}
