using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulatedPlace : Location
{
    public int population;
    // Start is called before the first frame update

    protected MapScript MapScriptInstance;
    
    public virtual void Awake()
    {
        MapScriptInstance = MapScript.GetMapScriptInstance();
    }

    protected void BuildTheBuilding(GameObject building, bool rotateAfterBuild, Transform bTransform = null)
    {
        if (bTransform is null) {
            bTransform = transform;
        }
        
        GameObject constructed = Instantiate(building, bTransform.position, bTransform.rotation, bTransform);

        if (rotateAfterBuild)
        {
            constructed.transform.Rotate(0f, UnityEngine.Random.Range(0, 350), 0f);
        }
    }
    
    protected void PlaceObjects(GameObject[] listOfObjects, int objectsCount, List<Vector2> positions, bool alignToGround = false)
    {
        for (int i = 0; i < objectsCount; i++)
        {
            Vector3 centralPosition = transform.position;
            // Pick one type of building - some buildings could be placed only once in the village!
            GameObject objectToPlace = listOfObjects[UnityEngine.Random.Range(0, listOfObjects.Length)];

            // Set position and rotation of the building in three steps:

            // get one position in x, and y dimensions:
            Vector2 position2D = positions[i];

            int currentTerrainIndex = MapScript.GetTerrainForCoordinates(position2D.x, position2D.y);

            float objectToPlaceY = Terrain.activeTerrains[currentTerrainIndex].SampleHeight(new Vector3(centralPosition.x + position2D.x, 0, centralPosition.z + position2D.y));
            objectToPlaceY += Terrain.activeTerrain.GetPosition().y;
            objectToPlaceY += 0.01f * i;

            // set position of the new object relative to the group center (parent transform):
            Vector3 desiredPosition = new Vector3(centralPosition.x + position2D.x, objectToPlaceY, centralPosition.z + position2D.y);

            // first, set random horizontal rotation along Y axis, then rotate verticaly acording to the ground:
            GameObject newObject = Instantiate(objectToPlace, desiredPosition, Quaternion.identity, transform);

            newObject.transform.Rotate(0f, UnityEngine.Random.Range(0, 350), 0f);

            if (alignToGround)
            {
                newObject.transform.rotation = AlignToGround.Align(newObject.transform);
            }
        }
    }
}
