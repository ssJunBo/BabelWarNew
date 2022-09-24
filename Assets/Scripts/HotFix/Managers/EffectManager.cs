using System.Collections.Generic;
using HotFix.SystemTools.Pool;
using Main.Game.Base;
using UnityEngine;

namespace HotFix.Managers
{
    public class EffectManager : Singleton<EffectManager>
    {
        private const string FreezeEffectPath = "Prefabs/Effect/Buff/FreezeBuff";
        private const string DizzyEffectPath = "Prefabs/Effect/Buff/DizzyBuff";
        private const string Hero01SkillPath = "Prefabs/Effect/Hero01/TemplarSwordsManSkill";
        private const string Hero02NormalAttackPath = "Prefabs/Effect/Hero02/Hero02NormalAttack";
        private const string Hero03NormalAttackPath = "Prefabs/Effect/Hero03/WindSwordsman_Attack";

        private readonly Dictionary<EffectType, ObjectPool> _effectPools = new Dictionary<EffectType, ObjectPool>();

        public ObjectPool GetEffectPool(EffectType effectType)
        {
            if (_effectPools.TryGetValue(effectType, out var pool))
                return pool;

            string effectPath = default;
            switch (effectType)
            {
                case EffectType.FreezeBuff:
                    effectPath = FreezeEffectPath;
                    break;
                case EffectType.DizzyBuff:
                    effectPath = DizzyEffectPath;
                    break;
                case EffectType.Hero01Skill:
                    effectPath = Hero01SkillPath;
                    break;
                case EffectType.Hero02NormalAttack:
                    effectPath = Hero02NormalAttackPath;
                    break;
                case EffectType.Hero03NormalAttack:
                    effectPath = Hero03NormalAttackPath;
                    break;
            }

            if (effectPath == default)
                Debug.LogError("特效类型无对应路径资源，请配置！");

            var obj = Resources.Load<GameObject>(effectPath);
            pool = new ObjectPool(obj, FightManager.Instance.effectTrs);

            _effectPools.Add(effectType, pool);
            return pool;
        }
    }

    public enum EffectType
    {
        FreezeBuff,
        DizzyBuff,
        Hero01Skill,
        Hero02NormalAttack,
        Hero03NormalAttack,
    }
}