using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class Town : PopulatedPlace
{
    private Transform[] _housesPositions;
    
    public override void Awake()
    {
        base.Awake();
        
        if (prespawned) // we do not spawn again a Village which is already on the map before first frame happens
        {
            return;
        }

        population = Random.Range(500, 1000);

        _housesPositions = transform.GetComponentsInChildren<Transform>()
            .Where(t => t.GetSiblingIndex() > 1)
            .ToArray();
        
        BuildMarket();

        BuildChurch();

        BuildTownHouses();

        gameObject.isStatic = true;
        
        prespawned = true;
    }
    
    private void BuildMarket()
    {
        GameObject marketPrefab = MapScriptInstance.Markets[Random.Range(0, MapScriptInstance.Markets.Length)];
        
        BuildTheBuilding(building: marketPrefab,false, bTransform: transform.GetChild(0).transform);
    }
    
    private void BuildChurch()
    {
        GameObject churchPrefab = MapScriptInstance.Churches[Random.Range(0, MapScriptInstance.Churches.Length)];
        
        BuildTheBuilding(building: churchPrefab,false, bTransform: transform.GetChild(1).transform);
    }
    
    private void BuildTownHouses()
    {
        Debug.Log((gameObject.name, _housesPositions.Length));

        foreach (Transform housePosition in _housesPositions)
        {
            GameObject house;
            
            if (housePosition.name.EndsWith("Right"))
            {
                house = MapScriptInstance.TownHousesLeft[Random.Range(0, MapScriptInstance.TownHousesLeft.Length)];
            } else if (housePosition.name.EndsWith("Middle")) {
                house = MapScriptInstance.TownHousesMiddle[Random.Range(0, MapScriptInstance.TownHousesMiddle.Length)];
            } else {
                house = MapScriptInstance.TownHousesRight[Random.Range(0, MapScriptInstance.TownHousesRight.Length)];
            }
            
            BuildTheBuilding(house, false, housePosition);
        }
    }
}
