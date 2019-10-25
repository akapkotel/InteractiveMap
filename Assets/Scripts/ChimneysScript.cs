using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DayAndNightCycle))]
public class ChimneysScript : ScheduledScript
{
    [SerializeField, Tooltip("How often should each chimney be randomly refreshed (seconds between each update)")] int chimneySmokeSwitchRate;

    public float switchChance = 0.25f;
    
    public bool smoking = true;
    
    private ParticleSystem[] _chimneys;

    private List<int> _smokingChimneys;

    private int _lastUpdate;

    private void Awake()
    {
        _smokingChimneys = new List<int>();
    }

    private void Start()
    {
       _chimneys = GameObject.FindGameObjectsWithTag("Chimney")
            .Select(t => t.GetComponent<ParticleSystem>())
            .ToArray();

        SwitchSmokeInChimneys();
    }

    public void ToggleSmokes()
    {
        smoking = !smoking;
        SwitchSmokeInChimneys(true);
    }
    
    private void Update()
    {
        if (!smoking) return;

        int time = (int) Time.realtimeSinceStartup;
        if (time - _lastUpdate > chimneySmokeSwitchRate)
        {
            _lastUpdate = time;
            Debug.Log("Chimneys switching!");
            SwitchSmokeInChimneys();
        }
    }

    private void SwitchSmokeInChimneys(bool supress = false)
    {
        for (int i = 0; i < _chimneys.Length; i++)
        {
            if (supress || (Random.value < switchChance))
            {
                if (_smokingChimneys.Contains(i)) {
                    _chimneys[i].Stop();
                    _smokingChimneys.Remove(i);
                } else {
                    _chimneys[i].Play();
                    _smokingChimneys.Add(i);
                }
            }
        }
    }
}
