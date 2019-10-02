using UnityEngine;

[ExecuteInEditMode]
public class SnapToTerrain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float height = Terrain.activeTerrain.SampleHeight(new Vector3(transform.position.x, 0, transform.position.z)) + Terrain.activeTerrain.GetPosition().y;

        transform.position.Set(transform.position.x, height, transform.position.z);

        Ray ray = new Ray(transform.position + Vector3.up * 10.0f, Vector3.down);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
    }

}
