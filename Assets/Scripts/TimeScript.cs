using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScript : MonoBehaviour
{

    public float Seconds { get; set; }
    public float epsilon = 0.01f;

    public int Hours { get; set; }

    public static TimeScript MapTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Seconds += Time.deltaTime;

        Hours = (int)Seconds;
    }

    public bool CheckIfTimePassed(int requestedHours)
    {
        return Seconds - Hours < epsilon && Hours % requestedHours == 0;
    }
}
