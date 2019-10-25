using System.Linq;
using UnityEngine;

public class RotatorsSystem : ScheduledScript
{
    public bool rotate = true;
    
    private RotatorScript[] _rotators;
    private Renderer[] _renderers;
    private Camera _cam;

    // Start is called before the first frame update
    private void Start()
    {
        _cam = Camera.main;
        
        _rotators = FindObjectsOfType<RotatorScript>();
        _renderers = _rotators.Select(t => t.GetComponent<Renderer>()).ToArray();
        
        Debug.Log("RotatorScript instances found:"+_rotators.Length);  
    }

    public void ToggleRotation()
    {
        rotate = !rotate;
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (!rotate || DayAndNightCycle.Instance.dayCycle == DayAndNightCycle.DayCycle.Night) return;
        
        for (int i = 0; i < _rotators.Length; i++)
        {
            RotatorScript rotator = _rotators[i];
            if (!rotator.isRotating || !(_renderers[i].isVisible)) continue;
            for (int j = 0; j < rotator.rotatingObjects.Length; j++)
            {
                if (rotator.direction == MapScript.RotationDirection.Counterclockwise)
                {
                    rotator.rotatingObjects[j].transform.Rotate(0f, 0f, rotator.rotationSpeed);
                }
                else
                {
                    rotator.rotatingObjects[j].transform.Rotate(0f, 0f, -rotator.rotationSpeed);
                }
            }
        }
    }
}
