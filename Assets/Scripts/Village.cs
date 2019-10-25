using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class Village: PopulatedPlace
{

    public MapScript.VillageSize size; // actually 'size' of the village depends on it's wealth

    public bool hasChurch;
    
    public override void Awake()
    {
        base.Awake();
        
        if (prespawned) // we do not spawn again a Village which is already on the map before first frame happens
        {
            return;
        }

        locationType = MapScript.LocationType.Wioska;
        size = RandomVillageWealth();
        population = (3+(int)size) * Random.Range(45, 65);
        gameObject.layer = 8;
        
        locationName = GetNewRandomName();

        PlaceHouses();
        
        PlaceSpecialBuilding();

        PlaceFields();

        gameObject.isStatic = true;
        
        prespawned = true;
    }

    private string GetNewRandomName()
    {
        return MapScriptInstance.GetRandomVillageName();
    }

    /// <summary>
    /// Assures that average size/wealth of a Village will be 'Przeciętna' (average)
    /// </summary>
    /// <returns></returns>
    private MapScript.VillageSize RandomVillageWealth()
    {
        MapScript.VillageSize desiredSize = (MapScript.VillageSize)Random.Range(0, 3);
        if (desiredSize == MapScript.VillageSize.Przeciętna) return desiredSize;
        if (Random.value > 0.5f)
        {
            desiredSize = MapScript.VillageSize.Przeciętna;
        }
        return desiredSize;
    }

    private void PlaceHouses()
    {
        int buildingsCount = 3 + (int)size;

        List<Vector2> positions = GetRandomPositions(buildingsCount, 15);

        GameObject[] houses = MapScriptInstance.VillageHouses;

        PlaceObjects(houses, buildingsCount, positions);
    }

    private void PlaceSpecialBuilding()
    {
        float buildingChance = Random.value;

        GameObject building;
        
        if (buildingChance < 0.2) {
            return;
        } else if (buildingChance < 0.4) { // windmill
            building = MapScriptInstance.Windmills[Random.Range(0, MapScriptInstance.Windmills.Length)];
        } else if (buildingChance < 0.6) { // inn
            building = MapScriptInstance.Taverns[Random.Range(0, MapScriptInstance.Taverns.Length)];
        } else if (buildingChance < 0.8) { // church
            hasChurch = true;
            building = MapScriptInstance.Churches[Random.Range(0, MapScriptInstance.Churches.Length)];
        } else {// marketplace 
            building = MapScriptInstance.Markets[0];
        } 

        BuildTheBuilding(building, buildingChance > 0.4);
    }

    private void PlaceFields()
    {
        int fieldsCount = 1 + (int)size;

        List<Vector2> fieldsPositions = GetRandomPositions(fieldsCount, 60);

        GameObject[] fields = MapScriptInstance.Fields;

        PlaceObjects(fields, fieldsCount, fieldsPositions, true);
    }

    private static List<Vector2> GetRandomPositions(int positionsCount, int distance)
    {
        List<Vector2> positions = new List<Vector2>();
        for (int i = 0; i < positionsCount; i++)
        {
            float angle = i * (360 / positionsCount);
            float rad = Mathf.Deg2Rad * angle;

            Vector2 position = new Vector2(math.sin(rad), math.cos(rad)) * distance;

            positions.Add(position);
        }
        return positions;
    }


}
