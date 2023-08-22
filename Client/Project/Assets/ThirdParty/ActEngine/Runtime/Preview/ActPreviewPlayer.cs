using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Act
{
    [Serializable]
    public class ActPreviewPlayer : MonoBehaviour
    {
        /// <summary>
        /// 预览数据源
        /// </summary>
        public ActPreviewAsset previewAsset;

        public int previewHeroIndex = 0;
        public int previewMonsterIndex = 0;
        public int previewSceneIndex = 0;

        public bool mainCharactorIsHero = true;

        public ActAsset hitAct;

        /// <summary>
        /// 预览英雄
        /// </summary>
        GameObject hero;

        /// <summary>
        /// 预览怪物
        /// </summary>
        GameObject monster;

        PreviewSceneData previewSceneData;

        void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            Time.fixedDeltaTime = 0.02f;
            Facade.Preview.GetMainCharactor = GetMainCharactor;
            Facade.Preview.GetTarget = GetTarget;
            Facade.Preview.GetHitAct = GetHitAct;
        }

        Action callback;
        public void StartPreview(Action callback)
        {
            this.callback = callback;
            LoadScene();
        }

        public void ChangeScene()
        {
            LoadScene();
        }

        public void LoadScene()
        {
            previewSceneData = previewSceneIndex < previewAsset.sceneList.Count ? previewAsset.sceneList[previewSceneIndex] : null;
            if (previewSceneData != null && previewSceneData.Scene != null)
            {
                string path = UnityEditor.AssetDatabase.GetAssetPath(previewSceneData.Scene);
                UnityEditor.SceneManagement.EditorSceneManager.sceneLoaded -= SceneChange;
                UnityEditor.SceneManagement.EditorSceneManager.sceneLoaded += SceneChange;
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(path, new UnityEngine.SceneManagement.LoadSceneParameters() { loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single, localPhysicsMode = UnityEngine.SceneManagement.LocalPhysicsMode.None });
            }
            else
            {
                SceneChange();
            }
        }

        void SceneChange(Scene scene = default, LoadSceneMode mode = LoadSceneMode.Single)
        {
            callback?.Invoke();
            CreateHero();
            CreateMonster();
        }

        private void Update()
        {
        }

        bool isShowDesc = false;
        private void OnGUI()
        {
            Rect rect = new Rect(10, 10, 70, 30);

            if (DrawButton(rect, "定位"))
            {
                SelectPreviewObj();
            }

            rect.y += rect.height + 3;
            if (DrawButton(rect, "重置"))
            {
                isShowDesc = false;
                Reload();
            }

            rect.y += rect.height + 3;

            if (DrawButton(rect, "说明"))
                isShowDesc = !isShowDesc;

            if (isShowDesc)
            {
                rect.y += 50;
                rect.width = 300;
                rect.height = 500;

                GUIStyle style = new GUIStyle();
                style.fontSize = 18;
                style.normal.textColor = Color.white;
                GUI.Label(rect, "方向键: 移动主角 \nQ: 瞬移 \nE: 朝向目标  \n空格: 执行上一个Act  \nF6: 选择Act目录  \n鼠标右键: 怪物移动中心设置", style);
            }
        }

        bool DrawButton(Rect rect, string title)
        {
            bool isClick = GUI.Button(rect, "");

            GUIStyle style = new GUIStyle();
            style.fontSize = 22;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(rect, title, style);

            return isClick;
        }

        private void FixedUpdate()
        {
            Act.Facade.Internals.Timeline.FixedUpdate?.Invoke();
        }

        public void Reload()
        {
            CreateHero();
            CreateMonster();
            Facade.Preview.ReloadPreview?.Invoke();
        }

        private void SelectPreviewObj()
        {
            Selection.activeObject = GameObject.FindObjectOfType<ActPreviewPlayer>();
        }

        void CreateHero()
        {
            var sceneData = GetSceneData();
            var obj = GameObject.Instantiate(GetHero().Model);
            var trans = obj.transform;
            trans.localScale = Vector3.one;
            trans.position = sceneData.HeroPosition;
            trans.LookAt(sceneData.MonsterPosition);

            hero = obj;
        }

        void CreateMonster()
        {
            var model = GetMonster().Model;
            if (model == null)
                return;
            var sceneData = GetSceneData();
            var obj = GameObject.Instantiate(model);
            var trans = obj.transform;
            trans.localScale = Vector3.one;
            trans.position = sceneData.MonsterPosition;
            trans.LookAt(sceneData.HeroPosition);

            monster = obj;
        }

        PreviewModelData GetHero()
        {
            return previewAsset.heroList[previewHeroIndex];
        }

        PreviewModelData GetMonster()
        {
            return previewAsset.monsterList[previewMonsterIndex];
        }

        PreviewSceneData GetSceneData()
        {
            return previewAsset.sceneList[previewSceneIndex];
        }

        GameObject GetMainCharactor()
        {
            return mainCharactorIsHero ? hero : monster;
        }

        GameObject GetTarget()
        {
            return mainCharactorIsHero ? monster : hero;
        }

        ActAsset GetHitAct()
        {
            return hitAct;
        }
    }
}

#endif