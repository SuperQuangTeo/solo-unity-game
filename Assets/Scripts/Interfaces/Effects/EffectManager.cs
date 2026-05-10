using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    public List<GameObject> effectPrefabs = new List<GameObject>();

    private Dictionary<System.Type, GameObject> prefabLookup = new Dictionary<System.Type, GameObject>();
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var prefab in effectPrefabs)
        {
            var effect = prefab.GetComponent<IEffect>();
            if (effect != null)
            {
                prefabLookup[effect.GetType()] = prefab;
            }
            else
            {
                Debug.LogWarning($"[EffectManager] Prefab {prefab.name}");
            }
        }
    }

    public T GetEffect<T>() where T : class, IEffect
    {
        if (prefabLookup.ContainsKey(typeof(T)))
        {
            GameObject prefab = prefabLookup[typeof(T)];
            GameObject go = ObjectPool.Instance.SpawnFromPool(prefab, Vector3.zero, Quaternion.identity);
            return go.GetComponent<T>();
        }
        Debug.LogWarning($"[EffectManager] {typeof(T).Name}");
        return null;
    }
}
