using Managers;
using UnityEngine;

namespace Helpers
{
    public class HpHelp : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation = FightManager.Instance.fightCamera.transform.rotation;
        }
    }
}
