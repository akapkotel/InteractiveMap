using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public Light[] ignoredLights;

    [Tooltip("How fast should camera move around the map?")] public float cameraMoveSpeed = 100f;

    [Tooltip("How fast should camera zoom in and out?")] public float cameraZoomSpeed = 100f;

    [Tooltip("How close mouse cursor should be placed to move camera around?")] public float mouseMoovementArea = 10f;

    public float minCameraX, maxCameraX, minCameraZ, maxCameraZ;

    [Tooltip("How close to the ground can camera zoom down?")] public float minCameraY = 75f;

    [Tooltip("How hight could camera fly?")] public float maxCameraY = 2000f;

    [Tooltip("How low camera should go to reveal locations on the map?")] public float locationsHidingDistance;

    [HideInInspector] public bool locationsVisible;

    private List<Location> locations;

    private Camera cam;

    private LayerMask mask; // culling-mask used to retrieve non-culled visibility state

    private Quaternion noRotation;
    private Vector3 tempRotation;

    float cameraSpeedModiffier; // modifies speed of camera acording to it's height above the map

    float deltaTime;

    Vector3 cameraPosition; // used to move Camera around map

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        
        cam = Camera.main;
        noRotation = transform.parent.rotation;
        mask = cam.cullingMask;

        locations = new List<Location>();
        locationsVisible = true;

        CheckLocationsVisibility();
    }

    void OnPreCull()
    {
        foreach (Light light in ignoredLights)
        {
            light.enabled = false;
        }
    }

    void OnPostRender()
    {
        foreach (Light light in ignoredLights)
        {
            light.enabled = true;
        }
    }


    void LateUpdate()
    {
        deltaTime = Time.deltaTime;

        cameraPosition = transform.parent.position;

        if (Input.GetMouseButton(1))
        {
            RotateCamera();
            return;
        }

        if (Input.GetMouseButtonUp(1))
        {
            transform.parent.rotation = noRotation;
        }


        cameraSpeedModiffier = cameraPosition.y / maxCameraY;

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - mouseMoovementArea) {
            cameraPosition.z += cameraMoveSpeed * cameraSpeedModiffier * deltaTime;
        }

        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= mouseMoovementArea) {
            cameraPosition.z -= cameraMoveSpeed * cameraSpeedModiffier * deltaTime;
        }

        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= mouseMoovementArea) {
            cameraPosition.x -= cameraMoveSpeed * cameraSpeedModiffier * deltaTime;
        }

        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - mouseMoovementArea)
        {
            cameraPosition.x += cameraMoveSpeed * cameraSpeedModiffier * deltaTime;
        }

        cameraPosition.z = Mathf.Clamp(cameraPosition.z, minCameraZ, maxCameraZ);
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, minCameraX, maxCameraX);

        float scroll = Input.mouseScrollDelta.y; // Input.GetAxis("Mouse ScrollWheel");
        cameraPosition.y -= scroll * cameraZoomSpeed * deltaTime;

        cameraPosition.y = Mathf.Clamp(cameraPosition.y, minCameraY, maxCameraY);

        transform.parent.position = cameraPosition;

        CheckLocationsVisibility();
    }

    private void RotateCamera()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            tempRotation.x += Input.GetAxis("Mouse X") * 10f;
            tempRotation.y -= Input.GetAxis("Mouse Y") * 10f;

            tempRotation.y = Mathf.Clamp(tempRotation.y, -30f, 150f);
        }

        Quaternion QT = Quaternion.Euler(tempRotation.y, tempRotation.x, 0f);

        transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, QT, Time.deltaTime * 10f);
    }

    private void CheckLocationsVisibility()
    {
        // Hiding and deactivating colliders of objects on the map when camera is too high
        if (transform.position.y >= locationsHidingDistance)
        {
            if (locationsVisible)
            {
                locationsVisible = false;
                cam.cullingMask = 1;
                ShowOrHideLocations();
            }
        }
        else
        {
            if (!locationsVisible)
            {
                locationsVisible = true;
                cam.cullingMask = mask;
                ShowOrHideLocations();
            }
        }
    }

    public void RegisterInCameraLocationsList(Location location)
    {
        locations.Add(location);
    }

    void ShowOrHideLocations()
    {
        for (int i = 0; i < MapScript.Instance.locations.Count; i++)
        {
            MapScript.Instance.locations[i].ShowOrHideOnMap();
        }
    }
}
