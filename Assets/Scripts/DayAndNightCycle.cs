using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This class controls artificial day-and-night cycle, and also switches on and off all lights on the map.
/// Simulation of day-and-night cycle is managed by simply rotating two directional lights around the scene.
/// During the night some buildings-windows are 'bright' thanks to the material swapping behind the scenes.
/// There are also a bunch of lanterns which emits light from point light components.
/// </summary>

public class DayAndNightCycle : ScheduledScript
{
    public static DayAndNightCycle Instance { get; private set; }

    [Tooltip("Should day-night cycle be active?")] public bool dayAndNightCycle;
    [SerializeField, Tooltip("How many seconds should 12-hour day last?")] private int dayCycleDuration;
    [SerializeField, Tooltip("How much percent of windows should be bright at the night?"), Range(10, 75)] private float brightWindowsRatio;
    [SerializeField, Tooltip("How much percent of lanterns should be bright at the night?"), Range(25, 95)] private float brightLanternsRatio;

    public Material lightInTheWindows;
    public Material hauntedGlow;
    public Material darknessInTheWindows;
    public Material lanternGlass;

    public enum DayCycle { Day, Night };

    public DayCycle dayCycle;

    private float _lastCycleChangeTime;

    private float _anglePerSecond; // how fast should system rotate around the world

    private MeshRenderer[] _windows;

    private MeshRenderer[] _haunted; // for red-glowing windows of haunted places

    private List<int> _brightWindows;

    private MeshRenderer[] _lanterns;
    
    private Light[] _lanternsLights;

    private List<int> _brightLanterns;
    
    private float _seconds;
    [HideInInspector] public float epsilon = 0.01f;

    private float _brightWindowsChance;
    private float _brightLanternsChance;

    private Quaternion _defaultRotation; //  used for resetting Day and Night Cycle in options to the midday

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        _seconds = dayCycleDuration * 0.5f; // hardcoded starting at the noon
        
        _defaultRotation = transform.rotation;
        
        _anglePerSecond = 180f / dayCycleDuration;
        
        _brightLanterns = new List<int>();
        _brightWindows = new List<int>();

        _brightWindowsChance = brightWindowsRatio / 100;
        _brightLanternsChance = brightLanternsRatio / 100;
    }

    private void Start()
    {
        FindAllWindows();
        FindAllLanterns();
    }

    private void FindAllWindows()
    {
        _windows = GameObject.FindGameObjectsWithTag("Window").Select(t => t.GetComponent<MeshRenderer>()).ToArray();
        //_windows = tempWindows.Select(window => window.GetComponent<MeshRenderer>()).ToArray();
        _haunted = GameObject.FindGameObjectsWithTag("Haunted").Select(t => t.GetComponent<MeshRenderer>()).ToArray();
        
        Debug.Log("Total windows on the Map:" + _windows.Length);
    }

    private void FindAllLanterns()
    {
        GameObject[] tempLanterns = GameObject.FindGameObjectsWithTag("Lantern");
        _lanternsLights = tempLanterns.Select(t => t.GetComponent<Light>()).ToArray();
        _lanterns = tempLanterns.Select(t => t.GetComponent<MeshRenderer>()).ToArray();
    }

    public void ToggleDayAndNightCycle()
    {
        dayAndNightCycle = !dayAndNightCycle;
        transform.rotation = _defaultRotation;
        _seconds = dayCycleDuration * 0.5f;
        dayCycle = DayCycle.Day;
        SwitchLights();
    }
    
    private void Update()
    {
        _seconds += Time.deltaTime;

        if (dayAndNightCycle)
        {
            // Day-night cycle is made by rotating a simple system of two light-sources: Sun and Moon around pivot,
            // which is this gameObject. The simpliest way to do this is rotating the gameObject with it's children -
            // Sun and Moon, simulating planet rotation.
            transform.Rotate(0f, 0f, _anglePerSecond * Time.deltaTime);
            
            if (_seconds > dayCycleDuration)
            {
                _seconds = 0;
                
                ChangeDayCycle();

                SwitchLights();
            }
        }
    }

    private void ChangeDayCycle()
    {
        dayCycle = dayCycle == DayCycle.Day ? DayCycle.Night : DayCycle.Day;
    }

    private void SwitchLights()
    {
        if (dayCycle == DayCycle.Night) {
            for (int i = 0; i < _windows.Length; i++) {
                if (Random.value <= _brightWindowsChance) {
                    _windows[i].sharedMaterial = lightInTheWindows;
                    _brightWindows.Add(i); // store index of enlightened windows to switch them off when days comes back
                }
            }

            for (int i = 0; i < _lanterns.Length; i++) {
                if (Random.value <= _brightLanternsChance) {
                    _lanterns[i].sharedMaterial = lightInTheWindows;
                    _lanternsLights[i].enabled = true;
                    _brightLanterns.Add(i);
                }
            }

            for (int i = 0; i < _haunted.Length; i++)
            {
                _haunted[i].sharedMaterial = hauntedGlow;
            }
            
        } else {
            foreach (int windowsIndex in _brightWindows) {
                _windows[windowsIndex].sharedMaterial = darknessInTheWindows;
            }
            _brightWindows.Clear();  // since all windows are dark again

            foreach (int lanternIndex in _brightLanterns) {
                _lanterns[lanternIndex].sharedMaterial = lanternGlass;
                _lanternsLights[lanternIndex].enabled = false;
            }
            _brightLanterns.Clear();

            foreach (MeshRenderer glow in _haunted)
            {
                glow.sharedMaterial = darknessInTheWindows;
            }
        }
    }
}
