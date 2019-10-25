using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

[ExecuteInEditMode]
public class Location: MonoBehaviour
{
    [Tooltip("How big should this object indicator on the minimap be?"), Range(1, 4)] public int minimapSize;
    public bool known = true;
    public bool prespawned;
    public bool noOwner;
    public string locationName;
    public MapScript.LocationType locationType;
    public string owner;
    public string chief;
    public string description;
    
    protected TextMesh LocationMapLabel;

    // We cache all MeshRenderers and their original Materials for this GameObjects and its children, then we are going
    // to replace them with another Material when User points at this Location with mouse, then, we retrieve original
    // Materials again, when he moves mouse pointer away.
    private MeshRenderer[] _meshRenderers;
    private readonly List<Material> _defaultMaterials = new List<Material>();
    private readonly List<int> _ignoredMaterials = new List<int>();
    // We also "light-up" a circle-shaped marker around Location, when it is pointed with cursor:
    private GameObject _selectionMarker;
    // Red-dot visible only on the minimap in the left-upper-corner
    private GameObject _minimapIndicator;
    // Name of the location displayed next to it
    private GameObject _mapLabel;

    private UserInterface _userInterfaceInstance;

    private void Start()
    {
        _userInterfaceInstance = UserInterface.GetUserInterfaceInstance();
        
        name = locationName;
        
        LocationMapLabel = CreateLocationMapLabel();
        
        if (!Application.isPlaying) return;
        
        CreateMinimapIndicator();

        CreateSelectionMarker();

        SetSphereCollider();

        SetMaterialsIndexes();
    }

    private void CreateMinimapIndicator()
    {
        if (_minimapIndicator != null) return;

        Transform minimapPos = transform;
        _minimapIndicator = Instantiate(_userInterfaceInstance.minimapIndicators[minimapSize - 1],
            minimapPos.position, minimapPos.rotation);
        _minimapIndicator.transform.SetParent(GameObject.Find("MinimapIndicators").transform);
        _minimapIndicator.name = locationName;
    }
    
    private void CreateSelectionMarker()
    {
        if (_selectionMarker != null) return;
        Transform parrentTransform = transform;
        _selectionMarker = Instantiate(_userInterfaceInstance.selectionMarker, parrentTransform.position, Quaternion.identity, parrentTransform);
        _selectionMarker.transform.localScale = new Vector3(minimapSize, 1, minimapSize);
    }

    protected virtual TextMesh CreateLocationMapLabel()
    {
        if (_mapLabel != null) return _mapLabel.GetComponent<TextMesh>();
        _mapLabel = new GameObject {layer = 8};
        _mapLabel.transform.position = transform.position + new Vector3(0f, 0f, minimapSize * 10);
        _mapLabel.transform.Rotate(35f, 0f, 0f);
        _mapLabel.transform.SetParent(GameObject.FindGameObjectWithTag("NameLabels").transform);
        
        TextMesh locationMapLabel = _mapLabel.AddComponent<TextMesh>();

        _mapLabel.name = locationName;
        locationMapLabel.text = locationName;
        locationMapLabel.anchor = TextAnchor.MiddleCenter;
        locationMapLabel.alignment = TextAlignment.Center;
        locationMapLabel.fontSize = 50 + (minimapSize * 20);
        locationMapLabel.fontStyle = FontStyle.Bold;
        locationMapLabel.richText = true;
        locationMapLabel.offsetZ = transform.localScale.z;
        return locationMapLabel;
    }
    
    private void SetSphereCollider()
    {
        if (GetComponent<SphereCollider>() != null) return;
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = minimapSize * 15;
    }

    private void OnBecameVisible()
    {
        enabled = true;
    }

    private void OnBecameInvisible()
    {
        enabled = false;
    }

    private void SetMaterialsIndexes()
    {
        //ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        _meshRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < _meshRenderers.Length; i++)
        {
            if (_meshRenderers[i].transform == transform) continue;
            if (_meshRenderers[i].transform.CompareTag("Window") ||
                _meshRenderers[i].transform.CompareTag("Lantern") ||
                _meshRenderers[i].transform.CompareTag("Haunted")) {
                _ignoredMaterials.Add(i);
            }
            
            _defaultMaterials.Add(_meshRenderers[i].sharedMaterial); // material
        }
    } 

    /// <summary>
    /// Changes all Materials for this Location buildings etc. when user points with mouse on this gameObject.
    /// All Materials are cached in List so they are retrieved back after mouse cursor leave the object.
    /// </summary>
    /// <param name="highlight"></param>
    protected virtual void HighlightLocationObjects(bool highlight)
    {
        Material higlightMaterial = _userInterfaceInstance.higlightLocationMaterial;
        
        _selectionMarker.SetActive(highlight);

        for (int i = 0; i < _defaultMaterials.Count; i++)
        {
            if (highlight) {
                LocationMapLabel.color = Color.green;
                LocationMapLabel.fontSize = 150;
                
                if (!_ignoredMaterials.Contains(i)) { // ignore all windows and lanterns 
                    _meshRenderers[i].sharedMaterial = higlightMaterial; // material
                }
            } else {
                LocationMapLabel.color = Color.white;
                LocationMapLabel.fontSize = 100;
                
                if (!_ignoredMaterials.Contains(i)) {
                    _meshRenderers[i].sharedMaterial = _defaultMaterials[i]; // material
                }
            }
        }
    }

    private void OnMouseEnter()
    {
        if (UserInterface.Instance.ActiveUi) return;
        HighlightLocationObjects(true);
    }

    private void OnMouseExit()
    {
        HighlightLocationObjects(false);
    }

    protected virtual void OnMouseDown()
    {
        if (UserInterface.Instance.ActiveUi) return;
        _userInterfaceInstance.SetLocationInfo(this);
    }
}
