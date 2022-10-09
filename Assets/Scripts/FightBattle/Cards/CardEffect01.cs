using Managers;

namespace FightBattle.Cards
{
    // 随机选一名敌人为中心，半径100内敌人造成伤害！
    public class CardEffect01
    {
        private const string effectPath = "";

        private int cardSkillLev;
        
        public CardEffect01(int lev)
        {
            cardSkillLev = lev;
        }

        private void Cast()
        {
            // FightManager.Instance.EnemyUnitLis
        }
    }
}