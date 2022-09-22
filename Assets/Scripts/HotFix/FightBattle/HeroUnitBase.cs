﻿using System;
using System.Collections.Generic;
using System.Linq;
using HotFix.Common;
using HotFix.Data;
using HotFix.Data.Account;
using HotFix.FightBattle.Skill;
using HotFix.Helpers;
using HotFix.Managers;
using HotFix.SystemTools.EventSys;
using HotFix.SystemTools.FSM;
using HotFix.SystemTools.FSM.State;
using UnityEngine;

namespace HotFix.FightBattle
{
    public class HeroUnitBase : BattleUnitBase
    {
        protected SkillRunner skillRunner;

        // 普攻完成回调
        protected Action normalAttackComplete;

        public override void SetData(int soliderCombineId)
        {
            base.SetData(soliderCombineId);
          
            AttributeInfo = AttributeInfo.CreateAttributeInfo(IDParseHelp.GetBattleUnitId(base.soliderCombineId));

            SetSkillData();

            // 开启辅助线
            _openGizmos = true;
        }

        protected override void Awake()
        {
            base.Awake();

            animationEventHelp.actDict["Attack"] = AttackCb;
            animationEventHelp.actDict["AttackComplete"] = AttackCompleteCb;

            EventManager.Subscribe(EventMessageType.Victory, VictoryRefresh);
        }

        private float _curTime;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            // 更新技能容器
            skillRunner?.Update();

