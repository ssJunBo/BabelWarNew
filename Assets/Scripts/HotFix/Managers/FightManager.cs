using System;
using System.Collections.Generic;
using HotFix.Common;
using HotFix.FightBattle;
using HotFix.Helpers;
using HotFix.Pool;
using Main.Game.Base;
using Main.Game.ResourceFrame;
using UnityEngine;

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
        [SerializeField] public Transform objPoolTrs;

        // 最终击败建筑物
        public Transform BuildPos => buildPos;

        // 所有敌人的obj
        [NonSerialized] public readonly List<BattleUnitBase> EnemyUnitLis = new List<BattleUnitBase>();

        // 自身单位
        [NonSerialized] public readonly List<BattleUnitBase> OwnUnitLis = new List<BattleUnitBase>();

        private ExcelManager _excelMana;

        public bool OpenQuickFight { get; set; }

        public ObjectPool<Blood> BloodPool { get; private set; }

        private void Start()
        {
            EventManager.Subscribe<int>(EventMessageType.ChangeTimeScale, ChangeTimeScale);

            Blood bloodPre = ResManager.Instance.LoadResource<GameObject>("Prefabs/Effect/Blood/WhiteBlood").GetComponent<Blood>();
            
            BloodPool = new ObjectPool<Blood>(bloodPre,objPoolTrs);
            
            _excelMana = ExcelManager.Instance;

            LoadEnemyUnit(DataManager.Instance.PersonInfo.LevelId);

            LoadOwnUnit();
            
            PauseFighting();
            
            DataManager.Instance.SetCurrentLevData(DataManager.Instance.PersonInfo.LevelId);
        }

        private void LoadEnemyUnit(int levelId)
        {
            LevelExcelItem itemInfo = _excelMana.GetExcelItem<LevelExcelData, LevelExcelItem>(levelId);

            EnemyUnitLis.Clear();

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

                EnemyUnitLis.Add(battleUnitBase);
            }
        }

        private void LoadOwnUnit()
        {
            for (int i = 0; i < DataManager.Instance.PersonInfo.HeroInfos.Count; i++)
            {
                var bornTrs = heroBornPos[i % heroBornPos.Length];
                LoadSelfUnit(DataManager.Instance.PersonInfo.HeroInfos[i], bornTrs);
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

            OwnUnitLis.Add(battleUnitBase);
        }

        // 刷新敌人列表
        public void RemoveDieEnemy(BattleUnitBase target)
        {
            for (int i = 0; i < EnemyUnitLis.Count; i++)
            {
                if (target == EnemyUnitLis[i])
                {
                    EnemyUnitLis.RemoveAt(i);
                    break;
                }
            }

            if (EnemyUnitLis.Count <= 0)
            {
                EventManager.DispatchEvent(EventMessageType.FightResult, 1);
            }
        }

        // 刷新英雄列表
        public void RemoveDieHero(BattleUnitBase target)
        {
            for (int i = 0; i < OwnUnitLis.Count; i++)
            {
                if (target == OwnUnitLis[i])
                {
                    OwnUnitLis.RemoveAt(i);
                    break;
                }
            }

            if (OwnUnitLis.Count <= 0)
            {
                EventManager.DispatchEvent(EventMessageType.FightResult, 0);
            }
        }

        public void StartFighting()
        {
            foreach (var unitBase in EnemyUnitLis)
            {
                unitBase.enabled = true;
            }

            foreach (var unitBase in OwnUnitLis)
            {
                unitBase.enabled = true;
            }
        }
        
        public void PauseFighting()
        {
            foreach (var unitBase in EnemyUnitLis)
            {
                unitBase.enabled = false;
            }

            foreach (var unitBase in OwnUnitLis)
            {
                unitBase.enabled = false;
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
