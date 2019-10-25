using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class UserProfile
{
    public static UserProfile Instance;
    
    private string _savePath = Application.persistentDataPath + "/userdata.dat";
    
    public List<string> FoundHiddenLocations { get; private set; }

    public List<string> UserPasswords;

    public UserProfile(string startPassword)
    {
        if (Instance == null){
            Instance = this;
        }
        
        if (File.Exists(_savePath)) {
            LoadData();
        } else {
            UserPasswords = new List<string>();
            FoundHiddenLocations = new List<string>();
        }
    }

    public void SaveData()
    {
        SavedData data = new SavedData()
        {
            foundLocations = FoundHiddenLocations,
            passwords = UserPasswords
        };
        
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Create(_savePath)) {
            bf.Serialize(file, data);
        }
        
        Debug.Log("User data saved successfully!");
    }

    private void LoadData()
    {
        SavedData loadedData = new SavedData();
        
        BinaryFormatter bf = new BinaryFormatter();

        using (FileStream file = File.Open(_savePath, FileMode.Open)) {
            loadedData = (SavedData) bf.Deserialize(file);
        }
            
        UserPasswords = loadedData.foundLocations;
        FoundHiddenLocations = loadedData.passwords;
            
        Debug.Log("User data loaded successfully!");
    }

    public void ClearSavedData()
    {
        if (File.Exists(_savePath)) {
            File.Delete(_savePath);
            Debug.Log("Saved data was deleted succesfully!");
        }
    }
}

[Serializable]
public class SavedData
{
    public List<string> foundLocations;
    public List<string> passwords;
}