using HotFix.Common;
using HotFix.FightBattle;
using UnityEngine;

namespace HotFix.FSM.State
{
    public class VictoryState : FsmState
    {
        public VictoryState(BattleUnitBase battleUnitBase, FsmSystem fsmSystem) : base(battleUnitBase, fsmSystem)
        {
            stateId = StateID.Victory;
        }

        public override void DoBeforeEnter(params object[] param)
        {
            base.DoBeforeEnter();

            if (battleUnitBase.GetAnimationState() != AnimationType.Victory)
            {
                battleUnitBase.SetAnimation(AnimationType.Victory);
            }

            battleUnitBase.navMeshAgent.isStopped = true;
            battleUnitBase.navMeshAgent.velocity = Vector3.zero;
        }

        public override void Update()
        {
            
        }
    }
}