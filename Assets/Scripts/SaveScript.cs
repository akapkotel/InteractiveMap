using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveScript : MonoBehaviour
{
    protected string savePath;

    public List<List<float>> pseudoVectors = new List<List<float>>();
    public List<List<float>> pseudoQuaternions = new List<List<float>>();

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/gamesave.sav";
    }

    public void SaveData(List<Transform> villagesTransforms)
    {
        var savedData = new Save();

        for (int i = 0; i < villagesTransforms.Count; i++)
        {
            List<float> pseudoVector = new List<float>(3);
            List<float> pseudoQuaternion = new List<float>(4);

            pseudoVector.Add(villagesTransforms[i].position.x);
            pseudoVector.Add(villagesTransforms[i].position.y);
            pseudoVector.Add(villagesTransforms[i].position.z);

            pseudoQuaternion.Add(villagesTransforms[i].rotation.x);
            pseudoQuaternion.Add(villagesTransforms[i].rotation.y);
            pseudoQuaternion.Add(villagesTransforms[i].rotation.z);
            pseudoQuaternion.Add(villagesTransforms[i].rotation.w);

            savedData.pseudoVectors.Add(pseudoVector);
            savedData.pseudoQuaternions.Add(pseudoQuaternion);
        }
        var binaryFormatter = new BinaryFormatter();
        using (var fileStream = File.Create(savePath))
        {
            binaryFormatter.Serialize(fileStream, savedData);
        }
        Debug.Log("Data saved!");
    }

    public List<Transform> LoadData()
    {
        if (File.Exists(savePath))
        {
            List<Transform> transforms = new List<Transform>();

            GameObject dummyObject = new GameObject();
            Transform _transform = dummyObject.transform;

            Vector3 position = new Vector3();
            Quaternion _quaternion = Quaternion.identity;

            Save loadedData;

            var binaryFormatter = new BinaryFormatter();
            using (var fileStream = File.OpenRead(savePath))
            {
                loadedData = (Save)binaryFormatter.Deserialize(fileStream);

                pseudoVectors = loadedData.pseudoVectors;
                pseudoQuaternions = loadedData.pseudoQuaternions;

                for (int i = 0; i < pseudoVectors.Count; i++)
                {
                    position.x = pseudoVectors[i][0];
                    position.y = pseudoVectors[i][1];
                    position.z = pseudoVectors[i][2];

                    _quaternion.x = pseudoQuaternions[i][0];
                    _quaternion.y = pseudoQuaternions[i][1];
                    _quaternion.z = pseudoQuaternions[i][2];
                    _quaternion.w = pseudoQuaternions[i][3];

                    _transform.position = position;
                    _transform.rotation = _quaternion;

                    transforms.Add(_transform);
                }
            }
            return transforms;
        }
        return null;
    }
}

[System.Serializable]
public class Save
{
    // stores all 'Transforms'
    public List<List<float>> pseudoVectors = new List<List<float>>();

    // stores all 'Quaternions'
    public List<List<float>> pseudoQuaternions = new List<List<float>>();
}
