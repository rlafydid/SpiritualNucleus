using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Act;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float interval = 1;

    public int column = 8;
    public float spacingX = 2;
    public float spacingZ = 2;
    
    List<ActControlParticlePlayable> particleList;

    // Start is called before the first frame update
    void Awake()
    {
        particleList = new List<ActControlParticlePlayable>();

        List<ParticleSystem> particles = new List<ParticleSystem>(ParticleComponnet.GetControllableParticleSystems(gameObject));
        foreach (var item in particles)
        {
            var control = new ActControlParticlePlayable();
            control.Initialize(item);
            particleList.Add(control);
        }
        for (int i = 0; i < particleList.Count; i++)
        {
            ActControlParticlePlayable particle = particleList[i];
            ParticleSystem.MainModule mainModule = particle.particleSystem.main;
            mainModule.loop = true;
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < transform.childCount ; i++)
        {
            int row = i / column;
            
            var trans = transform.GetChild(i);
            var x = spacingX * (i % column);
            var z = spacingZ * row;
            trans.localPosition = new Vector3(x, 0, z);
        }
    }

    [MenuItem("Tools/加载所有Avatar特效")]
    public static void GenerateAssetTable()
    {
        var test = GameObject.FindObjectOfType<Test>();
        if (test == null)
            return;
        string[] allAssets = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/ArtRes/Effects/Avatars" });
        Debug.Log($"加载{allAssets.Length}个特效");
        foreach (var asset in allAssets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(asset);
            var obj = GameObject.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(assetPath));
            obj.transform.parent = test.transform;
            obj.transform.localPosition = Vector3.zero;
        }
        AssetDatabase.Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
