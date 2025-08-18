using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    public List<GameObject> effectPrefabs = new List<GameObject>();

    private List<IEffect> effects = new List<IEffect>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var prefab in effectPrefabs)
        {
            GameObject go = Instantiate(prefab, transform); 
            go.SetActive(false); 

            var effect = go.GetComponent<IEffect>();
            if (effect != null)
            {
                effects.Add(effect);
                Debug.Log($"[EffectManager] Loaded effect: {effect.GetType().Name}");
            }
            else
            {
                Debug.LogWarning($"[EffectManager] Prefab {prefab.name}");
            }
        }
    }

    public T GetEffect<T>() where T : class, IEffect
    {
        foreach (var e in effects)
        {
            if (e is T) return e as T;
        }
        Debug.LogWarning($"[EffectManager] {typeof(T).Name}");
        return null;
    }
}
