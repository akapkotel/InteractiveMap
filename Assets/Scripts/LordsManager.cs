using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Entities;
using UnityEngine;

public class LordsManager
{
    public static LordsManager Instance;

    public enum Titles { Hrabia, Baron, Baronet, Kawaler }
    public enum Fractions { Rojaliści, Markalianie, Neutralny }

    public Dictionary<string, Lord> Lords { get; }

    private TextAsset _lordsFile;

    public LordsManager(Dictionary<string, Location> locationsDictionary)
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        Lords = new Dictionary<string, Lord>();

        AddLordsFromLocationsToDictionary(locationsDictionary);

        LoadOtherLordsFromFile();

        //DistributeLocationsAmongLords(locationsDictionary);
    }

    private void AddLordsFromLocationsToDictionary(Dictionary<string, Location> locationsDictionary)
    {

        foreach (KeyValuePair<string, Location> pair in locationsDictionary)
        {
            if (pair.Value.noOwner) continue;
            
            string lordName = pair.Value.owner;

            if (!Lords.ContainsKey(lordName)){
                Lord newLord = new Lord {Name = lordName};
                Lords.Add(lordName, newLord);
            }
            
            Lords[lordName].Possessions.Add(pair.Value);
        }
    }
    
    private void LoadOtherLordsFromFile()
    {
        _lordsFile = Resources.Load<TextAsset>("TextFiles/lords");
        string[] lordsString = _lordsFile.text.Split('.');

        string[] counts = lordsString[0].Split(';');
        string[] barons = lordsString[1].Split(';');
        string[] baronets = lordsString[2].Split(';');
        string[] chevaliers = lordsString[3].Split(';');

        List<string[]> allLords = new List<string[]> {counts, barons, baronets, chevaliers};
        Dictionary<int, Titles> titles = new Dictionary<int, Titles>
        {
            {0, Titles.Hrabia}, {1, Titles.Baron}, {2, Titles.Baronet}, {3, Titles.Kawaler}
        };
        
        foreach (string[] rank in allLords)
        {
            Titles titleFromIndex = (Titles) allLords.IndexOf(rank);
            foreach (string lord in rank)
            {
                AddNewLordToDictionary(lord, titleFromIndex);
            }
        }
    }

    private void AddNewLordToDictionary(string lordString, Titles title)
    {
        string[] lordData = lordString.Split(',');
        string lordName = lordData[0];

        if (!Lords.ContainsKey(lordName)) {
            Lord newLord = new Lord {Name = lordName};
            Lords.Add(lordName, newLord);
        }

        Lords[lordName].Title = title;
        Lords[lordName].Vassals.AddRange(lordData.Where(t => t !=lordName));
        
        foreach (string vassal in Lords[lordName].Vassals)
        {
            if (!Lords.ContainsKey(vassal))
            {
                Lord newVassal = new Lord {Name = vassal};
                Lords.Add(newVassal.Name, newVassal);
            }
            Lords[vassal].Seignior = lordName;
        }
    }
    
    public void DistributeLocationsAmongLords(Dictionary<string, Location> locationsDictionary)
    {
        // get list of locations which are not owned by anyone and could be possessed:
        List<Location> availableLocations = locationsDictionary.Values
            .Where(t => t.owner == null && t.noOwner == false)
            .ToList();
        
        // get positions of center-weight for the lands of the lords and build dictionary of these positions and names:
        GameObject lordsLands = GameObject.Find("LordsLands");
        
        // set amount of villages each lord should control according to his title:
        Dictionary<Titles, int> locationsPerTitle = new Dictionary<Titles, int>
        {
            {Titles.Hrabia, 206}, {Titles.Baron, 40}, {Titles.Baronet, 5}, {Titles.Kawaler, 3}
        };
        
        // how many villages should each lord control directly:
        Dictionary<Titles, int> personalFiefs = new Dictionary<Titles, int>
        {
            {Titles.Hrabia, 12}, {Titles.Baron, 8}, {Titles.Baronet, 5}, {Titles.Kawaler, 3}
        };

        // find actual center for each lord lands or generate new, random position:
        foreach (Lord lord in Lords.Values) {
            lord.LandsCenter = GetLordLandsCenter(lord, lordsLands);
        }

        // for each lord we have Vector3 (position of the center of it's lands) and his 
        
        List<string> takenLocations = new List<string>(); // keep track which location was assigned to some lord
        // iterate through all the noblemen ranks:
        for (int i = 0; i < locationsPerTitle.Count; i++) {
            Titles title = (Titles) i; 
            
            List<Lord> lordsOfTitle = Lords.Values
                .Where(t => t.Title == title && t.Possessions.Count < locationsPerTitle[title])
                .ToList(); // get list of proper rank lords

            for (int j = 0; j < locationsPerTitle[title]; j++) {
                for (int k = 0; k < lordsOfTitle.Count; k++)
                {
                    Lord lord = lordsOfTitle[i];
                    List<Location> lands = lord.Title == Titles.Hrabia ? availableLocations : Lords[lord.Seignior].Possessions;

                    Location acquired = lands.First(t => !takenLocations.Contains(t.locationName) && t.owner.Length == 0);

                    lands.Remove(acquired);
                    
                    lord.Possessions.Add(acquired);

                    if (j < personalFiefs[title] && lord.Possessions.Count < personalFiefs[title]) acquired.owner = lord.Name;
                    
                    takenLocations.Add(acquired.locationName);
                    
                    RecalculateCenterOfLands(lord.Name, lord.Possessions.Select(t => t.transform.position).ToArray());
                }
            }
            takenLocations.Clear(); // to make it possible for next rank to acquire locations
        }
    }

    private void RecalculateCenterOfLands(string lordName, Vector3[] ownedPositions)
    {
        float totalX = 0;
        float totalZ = 0;
        for (int i = 0; i < ownedPositions.Length; i++)
        {
            totalX += ownedPositions[i].x;
            totalZ += ownedPositions[i].z;
        }

        float x = totalX / ownedPositions.Length;
        float z = totalZ / ownedPositions.Length;
        float y = MapScript.GetTerrainHeightAtPosition(x, z);
        
        Vector3 position = new Vector3(x, y, z);

        Lords[lordName].LandsCenter = position;
    }
    
    private Vector3 GetLordLandsCenter(Lord lord, GameObject lordsLands)
    {
        Transform[] lordsLandsCenters = lordsLands.GetComponentsInChildren<Transform>();
        
        Transform landsCenter = lordsLandsCenters.FirstOrDefault(t => t.name.Equals(lord.Name));
        if (landsCenter != null && landsCenter.name.Equals(lord.Name)) {
            return landsCenter.position;
        } 
        
        return lord.Seignior.Length > 0 ? Lords[lord.Seignior].LandsCenter : GetRandomLandsCenter();
    }

    private Vector3 GetRandomLandsCenter()
    {
        float x = Random.Range(100, 11900);
        float z = Random.Range(100, 11900);
        float y = MapScript.GetTerrainForCoordinates(x, z);
        
        Vector3 newCenter = new Vector3(x, y, z);
        
        return newCenter;
    }
}

public class Lord
{
    public string Name { get; set; }
    public LordsManager.Titles Title { get; set; }
    public LordsManager.Fractions Fraction { get; set; }
    public string ChurchFunction { get; set; }
    public string Seignior { get; set; }
    public List<string> Vassals { get; set; }
    public Vector3 LandsCenter { get; set; }
    public List<Location> Possessions { get; private set;  }

    public Lord()
    {
        Vassals = new List<string>();
        Possessions = new List<Location>();
    }
}
