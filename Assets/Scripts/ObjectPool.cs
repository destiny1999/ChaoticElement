using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{

    private Queue<T> _objectQueue;
    private GameObject _prefab;

    private static ObjectPool<T> _instance = null;
    public static ObjectPool<T> instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new ObjectPool<T>();
            }
            return _instance;
        }
    }
    public int queueCount
    {
        get
        {
            return _objectQueue.Count;
        }
    }

    public void InitPool(GameObject prefab)
    {
        _prefab = prefab;
        _objectQueue = new Queue<T>();
    }

    public T Spawn(Vector3 position, Quaternion quaternion)
    {
        if(_prefab == null)
        {
            Debug.LogError(typeof(T).ToString() + " prefab not set");
            return default(T);
        }
        if(queueCount <= 0)
        {
            GameObject gameObject = Object.Instantiate(_prefab, position, quaternion);
            T t = gameObject.GetComponent<T>();
            if(t == null)
            {
                Debug.LogError(typeof(T).ToString() + "not found in prefab");
                return default(T);
            }
            _objectQueue.Enqueue(t);
        }
        T obj = _objectQueue.Dequeue();
        obj.gameObject.transform.position = position;
        obj.gameObject.transform.rotation = quaternion;
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Recycle(T obj)
    {
        _objectQueue.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }
}
