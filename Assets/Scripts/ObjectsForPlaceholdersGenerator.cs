using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectsForPlaceholdersGenerator : MonoBehaviour
{
    [Tooltip("Put here path to the directory containing your prefab/s, even if there is only single item to spawn. " +
             "The directory must contain only items you want to be used." +
             "Prefab/s directories to be spawned must be placed in 'Resources' folder." +
             "Path should contain only part inside 'Resources'.")] 
    public string pathToPrefabs;
    
    [Tooltip("To point location and rotation of spawned prefab/s make empty GO and use distinct Tag for them. Paste here the Tag. " +
             "Spawner will use this Tag to find all places where your object/s should be spawned.")]
    public string placeholderTag;
    
    [Tooltip("Set probability for each placeholder to get it's object spawned."), Range(0.01f, 1)] 
    public float spawningChance;
    
    private GameObject[] _objectsToSpawn;
    private Transform[] _placeholders;
    private List<GameObject> _spawnedPrefabs; // used for deleting objects later
    
    public void SpawnPrefabsInPlaceholders()
    {
        GetPlaceholdersAndPrefabs();
        
        int spawnedCount = 0;
        for (int i = 0; i < _placeholders.Length; i++)
        {
            if (_objectsToSpawn.Length == 0 || _placeholders.Length == 0) return;
            
            if (Random.value > spawningChance || _placeholders[i].childCount > 0) continue;

            Transform placeholder = _placeholders[i];
            
            GameObject prefab = _objectsToSpawn.Length == 1 ? _objectsToSpawn[0] : GetRandomPrefabFromList();

            GameObject spawned = Instantiate(prefab, placeholder.position, placeholder.rotation, placeholder);
            
            _spawnedPrefabs.Add(spawned);

            spawnedCount++;
        }
        
        Debug.Log("Objects spawned successfully! Number of created objects: " + spawnedCount);
    }

    private void GetPlaceholdersAndPrefabs()
    {
        _placeholders = GameObject.FindGameObjectsWithTag(tag: placeholderTag).Select(t => t.transform).ToArray();

        _objectsToSpawn = Resources.LoadAll<GameObject>(path: pathToPrefabs);
        
        _spawnedPrefabs = new List<GameObject>();
    }

    private GameObject GetRandomPrefabFromList()
    {
        return _objectsToSpawn[Random.Range(0, _objectsToSpawn.Length)];
    }
    
    public void RemoveSpawnedPrefabs()
    {
        for (int i = 0; i < _spawnedPrefabs.Count; i++)
        {
            DestroyImmediate(_spawnedPrefabs[i]);
        }
        
        _spawnedPrefabs.Clear();
        
        Debug.Log("All objects cleared!");
    }
}
