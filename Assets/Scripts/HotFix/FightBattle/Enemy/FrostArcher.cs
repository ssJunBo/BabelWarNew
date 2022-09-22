using HotFix.Helpers;
using HotFix.SystemTools.Buff;

namespace HotFix.FightBattle.Enemy
{
    public class FrostArcher : Archer
    {
        protected override void CauseDamage(BattleUnitBase targetUnit)
        {
            base.CauseDamage(targetUnit);

            if (targetUnit.AttributeInfo.IsFreeze)
                return;

            // 加冰冻值
            this.CauseFreeze(targetUnit, int.Parse(AttributeInfo.AttributeConfig.extraPara));
            
            if (targetUnit.AttributeInfo.FreezeVal >= 100)
            {
                // 添加冰冻buff
                var actualTime = IDParseHelp.GetBattleLev(soliderCombineId) * 1;
                targetUnit.AddBuff(new FreezeBuff(targetUnit, actualTime));
            }
        }
    }
}