using System;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[ExecuteInEditMode]
public class TreesManager : MonoBehaviour
{
    private Material[] _treesMaterials;

    private GameObject[] _trees;

    private void Awake()
    {
        LoadTreesMaterialsFromAssets();

        FindAllTreesOnMap();

        RandomizeTreesMaterialsInForest();

        DestroyImmediate(this);
    }

    private void FindAllTreesOnMap()
    {
        _trees = GameObject.FindGameObjectsWithTag("Tree").Where(t => t.transform.parent == transform).ToArray();
    }

    private void RandomizeTreesMaterialsInForest()
    {
        for (int i = 0; i < _trees.Length; i++)
        {
            Material randomMaterial = _treesMaterials[UnityEngine.Random.Range(0, _treesMaterials.Length)];
            
            for (int j = 0; j < _trees[i].transform.childCount; j++)
            {
                MeshRenderer[] renderers = _trees[i].GetComponentsInChildren<MeshRenderer>();

                for (int k = 0; k < renderers.Length; k++)
                {
                    if (!renderers[k].CompareTag("Trunk") || renderers[k].sharedMaterial.Equals(randomMaterial))
                    {
                        renderers[k].sharedMaterial = randomMaterial;
                    }
                }
            }

            AlignToGround.Align(_trees[i].transform);
        }
    }

    private void LoadTreesMaterialsFromAssets()
    {
        _treesMaterials = new[]
        {
            Resources.Load<Material>("Materials/Grain(green)"),
            Resources.Load<Material>("Materials/Grass"),
            Resources.Load<Material>("Materials/TreeLeafs"),
            Resources.Load<Material>("Materials/TreeLeafs2"),
            Resources.Load<Material>("Materials/TreeLeafs3"),
        };
    }
}
