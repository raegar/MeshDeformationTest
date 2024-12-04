using UnityEngine;

public class MeshDeformerInput : MonoBehaviour
{
    public float force = 1f;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            ApplyDeformingForce(true); // Inflate
        }
        if (Input.GetMouseButton(1))
        {
            ApplyDeformingForce(false); // Pinch
        }
    }

    private void ApplyDeformingForce(bool isInflate)
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(inputRay, out hit))
        {
            MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
            if (deformer != null)
            {
                deformer.AddDeformingForce(hit.point, force, isInflate);
            }
        }
    }
}
