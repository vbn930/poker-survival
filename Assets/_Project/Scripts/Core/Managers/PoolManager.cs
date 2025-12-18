using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolInfo
    {
        public string tag;
        public GameObject prefab;
        public int defaultCapacity;
        public int maxSize;
    }

    [Header("# Pool Info")]
    [SerializeField] private List<PoolInfo> pools;

    private Dictionary<string, IObjectPool<GameObject>> poolDict;

    private string currentTag;

    private void Awake()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        poolDict = new Dictionary<string, IObjectPool<GameObject>>();
        poolDict.Clear();

        foreach (PoolInfo pool in pools)
        {
            GameObject prefab = pool.prefab;
            Transform root = this.transform;
            var newPool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var obj = Instantiate(prefab, root);
                    obj.name = pool.tag; // 디버깅 용이하게 이름 설정
                    return obj;
                },
                OnGet,
                OnRelease,
                OnDestroyItem,
                true,
                pool.defaultCapacity,
                pool.maxSize
             );

            if (poolDict.ContainsKey(pool.tag))
            {
                Debug.LogError($"PoolManager: 중복된 태그가 있습니다! -> {pool.tag}");
                continue;
            }

            poolDict.Add(pool.tag, newPool);

            Debug.Log($"PoolManager: Pool for tag '{pool.tag}' initialized.");
        }
    }

    public GameObject GetObject(string tag)
    {
        return poolDict[tag].Get();
    }

    public void ReleaseObject(string tag, GameObject obj)
    {
        if (!obj.activeSelf)
        {
            return;
        }

        poolDict[tag].Release(obj);
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
