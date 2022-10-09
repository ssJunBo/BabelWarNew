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

            UiManager.Instance.OpenUi(EUiID.Main);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                DataManager.Instance.SaveData(new PersonInfo()
                {
                    IconExcelId = 1,
                    Name = "清风自来",
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
                            CombineId = 10101,
                        },
                        new()
                        {
                            CombineId = 10201,
                        },
                        new()
                        {
                            CombineId = 10301,
                        },
                        new()
                        {
                            CombineId = 10401,
                        },
                        new()
                        {
                            CombineId = 10501,
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
            UnityEngine.Resources.UnloadUnusedAssets();
            Debug.Log("application退出操作，同时清 空编 辑 器 缓 存 ！");
#endif
        }

        public void StartFight(int levId)
        {
            fightObj.SetActive(true);
            FightManager.Instance.LoadUnit(levId);
        }
        
        
        public void QuitFight()
        {
            fightObj.SetActive(false);
            FightManager.Instance.ClearUnit();
        }
    }
}
