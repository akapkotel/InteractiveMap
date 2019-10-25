using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapScript))]
public class MapScriptInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Clear saved map data")) {
            UserProfile.Instance.ClearSavedData();
        }

        if (GUILayout.Button("DistributeFiefsAmongLords"))
        {
            LordsManager.Instance.DistributeLocationsAmongLords(MapScript.Instance.locationsDictionary);
        }
    }
}
