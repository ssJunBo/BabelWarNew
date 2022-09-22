using HotFix.Common;
using HotFix.Data;
using HotFix.Data.Account;
using HotFix.Helpers;
using HotFix.Managers;
using HotFix.SystemTools.EventSys;
using HotFix.SystemTools.FSM;
using HotFix.SystemTools.FSM.State;
using UnityEngine;

namespace HotFix.FightBattle
{
    public class EnemyUnitBase : BattleUnitBase
    {
        [SerializeField] protected Transform bulletPos;

        // 当前 目标
        protected BattleUnitBase curTarget;
        
        protected override void Awake()
        {
            base.Awake();
            
            animationEventHelp.actDict["Attack"] = AttackCb;
            animationEventHelp.actDict["AttackComplete"] = AttackCompleteCb;

            EventManager.Subscribe(EventMessageType.Defeat, DefeatedRefresh);
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
            chaseState.AddTransition(Transition.ChasingTargetDie, StateID.Chase);
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
        
        public override void Attack(BattleUnitBase target)
        {
            // 初始化目标
            curTarget = target;

            SetAnimation(AnimationType.Attack);
        }

        public override void HpChange(float damageVal)
        {
            base.HpChange(damageVal);
            
            AttributeInfo.Hp += damageVal;
            if (AttributeInfo.Hp <= 0)
            {
                FightManager.Instance.RemoveDieEnemy(this);
                // 死亡
                Destroy(gameObject);
            }
            else
            {
                hpSlider.value = AttributeInfo.Hp / AttributeInfo.AttributeConfig.hp;
            }
        }

        public override void SetData(int soliderCombineId)
        {
            base.SetData(soliderCombineId);
            AttributeInfo = AttributeInfo.CreateAttributeInfo(IDParseHelp.GetBattleUnitId(base.soliderCombineId));
        }

        public override BattleUnitBase GetNearestTarget()
        {
            return TargetHelper.GetMinDistanceUnit(this, FightManager.Instance.ownUnitLis);
        }

        protected virtual void AttackCb()
        {
          
        }

        protected virtual void AttackCompleteCb()
        {
        }

        protected void UpdateTarget()
        {
            curTarget = GetNearestTarget();

            if (curTarget == null)
            {
                return;
            }

            if (curTarget.AttributeInfo.NotSelected)
            {
                fsm.PerformTransition(Transition.NoCanSelected);
                return;
            }

            // 最近敌人与自身距离
            var dis = transform.GetDistanceToOnePoint(curTarget.transform);

            if (AttributeInfo.AtkDistance > dis)
            {
                if (fsm.CurrentState is AttackState atkState) 
                    atkState.ChangeTarget(curTarget);
            }
            else
            {
                fsm.PerformTransition(Transition.OutOfRange, curTarget);
            }
        }

        private void DefeatedRefresh()
        {
            fsm.PerformTransition(Transition.AllTargetDie);
        }


        protected virtual void OnDestroy()
        {
            EventManager.UnSubscribe(EventMessageType.Defeat, DefeatedRefresh);
            fsm.SetNullState();
        }
    }
}