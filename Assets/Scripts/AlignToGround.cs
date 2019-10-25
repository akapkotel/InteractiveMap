using UnityEngine;


public class AlignToGround : ScriptableObject
{
    public static Quaternion Align(Transform transform)
    {
        Quaternion rotation = transform.rotation;
        
        Ray ray = new Ray(transform.position, -Vector3.up);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return rotation;
        Quaternion groundTilt = Quaternion.FromToRotation(Vector3.up, hit.normal);

        rotation *= groundTilt;
        return rotation;
    }
}
