using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FightBattle.Skill;
using FSM;
using FSM.State;
using Helpers;
using Managers;
using UnityEngine;

namespace FightBattle
{
    public class HeroUnitBase : BattleUnitBase
    {
        [SerializeField] private Transform passiveSkillTrs;
        
        protected SkillRunner SkillRunner;

        // 普攻完成回调
        protected Action NormalAttackComplete;

        protected override void Awake()
        {
            base.Awake();

            animationEventHelp.actDict["Attack"] = AttackCb;
            animationEventHelp.actDict["AttackComplete"] = AttackCompleteCb;

            EventManager.Subscribe(EventMessageType.FightResult, FightResultRefresh);
        }


        public override void SetData(int soliderCombineId)
        {
            base.SetData(soliderCombineId);

            AttributeInfo = AttributeInfo.CreateAttributeInfo(SoliderCombineId);

            SetSkillData();

            // 开启辅助线
            _openGizmos = true;

            passiveSkillTrs.gameObject.SetActive(PassiveSkillInfo != null && PassiveSkillInfo.OpenLevel <= HeroLev);
        }

        private float _curTime;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            // 更新技能容器
            SkillRunner?.Update();

            // 处理被动技能
            if (PassiveSkillInfo != null && PassiveSkillInfo.OpenLevel <= HeroLev)
            {
                _curTime += Time.fixedDeltaTime;

                if (_curTime >= 1f)
                {
                    // 触发掉血逻辑
                    for (var i = FightManager.Instance.EnemyUnitLis.Count - 1; i >= 0; i--)
                    {
                        var enemyBattleUnit = FightManager.Instance.EnemyUnitLis[i];
                        // 在攻击范围内 全部造成伤害一次
                        if (transform.GetDistanceToOnePoint(enemyBattleUnit.transform) <= PassiveSkillInfo.Radius)
                        {
                            DamageHelper.CauseDamage(enemyBattleUnit, PassiveSkillInfo.Damages);
                        }
                    }

                    _curTime = 0;
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            Fsm.Update();
        }

        protected override void InitFsm()
        {
            base.InitFsm();

            Fsm = new FsmSystem();

            FsmState victoryState = new VictoryState(this, Fsm);

            FsmState chaseState = new ChaseState(this, Fsm);
            chaseState.AddTransition(Transition.InAttackRange, StateID.Attack);
            chaseState.AddTransition(Transition.AllTargetDie, StateID.Victory);
            chaseState.AddTransition(Transition.NoCanSelected, StateID.NoSelectedTarget);

            FsmState attackState = new AttackState(this, Fsm);
            attackState.AddTransition(Transition.OutOfRange, StateID.Chase);
            attackState.AddTransition(Transition.AllTargetDie, StateID.Victory);
            attackState.AddTransition(Transition.NoCanSelected, StateID.NoSelectedTarget);

            FsmState noSelectedTargetState = new NoSelectedTargetState(this, Fsm);
            noSelectedTargetState.AddTransition(Transition.InAttackRange, StateID.Attack);
            noSelectedTargetState.AddTransition(Transition.OutOfRange, StateID.Chase);
            noSelectedTargetState.AddTransition(Transition.AllTargetDie, StateID.Victory);


            Fsm.AddState(victoryState);
            Fsm.AddState(chaseState);
            Fsm.AddState(attackState);
            Fsm.AddState(noSelectedTargetState);

            // 向目标移动 在攻击范围内开始 切换攻击状态
            Fsm.SetStartState(StateID.Chase, GetNearestTarget());
        }

        public override BattleUnitBase GetNearestTarget()
        {
            return BattleUnitHelper.GetMinDistanceUnit(this, FightManager.Instance.EnemyUnitLis);
        }

        protected virtual void OnDestroy()
        {
            EventManager.UnSubscribe(EventMessageType.FightResult, FightResultRefresh);
            if (Fsm != null)
            {
                Fsm.SetNullState();
                Fsm = null;
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

        private void FightResultRefresh(object arg)
        {
            int result = (int)arg;
            
            if(result==1) 
                Fsm.PerformTransition(Transition.AllTargetDie);
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
                NormalAttackComplete?.Invoke();

                DamageEnemy(_curTarget);

                if (_curTarget.AttributeInfo.Hp > 0)
                {
                    if (AttributeInfo.AtkDistance > transform.GetDistanceToOnePoint(_curTarget.transform))
                    {
                        SkillBase skill = null;

                        if (SkillRunner != null)
                        {
                            skill = SkillRunner.GetCurrentCanCastSkill();
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
                        if (Fsm.CurrentState is ChaseState state)
                        {
                            state.target = _curTarget;
                        }
                        else
                        {
                            Fsm.PerformTransition(Transition.OutOfRange, _curTarget);
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
                Fsm.PerformTransition(Transition.NoCanSelected);
                return;
            }

            // 最近敌人与自身距离
            var dis = transform.GetDistanceToOnePoint(_curTarget.transform);

            // 在攻击范围内
            if (AttributeInfo.AtkDistance > dis)
            {
                if (Fsm.CurrentState is AttackState atkState)
                {
                    atkState.ChangeTarget(_curTarget);
                }
            }
            else
            {
                Fsm.PerformTransition(Transition.OutOfRange, _curTarget);
            }
        }

        #region 辅助线

        private bool _openGizmos;
        
        protected virtual void OnDrawGizmos()
        {
            if (!_openGizmos) return;
            if (PassiveSkillInfo == null || PassiveSkillInfo.OpenLevel > HeroLev) return;

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