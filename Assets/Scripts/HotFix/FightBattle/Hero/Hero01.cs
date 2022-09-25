using HotFix.FightBattle.Skill;
using HotFix.Helpers;
using HotFix.Managers;
using UnityEngine;

namespace HotFix.FightBattle.Hero
{
    public class Hero01 : HeroUnitBase
    {
        protected override void Awake()
        {
            base.Awake();

            InitSkill();
        }

        private void InitSkill()
        {
            SkillRunner = new SkillRunner();

            ShadowlessSword shadowlessSword = new ShadowlessSword(this, 10101, true);
            NormalAttackComplete += shadowlessSword.AddCharge;

            SkillRunner.AddSkill(shadowlessSword);
        }

        protected override void GenerateNormalEffect()
        {
            base.GenerateNormalEffect();

            GameObject effect = EffectManager.Instance.GetEffectPool(EffectType.Hero02NormalAttack).Spawn();
            effect.transform.position = effectTrs.position;
            effect.transform.rotation = effectTrs.rotation;
            TimerEventManager.Instance.DelaySeconds(1.5f,
                () =>
                {
                    if(effect!=null) 
                        EffectManager.Instance.GetEffectPool(EffectType.Hero02NormalAttack).Cycle(effect);
                });

        }

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

        // 辅助线

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (AttributeInfo != null)
            {
                var radius = ExcelManager.Instance.GetExcelData<SkillExcelData>().GetRadius(10101);

                var angle = transform.rotation.eulerAngles.y;

                Vector3 center = new Vector3(transform.position.x + radius * Mathf.Sin(angle*Mathf.Deg2Rad), 0, transform.position.z + radius * Mathf.Cos(angle*Mathf.Deg2Rad));
                
                GeometryHelper.RoundLine(center, radius, transform.forward);
            }
        }
    }
}