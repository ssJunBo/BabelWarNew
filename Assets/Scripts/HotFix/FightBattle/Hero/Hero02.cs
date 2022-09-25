using HotFix.Helpers;
using HotFix.Managers;
using UnityEngine;

namespace HotFix.FightBattle.Hero
{
    public class Hero02 : HeroUnitBase
    {
        protected override void DamageEnemy(BattleUnitBase targetUnit)
        {
            // 剑圣普通攻击扇形范围伤害
            for (var i = FightManager.Instance.EnemyUnitLis.Count - 1; i >= 0; i--)
            {
                var battleUnit = FightManager.Instance.EnemyUnitLis[i];
                // 在攻击范围内 全部造成伤害一次
                if (this.IsInSectorArea(AttributeInfo.AttributeConfig.atkAngle,
                        AttributeInfo.AttributeConfig.atkRadius, battleUnit.transform))
                {
                    this.CauseNormalDamage(battleUnit);
                }
            }
        }

        protected override void GenerateNormalEffect()
        {
            base.GenerateNormalEffect();

            GameObject effect = EffectManager.Instance.GetEffectPool(EffectType.Hero03NormalAttack).Spawn();
            effect.transform.position = effectTrs.position;
            effect.transform.rotation = effectTrs.rotation;

            TimerEventManager.Instance.DelaySeconds(1.5f,
                () =>
                {
                    if (effect != null)
                        EffectManager.Instance.GetEffectPool(EffectType.Hero03NormalAttack).Cycle(effect);
                });
        }
    }
}