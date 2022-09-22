using System;
using System.Collections.Generic;
using HotFix.Common;
using HotFix.Data.Account;
using HotFix.FightBattle;
using HotFix.Helpers;
using HotFix.SystemTools.EventSys;
using HotFix.SystemTools.Pool;
using Main.Game.Base;
using Main.Game.ResourceFrame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HotFix.Managers
{
    public class FightManager : MonoSingleton<FightManager>
    {
        // 敌人生成位置
        [SerializeField] private Transform[] enemyPos;
        [SerializeField] private Transform[] heroBornPos;
        [SerializeField] private Transform buildPos;
        [SerializeField] private Transform allBattleUnitTrs;
        [SerializeField] public Camera fightCamera;
        [SerializeField] private Transform bloodTrs;
        [SerializeField] public Transform effectTrs;

        // 最终击败建筑物
        public Transform BuildPos => buildPos;

        // 所有敌人的obj
        [NonSerialized] public readonly List<BattleUnitBase> enemyUnitLis = new List<BattleUnitBase>();

        // 自身单位
        [NonSerialized] public readonly List<BattleUnitBase> ownUnitLis = new List<BattleUnitBase>();

        private ExcelManager _excelMana;

        private readonly PersonInfo _personInfo = GameManager.Instance.PersonInfo;

        public bool OpenQuickFight { get; set; }

        public ObjectPool<Blood> BloodPool { get; private set; }

        private void Start()
        {
            EventManager.Subscribe<int>(EventMessageType.ChangeTimeScale, ChangeTimeScale);

            Blood bloodPre = ResManager.Instance.LoadResource<GameObject>("Prefabs/Effect/Blood/WhiteBlood").GetComponent<Blood>();
            
            BloodPool = new ObjectPool<Blood>(bloodPre,bloodTrs);
            
            _excelMana = ExcelManager.Instance;

            LoadLevelEnemy(_personInfo.levelId);

            for (int i = 0; i < _personInfo.heroInfos.Count; i++)
            {
                var bornTrs = heroBornPos[i % heroBornPos.Length];
                LoadSelfUnit(_personInfo.heroInfos[i], bornTrs);
            }
        }

        private void LoadLevelEnemy(int levelId)
        {
            LevelExcelItem itemInfo = _excelMana.GetExcelItem<LevelExcelData, LevelExcelItem>(levelId);

            enemyUnitLis.Clear();

            for (var i = 0; i < itemInfo.enemyCombineId.Length; i++)
            {
                int soliderCombineId = itemInfo.enemyCombineId[i];
                var battleUnitId = IDParseHelp.GetBattleUnitId(soliderCombineId);
                var enemyPathId = _excelMana.GetExcelData<BattleUnitExcelData>().GetPathId(battleUnitId);
                string pathStr = _excelMana.GetExcelData<PathExcelData>().GetDetailPath(enemyPathId);

                var go = ResManager.Instance.LoadResource(pathStr);
                var obj = Instantiate(go, allBattleUnitTrs);
                obj.transform.position = enemyPos[i % heroBornPos.Length].position;
                obj.transform.localScale = Vector3.one;
                obj.transform.localRotation = Quaternion.Euler(0, 180, 0);

                BattleUnitBase battleUnitBase = obj.GetComponent<BattleUnitBase>();
                battleUnitBase.enabled = true;
                battleUnitBase.navMeshAgent.enabled = true;
                battleUnitBase.SetData(soliderCombineId);
                battleUnitBase.hpObj.gameObject.SetActive(true);

                enemyUnitLis.Add(battleUnitBase);
            }
        }

        private void LoadSelfUnit(int combineId, Transform bornTrs)
        {
            int unitId= IDParseHelp.GetBattleUnitId(combineId);
            
            var pathId = _excelMana.GetExcelItem<BattleUnitExcelData, BattleUnitExcelItem>(unitId).PathId;
            string pathStr = _excelMana.GetExcelData<PathExcelData>().GetDetailPath(pathId);

            var go = ResManager.Instance.LoadResource(pathStr);
            var obj = Instantiate(go, allBattleUnitTrs);
            obj.transform.position = bornTrs.position;
            obj.transform.localScale = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;

            var battleUnitBase = obj.GetComponent<BattleUnitBase>();
            battleUnitBase.enabled = true;
            battleUnitBase.navMeshAgent.enabled = true;
            battleUnitBase.hpObj.SetActive(true);
            battleUnitBase.SetData(combineId);

            ownUnitLis.Add(battleUnitBase);
        }

        // 刷新敌人列表
        public void RemoveDieEnemy(BattleUnitBase target)
        {
            for (int i = 0; i < enemyUnitLis.Count; i++)
            {
                if (target == enemyUnitLis[i])
                {
                    enemyUnitLis.RemoveAt(i);
                    break;
                }
            }

            if (enemyUnitLis.Count <= 0)
            {
                EventManager.DispatchEvent(EventMessageType.Victory);
            }
        }

        // 刷新英雄列表
        public void RemoveDieHero(BattleUnitBase target)
        {
            for (int i = 0; i < ownUnitLis.Count; i++)
            {
                if (target == ownUnitLis[i])
                {
                    ownUnitLis.RemoveAt(i);
                    break;
                }
            }

            if (ownUnitLis.Count <= 0)
            {
                EventManager.DispatchEvent(EventMessageType.Defeat);
            }
        }

        private void ChangeTimeScale(int multiple)
        {
            Time.timeScale = multiple;

            if (multiple > 1)
                OpenQuickFight = true;
        }

        public void OnDestroy()
        {
            BloodPool.DestroyAllItem();
            EventManager.UnSubscribe<int>(EventMessageType.ChangeTimeScale, ChangeTimeScale);
        }
    }
}
