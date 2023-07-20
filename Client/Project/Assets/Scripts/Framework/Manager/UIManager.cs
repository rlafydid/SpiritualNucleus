using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : SingletonTamplate<UIManager> {

	private const string panelPath = "Prefabs/UI/";
	
	private List<BaseUIPanel> uiPanels = new List<BaseUIPanel> ();

		public void OpenUI(string path, object param = null)
	{
		GameObject panel = GameObject.Instantiate(Resources.Load(panelPath + path, typeof(GameObject))) as GameObject;
		Transform panelTrans = panel.transform;
		panelTrans.SetParent (GameHelper.canvasPanel.transform);
		panelTrans.localScale = Vector3.one;
		panelTrans.localPosition = Vector3.zero;
		RectTransform rectTrans = panelTrans.GetComponent<RectTransform> ();
		rectTrans.anchorMax = Vector2.one; 
		rectTrans.anchorMin = Vector2.zero;
		rectTrans.offsetMin = Vector2.zero;
		rectTrans.offsetMax = Vector2.zero;
		
		BaseUIPanel baseUIPanel = panel.GetComponent<BaseUIPanel> ();
		baseUIPanel.panelObj = baseUIPanel.gameObject;
		baseUIPanel.path = path;
		baseUIPanel.OnInitUI ();
				baseUIPanel.OnEnterUI (param);
		uiPanels.Add (baseUIPanel);
	}
	

	public void CloseUI(string path)
	{
		int count = uiPanels.Count;
		for (int i = 0; i < count; i++) {
			BaseUIPanel panel = uiPanels [i];
			if (panel.path == path) {
				uiPanels.Remove (panel);
				panel.OnRelease ();
				GameObject.Destroy (panel.panelObj);
				panel.panelObj = null;
				break;
			}
		}
	}

	public void OnUpdate()
	{
		foreach (BaseUIPanel panel in uiPanels) {
			panel.OnUpdate ();
		}
	}



	public void CloseAll()
	{
		int count = uiPanels.Count;
		for (int i = 0; i < count; i++) {
			BaseUIPanel panel = uiPanels [i];
			if (panel == null || panel.panelObj == null) {
					continue;
			}
			panel.OnRelease ();
			GameObject.Destroy(panel.panelObj);
		}
		uiPanels.Clear ();
	}
}
