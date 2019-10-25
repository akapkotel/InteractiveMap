using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
// <summary>
// Spawn and place randomly on the terrain any prefab.
// </summary>
public class ObjectsSpawner : MonoBehaviour
{   
    [SerializeField, Tooltip("Do you want this prefab to be spawned?")] bool spawn;
    [SerializeField, Tooltip("Size, or 'width' of the map")] int mapSize;
    [SerializeField, Tooltip("How close to the map-edges could be this prefab spawned?")] int mapMargin;

    [SerializeField, Tooltip("Put here prefab you want to spawn randomly on the map.")] GameObject objectToSpawn;
    [SerializeField, Tooltip("How many gameObjects do you want to place on the map?")] int numberOfObjectsToSpawn;
    [SerializeField, Tooltip("Put here gameObject from hierarchy, you want to be parrent of the spawned prefab.")] Transform parentTransform;

    private List<Vector3> _positions3D;

    private List<int> _terrainIndexes;

    private MapScript _mapScriptInstance;
    
    private void Awake()
    {
        _mapScriptInstance = MapScript.GetMapScriptInstance();
        
        _terrainIndexes = new List<int>(numberOfObjectsToSpawn);
    }
    
    public void RunSpawning()
    {
        if (spawn) {
            _positions3D = new List<Vector3>(numberOfObjectsToSpawn);

            GenerateRandomPositionsOnMap();

            SpawnNewObjectsForEachPositionOnMap();
            
            _mapScriptInstance.finishedSpawners += 1;
        }
        else {
            _mapScriptInstance.finishedSpawners += 1;
        }
    }

    private void GenerateRandomPositionsOnMap()
    {
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            _positions3D.Add(GetOneRandomPositionOnMap());
        }
    }

    /// <summary>
    /// Public method to obtaining randomly placed Vector3 positions on the map.
    /// </summary>
    /// <returns>Vector3</returns>
    private static Vector3 GetOneRandomPositionOnMap()
    {
        int posx = Random.Range(100, 11900);
        int posz = Random.Range(100, 11900);

        //int currentTerrainIndex = MapScript.GetTerrainForCoordinates(posx, posz);

        float posy = MapScript.GetTerrainHeightAtPosition(posx, posz);//Terrain.activeTerrains[currentTerrainIndex].terrainData.GetHeight(posx, posz);

        return new Vector3(posx, posy, posz);
    }

    private void SpawnNewObjectsForEachPositionOnMap()
    {
        if (objectToSpawn == null) return;
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            GameObject newObject = Instantiate(objectToSpawn, _positions3D[i], Quaternion.identity);

            if (parentTransform != null)
            {
                newObject.transform.SetParent(parentTransform);
            }
        }
    }
}
