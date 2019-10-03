using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[RequireComponent(typeof(DayAndNightCycle), typeof(ChimneysScript), typeof(UserInterface))]
public class MapScript : MonoBehaviour
{
    public static MapScript Instance;

    public enum RotationDirection { Clockwise, Counterclockwise };
    public enum Fortresses { MilitaryPost, Castel, Castle, SmallFortress, Fortress, FortifiedCity }
    [HideInInspector]
    public Dictionary<Fortresses, int> garrisonsSizes = new Dictionary<Fortresses, int>
    {
        [Fortresses.Castel] = 20,
        [Fortresses.Castle] = 50,
        [Fortresses.MilitaryPost] = 20,
        [Fortresses.SmallFortress] = 300,
        [Fortresses.Fortress] = 1000,
        [Fortresses.FortifiedCity] = 500
    };

    [SerializeField,
     Tooltip("Put here scripts you wan to be started AFTER spawning all objects on the map. Scripts must inherit from ScheduledScript, not from MonoBehaviour.")]
    private ScheduledScript[] scheduledScripts;
    private int completedScheduleScripts; // keeps track of scripts execution, required for proper working of some indexers later

    [SerializeField] private ObjectsSpawner[] spawners;
    public int completedSpawnings;

    public enum VillageSize { Uboga, Przeciętna, Bogata };

    public TextAsset generatedVillagesFile;
    public TextAsset vilagesNamesTextFile;

    public List<Location> locations = new List<Location>();
    public List<string> locationNames = new List<string>();

    public GameObject[] fields;
    public List<string> vilagesNames = new List<string>();
    public Fortress[] fortresses;

    public GameObject[] villageBuildings;
    public GameObject[] townBuildings;
    public GameObject[] specialBuildings;

    public Dictionary<int, Material> materialsDict = new Dictionary<int, Material>();

    private Camera cam;
    private bool infoPanel;
    private bool saved, loaded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        vilagesNames = LoadNamesFromFile();

        spawners = GetComponents<ObjectsSpawner>();
    }

    private void Start()
    {
        fortresses = FindObjectsOfType<Fortress>();

        RunObjectSpawners();

        InitializeScheduledScripts();

        BuildMaterialsDict();
    }

    private void CreateLocationsList()
    {
        Location[] temp = FindObjectsOfType<Location>();
        foreach (Location location in temp)
        {
            locations.Add(location);
            locationNames.Add(location.locationName);
        }
    }

    void BuildMaterialsDict()
    {
        Material[] materials = Resources.LoadAll<Material>("_Materials/"); //Resources.FindObjectsOfTypeAll<Material>();
        Debug.Log("Materials found: " + materials.Length);
        Debug.Log(materials);
        for (int i = 0; i < materials.Length; i++)
        {
            materialsDict.Add(i, materials[i]);
        }
    }

    void SaveVillages()
    {
        List<Transform> villagesTransforms = new List<Transform>(locations.Count);
        foreach (Village villageScript in locations)
        {
            villagesTransforms.Add(villageScript.GetComponent<Transform>());
        }

        SaveScript saveScript = FindObjectOfType<SaveScript>();

        saveScript.SaveData(villagesTransforms);
        saved = true;
    }

    void LoadVillages()
    {
        SaveScript saveScript = FindObjectOfType<SaveScript>();

        List<Transform> loadedTransforms = saveScript.LoadData();

        for (int i = 0; i < loadedTransforms.Count; i++)
        {
            locations[i].transform.SetPositionAndRotation(loadedTransforms[i].position, loadedTransforms[i].rotation);
        }
        loaded = true;
    }

    void RunObjectSpawners()
    {
        foreach (ObjectsSpawner spawner in spawners)
        {
            spawner.enabled = true;
        }
    }

    void InitializeScheduledScripts()
    {
        foreach (ScheduledScript scheduled in scheduledScripts)
        {
            scheduled.enabled = true;
        }
    }

    private void Update()
    {

        if (locations.Count == 0 && completedSpawnings == spawners.Length)
        {
            CreateLocationsList();
        }
        //if (!saved && villages.Count > 0)
        //{
        //    SaveVillages();
        //}

        //if (!loaded)
        //{
        //    LoadVillages();
        //}
    }

    private List<string> LoadNamesFromFile()
    {
        List<string> result = new List<string>();

        string[] names = vilagesNamesTextFile.text.Split(',');

        foreach (string name in names)
        {
            result.Add(name.Trim());
        }

        return result;
    }

    private void CreateVillagesDataFile()
    {
        string serializedData = string.Empty;

        for (int i = 0; i < locations.Count; i++)
        {
            //TODO:
        }

        // Write to disk
        StreamWriter writer = new StreamWriter(Application.dataPath + "/_TextFiles/villages.txt", true);
        writer.Write(serializedData);
    }

    private void ReadVillagesData()
    {
        // Read
        StreamReader reader = new StreamReader(Application.dataPath + "/_TextFiles/villages.txt");
        // string lineA = reader.ReadLine();
    }

    public int GetTerrainForCoordinates(float x, float y)
    {
        int terrainIndex;

        if (x < 6001) {
            if (y < 6001) {
                terrainIndex = 0;
            } else {
                terrainIndex = 2;
            }
        } else {
            if (y < 6001) {
                terrainIndex = 1;
            } else {
                terrainIndex = 3;
            }
        }
        return terrainIndex;
    }
}
/// <summary>
/// This wrapper class is required only as an abstraction to pack some other scripts into the one group off "schedulable" scripts,
/// which would be able to be serialized in inspector and run by another script in particular moment after game initialization.
/// </summary>
public abstract class ScheduledScript : MonoBehaviour { }
