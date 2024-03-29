﻿namespace Helpers
{
    public static class IDParseHelp
    {
        public static int GetBattleUnitId(int combinationId)
        {
            return combinationId / 100;
        }
        
        public static int GetBattleLev(int combinationId)
        {
            return combinationId % 100;
        }
        
        public static int GetCardId(int combinationId)
        {
            return combinationId / 100;
        }
        
        public static int GetCardLev(int combinationId)
        {
            return combinationId % 100;
        }
    }
}