using HotFix.FightBattle;
using UnityEngine;

namespace HotFix.SystemTools.FSM.State
{
    public class AttackState : FsmState
    {
        public AttackState(BattleUnitBase battleUnitBase, FsmSystem fsmSystem) : base(battleUnitBase, fsmSystem)
        {
            stateId = StateID.Attack;
        }

        private BattleUnitBase _targetUnitBase;

        public override void DoBeforeEnter(params object[] param)
        {
            base.DoBeforeEnter();

            _targetUnitBase = param[0] as BattleUnitBase;

            battleUnitBase.navMeshAgent.isStopped = true;
            battleUnitBase.navMeshAgent.velocity = Vector3.zero;

            battleUnitBase.Attack(_targetUnitBase);
        }

        public void ChangeTarget(BattleUnitBase _targetUnitBase)
        {
            this._targetUnitBase = _targetUnitBase;
            battleUnitBase.Attack(_targetUnitBase);
        }
        
        public override void Update()
        {
            if (_targetUnitBase != null)
            {
                battleUnitBase.transform.LookAt(_targetUnitBase.transform);
            }
        }
    }
}