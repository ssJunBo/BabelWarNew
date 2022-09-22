using System;
using System.Collections.Generic;
using HotFix.Common;
using HotFix.Data.Account;
using HotFix.Managers.Model;
using Main.Game.Base;
using UnityEngine;

namespace HotFix.Managers
{
    [RequireComponent(typeof(TimerEventManager))]
    [RequireComponent(typeof(AudioManager))]
    public class GameManager : MonoSingleton<GameManager>
    {
        #region UI
        // [SerializeField, Space] private bool loadFromAssetBundle;
        [Header("最底层 Dialog 放在此节点下"), Space] public Transform ui2DTrsLow;
        [Header("普通层 Dialog 放在此节点下")] public Transform ui2DTrsHigh;
        [Header("对象池回收节点")] public Transform recyclePoolTrs;
        [Header("ui相机"), Space] public Camera uiCamera;
        [Header("场景相机")] public Camera gameCamera;
        [Header("3d 人物位置")] public Transform personTrs;
        
        #endregion

        #region moudle play

        private CModelPlay _modelPlay;
        public CModelPlay ModelPlay => _modelPlay ??= new CModelPlay();

        private SceneManager _sceneManager;
        public SceneManager SceneManager => _sceneManager ??= new SceneManager(this);
        
        #endregion

        /// <summary>
        /// 个人缓存本地数据
        /// </summary>
        public PersonInfo PersonInfo { get; set; }
        private void Start()
        {
            AudioManager.Instance.PlayBg("bg01");
            // TODO 测试用例 
            PersonInfo = ArchiveManager.Instance.LoadData();
            
            DontDestroyOnLoad(gameObject);

            // 从ab包加载就要先加载配置表
            // ResourceManager.Instance.MLoadFromAssetBundle = loadFromAssetBundle;
            // if (ResourceManager.Instance.MLoadFromAssetBundle)
            // AssetBundleManager.Instance.LoadAssetBundleConfig();

            InitManager();

            LoadConfig();

            ModelPlay.OpenUiByType(EUiType.Main);

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                ArchiveManager.Instance.SaveData(new PersonInfo()
                {
                    levelId = 1,
                    heroInfos = new List<int>
                    {
                        10101,
                        10201,
                    }
                });
            }
        }

        private void InitManager()
        {
            ExcelManager.Instance.InitData();
        }

        /// <summary>
        /// 加载配置表 需要什么配置表都在这里加载
        /// </summary>
        static void LoadConfig()
        {
            //ConfigerManager.Instance.LoadData<BuffData>(CFG.TABLE_BUFF);
            //ConfigerManager.Instance.LoadData<MonsterData>(CFG.TABLE_MONSTER);
        }


        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            Resources.UnloadUnusedAssets();
            Debug.Log("application退出操作，同时清 空编 辑 器 缓 存 ！");
#endif
        }

        public void Clear()
        {
            
        }
    }
}
