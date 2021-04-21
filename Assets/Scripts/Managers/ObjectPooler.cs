using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler m_Instance { get; private set; }

    [SerializeField] private Dictionary<string, Queue<GameObject>> m_PoolDictionary;
    [SerializeField] private List<Pool> m_Pools = new List<Pool>();
    private GameObject m_ObjectToSetActive;

    [System.Serializable]
    public class Pool
    {
        public string m_PrefabName;
        public GameObject m_Prefab;
        public Transform m_Parent;
        public int m_CopyAmount;
    }

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != null)
        {
            Destroy(this);
        }

        m_PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in m_Pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.m_CopyAmount; i++)
            {
                GameObject copy = Instantiate(pool.m_Prefab, pool.m_Parent);
                copy.SetActive(false);

                objectPool.Enqueue(copy);
            }

            m_PoolDictionary.Add(pool.m_PrefabName, objectPool);
        }
    }

    public GameObject SetActiveFromPool(string prefabName, Vector2 position, Quaternion rotation)
    {
        if (!m_PoolDictionary.ContainsKey(prefabName))
        {
            Debug.LogError("Hey! Ja jij daar! Check of je geen spellingsfouten hebt gemaakt in de prefab naam: " + prefabName + " :)");
            return null;
        }

        m_ObjectToSetActive = m_PoolDictionary[prefabName].Dequeue();

        //Net zoals de Instantiate :D
        m_ObjectToSetActive.SetActive(true);
        m_ObjectToSetActive.transform.position = position;
        m_ObjectToSetActive.transform.rotation = rotation;

        m_PoolDictionary[prefabName].Enqueue(m_ObjectToSetActive);

        return m_ObjectToSetActive;
    }

    public GameObject GetObjectFromPool()
    {
        return m_ObjectToSetActive;
    }
}
