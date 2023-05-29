using _GameBase;
using FightBattle;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class FightManager : MonoSingleton<FightManager>
    {
        // 敌人生成位置
        [FormerlySerializedAs("enemyPos")] [SerializeField] public Transform enemyUnitBornPos;
        [FormerlySerializedAs("heroBornPos")] [SerializeField] public Transform ownUnitBornPos;
        [SerializeField] public Transform allBattleUnitTrs;
        [SerializeField] public Camera fightCamera;
        [SerializeField] public Transform objPoolTrs;

        private BattleWorld _battleWorlds = new();
        public BattleWorld BattleWorld => _battleWorlds;
        public void CreateBattleWorld(int lev)
        {
            _battleWorlds = new();
            _battleWorlds.Init(lev);
        }

        public void ReleaseBattleWorld()
        {
            _battleWorlds.Release();
        }
    }
}
