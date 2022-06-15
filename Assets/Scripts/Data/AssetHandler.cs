using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetHandler : MonoBehaviour
{
    public static AssetHandler Instance;

    public struct PrefabData
    {
        public Transform Prefab;
        public string Name;
    }

    private PrefabData _nullPrefab = new PrefabData { Prefab = null, Name = null };

    private List<PrefabData> _prefabData = new List<PrefabData>();
    private Dictionary<string, PrefabData> _prefabMap;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        _prefabMap = new Dictionary<string, PrefabData>();

        foreach (PrefabData pd in _prefabData)
            _prefabMap.Add(pd.Name, pd);
    }

    public PrefabData GetPrefabData(string name)
    {
        if (_prefabMap.ContainsKey(name))
            return _prefabMap[name];

        return _nullPrefab;
    }
}
