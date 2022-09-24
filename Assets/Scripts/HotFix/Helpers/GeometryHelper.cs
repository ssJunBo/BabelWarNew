using HotFix.FightBattle;
using UnityEngine;

namespace HotFix.Helpers
{
    public static class GeometryHelper
    {
        #region 区域选敌

        /// <summary>
        /// 判断target是否在扇形区域内
        /// </summary>
        /// <param name="battleUnit">攻击者的transform信息</param>
        /// <param name="sectorAngle">扇形角度</param>
        /// <param name="sectorRadius">扇形半径</param>
        /// <param name="target">目标</param>
        /// <returns>目标target在扇形区域内返回true 否则返回false</returns>
        public static bool IsInSectorArea(this BattleUnitBase battleUnit, float sectorAngle, float sectorRadius,
            Transform target)
        {
            var transform = battleUnit.transform;

            var position = target.position;
            var position1 = transform.position;

            //攻击者位置指向目标位置的向量
            Vector2 direction = new Vector2(position.x, position.z) -
                                new Vector2(position1.x, position1.z);

            var forward = transform.forward;

            //点乘积结果
            float dot = Vector2.Dot(
                new Vector2(direction.normalized.x, direction.normalized.y),
                new Vector2(forward.x, forward.z));


            if (Mathf.Approximately(dot, 1))
            {
                return direction.magnitude < sectorRadius;
            }

            //反余弦计算角度
            float offsetAngle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            return offsetAngle < sectorAngle * .5f && direction.magnitude < sectorRadius;
        }

        // 是否在圆形区域内 
        public static bool InRoundArea(Vector2 centerPos, Vector2 targetPos, float radius)
        {
            float distance = Vector2.Distance(new Vector2(centerPos.x, centerPos.y),
                new Vector2(targetPos.x, targetPos.y));

            return distance < radius;
        }

        #endregion

        public static Vector3 RandomPointOnOnCircle(Vector3 selfPos, float angel, float distance)
        {
            var xDis = distance * Mathf.Sin(angel);
            var yDis = distance * Mathf.Cos(angel);

            var v2X = selfPos.x + xDis;
            var v2Y = selfPos.y + yDis;

            return new Vector3(v2X, 0, v2Y);
        }

        // 获取自身到一个点的距离 2维坐标系
        public static float GetDistanceToOnePoint(this Transform transform, Transform targetTrs)
        {
            var position = transform.position;
            var position1 = targetTrs.transform.position;

            return Vector2.Distance(new Vector2(position.x, position.z),
                new Vector2(position1.x, position1.z));
        }

        /// <summary>
        /// 用某个轴去朝向物体
        /// </summary>
        /// <param name="trSelf">朝向的本体</param>
        /// <param name="lookPos">朝向的目标</param>
        /// <param name="directionAxis">方向轴，取决于你用那个方向去朝向</param>
        public static void AxisLookAt(Transform trSelf, Vector3 lookPos, Vector3 directionAxis)
        {
            var rotation = trSelf.rotation;
            var targetDir = lookPos - trSelf.position;
            //指定哪根轴朝向目标,自行修改Vector3的方向
            var fromDir = trSelf.rotation * directionAxis;
            //计算垂直于当前方向和目标方向的轴
            var axis = Vector3.Cross(fromDir, targetDir).normalized;
            //计算当前方向和目标方向的夹角
            var angle = Vector3.Angle(fromDir, targetDir);
            //将当前朝向向目标方向旋转一定角度，这个角度值可以做插值
            trSelf.rotation = Quaternion.AngleAxis(angle, axis) * rotation;
            trSelf.localEulerAngles = new Vector3(0, trSelf.localEulerAngles.y, 0); //后来调试增加的，因为我想让x，z轴向不会有任何变化
        }


        #region 几何辅助线支持

