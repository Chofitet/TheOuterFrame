using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsPoolController : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 20;

    private readonly Queue<GameObject> pool = new();

    private void Awake()
    {
        FillPool();
    }

    private void FillPool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetFromPool(Transform parent = null)
    {
        if (pool.Count == 0)
        {
            ExpandPool();
        }

        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        if (parent != null)
            obj.transform.SetParent(parent, false);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }

    private void ExpandPool()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
