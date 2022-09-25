using HotFix.Common;
using HotFix.Data;
using HotFix.Data.Account;
using HotFix.FSM;
using HotFix.FSM.State;
using HotFix.Helpers;
using HotFix.Managers;
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

            EventManager.Subscribe<int>(EventMessageType.FightResult, FightResultRefresh);
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
            chaseState.AddTransition(Transition.ChasingTargetDie, StateID.Chase);
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
            AttributeInfo = AttributeInfo.CreateAttributeInfo(IDParseHelp.GetBattleUnitId(base.SoliderCombineId));
        }

        public override BattleUnitBase GetNearestTarget()
        {
            return BattleUnitHelper.GetMinDistanceUnit(this, FightManager.Instance.OwnUnitLis);
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
                Fsm.PerformTransition(Transition.NoCanSelected);
                return;
            }

            // 最近敌人与自身距离
            var dis = transform.GetDistanceToOnePoint(curTarget.transform);

            if (AttributeInfo.AtkDistance > dis)
            {
                if (Fsm.CurrentState is AttackState atkState)
                    atkState.ChangeTarget(curTarget);
            }
            else
            {
                Fsm.PerformTransition(Transition.OutOfRange, curTarget);
            }
        }

        private void FightResultRefresh(int result)
        {
            if (result==0)
            {
                Fsm.PerformTransition(Transition.AllTargetDie);
            }
        }


        protected virtual void OnDestroy()
        {
            EventManager.UnSubscribe<int>(EventMessageType.FightResult, FightResultRefresh);
            Fsm?.SetNullState();
        }
    }
}