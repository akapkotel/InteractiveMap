using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectsSpawner))]
public class ObjectsSpawnerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ObjectsSpawner spawner = (ObjectsSpawner) target;
        
        if (GUILayout.Button("Spawn objects"))
        {
            spawner.RunSpawning();
        }
    }
}
