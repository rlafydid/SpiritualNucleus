using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Facade
{
	public class Asset
    {
		public static T Load<T>(string resName) where T : UnityEngine.Object
		{
			return ResourceManager.Instance.Load<T>(resName);
		}

		public static T Instantiate<T>(string resName) where T : UnityEngine.Object
		{
			return ResourceManager.Instance.Instantiate<T>(resName);
		}
	}
}

public class ResourceManager : BaseModule {
	public static ResourceManager Instance;

	Dictionary<int, string> instanceMapping = new Dictionary<int, string>();

    public override void Init()
    {
        base.Init();
		Instance = this;
    }

    public override void Start()
    {
        base.Start();
		AssetUtil.InitializeAssetTableInEditor();
	}

	public T Load<T>(string name) where T : UnityEngine.Object
		{
		string path = AssetUtil.GetPath(name);

		return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
	}

	public T Instantiate<T>(string name) where T : UnityEngine.Object
    {
		try
        {
			T res = GameObject.Instantiate<T>(Load<T>(name));
			instanceMapping.Add(res.GetInstanceID(), name);
			return res;
        }catch(Exception e)
        {
			Debug.LogError($"实例化{name}资源错误 {e.ToString()}");
			return null;
        }
	}

	public void ReleaseInstantiate(UnityEngine.Object asset)
    {
		if(instanceMapping.TryGetValue(asset.GetInstanceID(), out string resName))
        {
			GameObject.Destroy(asset);
        }
    }
}

