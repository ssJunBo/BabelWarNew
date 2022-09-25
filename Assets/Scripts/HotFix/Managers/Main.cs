using Main.Game.ResourceFrame;
using UnityEngine;

namespace HotFix.Managers
{
    public class Main : MonoBehaviour
    {
        [Header("倍速")] public int gameSpeed = 1;

        void Start()
        {
            if (!GameManager.IsInitGameManager)
            {
                var obj = ResManager.Instance.LoadResource("Prefabs/GameManager");
                Instantiate(obj);
                GameManager.IsInitGameManager = true;
            }

            Time.timeScale = gameSpeed;
        }
    }
}
