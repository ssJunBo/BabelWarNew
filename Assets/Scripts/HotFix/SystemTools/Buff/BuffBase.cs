using HotFix.Common;
using HotFix.FightBattle;

namespace HotFix.SystemTools.Buff
{
    public class BuffBase
    {
        // Buff的类型
        public virtual BuffType mBuffType { get; set; }

        // 计时器
        protected float timer;

        // Buff持续时间,我们约定,Buff时间为0,则为瞬时Buff,只执行OnAdd
        protected float mLength;

        // 所归属的实体
        protected readonly BattleUnitBase battleUnit;

        protected BuffBase(BattleUnitBase battleUnit, float length)
        {
            this.battleUnit = battleUnit;
            mLength = length;
        }

        /// <summary>
        /// 当添加到实体时执行逻辑
        /// </summary>
        public virtual void OnAdd()
        {
        }

        /// <summary>
        /// 跟随实体每帧更新
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// 当从实体移除时
        /// </summary>
        public virtual void OnRemove()
        {
        }
    }
}