using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FightBattle.Buff;
using FSM;
using Helpers;
using Managers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace FightBattle
{
    public abstract class BattleUnitBase : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected AnimationEventHelp animationEventHelp;
        [SerializeField] public GameObject hpObj;
        [SerializeField] protected Slider hpSlider;
        [SerializeField] public Transform hitPoint;
        [SerializeField] public Transform effectTrs;
        [NonSerialized] public NavMeshAgent navMeshAgent;

        // 基本属性 读表
        public AttributeInfo AttributeInfo { get; protected set; }

        // 当前士兵组合id xxx xx 前三位 battleUnitId 后两位等级
        protected int SoliderCombineId;

        protected int HeroLev;
        // buff列表
        private readonly List<BuffBase> _buffList = new List<BuffBase>();

        protected FsmSystem Fsm;

        public AnimationEventHelp AnimationEventHelp => animationEventHelp;
        protected virtual void Awake()
        {
            _buffList.Clear();
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
        }

        public void Start()
        {
            InitNavInfo();

            InitFsm();
        }

        protected virtual void FixedUpdate()
        {
            ReFreshBuff();
        }

        protected virtual void Update()
        {

        }

        public virtual void SetData(int soliderCombineId)
        {
            SoliderCombineId = soliderCombineId;

            HeroLev = IDParseHelp.GetBattleLev(SoliderCombineId);
        }

        private void InitNavInfo()
        {
            if (AttributeInfo != null)
            {
                navMeshAgent.speed = AttributeInfo.MoveSpeed;
            }
        }

        protected virtual void InitFsm()
        {

        }

        // 获取最近攻击目标
        public virtual BattleUnitBase GetNearestTarget()
        {
            return default;
        }

        public void SetAnimation(AnimationType animationType)
        {
            animator.SetTrigger(animationType.ToString());

            // 通过修改状态机 来根据当前攻速设置 普通攻击速度
            animator.speed = animationType == AnimationType.Attack ? AttributeInfo.AtkSpeed : 1;
        }

        public AnimationType GetAnimationState()
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(AnimationType.Idle.ToString()))
                return AnimationType.Idle;

            if (stateInfo.IsName(AnimationType.Victory.ToString()))
                return AnimationType.Victory;

            if (stateInfo.IsName(AnimationType.Run.ToString()))
                return AnimationType.Run;

            if (stateInfo.IsName(AnimationType.Attack.ToString()))
                return AnimationType.Attack;

            if (stateInfo.IsName(AnimationType.Skill1.ToString()))
                return AnimationType.Skill1;

            if (stateInfo.IsName(AnimationType.Skill2.ToString()))
                return AnimationType.Skill2;

            if (stateInfo.IsName(AnimationType.Skill3.ToString()))
                return AnimationType.Skill3;

            if (stateInfo.IsName(AnimationType.Victory.ToString()))
                return AnimationType.Victory;

            if (stateInfo.IsName(AnimationType.Die.ToString()))
                return AnimationType.Die;

            return AnimationType.none;
        }

        public virtual void Attack(BattleUnitBase target)
        {

        }

        public virtual void HpChange(float damageVal)
        {
            FightManager.Instance.BloodPool.Spawn().Init(hitPoint.position, (int)damageVal);
        }

        // 停止所有行为
        public void StopAllBehavior()
        {
            navMeshAgent.speed = 0;
            animator.speed = 0;
        }

        // 恢复行为动作
        public void ResumeBehavior()
        {
            navMeshAgent.speed = AttributeInfo.MoveSpeed;
            animator.speed = GetAnimationState() == AnimationType.Attack ? AttributeInfo.AtkSpeed : 1;
        }

        // --------------------------- BUFF 相关 ---------------------------
        public void AddBuff(BuffBase buffBase)
        {
            _buffList.Add(buffBase);
            buffBase.OnAdd();
        }

        public void RemoveBuff(BuffBase buffBase)
        {
            _buffList.Remove(buffBase);
            buffBase.OnRemove();
        }

        private void ReFreshBuff()
        {
            for (int i = _buffList.Count - 1; i >= 0; i--)
            {
                _buffList[i].OnUpdate();
            }
        }

        public bool ContainBuff(BuffType type)
        {
            return _buffList.Any(t => t.mBuffType == type);
        }
        // --------------------------- BUFF 相关 ---------------------------
    }

    public class AttributeInfo
    {
        // 基础配置信息
        public AttributeConfig AttributeConfig { get; }

        // ------变动数据
        //当前血量
        public float Hp;
        public float AtkSpeed;
        public float MoveSpeed;
        public int Atk; // 攻击力
        public float Def;

        public int AtkDistance;
        // ------变动数据

        // ------------------- 自身状态 -------------------

        // 不可选中状态 一般为释放技能时候 设置不可选中
        public bool NotSelected = false;
        
        // 大于100 被冰冻 
        public int FreezeVal { get; set; }
        public bool IsFreeze { get; set; }

        // 是否眩晕
        public bool IsDizzy { get; set; }

        // 是否处于无敌状态
        public bool IsGodDefend { get; set; }
        // ------------------- 自身状态 -------------------

        public AttributeInfo(AttributeConfig attributeConfig)
        {
            AttributeConfig = attributeConfig;

            // ------变动数据赋值
            Hp = AttributeConfig.hp;
            AtkSpeed = AttributeConfig.atkSpeed;
            MoveSpeed = AttributeConfig.moveSpeed;
            AtkDistance = AttributeConfig.atkDistance;
            Atk = AttributeConfig.atk;
            Def = AttributeConfig.def;
        }

        public static AttributeInfo CreateAttributeInfo(int id)
        {
            var baseAttributeData = AttributeConfig.CreateBaseAttribute(id);
            AttributeInfo attributeInfo = new AttributeInfo(baseAttributeData);

            return attributeInfo;
        }
    }

    /// <summary>
    /// 基础属性 配置获得
    /// </summary>
    public class AttributeConfig
    {
        public int dataId; // 英雄id，或敌人id
        public BattleUnitType battleUnitType;
        public NormalAtkType atkType;
        public int hp;
        public int atk; // 攻击力

        // 攻击距离
        public int atkDistance;

        // 如果aoe攻击
        public int atkAngle; // 角度
        public int atkRadius; // 半径

        public float atkSpeed;

        public float moveSpeed;
        public int[] skillIds;
        
        public float def;
        public string extraPara;
        public static AttributeConfig CreateBaseAttribute(int id)
        {
            var data = ExcelManager.Instance.GetExcelItem<BattleUnitExcelData, BattleUnitExcelItem>(id);

            var baseAttributeData = new AttributeConfig()
            {
                dataId = id,
                atkType = data.NormalAtkType,
                battleUnitType = data.UnitType,
                hp = data.Hp,
                atk = data.Atk,
                atkDistance = data.AtkDistance,
                atkSpeed = data.AtkSpeed,
                moveSpeed = data.MoveSpeed,
                atkAngle = data.Angle,
                atkRadius = data.Radius,
                def = data.Def,
                skillIds = data.SkillIds,
                extraPara = data.param
            };

            return baseAttributeData;
        }
    }
}
