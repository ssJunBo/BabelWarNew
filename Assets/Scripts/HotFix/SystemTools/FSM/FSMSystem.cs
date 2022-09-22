using System.Collections.Generic;
using UnityEngine;

namespace HotFix.SystemTools.FSM
{
    public class FsmSystem
    {
        private readonly Dictionary<StateID, FsmState> _stateDic = new Dictionary<StateID, FsmState>();
        private StateID _currentStateID;
        public FsmState CurrentState { get; private set; }

        /// <summary>
        /// 更新npc的动作
        /// </summary>
        public void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }

        public void SetStartState(StateID id, params object[] param)
        {
            FsmState state = _stateDic[id];
            CurrentState = state;
            _currentStateID = state.StateID;
            CurrentState.DoBeforeEnter(param);
        }

        public void SetNullState()
        {
            CurrentState = null;
        }

        /// <summary>
        /// 添加新状态
        /// </summary>
        public void AddState(FsmState state)
        {
            if (state == null)
            {
                Debug.LogError("FSMState不能为空");
                return;
            }

            if (CurrentState == null)
            {
                CurrentState = state;
                _currentStateID = state.StateID;
            }

            if (_stateDic.ContainsKey(state.StateID))
            {
                Debug.LogError("状态" + state.StateID + "已经存在，无法重复添加");
                return;
            }

            _stateDic.Add(state.StateID, state);
        }

        /// <summary>
        /// 删除状态
        /// </summary>
        public void DeleteState(StateID stateID)
        {
            if (stateID == StateID.NullState)
            {
                Debug.LogError("无法删除空状态");
                return;
            }

            if (!_stateDic.ContainsKey(stateID))
            {
                Debug.LogError("无法删除不存在的状态");
                return;
            }

            _stateDic.Remove(stateID);
        }

        /// <summary>
        /// 执行过渡条件满足时对应状态该做的事
        /// </summary>
        public void PerformTransition(Transition transition, params object[] param)
        {
            if (transition == Transition.NullTransition)
            {
                Debug.LogError("无法执行空的转换条件");
                return;
            }

            StateID id = CurrentState.GetOutputState(transition);
            if (id == StateID.NullState)
            {
                Debug.LogWarning("当前状态" + _currentStateID + "无法根据转换条件" + transition + "发生转换");
                return;
            }

            if (!_stateDic.ContainsKey(id))
            {
                Debug.LogError("在状态机里面不存在状态" + id + ",无法进行状态转换");
                return;
            }

            FsmState state = _stateDic[id];
            CurrentState.DoAfterLeave();
            CurrentState = state;
            _currentStateID = state.StateID;
            CurrentState.DoBeforeEnter(param);
        }
    }
}

