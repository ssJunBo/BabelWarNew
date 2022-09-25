using HotFix.Common;
using HotFix.Managers;
using UnityEngine;

namespace HotFix.FightBattle.Buff
{
    public class FreezeBuff : BuffBase
    {
        public override BuffType mBuffType => BuffType.FreezeBuff;
        public FreezeBuff(BattleUnitBase battleUnit, float length) : base(battleUnit, length) { }

        private GameObject _effectObj;

        public override void OnAdd()
        {
            base.OnAdd();
            battleUnit.AttributeInfo.IsFreeze = true;
            battleUnit.StopAllBehavior();
            _effectObj = EffectManager.Instance.GetEffectPool(EffectType.FreezeBuff).Spawn();
            _effectObj.transform.SetParent(battleUnit.hitPoint);
            _effectObj.transform.localPosition=Vector3.zero;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (timer >= mLength)
            {
                battleUnit.RemoveBuff(this);
            }

            timer += Time.fixedDeltaTime;
        }

        public override void OnRemove()
        {
            base.OnRemove();

            EffectManager.Instance.GetEffectPool(EffectType.FreezeBuff).Cycle(_effectObj);
            
            battleUnit.AttributeInfo.IsFreeze = false;
            battleUnit.AttributeInfo.FreezeVal = 0;

            battleUnit.ResumeBehavior();
        }
    }
}