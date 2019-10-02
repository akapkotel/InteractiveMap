using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WoodsGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] leafTrees;
    [SerializeField] private GameObject[] pineTrees;

    // Get prefabs and terrain
    private void Awake()
    {
        RandomizeForestType();
    }

    // Randomly decide if forest is: leaf or pine or mixed

    void RandomizeForestType()
    {
        Vector3 forestPosition = GetComponent<ObjectsSpawner>().GetOneRandomPositionOnMap();
    }

    // Randomly decide size and density of the forest

    // Spawn forest dummy empty gameObject, then spawn all the trees

    // Make trees children of the forest gameObject

    // make forest gameObject a child of the 'woods' gameObject

}
