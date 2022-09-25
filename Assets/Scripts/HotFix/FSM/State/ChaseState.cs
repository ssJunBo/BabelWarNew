using HotFix.Common;
using HotFix.FightBattle;
using UnityEngine;

namespace HotFix.FSM.State
{
    /// <summary>
    /// 追赶状态
    /// </summary>
    public class ChaseState : FsmState
    {
        public ChaseState(BattleUnitBase battleUnitBase, FsmSystem fsmSystem) : base(battleUnitBase, fsmSystem)
        {
            stateId = StateID.Chase;
        }

        public override void DoBeforeEnter(params object[] param)
        {
            base.DoBeforeEnter();

            target = param[0] as BattleUnitBase;

            if(battleUnitBase.GetAnimationState()!=AnimationType.Run) 
                battleUnitBase.SetAnimation(AnimationType.Run);

            battleUnitBase.navMeshAgent.isStopped = false;
            battleUnitBase.navMeshAgent.velocity = Vector3.one;

            if (target != null) 
                battleUnitBase.navMeshAgent.destination = target.transform.position;
        }

        public BattleUnitBase target;

        private void SetChaseTarget()
        {
            // 重新找目标
            target = battleUnitBase.GetNearestTarget();

            // 仍为空则切换胜利状态
            if (target == null)
                return;
            
            if (target.AttributeInfo.NotSelected)
            {
                fsmSystem.PerformTransition(Transition.NoCanSelected);
                return;
            }

            battleUnitBase.navMeshAgent.destination = target.transform.position;
        }

        private void Check()
        {
            var curTarget= battleUnitBase.GetNearestTarget();

            if (curTarget!=null)
            {
                target = curTarget;
                battleUnitBase.navMeshAgent.destination = target.transform.position;
            }
            
            battleUnitBase.transform.LookAt(target.transform);

            if (Vector3.Distance(target.transform.position, battleUnitBase.transform.position) <
                battleUnitBase.AttributeInfo.AtkDistance)
            {
                fsmSystem.PerformTransition(Transition.InAttackRange, target);
            }

            // 到达目标点但是敌人移动 导致攻击距离不够 此时更新敌人位置
            if (battleUnitBase.navMeshAgent.remainingDistance < 100)
            {
                battleUnitBase.navMeshAgent.destination = target.transform.position;
            }
        }

        /// <summary>
        /// 当前状态所做的事，追赶
        /// </summary>
        public override void Update()
        {
            // 不需每帧去设置
            if (target==null)
            {
                SetChaseTarget();
            }

            Check();
        }
    }
}