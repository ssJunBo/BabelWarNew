using Common;
using Managers;
using UnityEngine;

namespace FightBattle.Buff
{
    public class DizzyBuff: BuffBase
    {
        public override BuffType mBuffType => BuffType.DizzyBuff;
        public DizzyBuff(BattleUnitBase battleUnit, float length) : base(battleUnit, length) { }

        private GameObject effectObj;

        public override void OnAdd()
        {
            base.OnAdd();
            battleUnit.AttributeInfo.IsDizzy = true;
            battleUnit.StopAllBehavior();

            effectObj = EffectManager.Instance.GetEffectPool(EffectType.DizzyBuff).Spawn();
            effectObj.transform.SetParent(battleUnit.hitPoint);
            effectObj.transform.localPosition=Vector3.zero;
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
            EffectManager.Instance.GetEffectPool(EffectType.DizzyBuff).Cycle(effectObj);
            battleUnit.AttributeInfo.IsDizzy = false;
            battleUnit.ResumeBehavior();
        }
    }
}