using Common;
using FightBattle.Bullet;
using FSM;
using FSM.State;
using Helpers;
using Managers;
using Pool;
using UnityEngine;

namespace FightBattle.Enemy
{
    public class Archer : EnemyUnitBase
    {
        [SerializeField] private AirArrow airArrowPrefab;

        private ObjectPool<AirArrow> _objectPoolArrow;
        
        protected override void Awake()
        {
            base.Awake();

            _objectPoolArrow = new ObjectPool<AirArrow>(airArrowPrefab, bulletPos);
        }

        protected override void AttackCb()
        {
            var nearlyUnit= GetNearestTarget();// 获取最近敌人单位
            
            if (curTarget != nearlyUnit)
            {
                UpdateTarget();
            }
            else
            {
                if (curTarget!=null)
                {
                    AudioManager.Instance.PlaySound("archer_attack_5", 0.2f);
                  
                    var endPoint =   curTarget.hitPoint != null ? curTarget.hitPoint.transform.position : curTarget.transform.position;

                    AirArrow bullet = _objectPoolArrow.Spawn();
                    bullet.Init(bulletPos.position, endPoint, 2, 0.3f, () =>
                    {
                        if (bullet != null) _objectPoolArrow.Cycle(bullet);
                    });
                }
            
            }
        }

        protected override void AttackCompleteCb()
        {
            if (curTarget == null || curTarget.AttributeInfo.NotSelected)
            {
                UpdateTarget();
            }
            else
            {
                CauseDamage(curTarget);

                if (curTarget.AttributeInfo.Hp > 0)
                {
                    if (AttributeInfo.AtkDistance > transform.GetDistanceToOnePoint(curTarget.transform))
                    {
                        // 如果敌人活着 继续普通攻击
                        SetAnimation(AnimationType.Attack);
                    }
                    else
                    {
                        if (Fsm.CurrentState is ChaseState chaseState)
                        {
                            chaseState.target = curTarget;
                        }
                        else
                        {
                            Fsm.PerformTransition(Transition.OutOfRange, curTarget);
                        }
                    }
                }
                else
                {
                    UpdateTarget();
                }
            }
        }

        protected virtual void CauseDamage(BattleUnitBase targetUnit)
        {
            this.CauseNormalDamage(targetUnit);
        }

        protected override void OnDestroy()
        {
            _objectPoolArrow.DestroyAllItem();

            base.OnDestroy();
        }

    }
}