        // 扇形画线
        public static void SectorLine(this Transform transform, int angle, float radius)
        {
            Gizmos.color = Color.red; // Color.cyan Color.red;

            Vector3 origin = transform.position;

            var forward = transform.forward;

            Vector3 leftDir = Quaternion.AngleAxis(-angle / 2, Vector3.up) * forward;
            Vector3 rightDir = Quaternion.AngleAxis(angle / 2, Vector3.up) * forward;

            Vector3 currentP = origin + leftDir * radius;
            Vector3 oldP;
            if (angle != 360)
            {
                Gizmos.DrawLine(origin, currentP);
            }

            for (int i = 0; i < angle / 10; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(10 * i, Vector3.up) * leftDir;
                oldP = currentP;
                currentP = origin + dir * radius;
                Gizmos.DrawLine(oldP, currentP);
            }

            oldP = currentP;
            currentP = origin + rightDir * radius;
            Gizmos.DrawLine(oldP, currentP);

            if (angle != 360)
            {
                Gizmos.DrawLine(currentP, origin);
            }
        }

        public static void RoundLine(this Transform transform, float radius)
        {
            Gizmos.color = Color.red;
            Vector3 origin = transform.position;

            var currentP = origin + transform.forward * radius;

            for (int i = 0; i < 360 / 10; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(10 * i, Vector3.up) * transform.forward;
                var oldP = currentP;
                currentP = origin + dir * radius;
                Gizmos.DrawLine(oldP, currentP);
            }
        }

        public static void RoundLine(Vector3 centerTrs, float radius, Vector3 direction)
        {
            Gizmos.color = Color.red;

            Vector3 origin = centerTrs;

            var currentP = origin + direction * radius;

            for (int i = 0; i < 360 / 3; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(10 * i, Vector3.up) * direction;
                var oldP = currentP;
                currentP = origin + dir * radius;
                Gizmos.DrawLine(oldP, currentP);
            }
        }

        #region old line

        // public static void SetLineInfo(this Transform transform, float angle, float radius, bool _flag)
        // {
        //     Handles.color = _flag ? Color.cyan : Color.red;
        //
        //     float selfAngle = transform.rotation.eulerAngles.y;
        //
        //     var position1 = transform.position;
        //     
        //     float x1 = radius * Mathf.Sin((angle / 2f - selfAngle) * Mathf.Deg2Rad);
        //     float y1 = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x1, 2f));
        //     Vector3 a = new Vector3(position1.x - x1, position1.y, position1.z + y1);
        //  
        //     
        //     float x2 = radius * Mathf.Sin((angle / 2f + selfAngle) * Mathf.Deg2Rad);
        //     float y2 = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x2, 2f));
        //     Vector3 b = new Vector3(position1.x + x2, position1.y, position1.z + y2);
        //
        //     Handles.DrawLine(position1, a);
        //     Handles.DrawLine(position1, b);
        //     
        //
        //     float half = angle / 2;
        //     for (int i = 0; i < half; i++)
        //     {
        //         x1 = radius * Mathf.Sin((half - i) * Mathf.Deg2Rad);
        //         y1 = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x1, 2f));
        //         a = new Vector3(position1.x - x1, position1.y, position1.z + y1);
        //         
        //         x1 = radius * Mathf.Sin((half - i - 1) * Mathf.Deg2Rad);
        //         y1 = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x1, 2f));
        //         b = new Vector3(position1.x - x1, position1.y, position1.z + y1);
        //
        //         Handles.DrawLine(a, b);
        //     }
        //
        //     for (int i = 0; i < half; i++)
        //     {
        //         x2 = radius * Mathf.Sin((half - i) * Mathf.Deg2Rad);
        //         y2 = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x2, 2f));
        //         a = new Vector3(position1.x + x2, position1.y, position1.z + y2);
        //         
        //         x2 = radius * Mathf.Sin((half - i - 1) * Mathf.Deg2Rad);
        //         y2 = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x2, 2f));
        //         b = new Vector3(position1.x + x2, position1.y, position1.z + y2);
        //
        //         Handles.DrawLine(a, b);
        //     }
        // }

        #endregion

        #endregion
    }
}