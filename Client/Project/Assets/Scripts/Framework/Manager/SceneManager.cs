using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace LKEngine
{
    public enum SceneType
    {
        None,
        Main,
        Battle
    }
    public class BaseScene
    {
        public SceneType sceneType;
        public void Enter()
        {
            OnEnter();
        }
        public void Exit()
        {
            OnExit();
        }
        public virtual void OnEnter()
        {

        }

        public virtual void OnExit()
        {

        }
    }
    public class SceneManager : BaseModule
    {

        private static SceneManager instance;
        public static SceneManager Instance
        {
            get
            {
                return instance;
            }
        }

        Component _scene;
        public Component Scene { get => _scene; }
        LKCamera _camera;
        public LKCamera Camera { get => _camera; }

        public SceneType currentType = SceneType.None;
        Dictionary<SceneType, BaseScene> AllScene = new Dictionary<SceneType, BaseScene>();
        private BaseScene oldScene;
        public override void Init()
        {
            //SetScene(SceneType.Main, new MainScene());
            SetScene(SceneType.Battle, new BattleScene());
            currentType = SceneType.None;
            _scene = new Component();
            _scene.AddComponent<SceneEntityContainer>();
            _camera = new LKCamera();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= SceneChanged;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += SceneChanged;
            instance = this;
        }

        public void SetScene(SceneType type, BaseScene scene)
        {
            if (!AllScene.ContainsKey(type))
            {
                AllScene[type] = scene;
            }
        }

        public void LoadScene(SceneType sceneType)
        {
            if (currentType == SceneType.None)
            {
                //currentType = SceneType.Main;
            }


            //BaseScene baseScene = AllScene [sceneType];
            string sceneName = GetSceneName(sceneType);

            if (string.IsNullOrEmpty(sceneName))
                return;

            if (currentType != SceneType.None)
            {
                oldScene = AllScene[currentType];
                if (oldScene != null)
                {
                    oldScene.OnExit();
                    oldScene = null;
                    UIManager.Instance.CloseAll();
                    EffectManager.Instance.ClearEffect();
                    AudioManager.Instance.ClearAudio();
                }
            }

            currentType = sceneType;
            //UIManager.Instance.OpenUI (UIPanel.Loading, true);
            Debug.Log($"LoadScene {sceneName}");
            _lastScene = null;
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        }

        private string GetSceneName(SceneType sceneType)
        {
            string sceneName = "";
            switch (sceneType)
            {
                case SceneType.Main:
                    sceneName = "ProjectMain";
                    break;
                case SceneType.Battle:
                    sceneName = "World";
                    break;
            }
            return sceneName;
        }

        private string _lastScene;
        void SceneChanged(Scene arg1, Scene arg2)
        {
            if (currentType == SceneType.None)
                return;

            if (_lastScene == arg2.name)
                return;
            _lastScene = arg2.name;
            Debug.Log($"SceneChanged {arg2.name}");
            
            GameHelper.Instance.StartCoroutine(WaitTime(SceneEnter));

        }

        void SceneEnter()
        {
            Camera.SwitchCamera(UnityEngine.Camera.main);
            //UIManager.Instance.CloseUI (UIPanel.Loading);
            if (AllScene.ContainsKey(currentType))
            {
                AllScene[currentType].Enter();
            }
        }

        IEnumerator WaitTime(Action action)
        {
            yield return new WaitForSeconds(0.01f);
            action();
        }

        public override void Update()
        {
            base.Update();
            _scene.Update();
            _camera.Update();
        }

        public Entity GetEntity(long id)
        {
            return _scene.GetComponent<SceneEntityContainer>().GetEntity(id);
        }

        public Entity AddEntity(string resName, ComponentGroup group)
        {
            Entity entity = new Entity(group);
            entity.ResName = resName;
            _scene.GetComponent<SceneEntityContainer>().AddEntity(entity);
            return entity;
        }

        public Entity AddEntity(string resName, params Type[] types)
        {
            Entity entity = new Entity(types);
            entity.ResName = resName;
            _scene.GetComponent<SceneEntityContainer>().AddEntity(entity);
            return entity;
        }

        public void RemoveEntity(Entity entity)
        {
            entity.Destroy();
            _scene.GetComponent<SceneEntityContainer>().RemoveEntity(entity);
        }
    }
}
