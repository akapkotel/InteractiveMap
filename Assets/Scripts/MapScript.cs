using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
[RequireComponent(typeof(DayAndNightCycle), typeof(ChimneysScript), 
    typeof(UserInterface))]
public class MapScript : MonoBehaviour
{
    public static MapScript Instance { get; private set; }

    public enum RotationDirection { Clockwise, Counterclockwise };
    
    [HideInInspector]
    public enum LocationType { Wioska, Miasteczko, Miasto, Opactwo, Inne, Forteca, Pałac, Kamieniołom, Dworek, Kasztel, Gospoda, Kościół }
    public enum Fortresses { PosterunekWojskowy, Kasztel, Zamek, Fort, Forteca, UfortyfikowaneMiasto }
    [HideInInspector]
    public static readonly Dictionary<Fortresses, int> GarrisonsSizes = new Dictionary<Fortresses, int>
    {
        [Fortresses.Kasztel] = 20,
        [Fortresses.Zamek] = 50,
        [Fortresses.PosterunekWojskowy] = 20,
        [Fortresses.Fort] = 300,
        [Fortresses.Forteca] = 1000,
        [Fortresses.UfortyfikowaneMiasto] = 500
    };

    [SerializeField,
     Tooltip("Put here scripts you wan to be started AFTER spawning all objects on the map. Scripts must inherit from ScheduledScript, not from MonoBehaviour.")]
    private ScheduledScript[] scheduledScripts;
    private int _completedScheduleScripts; // keeps track of scripts execution, required for proper working of some indexers later

    [SerializeField] private ObjectsSpawner[] spawners;
    [HideInInspector] public int finishedSpawners;

    public enum VillageSize { Uboga, Przeciętna, Bogata };

    public TextAsset vilagesNamesTextFile;
    
    //Intentionally use three data-structures: Dictionary for fast key-lookups, and lists for fast iterating through in updates: 
    [FormerlySerializedAs("locations")] public Dictionary<string, Location> locationsDictionary = new Dictionary<string, Location>();
    public Location[] LocationsList { get; private set; }
    public List<string> LocationNames { get; private set; }
    
    private List<string> _villagesNames = new List<string>();
    
    public GameObject[] Fields { get; private set; }
    public GameObject[] VillageHouses { get; private set; }
    public GameObject[] TownHousesLeft { get; private set; }
    public GameObject[] TownHousesMiddle { get; private set; }
    public GameObject[] TownHousesRight { get; private set; }
    public GameObject[] Churches { get; private set; }
    public GameObject[] Taverns { get; private set; }
    public GameObject[] Markets { get; private set; }
    public GameObject[] Windmills { get; private set; }

    public Dictionary<int, Material> materialsDict = new Dictionary<int, Material>();
    
    private Camera _cam;
    private bool _infoPanel;
    private bool _saved, _loaded;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        new UserProfile("testProfile");

        _villagesNames = LoadVillagesNamesFromFile();
        
        LoadPrefabs();

