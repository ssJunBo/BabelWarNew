using HotFix.Managers;
using UnityEngine;

namespace HotFix.Helpers
{
    public class HpHelp : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = FightManager.Instance.fightCamera.transform.rotation;
        }
    }
}
