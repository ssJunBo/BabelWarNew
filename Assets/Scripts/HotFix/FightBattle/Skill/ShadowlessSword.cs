using System;
using HotFix.Common;
using HotFix.Helpers;
using HotFix.Managers;
using UnityEngine;

namespace HotFix.FightBattle.Skill
{
    // 无影剑阵
    public class ShadowlessSword : SkillBase
    {
        public ShadowlessSword(BattleUnitBase battleUnitBase, int skillId, bool isOwnSide) : base(battleUnitBase,
            skillId, isOwnSide)
        {
            battleUnitBase.AnimationEventHelp.actDict["SkillStart"] = SkillStart;
            battleUnitBase.AnimationEventHelp.actDict["SkillEnd"] = SkillEnd;
        }

        public override void CastSkill()
        {
            battleUnitBase.SetAnimation(AnimationType.Skill1);
            _curChargeCount = 0;
            isReady = false;
        }

        private int _curChargeCount;

        public void AddCharge()
        {
            _curChargeCount++;
            if (_curChargeCount >= skillExcelItem.ChargeCount)
            {
                isReady = true;
            }
        }

        private GameObject _skillEffect;

        private void SkillStart()
        {
            battleUnitBase.AttributeInfo.NotSelected = true;

            _skillEffect = EffectManager.Instance.GetEffectPool(EffectType.Hero01Skill).Spawn();

            var transform1 = battleUnitBase.transform;
            var position = transform1.position;

            _skillEffect.transform.position =
                new Vector3(position.x, position.y, position.z + 20);

            _skillEffect.transform.rotation = transform1.rotation;
        }

        private void SkillEnd()
        {
            var unitList = isOwnSide ? FightManager.Instance.EnemyUnitLis : FightManager.Instance.OwnUnitLis;

            var damageRadius = ExcelManager.Instance.GetExcelData<SkillExcelData>().GetRadius(skillId);

            var radius = ExcelManager.Instance.GetExcelData<SkillExcelData>().GetRadius(skillId);

            var transform = battleUnitBase.transform;
            
            var angle = transform.rotation.eulerAngles.y;

            Vector2 center = new Vector3(transform.position.x + radius * Mathf.Sin(angle*Mathf.Deg2Rad),
                battleUnitBase.transform.position.z + radius * Mathf.Cos(angle*Mathf.Deg2Rad));

            // 计算伤害
            for (var i = unitList.Count - 1; i >= 0; i--)
            {
                var battleUnit = unitList[i];

                // 在攻击范围内 全部造成伤害一次
                if (GeometryHelper.InRoundArea(center, new Vector2(battleUnit.transform.position.x, battleUnit.transform.position.z), damageRadius))
                {
                    DamageHelper.CauseSkillDamage(battleUnit, skillId);
                }
            }

            battleUnitBase.AttributeInfo.NotSelected = false;
            
            if (unitList.Count>0)
                battleUnitBase.SetAnimation(AnimationType.Attack);
            
            EffectManager.Instance.GetEffectPool(EffectType.Hero01Skill).Cycle(_skillEffect);
        }

        public override void Update()
        {

        }
    }
}