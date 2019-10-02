using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls artificial day-and-night cycle, and also switches on and off all lights on the map.
/// Simulation of day-and-night cycle is managed by simply rotating two directional lights around the scene.
/// During the night some buildings-windows are 'bright' thanks to the material swapping behind the scenes.
/// There are also a bunch of lanterns which emits light from point light components.
/// </summary>

public class DayAndNightCycle : ScheduledScript
{
    public static DayAndNightCycle Instance;

    [SerializeField, Tooltip("Should day-night cycle be active?")] bool dayAndNightCycye;
    [SerializeField, Tooltip("How many seconds should 12-hour day last?")] int dayCycleDuration;
    [SerializeField, Tooltip("How much percent of windows should be bright at the night?"), Range(10, 75)] float brightWindowsRatio;
    [SerializeField, Tooltip("How much percent of lanterns should be bright at the night?"), Range(25, 95)] float brightLanternsRatio;

    public Material lightInTheWindows;
    public Material darknessInTheWindows;
    public Material lanternGlass;

    public static Light Sun;
    public static Light Moon;

    public enum DayCycle { Day, Night };

    private DayCycle dayCycle;

    private float lastCycleChangeTime;

    private float anglePerSecond; // how fast should system rotate around the world

    private List<MeshRenderer> windows;

    private List<int> brightWindows;

    private List<MeshRenderer> lanterns;

    private List<Light> lanternsLights;

    private List<int> brightLanterns;

    private int hours { get; set; }
    private float seconds;
    [HideInInspector] public float epsilon = 0.01f;

    private float brightWindowsChance;
    private float brightLanternsChance;

    private bool reallyActive; // hidden, internal switch controlling if the system is working or not

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        Sun = GameObject.Find("Sun").GetComponent<Light>();
        Moon = GameObject.FindGameObjectWithTag("Moon").GetComponent<Light>();
        anglePerSecond = 180f / dayCycleDuration;

        lanterns = new List<MeshRenderer>();
        lanternsLights = new List<Light>();
        brightLanterns = new List<int>();
        windows = new List<MeshRenderer>();
        brightWindows = new List<int>();

        brightWindowsChance = brightWindowsRatio / 100;
        brightLanternsChance = brightLanternsRatio / 100;
    }

    private void Start()
    {
        FindAllWindows();
        FindAllLanterns();
        if (dayAndNightCycye)
        {
            reallyActive = true; // here we start the cycle for real
        }
    }

    public void Enable()
    {
        this.enabled = true;
    }

    void FindAllWindows()
    {
        GameObject[] tempWindows = GameObject.FindGameObjectsWithTag("Window");
        foreach (GameObject window in tempWindows)
        {
            windows.Add(window.GetComponent<MeshRenderer>());
        }
        Debug.Log("Total windows on the Map:" + windows.Count);
    }

    void FindAllLanterns()
    {
        GameObject[] tempLanterns = GameObject.FindGameObjectsWithTag("Lantern");
        foreach (GameObject lantern in tempLanterns)
        {
            lanternsLights.Add(lantern.GetComponent<Light>());
            lanterns.Add(lantern.GetComponent<MeshRenderer>());
        }
    }

    void Update()
    {
        if (reallyActive)
        {
            // Day-night cycle is made by rotating a simple system of two light-sources: Sun and Moon around pivot, which is this gameObject.
            // The simpliest way to do this is rotating the gameObject with it's children - Sun and Moon, simulating planet rotation.
            transform.Rotate(0f, 0f, anglePerSecond * Time.deltaTime);

            seconds += Time.deltaTime;

            hours = (int)seconds;

            if (seconds > dayCycleDuration)
            {
                seconds = 0;
                ChangeDayCycle();

                SwitchLights();
            }
        }
    }

    void ChangeDayCycle()
    {
        if (dayCycle == DayCycle.Day) {
            dayCycle = DayCycle.Night;
        } else {
            dayCycle = DayCycle.Day;
        }
    }

    void SwitchLights()
    {
        if (dayCycle == DayCycle.Night) {
            for (int i = 0; i < windows.Count; i++) {
                if (Random.value <= brightWindowsChance) {
                    windows[i].material = lightInTheWindows;
                    brightWindows.Add(i); // store index of enlightened windows to switch them off when days comes back
                }
            }

            for (int i = 0; i < lanterns.Count; i++) {
                if (Random.value <= brightLanternsChance) {
                    lanterns[i].material = lightInTheWindows;
                    lanternsLights[i].enabled = true;
                    brightLanterns.Add(i);
                }
            }
        } else {
            foreach (int windowsIndex in brightWindows) {
                windows[windowsIndex].material = darknessInTheWindows;
            }
            brightWindows.Clear();  // since all windows are dark again

            foreach (int lanternIndex in brightLanterns) {
                lanterns[lanternIndex].material = lanternGlass;
                lanternsLights[lanternIndex].enabled = false;
            }
            brightLanterns.Clear();
        }
    }

    public bool CheckIfTimePassed(int requestedHours, int lastUpdate)
    {
        return seconds - hours < epsilon && (hours - lastUpdate) == requestedHours;
    }
}
