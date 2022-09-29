using System.Collections.Generic;
using FightBattle;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// 转换条件
    /// </summary>
    public enum Transition
    {
        NullTransition = 0, // 空的转换条件
        VisibleRange, // 目标在视野范围内
        InAttackRange, // 目标在攻击范围内
        OutOfRange, // 目标不在攻击范围内
        ChasingTargetDie, // 目标在攻击范围内
        AllTargetDie, // 全部目标死亡
        Victory, // 目标死亡
        NoCanSelected, // 当前无目标可选择
    }

    /// <summary>
    /// 当前状态
    /// </summary>
    public enum StateID
    {
        NullState, //空的状态
        Patrol, //巡逻状态
        Chase, //追赶状态
        Attack, //攻击状态
        Victory,//胜利状态
        NoSelectedTarget,// 当前所有目标不可选择
    }

    public abstract class FsmState
    {
        protected StateID stateId;
        public StateID StateID => stateId;
        protected readonly Dictionary<Transition, StateID> _transitionStateDic = new Dictionary<Transition, StateID>();
        protected readonly FsmSystem fsmSystem;

        protected readonly BattleUnitBase battleUnitBase;
        
        protected FsmState(BattleUnitBase battleUnitBase, FsmSystem fsmSystem)
        {
            this.battleUnitBase = battleUnitBase;
            this.fsmSystem = fsmSystem;
        }

        /// <summary>
        /// 添加转换条件
        /// </summary>
        /// <param name="trans">转换条件</param>
        /// <param name="id">转换条件满足时执行的状态</param>
        public void AddTransition(Transition trans, StateID id)
        {
            if (trans == Transition.NullTransition)
            {
                Debug.LogError("不允许NullTransition");
                return;
            }

            if (id == StateID.NullState)
            {
                Debug.LogError("不允许NullStateID");
                return;
            }

            if (_transitionStateDic.ContainsKey(trans))
            {
                Debug.LogError("添加转换条件的时候" + trans + "已经存在于transitionStateDic中");
                return;
            }

            _transitionStateDic.Add(trans, id);
        }

        /// <summary>
        /// 删除转换条件
        /// </summary>
        public void DeleteTransition(Transition trans)
        {
            if (trans == Transition.NullTransition)
            {
                Debug.LogError("不允许NullTransition");
                return;
            }

            if (!_transitionStateDic.ContainsKey(trans))
            {
                Debug.LogError("删除转换条件的时候" + trans + "不存在于transitionStateDic中");
                return;
            }

            _transitionStateDic.Remove(trans);
        }

        /// <summary>
        /// 获取当前转换条件下的状态
        /// </summary>
        public StateID GetOutputState(Transition trans)
        {
            if (_transitionStateDic.ContainsKey(trans))
            {
                return _transitionStateDic[trans];
            }

            return StateID.NullState;
        }

        /// <summary>
        /// 进入新状态之前做的事
        /// </summary>
        public virtual void DoBeforeEnter(params object[] param)
        {
        
        }

        /// <summary>
        /// 离开当前状态时做的事
        /// </summary>
        public virtual void DoAfterLeave()
        {
        }

        /// <summary>
        /// 当前状态所做的事
        /// </summary>
        public abstract void Update();
    }
}