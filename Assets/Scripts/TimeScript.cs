using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScript : MonoBehaviour
{

    public float seconds { get; set; }
    public float epsilon = 0.01f;

    public int hours { get; set; }

    public static TimeScript MapTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        seconds += Time.deltaTime;

        hours = (int)seconds;
    }

    public bool CheckIfTimePassed(int requestedHours)
    {
        return seconds - hours < epsilon && hours % requestedHours == 0;
    }
}
