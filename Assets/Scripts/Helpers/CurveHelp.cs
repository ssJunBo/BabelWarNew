using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// 曲线运动 抛物线
    /// </summary>
    public class ParaCurve
    {
        private readonly Transform _transform;

        public float g = 10f;

        private readonly float _acceleration;
        private readonly float _speedX; // 速度水平x分量
        private readonly float _speedZ; // 速度水平z分量
        private float _speedY; // 垂直方向分量

        private readonly float _durTime;

        public ParaCurve(Vector3 startPos, Vector3 endPos, float height, float durTime, Transform transform)
        {
            _durTime = durTime;

            _transform = transform;

            var position1 = startPos;
            var position = endPos;

            float distanceX = position.x - position1.x;
            float distanceZ = position.z - position1.z;

            _speedX = distanceX / durTime;
            _speedZ = distanceZ / durTime;

            _speedY = 4 * height / durTime;
            _acceleration = _speedY / (0.5f * durTime);
            _transform.position = position1;

            _transform.rotation = Quaternion.LookRotation(new Vector3(_speedX, _speedY, _speedZ), Vector3.up);
        }

        private float _curTime;

        // 放在fix update中
        public void Update()
        {
            _curTime += Time.deltaTime;

            if (_curTime < _durTime-0.02f)//  -0.02 增加偏移值
            {
                _speedY -= _acceleration * Time.fixedDeltaTime;
                _transform.rotation = Quaternion.LookRotation(new Vector3(_speedX, _speedY, _speedZ), Vector3.up);

                float speed = new Vector3(_speedX, _speedY, _speedZ).magnitude;
                _transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);
            }
        }
    }
}