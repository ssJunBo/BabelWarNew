using System.Collections.Generic;
using HotFix.Common;
using HotFix.Data.Account;
using HotFix.Tools;
using Main.Game.Base;

namespace HotFix.Managers
{
    public class CardManager : Singleton<CardManager>
    {
        private Turn _turn;
        public Turn Turn => _turn;
        public void ChangeTurn(Turn turn)
        {
            _turn = turn;

            IssueCard();
        }

        // 当前自己拥有的卡片
        public readonly List<CardInfo> CurOwnHaveCards = new();

        // 当前敌人拥有的卡片
        public readonly List<CardInfo> CurEnemyHaveCards = new();

        
        private void IssueCard()
        {
            switch (_turn)
            {
                case Turn.Own:
                    var newCards= GetOwnCard();
                    foreach (var card in newCards)
                    {
                        CurOwnHaveCards.Add(card);
                    }

                    EventManager.DispatchEvent(EventMessageType.IssueCard,newCards);
                    break;
                case Turn.Enemy:
                    var newEnemyCards= GetEnemyCard();
                    foreach (var card in newEnemyCards)
                    {
                        CurEnemyHaveCards.Add(card);
                    }
                    
                    EventManager.DispatchEvent(EventMessageType.IssueCard, newEnemyCards);
                    break;
            }
        }

        // 发卡逻辑
        // 从卡池随机卡 默认两张卡
        // 自身发卡
        private List<CardInfo> _ownCards;
        private int _ownIndex;
        private readonly List<CardInfo> _ownResultCards = new();

        private List<CardInfo> GetOwnCard()
        {
            _ownResultCards.Clear();

            // 卡池 重置
            if (_ownCards == null || _ownIndex + 1 >= _ownCards.Count)
            {
                _ownIndex = 0;
                _ownCards = DataManager.Instance.OwnCardsList.DisorderItems();
            }

            // 默认抽卡两张
            _ownResultCards.Add(_ownCards[_ownIndex]);
            _ownResultCards.Add(_ownCards[_ownIndex + 1]);

            _ownIndex += 2;

            return _ownResultCards;
        }
        
        // 敌人发卡 从敌人卡池里取出卡牌
        private List<CardInfo> _enemyCards;
        private int _enemyIndex;
        private readonly List<CardInfo> _enemyResultCards = new();

        private List<CardInfo> GetEnemyCard()
        {
            _enemyResultCards.Clear();

            // 卡池
            if (_enemyCards == null || _enemyIndex + 1 >= _enemyCards.Count)
            {
                _enemyIndex = 0;
                _enemyCards = DataManager.Instance.EnemyCardsList.DisorderItems();
            }

            // 默认抽卡两张
            _enemyResultCards.Add(_enemyCards[_enemyIndex]);
            _enemyResultCards.Add(_enemyCards[_enemyIndex + 1]);

            _enemyIndex += 2;

            return _enemyResultCards;
        }
    }

    public enum Turn
    {
        Own,
        Enemy
    }
}