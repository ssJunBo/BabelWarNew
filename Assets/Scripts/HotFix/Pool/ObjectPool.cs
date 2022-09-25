using System.Collections.Generic;
using UnityEngine;

namespace HotFix.Pool
{
    /// <summary>
    /// 对象池item基类
    /// </summary>
    public abstract class PoolItemBase : MonoBehaviour
    {
        public abstract void OnSpawned();
        public abstract void OnCycle();
    }

    // 通用型对象池 unity object 使用
    public class ObjectPool<T> where T : PoolItemBase
    {
        private readonly Queue<T> _objectsPool = new Queue<T>();
        private readonly T _mPrefab;

        private readonly Transform parentTrs;

        public ObjectPool(T prefab, Transform parentTrs)
        {
            _mPrefab = prefab;
            this.parentTrs = parentTrs;
        }

        public T Spawn()
        {
            T item = default;
            if (_objectsPool.Count > 0)
            {
                item = _objectsPool.Dequeue();
            }

            if (item == null)
                item = Object.Instantiate(_mPrefab);

            item.transform.SetParent(parentTrs);
            
            item.OnSpawned();

            return item;
        }

        public void Cycle(T item)
        {
            item.OnCycle();
            item.transform.SetParent(parentTrs);
            item.transform.SetAsLastSibling();
            _objectsPool.Enqueue(item);
        }

        public void DestroyAllItem()
        {
            while (_objectsPool.Count > 0)
            {
                GameObject.Destroy(_objectsPool.Dequeue());
            }
        }
    }
    
    /// <summary>
    /// game object 对象池
    /// </summary>
    public class ObjectPool 
    {
        private readonly Queue<GameObject> _objectsPool = new Queue<GameObject>();
        private readonly GameObject _mPrefab;

        private Transform parentTrs;

        public ObjectPool(GameObject prefab, Transform parentTrs)
        {
            _mPrefab = prefab;
            this.parentTrs = parentTrs;
        }

        public GameObject Spawn()
        {
            GameObject item = default;
            if (_objectsPool.Count > 0)
            {
                item = _objectsPool.Dequeue();
            }

            if (item == null)
                item = Object.Instantiate(_mPrefab);


            item.SetActive(true);
            item.transform.SetParent(parentTrs);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;

            return item;
        }

        public void Cycle(GameObject item)
        {
            item.transform.SetAsLastSibling();
            item.SetActive(false);

            _objectsPool.Enqueue(item);
        }

        public void DestroyAllItem()
        {
            while (_objectsPool.Count > 0)
            {
                GameObject.Destroy(_objectsPool.Dequeue());
            }
        }
    }
}