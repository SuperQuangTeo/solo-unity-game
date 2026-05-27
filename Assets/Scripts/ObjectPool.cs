using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class PoolItem
    {
        public GameObject prefab;
        public int size;
    }

    public List<PoolItem> itemsToPool;

    Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, GameObject> instanceToPrefabMap = new Dictionary<GameObject, GameObject>();


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        foreach (PoolItem poolItem in itemsToPool)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < poolItem.size; i++)
            {
                GameObject obj = Instantiate(poolItem.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(poolItem.prefab, objectPool);
        }
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning("Prefab not found in pool: " + prefab.name);
            return null;
        }
        GameObject obj = poolDictionary[prefab].Count > 0 ? poolDictionary[prefab].Dequeue() : Instantiate(prefab);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        if (!instanceToPrefabMap.ContainsKey(obj))
        {
            //Debug.Log("Adding instance to map: " + obj.name);
            instanceToPrefabMap[obj] = prefab;
        }

        return obj;
    }

    public void ReturnToPool(GameObject instance)
    {
        if (!instanceToPrefabMap.TryGetValue(instance, out GameObject prefab))
        {
            Debug.LogWarning("Instance not found in map, destroying.");
            Destroy(instance);
            return;
        }

        instance.SetActive(false);
        poolDictionary[prefab].Enqueue(instance);

    }
}
