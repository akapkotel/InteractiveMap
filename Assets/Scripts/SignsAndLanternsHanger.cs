using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
[RequireComponent(typeof(SignsAndLanternsHangerInspector))]
public class SignsAndLanternsHanger : MonoBehaviour
{
    private GameObject[] _signPrefabs;
    private GameObject _lanternPrefab;
    
    private GameObject[] _signsPlaceholders;
    private GameObject[] _lanternsPlaceholders;

    private List<GameObject> _instantiatedObjects; // used for deleting objects later

    private float _signChance;
    private float _lanternChance;
    
    private void Awake()
    {
        FindPrefabs();

        FindPlaceholders();
        
        _instantiatedObjects = new List<GameObject>();
    }

    private void FindPlaceholders()
    {
        _signsPlaceholders = GameObject.FindGameObjectsWithTag("SignPlaceholder");
        _lanternsPlaceholders = GameObject.FindGameObjectsWithTag("LanternPlaceholder");
    }

    private void FindPrefabs()
    {
        _signPrefabs = Resources.LoadAll<GameObject>("Prefabs/Signs");
        _lanternPrefab = Resources.Load<GameObject>("Prefabs/Lantern/LanternOnTheHouse");
    }

    public void HangSignsInPlaceholders()
    {
        for (int i = 0; i < _signsPlaceholders.Length; i++)
        {
            if (Random.value > 0.5 || _signsPlaceholders[i].transform.childCount > 0) continue;

            Transform placeholder = _signsPlaceholders[i].transform;
            
            GameObject prefab = _signPrefabs[Random.Range(0, _signPrefabs.Length)];

            GameObject sign = Instantiate(prefab, placeholder.position, placeholder.rotation, placeholder);
            
            _instantiatedObjects.Add(sign);
        }
    }

    public void HangLanternsInPlaceholders()
    {
        for (int i = 0; i < _lanternsPlaceholders.Length; i++)
        {
            if (Random.value > 0.2 || _lanternsPlaceholders[i].transform.childCount > 0) continue;
            
            Transform placeholder = _lanternsPlaceholders[i].transform;

            GameObject lantern = Instantiate(_lanternPrefab, placeholder.position, placeholder.rotation, placeholder);
            
            _instantiatedObjects.Add(lantern);
        }
    }

    public void ClearSignsAndLanterns()
    {
        for (int i = 0; i < _instantiatedObjects.Count; i++)
        {
            DestroyImmediate(_instantiatedObjects[i]);
        }
        
        _instantiatedObjects.Clear();
        
        Debug.Log("Signs and Lanterns cleared!");
    }
}
