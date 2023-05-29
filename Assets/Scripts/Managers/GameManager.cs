using System.Collections.Generic;
using _GameBase;
using Common;
using Data.Account;
using ET;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(TimerEventManager))]
    [RequireComponent(typeof(AudioManager))]
    public class GameManager : MonoSingleton<GameManager>
    {
        #region UI
        [Header("最底层 Dialog 放在此节点下"), Space] public RectTransform ui2DTrsLow;
        [Header("普通层 Dialog 放在此节点下")] public RectTransform ui2DTrsHigh;
        [Header("对象池回收节点")] public Transform recyclePoolTrs;
        [Header("ui相机"), Space] public Camera uiCamera;
        [Header("场景相机")] public Camera gameCamera;
        [Header("3d 人物位置")] public Transform personTrs;
        [Header("战斗Obj")] public GameObject fightObj;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            ABUpdateManager.Instance.CheckUpdate(isOver => { Log.Info(isOver ? "检测更新结束" : "提示网络出错 重启"); }, str =>
            {
                // 以后可以在这里处理更新 加载界面上的显示信息的相关逻辑
                Log.Info(str);
            });
        }

        private async void Start()
        {
            AudioManager.Instance.PlayBg("bg01");

            
            ETTask.ExceptionHandler += Log.Error;
            
            Game.AddSingleton<TimeInfo>();
            Game.AddSingleton<ObjectPool>();
            Game.AddSingleton<TimerComponent>();
            Game.AddSingleton<CoroutineLockComponent>();
            Game.AddSingleton<CodeLoader>().Start();
            
            // DontDestroyOnLoad(gameObject);

            // 从ab包加载就要先加载配置表
            // ResourceManager.Instance.MLoadFromAssetBundle = loadFromAssetBundle;
            // if (ResourceManager.Instance.MLoadFromAssetBundle)
            // AssetBundleManager.Instance.LoadAssetBundleConfig();

            Log.Info("初始化表之前时间 "+ Time.realtimeSinceStartup);
            
            await ExcelManager.Instance.InitData();

            Log.Info("初始化表之后时间 "+ Time.realtimeSinceStartup);

            LoadConfig();

            UIManager.Instance.OpenUi(EUiID.UIMain);
        }

        private void Update()
        {
            Game.Update();
            
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

        private void LateUpdate()
        {
            Game.LateUpdate();
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
            FightManager.Instance.CreateBattleWorld(levId);
        }
        
        
        public void QuitFight()
        {
            fightObj.SetActive(false);
            FightManager.Instance.ReleaseBattleWorld();
        }
    }
}
