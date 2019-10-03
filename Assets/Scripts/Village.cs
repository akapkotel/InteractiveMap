using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[ExecuteInEditMode]
public class Village: PopulatedPlace
{

    public MapScript.VillageSize size; // actually 'size' of the village depends on it's wealth

    //private MapScript mapScript;

    void Awake()
    {
        size = RandomVillageWealth();
        population = (3+(int)size) * UnityEngine.Random.Range(45, 65);
        gameObject.layer = 8;

        GetRandomVillageName();

        PlaceBuildings();

        PlaceFields();

        SetSphereCollider();

        MoveSelectionMarketToEndOfHierarchy();
    }

    private void MoveSelectionMarketToEndOfHierarchy()
    {
        transform.GetChild(0).SetAsLastSibling();
    }

    private void SetSphereCollider()
    {
        SphereCollider collider = gameObject.AddComponent<SphereCollider>();
        collider.radius = 30;
    }

    /// <summary>
    /// Assures that average size/wealth of a Village will be 'Przeciętna' (average)
    /// </summary>
    /// <returns></returns>
    private MapScript.VillageSize RandomVillageWealth()
    {
        MapScript.VillageSize _size = (MapScript.VillageSize)UnityEngine.Random.Range(0, 3);
        if (_size != MapScript.VillageSize.Przeciętna)
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                _size = MapScript.VillageSize.Przeciętna;
            }
        }
        return _size;
    }

    void GetRandomVillageName()
    {
        int number = UnityEngine.Random.Range(0, MapScript.Instance.vilagesNames.Count);
        string villageName = MapScript.Instance.vilagesNames[number];
        MapScript.Instance.vilagesNames.RemoveAt(number);
        this.locationName = villageName;
        this.name = villageName;
    }

    private void PlaceBuildings()
    {
        int buildingsCount = 3 + (int)size;

        List<Vector2> positions = GetRandomPositions(buildingsCount, 15);

        GameObject[] buildings = MapScript.Instance.villageBuildings;

        PlaceObjects(buildings, buildingsCount, positions);
    }

    void PlaceFields()
    {
        int fieldsCount = 1 + (int)size;

        List<Vector2> fieldsPositions = GetRandomPositions(fieldsCount, 60);

        GameObject[] fields = MapScript.Instance.fields;

        PlaceObjects(fields, fieldsCount, fieldsPositions, true);
    }

    List<Vector2> GetRandomPositions(int positionsCount, int distance)
    {
        List<Vector2> positions = new List<Vector2>();
        for (int i = 0; i < positionsCount; i++)
        {
            float angle = i * (360 / positionsCount);
            float rad = Mathf.Deg2Rad * angle;

            Vector2 position = new Vector2(math.sin(rad), math.cos(rad)) * distance;  //UnityEngine.Random.insideUnitCircle * distance;

            positions.Add(position);
        }
        return positions;
    }

    void PlaceObjects(GameObject[] listOfObjects, int objectsCount, List<Vector2> positions, bool alignToGround = false)
    {
        for (int i = 0; i < objectsCount; i++)
        {
            // Pick one type of building - some buildings could be placed only once in the village!
            GameObject objectToPlace = listOfObjects[UnityEngine.Random.Range(0, listOfObjects.Length)];

            // Set position and rotation of the building in three steps:

            // get one position in x, and y dimensions:
            Vector2 position2D = positions[i];

            int currentTerrainIndex = MapScript.Instance.GetTerrainForCoordinates(position2D.x, position2D.y);

            float objectToPlaceY = Terrain.activeTerrains[currentTerrainIndex].SampleHeight(new Vector3(transform.position.x + position2D.x, 0, transform.position.z + position2D.y));
            objectToPlaceY += Terrain.activeTerrain.GetPosition().y;
            objectToPlaceY += 0.01f * i;

            // set position of the new object relative to the group center (parent transform):
            Vector3 desiredPosition = new Vector3(transform.position.x + position2D.x, objectToPlaceY, transform.position.z + position2D.y);

            // first, set random horizontal rotation along Y axis, then rotate verticaly acording to the ground:
            //Quaternion desiredRotation = AlignToGround.Align(objectToPlace.transform);

            GameObject newObject = Instantiate(objectToPlace, desiredPosition, Quaternion.identity, transform);

            newObject.transform.Rotate(0f, UnityEngine.Random.Range(0, 350), 0f);

            if (alignToGround)
            {
                newObject.transform.rotation = AlignToGround.Align(newObject.transform);
            }
        }
    }
}
