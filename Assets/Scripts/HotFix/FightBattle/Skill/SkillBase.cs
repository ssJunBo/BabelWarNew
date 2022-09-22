using HotFix.Managers;

namespace HotFix.FightBattle.Skill
{
    public abstract class SkillBase
    {
        // 是否准备好 可以释放
        public bool isReady;

        // 是己方 or 敌方
        protected readonly bool isOwnSide;

        protected readonly int skillId;

        // 配置信息
        protected readonly SkillExcelItem skillExcelItem;

        protected readonly BattleUnitBase battleUnitBase;

        protected SkillBase(BattleUnitBase battleUnitBase, int skillId, bool isOwnSide)
        {
            skillExcelItem = ExcelManager.Instance.GetExcelItem<SkillExcelData, SkillExcelItem>(skillId);
            this.battleUnitBase = battleUnitBase;
            this.isOwnSide = isOwnSide;
            this.skillId = skillId;
        }

        public abstract void CastSkill();

        public abstract void Update();
    }
}