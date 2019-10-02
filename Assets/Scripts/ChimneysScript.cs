using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DayAndNightCycle))]
public class ChimneysScript : ScheduledScript
{
    [SerializeField, Tooltip("How often should each chimney be randomly refreshed (seconds between each update)")] int chimneySmokeSwitchRate;

    private List<ParticleSystem> chimneys;

    private List<int> smokingChimneys;

    private int lastUpdate;

    void Awake()
    {
        chimneys = new List<ParticleSystem>();

        smokingChimneys = new List<int>();
    }

    private void Start()
    {
        GameObject[] chimneysTransforms = GameObject.FindGameObjectsWithTag("Chimney");

        for (int i = 0; i < chimneysTransforms.Length; i++)
        {
            ParticleSystem chimneyParticleSystem = chimneysTransforms[i].GetComponent<ParticleSystem>();
            chimneys.Add(chimneyParticleSystem);
            SwitchSmokeInChimneys();
        }
    }

    private void Update()
    {
        if (DayAndNightCycle.Instance.CheckIfTimePassed(3, lastUpdate))
        {
            SwitchSmokeInChimneys();
        }
    }

    public void SwitchSmokeInChimneys()
    {
        for (int i = 0; i < chimneys.Count; i++) {
            if (Random.value < 0.25) {
                if (smokingChimneys.Contains(i)) {
                    chimneys[i].Stop();
                    smokingChimneys.Remove(i);
                } else {
                    chimneys[i].Play();
                    smokingChimneys.Add(i);
                }
            }
        }
    }
}
