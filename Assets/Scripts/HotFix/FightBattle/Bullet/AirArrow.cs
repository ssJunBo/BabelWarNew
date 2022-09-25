using System;
using HotFix.Helpers;
using HotFix.Managers;
using HotFix.Pool;
using UnityEngine;

namespace HotFix.FightBattle.Bullet
{
    public class AirArrow : PoolItemBase
    {
        [SerializeField] private GameObject airArrowBombPrefab;
        
        public override void OnSpawned()
        {
            gameObject.SetActive(true);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;

            if (airArrowBombPrefab!=null) 
                airArrowBombPrefab.SetActive(false);
        }

        public override void OnCycle()
        {
            gameObject.SetActive(false);
            
            if (airArrowBombPrefab!=null) 
                airArrowBombPrefab.SetActive(false);
        }
     
        private ParaCurve _paraCurve;
        public void Init(Vector3 startPos, Vector3 endPos, float height, float durTime,Action action)
        {
            _paraCurve = new ParaCurve(startPos, endPos, height, durTime, transform);
            TimerEventManager.Instance.DelaySecondsTimeScale(durTime, () =>
            {
                if (airArrowBombPrefab!=null)
                {
                    airArrowBombPrefab.SetActive(true);
                    TimerEventManager.Instance.DelaySecondsTimeScale(0.6f, () =>
                    {
                        action?.Invoke();
                    });
                }
                else
                {
                    action?.Invoke();
                }
            });
        }

        private void FixedUpdate()
        {
            _paraCurve.Update();
        }
    }
}