using UnityEngine;

[ExecuteInEditMode]
public class AlignToGround : ScriptableObject
{
    public static Quaternion Align(Transform transform)
    {
        Ray ray = new Ray(transform.position, -Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Quaternion grndTilt = Quaternion.FromToRotation(Vector3.up, hit.normal);

            transform.rotation = grndTilt * transform.rotation;
        }
        return transform.rotation;
    }
}
