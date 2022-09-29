using Common;
using FightBattle;
using UnityEngine;

namespace FSM.State
{
    /// <summary>
    /// 巡逻状态
    /// </summary>
    public class PatrolState : FsmState
    {
        // 自身transform
        private readonly Transform selfTransform;

        // 敌人只有自身范围巡逻逻辑
        private readonly Vector3 oriSelfV3;

        public PatrolState(BattleUnitBase battleUnitBase, FsmSystem fsmSystem) : base(battleUnitBase, fsmSystem)
        {
            selfTransform = battleUnitBase.transform;

            var position = selfTransform.position;

            oriSelfV3 = new Vector3(position.x, position.y, position.z);

            stateId = StateID.Patrol;
        }

        public override void DoBeforeEnter(params object[] param)
        {
            base.DoBeforeEnter();

            battleUnitBase.SetAnimation(AnimationType.Run);
        }

        private Vector3 currentPatrolPoint;
        private bool _isNeedGeneratePoint = true;

        /// <summary>
        /// 当前状态所做的事,巡逻
        /// </summary>
        public override void Update()
        {
            // if (isNeedGeneratePoint)
            // {
            //     isNeedGeneratePoint = false;
            //     currentPatrolPoint =
            //         GeometryHelper.RandomPointOnOnCircle(oriSelfV3, Random.Range(0, 361),
            //             battleUnitBase.BaseAttribute.patrolRange);
            //
            //     while (currentPatrolPoint.x > GameConst.MapWidth / 2
            //            || currentPatrolPoint.x < -GameConst.MapWidth / 2
            //            || currentPatrolPoint.y > GameConst.MapHeight / 2
            //            || currentPatrolPoint.y < -GameConst.MapHeight / 2)
            //     {
            //         currentPatrolPoint =
            //             GeometryHelper.RandomPointOnOnCircle(oriSelfV3, Random.Range(0, 361),
            //                 battleUnitBase.BaseAttribute.patrolRange);
            //     }
            //
            //     battleUnitBase.transform.LookAt(currentPatrolPoint);
            //
            //     battleUnitBase.navMeshAgent.destination = currentPatrolPoint;
            // }

            // 单位与当前巡逻点小于一定距离，随机新的点
            // if (Vector3.Distance(battleUnitBase.transform.position, currentPatrolPoint) < GameConst.PatrolChangeVal)
            // {
            //     isNeedGeneratePoint = true;
            // }

            // TODO 配置数据
            // 如果当前单位 与 目标位置小于 当前单位可视范围 则当前单位开始切换状态 追赶
            // if (Vector3.Distance(玩家位置, battleUnit.transform.position) < battleUnit.BaseAttribute.VisibleRange)
            // {
            //     FSmSystem.PerformTransition(Transition.VisibleRange);
            // }
        }
    }
}