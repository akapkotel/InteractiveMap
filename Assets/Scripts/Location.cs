using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Location: MonoBehaviour
{
    public string locationName;
    public string house;
    public string owner;

    public bool locationHiglighted;

    private Collider _collider;

    // stores integer indexec of eachchildren's Material, indexes will be used to retrieve correct Material for each child
    //private List<int> childrenMaterials;

    private List<Material> childrenMaterials = new List<Material>();
    private List<int> ignoredMaterials = new List<int>();
    private GameObject selectionMarker;

    void Awake()
    {
        name = locationName;
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
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        int windows = 0;
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshRenderers[i].transform != transform)
            {
                if (meshRenderers[i].transform.CompareTag("Window"))
                {
                    windows++;
                    ignoredMaterials.Add(i);
                }
                childrenMaterials.Add(meshRenderers[i].material);
            }
        }
        Debug.Log("Found windows: " + windows);
    }

    public void HiglightLocationObjects(bool highlight)
    {
        if (childrenMaterials.Count == 0) {
            SetMaterialsIndexes();
        }

        if (selectionMarker == null) {
            selectionMarker = transform.GetChild(transform.childCount-1).gameObject;
        }

        Material higlightMaterial = UserInterface.Instance.higlightLocationMaterial;
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        selectionMarker.SetActive(highlight);

        for (int i = 0; i < childrenMaterials.Count; i++)
        {
            if (highlight) {
                if (!ignoredMaterials.Contains(i))
                {
                    meshRenderers[i].material = higlightMaterial;
                }
            } else {
                if (!ignoredMaterials.Contains(i))
                {
                    meshRenderers[i].material = childrenMaterials[i];
                }
            }
        }
    }

    public void ShowOrHideOnMap() {
        _collider = gameObject.GetComponent<Collider>();
        _collider.enabled = !_collider.enabled;
    }
}
