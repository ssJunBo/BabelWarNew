using System;
using Helpers;
using Managers;
using Pool;
using UnityEngine;

namespace FightBattle.Bullet
{
    public class AirArrow : PoolItemBase
    {
        [SerializeField] private GameObject airArrowBombPrefab;
        
        public override void OnSpawned()
        {
            base.OnSpawned();
            
            if (airArrowBombPrefab!=null) 
                airArrowBombPrefab.SetActive(false);
        }

        public override void OnCycle()
        {
            base.OnCycle();
            
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