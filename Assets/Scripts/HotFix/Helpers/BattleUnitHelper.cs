using System.Collections.Generic;
using HotFix.FightBattle;
using UnityEngine;

namespace HotFix.Helpers
{
    public static class BattleUnitHelper
    {
        /// <summary>
        /// 获取最近敌人单位 无飞行单位
        /// </summary>
        public static BattleUnitBase GetMinDistanceUnit(BattleUnitBase ownUnitBase, List<BattleUnitBase> targetUnits)
        {
            float distance = float.MaxValue;
            BattleUnitBase targetBattleUnit = null;

            BattleUnitBase noSelectedBattleUnit = null;

            if (targetUnits!=null)
            {
                foreach (var battleUnit in targetUnits)
                {
                    if (battleUnit.AttributeInfo.NotSelected)
                    {
                        noSelectedBattleUnit = battleUnit;
                        continue;
                    }

                    if (battleUnit.AttributeInfo.Hp <= 0)
                        continue;

                    var position = ownUnitBase.transform.position;
                    var position1 = battleUnit.transform.position;
                    
                    var tmpDistance = Vector2.Distance(new Vector2(position.x, position.z), new Vector2(position1.x, position1.z));
                    
                    if (distance > tmpDistance)
                    {
                        distance = tmpDistance;
                        targetBattleUnit = battleUnit;
                    }
                }
            }

            if (targetBattleUnit == null && noSelectedBattleUnit != null)
            {
                targetBattleUnit = noSelectedBattleUnit;
            }

            return targetBattleUnit;
        }
    }
}