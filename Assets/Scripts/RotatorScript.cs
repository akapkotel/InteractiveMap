using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotatorScript : MonoBehaviour
{
    public bool isRotating = true;
    public float rotationSpeed;

    public GameObject[] rotatingObjects;
    public MapScript.RotationDirection direction;
}
