using HotFix.FightBattle;
using HotFix.Managers;

namespace HotFix.Helpers
{
    public static class DamageHelper
    {
        // 造成普通攻击伤害
        public static void CauseNormalDamage(this BattleUnitBase unitBase, BattleUnitBase target)
        {
            if (target.AttributeInfo.Hp <= 0) 
                return;
            
            int atk = unitBase.AttributeInfo.Atk;
            float enemyDef = target.AttributeInfo.Def;
            float damageVal = atk * (1 - enemyDef);
            
            target.HpChange(-damageVal);
        }
        
        // 造成技能伤害
        public static void CauseSkillDamage(this BattleUnitBase unitBase, BattleUnitBase target, int skillId)
        {
            if (target.AttributeInfo.Hp <= 0)
                return;

            var damageVal = ExcelManager.Instance.GetExcelData<SkillExcelData>().GetDamages(skillId);
            
            target.HpChange(-damageVal);
        }

        public static void CauseDamage(this BattleUnitBase unitBase, BattleUnitBase target,int damageVal)
        {
            if (target.AttributeInfo.Hp<= 0) 
                return;
            
            target.HpChange(-damageVal);
        }
        
        public static void CauseFreeze(this BattleUnitBase unitBase, BattleUnitBase target,int freezeVal)
        {
            if (target.AttributeInfo.Hp<= 0) 
                return;

            target.AttributeInfo.FreezeVal += freezeVal;
        }
    }
}