            // 处理被动技能
            if (PassiveSkillInfo != null)
            {
                _curTime += Time.fixedDeltaTime;

                if (_curTime >= 1f)
                {
                    // 触发掉血逻辑
                    for (var i = FightManager.Instance.enemyUnitLis.Count - 1; i >= 0; i--)
                    {
                        var enemyBattleUnit = FightManager.Instance.enemyUnitLis[i];
                        // 在攻击范围内 全部造成伤害一次
                        if (transform.GetDistanceToOnePoint(enemyBattleUnit.transform) <= PassiveSkillInfo.Radius)
                        {
                            this.CauseDamage(enemyBattleUnit, 100);
                        }

                    }

                    _curTime = 0;
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            fsm.Update();
        }

        protected override void InitFsm()
        {
            base.InitFsm();

            fsm = new FsmSystem();

            FsmState victoryState = new VictoryState(this, fsm);

            FsmState chaseState = new ChaseState(this, fsm);
            chaseState.AddTransition(Transition.InAttackRange, StateID.Attack);
            chaseState.AddTransition(Transition.AllTargetDie, StateID.Victory);
            chaseState.AddTransition(Transition.NoCanSelected, StateID.NoSelectedTarget);

            FsmState attackState = new AttackState(this, fsm);
            attackState.AddTransition(Transition.OutOfRange, StateID.Chase);
            attackState.AddTransition(Transition.AllTargetDie, StateID.Victory);
            attackState.AddTransition(Transition.NoCanSelected, StateID.NoSelectedTarget);

            FsmState noSelectedTargetState = new NoSelectedTargetState(this, fsm);
            noSelectedTargetState.AddTransition(Transition.InAttackRange, StateID.Attack);
            noSelectedTargetState.AddTransition(Transition.OutOfRange, StateID.Chase);
            noSelectedTargetState.AddTransition(Transition.AllTargetDie, StateID.Victory);


            fsm.AddState(victoryState);
            fsm.AddState(chaseState);
            fsm.AddState(attackState);
            fsm.AddState(noSelectedTargetState);

            // 向目标移动 在攻击范围内开始 切换攻击状态
            fsm.SetStartState(StateID.Chase, GetNearestTarget());
        }

        public override BattleUnitBase GetNearestTarget()
        {
            return TargetHelper.GetMinDistanceUnit(this, FightManager.Instance.enemyUnitLis);
        }

        protected virtual void OnDestroy()
        {
            EventManager.UnSubscribe(EventMessageType.Victory, VictoryRefresh);
            if (fsm != null)
            {
                fsm.SetNullState();
                fsm = null;
            }
        }

        // ------------技能相关
        private List<SkillExcelItem> SkillItem { get; set; }

        // 初始化技能数据
        private void SetSkillData()
        {
            SkillItem = new List<SkillExcelItem>();

            var data = ExcelManager.Instance.GetExcelItem<BattleUnitExcelData, BattleUnitExcelItem>(AttributeInfo
                .AttributeConfig.dataId);

            int[] skillArr = data.SkillIds;

            foreach (var skillId in skillArr)
            {
                var skillInfo = ExcelManager.Instance.GetExcelItem<SkillExcelData, SkillExcelItem>(skillId);
                SkillItem.Add(skillInfo);
            }
        }

        // 被动技能
        private SkillExcelItem _passiveSkillInfo;

        private SkillExcelItem PassiveSkillInfo
        {
            get { return _passiveSkillInfo ??= SkillItem.FirstOrDefault(t => t.SkillType == SkillType.Passive); }
        }

        // ------------技能相关

        private void VictoryRefresh()
        {
            fsm.PerformTransition(Transition.AllTargetDie);
        }

        public override void Attack(BattleUnitBase target)
        {
            _curTarget = target;

            SetAnimation(AnimationType.Attack);
        }

        public override void HpChange(float damageVal)
        {
            base.HpChange(damageVal);

            AttributeInfo.Hp += damageVal;

            if (AttributeInfo.Hp > AttributeInfo.AttributeConfig.hp)
                AttributeInfo.Hp = AttributeInfo.AttributeConfig.hp;

            if (AttributeInfo.Hp <= 0)
            {
                FightManager.Instance.RemoveDieHero(this);
                // 死亡
                Destroy(gameObject);
            }
            else
            {
                hpSlider.value = AttributeInfo.Hp / AttributeInfo.AttributeConfig.hp;
            }
        }

        private BattleUnitBase _curTarget;

        private void AttackCb()
        {
            var nearlyUnit = GetNearestTarget(); // 获取最近敌人单位

            if (_curTarget != nearlyUnit)
            {
                UpdateTarget();
            }

            GenerateNormalEffect();
        }

        protected virtual void GenerateNormalEffect()
        {

        }

        private void AttackCompleteCb()
        {
            if (_curTarget == null || _curTarget.AttributeInfo.NotSelected)
            {
                UpdateTarget();
            }
            else
            {
                normalAttackComplete?.Invoke();

                DamageEnemy(_curTarget);

                if (_curTarget.AttributeInfo.Hp > 0)
                {
                    if (AttributeInfo.AtkDistance > transform.GetDistanceToOnePoint(_curTarget.transform))
                    {
                        SkillBase skill = null;

                        if (skillRunner != null)
                        {
                            skill = skillRunner.GetCurrentCanCastSkill();
                        }

                        if (skill == null)
                        {
                            // 如果敌人活着 继续普通攻击
                            SetAnimation(AnimationType.Attack);
                        }
                        else
                        {
                            skill.CastSkill();
                        }
                    }
                    else
                    {
                        if (fsm.CurrentState is ChaseState state)
                        {
                            state.target = _curTarget;
                        }
                        else
                        {
                            fsm.PerformTransition(Transition.OutOfRange, _curTarget);
                        }
                    }
                }
                else
                {
                    UpdateTarget();
                }
            }
        }

        protected virtual void DamageEnemy(BattleUnitBase targetUnit)
        {
            // 普通攻击 常规英雄单体攻击 特殊范围伤害
            this.CauseNormalDamage(_curTarget);
        }

        private void UpdateTarget()
        {
            _curTarget = GetNearestTarget();

            if (_curTarget == null)
                return;

            if (_curTarget.AttributeInfo.NotSelected)
            {
                fsm.PerformTransition(Transition.NoCanSelected);
                return;
            }

            // 最近敌人与自身距离
            var dis = transform.GetDistanceToOnePoint(_curTarget.transform);

            // 在攻击范围内
            if (AttributeInfo.AtkDistance > dis)
            {
                if (fsm.CurrentState is AttackState atkState)
                {
                    atkState.ChangeTarget(_curTarget);
                }
            }
            else
            {
                fsm.PerformTransition(Transition.OutOfRange, _curTarget);
            }
        }

        #region 辅助线

        private bool _openGizmos;
        
        protected virtual void OnDrawGizmos()
        {
            if (!_openGizmos) return;

            if (AttributeInfo.AttributeConfig.atkType == NormalAtkType.Sector)
            {
                transform.SectorLine(AttributeInfo.AttributeConfig.atkAngle,
                    AttributeInfo.AttributeConfig.atkRadius);
            }

            // 绘制被动辅助线
            if (PassiveSkillInfo != null)
                transform.RoundLine(PassiveSkillInfo.Radius);
        }

        #endregion
    }
}