        spawners = GetComponents<ObjectsSpawner>();
        finishedSpawners = 0;
    }

    private void LoadPrefabs()
    {
        Fields = Resources.LoadAll<GameObject>("Prefabs/Fields");
        VillageHouses = Resources.LoadAll<GameObject>("Prefabs/VillageHouses");
        Taverns = Resources.LoadAll<GameObject>("Prefabs/Taverns");
        Markets = Resources.LoadAll<GameObject>("Prefabs/Markets");
        Windmills = Resources.LoadAll<GameObject>("Prefabs/Windmills");
        Churches = Resources.LoadAll<GameObject>("Prefabs/Churches");
        TownHousesLeft = Resources.LoadAll<GameObject>("Prefabs/TownHouses/LeftCorner");
        TownHousesMiddle = Resources.LoadAll<GameObject>("Prefabs/TownHouses/Middle");
        TownHousesRight = Resources.LoadAll<GameObject>("Prefabs/TownHouses/RightCorner");
    }

    private void Start()
    {
        RunObjectSpawners();

        InitializeScheduledScripts();

        //BuildMaterialsDict();
    }

    private void CreateLocationsList()
    {
        LocationsList = FindObjectsOfType<Location>();
        List<string> revealedLocations = UserProfile.Instance.FoundHiddenLocations;
        LocationNames = LocationsList
            .Where(t => t.known || revealedLocations.Contains(t.locationName)).Select(t => t.locationName).ToList();
        for (int i = 0; i < LocationsList.Length; i++)
        {
            locationsDictionary.Add(LocationsList[i].locationName, LocationsList[i]);
        }
        
        Debug.Log("Location lists created. Locations found: " + LocationsList.Length);
    }

    private void BuildMaterialsDict()
    {
        Material[] materials = Resources.LoadAll<Material>("Materials/");
        for (int i = 0; i < materials.Length; i++)
        {
            materialsDict.Add(i, materials[i]);
        }
    }
    
    private void RunObjectSpawners()
    {
        foreach (ObjectsSpawner spawner in spawners)
        {
            spawner.RunSpawning();
        }
    }

    private void InitializeScheduledScripts()
    {
        foreach (ScheduledScript scheduled in scheduledScripts)
        {
            scheduled.enabled = true;
        }
    }

    private void Update()
    {
        if (locationsDictionary.Count != 0 || finishedSpawners != spawners.Length) return;
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        // This is called only once, not in every frame!
        CreateLocationsList();

        if (LordsManager.Instance == null){
            new LordsManager(locationsDictionary);
        }
    }

    private List<string> LoadVillagesNamesFromFile()
    {
        string[] names = vilagesNamesTextFile.text.Split(',');

        return names.Select(loadedName => loadedName.Trim()).ToList();
    }

    public string GetRandomVillageName()
    {
        int number = Random.Range(0, _villagesNames.Count);
        string villageName = _villagesNames[number];
        _villagesNames.RemoveAt(number);
        return villageName;
    }
    
    /// <summary>
    /// Calculates current Terrain index for pair of (x, y) coordinates on the Scene. 
    /// </summary>
    /// <param name="x">horizontal, or east-west position</param>
    /// <param name="y">vertical, or north-south position</param>
    /// <returns>int</returns>
    public static int GetTerrainForCoordinates(float x, float y)
    {
        int terrainIndex = 0;
        
        for (int i = 0; i < Terrain.activeTerrains.Length; i++)
        {
            Terrain terrain = Terrain.activeTerrains[i];
            TerrainData terrainData = terrain.terrainData;
            Bounds worldBounds = new Bounds(terrainData.bounds.center + terrain.transform.position, terrainData.bounds.size);

            if (!worldBounds.Contains(new Vector3(x, y))) continue;
            terrainIndex = i;
            break;
        }
        return terrainIndex;
    }

    public static float GetTerrainHeightAtPosition(float posX, float posZ)
    {
        int currentTerrainIndex = GetTerrainForCoordinates(posX, posZ);
        return Terrain.activeTerrains[currentTerrainIndex].terrainData.GetHeight((int)posX, (int)posZ);
    }

    // Because other scripts sometimes have troubles with finding static references in edit mode, we need assure that some
    // instance would exist and be accessible at any moment, even in edit mode.
    public static MapScript GetMapScriptInstance()
    {
        return Instance == null ? FindObjectOfType<MapScript>() : Instance;
    }

    private void OnDestroy()
    {
        UserProfile.Instance.SaveData();
    }
}

/// <summary>
/// This wrapper class is required only as an abstraction to pack some other scripts into the one group off "schedulable" scripts,
/// which would be able to be serialized in inspector and run by another script in particular moment after game initialization.
/// </summary>
public abstract class ScheduledScript : MonoBehaviour { }
