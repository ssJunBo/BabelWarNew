using System.Collections.Generic;
using _GameBase;
using Common;
using Data.Account;
using Managers.Model;
using UnityEngine;

namespace Managers
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
        [Header("战斗Obj")] public GameObject fightObj;

        #endregion

        #region moudle play

        private CModelPlay _modelPlay;
        public CModelPlay ModelPlay => _modelPlay ??= new CModelPlay();

        private SceneManager _sceneManager;
        public SceneManager SceneManager => _sceneManager ??= new SceneManager(this);
        
        #endregion

        private void Start()
        {
            AudioManager.Instance.PlayBg("bg01");
            
            // DontDestroyOnLoad(gameObject);

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
                DataManager.Instance.SaveData(new PersonInfo()
                {
                    iconExcelId = 1,
                    name = "清风自来",
                    LevelId = 1,
                    HeroInfos = new List<int>
                    {
                        10101,
                        10201,
                    },
                    OwnCardsList = new List<CardInfo>
                    {
                        new()
                        {
                            ID = 101,
                            StarLev = 1
                        },
                        new()
                        {
                            ID = 102,
                            StarLev = 1
                        },
                        new()
                        {
                            ID = 103,
                            StarLev = 1
                        },
                        new()
                        {
                            ID = 104,
                            StarLev = 1
                        },
                        new()
                        {
                            ID = 101,
                            StarLev = 2
                        },
                        new()
                        {
                            ID = 102,
                            StarLev = 2
                        },
                        new()
                        {
                            ID = 103,
                            StarLev = 2
                        },
                        new()
                        {
                            ID = 104,
                            StarLev = 2
                        },
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

        public void StartFight()
        {
            fightObj.SetActive(true);
            
            FightManager.Instance.LoadUnit();
        }
        
        
        public void QuitFight()
        {
            fightObj.SetActive(false);
        }
    }
}
