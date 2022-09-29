using Common;
using FightBattle;
using Helpers;

namespace FSM.State
{
    public class NoSelectedTargetState : FsmState
    {
        // 多少帧检测一次
        private const int IntervalFrame = 10;

        public NoSelectedTargetState(BattleUnitBase battleUnitBase, FsmSystem fsmSystem) : base(battleUnitBase,
            fsmSystem)
        {
            stateId = StateID.NoSelectedTarget;
        }

        public override void DoBeforeEnter(params object[] param)
        {
            base.DoBeforeEnter();
            _curFrame = 0;
            battleUnitBase.SetAnimation(AnimationType.Idle);
        }

        private BattleUnitBase _target;
        private int _curFrame;

        public override void Update()
        {
            if (_curFrame >= IntervalFrame)
            {
                _target = battleUnitBase.GetNearestTarget();
                if (_target != null && !_target.AttributeInfo.NotSelected)
                {
                    var distance = battleUnitBase.transform.GetDistanceToOnePoint(_target.transform);

                    fsmSystem.PerformTransition(battleUnitBase.AttributeInfo.AttributeConfig.atkDistance >= distance ? Transition.InAttackRange : Transition.OutOfRange, _target);
                }

                _curFrame = 0;
            }

            _curFrame++;
        }
    }
}