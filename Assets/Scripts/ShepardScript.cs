using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShepardScript : ScheduledScript
{
    private Dictionary<int, List<int>> _animalsToPasturesDict;

    private Camera _camera;
    
    private GameObject[] _pastures;
    private Renderer[] _pasturesRenderers; // used to check if pasture is visible
    private Dictionary<int, bool> _herdedPastures; // used to check if animals on the pasture are displayed
    private Dictionary<int, AnimalSpecimen> _pasturesTypes; // to remember what kind of animals are kept on pasture
    private Dictionary<int, int> _pasturesCapacities; // how many animals could each pasture handle
    private Dictionary<int, int> _herdSizes; // how many animals are grazing today on each pasture (reset each day)
    
    private GameObject _pigPrefab, _sheepPrefab, _cowPrefab;
    private GameObject[] _pigsPool, _sheepPool, _cowsPool;

    private bool _herdsInBarns;
    
    public int animalsPoolsSize = 150;
    

    private void Awake()
    {
        _camera = Camera.main;
        _pigPrefab = Resources.Load<GameObject>("Prefabs/Animals/pig");
        _sheepPrefab = Resources.Load<GameObject>("Prefabs/Animals/sheep");
        _cowPrefab = Resources.Load<GameObject>("Prefabs/Animals/cow");
        SpawnAnimalsPools();
        FindPastures();
    }

    private void SpawnAnimalsPools()
    {
        _pigsPool = new GameObject[animalsPoolsSize];
        _sheepPool = new GameObject[animalsPoolsSize];
        _cowsPool = new GameObject[animalsPoolsSize];

        Dictionary<GameObject, GameObject[]> animalsTypes = new Dictionary<GameObject, GameObject[]>
        {
            {_pigPrefab, _pigsPool}, {_sheepPrefab, _sheepPool}, {_cowPrefab, _cowsPool}
        };

        foreach (KeyValuePair<GameObject, GameObject[]> pair in animalsTypes)
            for (int i = 0; i < animalsPoolsSize; i++)
            {
                GameObject newAnimal = Instantiate(pair.Key, transform);
                newAnimal.SetActive(false);
                pair.Value[i] = newAnimal;
            }
    }

    private void FindPastures()
    {
        _pastures = GameObject.FindGameObjectsWithTag("Pasture");
        _pasturesRenderers = _pastures.Select(t => t.GetComponent<Renderer>()).ToArray();

        _herdedPastures = new Dictionary<int, bool>();
        _pasturesTypes = new Dictionary<int, AnimalSpecimen>();
        _animalsToPasturesDict = new Dictionary<int, List<int>>();
        _pasturesCapacities = new Dictionary<int, int>();
        _herdSizes = new Dictionary<int, int>();

        for (int i = 0; i < _pastures.Length; i++)
        {
            _herdedPastures.Add(i, false);
            _animalsToPasturesDict.Add(i, new List<int>());
            _pasturesTypes.Add(i, (AnimalSpecimen) Random.Range(0, 3));
            _pasturesCapacities.Add(i, GetPastureCapacity(_pastures[i].name));
            _herdSizes.Add(i, 0);
        }
    }

    private int GetPastureCapacity(string pastureName)
    {
        if (pastureName.Contains("large")) return 30;
        return pastureName.Contains("square") ? 20 : 10;
    }
    
    private void PlaceAnimalOnPasture(int pastureIndex, AnimalSpecimen typeOfAnimal)
    {
        GameObject animal = GetAnimal(pastureIndex, typeOfAnimal);

        if (animal == null) return;

        animal.transform.position = GetRandomPositionInsideTransformBounds(pastureIndex);
    }

    private Vector3 GetRandomPositionInsideTransformBounds(int pastureIndex) {
        Vector3 position;
    	position.x = Random.Range(-0.5f, 0.5f);
        position.y = 0;
    	position.z = Random.Range(-0.5f, 0.5f);
    	return _pastures[pastureIndex].transform.TransformPoint(position);

    }

    [CanBeNull]
    private GameObject GetAnimal(int pastureIndex, AnimalSpecimen typeOfAnimal)
    {
        GameObject[] pool = GetAnimalsPoolType(typeOfAnimal);

        for (int i = 0; i < pool.Length; i++)
            if (!pool[i].activeInHierarchy)
            {
                GameObject animal = pool[i];
                animal.SetActive(true);
                _animalsToPasturesDict[pastureIndex].Add(i);
                return animal;
            }

        return null;
    }

    private GameObject[] GetAnimalsPoolType(AnimalSpecimen typeOfAnimal)
    {
        switch (typeOfAnimal)
        {
            case AnimalSpecimen.Pig:
            {
                return _pigsPool;
            }
            case AnimalSpecimen.Sheep:
            {
                return _sheepPool;
            }
            default:
            {
                return _cowsPool;
            }
        }
    }

    private void Update()
    {
        // animals are moved from pastures to barns for night:
        if (DayAndNightCycle.Instance.dayCycle.Equals(DayAndNightCycle.DayCycle.Night))
        {
            if (!_herdsInBarns)
            {
                HideAllAnimals();
                return;
            }
        } else {
            if (_herdsInBarns)
            {
                _herdsInBarns = false;
            }
        }
        
        // display animals on currently visible pastures and hide those on invisible:
        for (int i = 0; i < _pastures.Length; i++)
            if (_pasturesRenderers[i].isVisible)
            {
                if (_herdedPastures[i]) continue;
                PopulatePasture(i);
            }
            else
            {
                if (!_herdedPastures[i]) continue;
                DepopulatePasture(i);
            }
    }

    private void HideAllAnimals()
    {
        for (int i = 0; i < _pastures.Length; i++)
        {
            if (_herdSizes[i] == 0) continue;
            DepopulatePasture(i);
            _herdedPastures[i] = false;
            _herdSizes[i] = 0;
        }
    }

    private void PopulatePasture(int pastureIndex)
    {
        AnimalSpecimen animalType = _pasturesTypes[pastureIndex];


        int herdSize = _herdSizes[pastureIndex] == 0 ? Random.Range(0, _pasturesCapacities[pastureIndex]) : _herdSizes[pastureIndex];
        _herdSizes[pastureIndex] = herdSize;
        
        for (int i = 0; i < herdSize; i++) PlaceAnimalOnPasture(pastureIndex, animalType);
        
        _herdedPastures[pastureIndex] = true;
    }

    private void DepopulatePasture(int pastureIndex)
    {
        //Debug.Log("Depopulating pasture" + pastureIndex);

        AnimalSpecimen typeOfAnimal = _pasturesTypes[pastureIndex];

        GameObject[] pool = GetAnimalsPoolType(typeOfAnimal);

        for (int index = 0; index < _animalsToPasturesDict[pastureIndex].Count; index++)
        {
            int animalIndex = _animalsToPasturesDict[pastureIndex][index];
            pool[animalIndex].SetActive(false);
        }

        _animalsToPasturesDict[pastureIndex].Clear();
        
        _herdedPastures[pastureIndex] = false;
    }

    private enum AnimalSpecimen
    {
        Pig,
        Sheep,
        Cow
    }
}