using System.Collections.Generic;
using _GameBase;
using Common;
using Data.Account;
using Tools;

namespace Managers
{
    public class CardManager : Singleton<CardManager>
    {
        private Round _round;
        public Round Round => _round;
        public void ChangeRound(Round round)
        {
            _round = round;

            IssueCard();
        }

        // 当前自己拥有的卡片
        public  List<CardInfo> CurOwnHaveCards = new();

        // 当前敌人手里拥有的卡片
        public  List<CardInfo> CurEnemyHaveCards = new();
        
        /// <summary>
        /// 发牌
        /// </summary>
        private void IssueCard()
        {
            switch (_round)
            {
                case Round.Own:
                    var newCards= GetOwnCard();
                    foreach (var card in newCards)
                    {
                        CurOwnHaveCards.Add(card);
                    }

                    EventManager.DispatchEvent(EventMessageType.IssueCard,newCards);
                    break;
                case Round.Enemy:
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
        
        // ----------------------- 敌人卡牌逻辑 ---------------------------
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

        public CardInfo GetOneCardPlay()
        {
            CardInfo resultCard = default;
            if (CurEnemyHaveCards.Count > 0)
            {
                resultCard = CurEnemyHaveCards[0];
                CurEnemyHaveCards.RemoveAt(0);
            }

            return resultCard;
        }
        
        // ----------------------- 敌人卡牌逻辑 ---------------------------
        
        public CardExcelItem GetCardExcelItem(CardInfo cardInfo)
        {
            return ExcelManager.Instance.GetExcelItem<CardExcelData, CardExcelItem>(cardInfo.ID);
        }
    }

    /// <summary>
    /// 回合
    /// </summary>
    public enum Round
    {
        Own,
        Enemy
    }
}