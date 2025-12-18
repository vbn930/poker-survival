using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int defaultCapacity;
        public int maxSize;
    }

    [Header("# Pool Info")]
    [SerializeField] private List<Pool> pools;
    private Dictionary<string, ObjectPool<GameObject>> poolDict;
    private Dictionary<string, GameObject> objectCreationDict;

    private string currentTag;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        objectCreationDict = new Dictionary<string, GameObject>();
        objectCreationDict.Clear();

        poolDict = new Dictionary<string, ObjectPool<GameObject>>();
        poolDict.Clear();

        foreach (Pool pool in pools)
        {
            currentTag = pool.tag;
            objectCreationDict.Add(pool.tag, pool.prefab);
            poolDict.Add(pool.tag, new ObjectPool<GameObject>(
                CreateItem,
                OnGet,
                OnRelease,
                OnDestroyItem,
                true,
                pool.defaultCapacity,
                pool.maxSize)
             );
        }
    }

    public GameObject GetObject(string tag)
    {
        if (poolDict == null)
        {
            Init();
        }
        currentTag = tag;
        return poolDict[tag].Get();
    }

    public void ReleaseObject(string tag, GameObject obj)
    {
        if (poolDict == null)
        {
            Init();
        }
        currentTag = tag;
        poolDict[tag].Release(obj);
    }
    private GameObject CreateItem()
    {
        GameObject obj = Instantiate(objectCreationDict[currentTag]);
        obj.SetActive(false);

        return obj;
    }

    private void OnGet(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnRelease(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDestroyItem(GameObject obj)
    {
        Destroy(obj);
    }
}
