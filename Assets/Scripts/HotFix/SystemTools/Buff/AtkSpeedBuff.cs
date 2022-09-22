using HotFix.Common;
using HotFix.FightBattle;
using UnityEngine;

namespace HotFix.SystemTools.Buff
{
    public class AtkSpeedBuff: BuffBase
    {
        public override BuffType mBuffType => BuffType.AtkSpeed;
      
        protected AtkSpeedBuff(BattleUnitBase battleUnit, float length) : base(battleUnit, length)
        {
        }
      
        public override void OnAdd()
        {
            base.OnAdd();
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
        }
    }
}