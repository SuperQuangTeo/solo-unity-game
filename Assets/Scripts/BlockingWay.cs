using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockingWay : MonoBehaviour
{
    public List<GameObject> blockingWays;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(GameObject e in blockingWays)
        {
            e.SetActive(false);
        }
    }

    public void SetStateWay(bool state)
    {
        foreach (GameObject e in blockingWays)
        {
            e.SetActive(state);
        }
    }

}
