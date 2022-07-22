using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void FixedUpdate()
    {
        FaceTextMeshToCamera();
    }

    void FaceTextMeshToCamera()
    {
        Vector3 origRot = gameObject.transform.eulerAngles;
        gameObject.transform.LookAt(Camera.main.transform);
        Vector3 desiredRot = gameObject.transform.eulerAngles;
        origRot.y = desiredRot.y + 180;
        gameObject.transform.eulerAngles = origRot;
    }
}
