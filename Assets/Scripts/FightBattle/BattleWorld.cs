using System;
using System.Collections.Generic;
using _GameBase;
using Common;
using Helpers;
using Managers;
using Pool;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FightBattle
{
    /// <summary>
    /// 战场类  每此开启一场战斗 就会创建一个此类
    /// </summary>
    public class BattleWorld
    {
        // 所有敌人的obj
         public readonly List<BattleUnitBase> EnemyUnitLis = new();

        // 自身单位
        public readonly List<BattleUnitBase> OwnUnitLis = new();

        private ExcelManager _excelMana => ExcelManager.Instance;

        private bool _isInit;

        public bool OpenQuickFight { get; set; }

        public ObjectPool<Blood> BloodPool { get; private set; }

        public async void Init(int lev)
        {
            if (!_isInit)
            {
                EventManager.Subscribe(EventMessageType.ChangeTimeScale, ChangeTimeScale);
                await ResourcesComponent.Instance.LoadBundleAsync("WhiteBlood".StringToAB());
            
                Blood bloodPre = ResourcesComponent.Instance.GetAsset("WhiteBlood".ToLower() + ".unity3d", "WhiteBlood")
                    .GetComponent<Blood>();

                BloodPool = new ObjectPool<Blood>(bloodPre, FightManager.Instance.objPoolTrs);

                _isInit = true;
            }

            LoadUnit(lev);
        }

        private void LoadUnit(int levId)
        {
            LoadEnemyUnit(levId);

            LoadOwnUnit();

            PauseFighting();

            DataManager.Instance.SetCurrentLevData(DataManager.Instance.PersonInfo.LevelId);
        }

        public void ClearUnit()
        {
            for (int i = OwnUnitLis.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(OwnUnitLis[i].gameObject);
                OwnUnitLis.RemoveAt(i);
            }
            
            for (int i = EnemyUnitLis.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(EnemyUnitLis[i].gameObject);
                EnemyUnitLis.RemoveAt(i);
            }
        }

        private void LoadEnemyUnit(int levelId)
        {
            LevelExcelItem itemInfo = _excelMana.GetExcelItem<LevelExcelData, LevelExcelItem>(levelId);

            EnemyUnitLis.Clear();
            
            if (itemInfo==null)
            {
                return;
            }
            for (var i = 0; i < itemInfo.enemyCombineId.Length; i++)
            {
                int soliderCombineId = itemInfo.enemyCombineId[i];
                var battleUnitId = IDParseHelp.GetBattleUnitId(soliderCombineId);
                var enemyPathId = _excelMana.GetExcelData<BattleUnitExcelData>().GetPathId(battleUnitId);
                string pathStr = _excelMana.GetExcelData<PathExcelData>().GetDetailPath(enemyPathId);

                var go = UnityEngine.Resources.Load<GameObject>(pathStr);
                var obj = GameObject.Instantiate(go, FightManager.Instance.allBattleUnitTrs);

                var localPosition = FightManager.Instance.enemyUnitBornPos.localPosition;

                float x = Random.Range(localPosition.x - 120, localPosition.x + 120);
                float z = Random.Range(localPosition.z, localPosition.z - 120 / 2);

                obj.transform.localPosition = new Vector3(x, 0, z);
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
                var centerPos = FightManager.Instance.ownUnitBornPos.localPosition;
             
                float x = Random.Range(centerPos.x - 120, centerPos.x + 120);
                float z = Random.Range(centerPos.z, centerPos.z + 120 / 2);

                var bornPos = new Vector3(x, 0, z);
                
                LoadSelfUnit(DataManager.Instance.PersonInfo.HeroInfos[i], bornPos);
            }
        }
        
        private void LoadSelfUnit(int combineId, Vector3 bornPos)
        {
            int unitId= IDParseHelp.GetBattleUnitId(combineId);
            
            var pathId = _excelMana.GetExcelItem<BattleUnitExcelData, BattleUnitExcelItem>(unitId).PathId;
            string pathStr = _excelMana.GetExcelData<PathExcelData>().GetDetailPath(pathId);

            var go = UnityEngine.Resources.Load<GameObject>(pathStr);
            var obj = GameObject.Instantiate(go, FightManager.Instance.allBattleUnitTrs);
            obj.transform.localPosition = bornPos;
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
        
        private void ChangeTimeScale(object arg)
        {
            int multiple = (int)arg;
       
            Time.timeScale = multiple;

            if (multiple > 1)
                OpenQuickFight = true;
        }

        public void Release()
        {
            BloodPool.DestroyAllItem();
            EventManager.UnSubscribe(EventMessageType.ChangeTimeScale, ChangeTimeScale);
        }
    }
}