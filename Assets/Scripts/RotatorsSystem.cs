using UnityEngine;

public class RotatorsSystem : MonoBehaviour
{
    private RotatorScript[] rotators;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        rotators = FindObjectsOfType<RotatorScript>();
        Debug.Log("RotatorScript instances found:"+rotators.Length);  
    }

    // Update is called once per frame
    void Update()
    {
        foreach (RotatorScript rotator in rotators)
        { 
            if (rotator.isRotating && Vector3.Distance(rotator.transform.position, cam.transform.position) < 500f)
            {
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
}
