using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectsForPlaceholdersGenerator))]
public class PlaceholdersGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ObjectsForPlaceholdersGenerator generator = (ObjectsForPlaceholdersGenerator) target;

        if (GUILayout.Button("Generate objects in placeholders"))
        {
            generator.SpawnPrefabsInPlaceholders();
        }

        if (GUILayout.Button("Remove spawned objects"))
        {
            generator.RemoveSpawnedPrefabs();
        }
    }
}
