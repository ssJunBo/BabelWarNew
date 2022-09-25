using Main.Game.Base;

namespace HotFix.Managers
{
    public class CardManager : Singleton<CardManager>
    {
        private Turn turn;



        public void ChangeTurn(Turn turn)
        {
            this.turn = turn;
        }
    }

    public enum Turn
    {
        Own,
        Enemy
    }
}