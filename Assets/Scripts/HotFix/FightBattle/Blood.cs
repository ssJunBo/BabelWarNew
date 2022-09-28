using HotFix.Managers;
using HotFix.Pool;
using TMPro;
using UnityEngine;

namespace HotFix.FightBattle
{
    public class Blood : PoolItemBase
    {
        [SerializeField] private TextMeshPro _textMeshPro;

        public void Init(Vector3 targetPos, int val)
        {
            transform.position = targetPos;
            transform.rotation = FightManager.Instance.fightCamera.transform.rotation;

            _textMeshPro.text = $"{val}";

            TimerEventManager.Instance.DelaySeconds(1.5f, () =>
            {
                if (FightManager.Instance != null) // 防止退出战场 报null
                    FightManager.Instance.BloodPool.Cycle(this);
            });
        }
    }
}