using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDeformer : MonoBehaviour
{
    private Mesh deformingMesh;
    private Vector3[] originalVertices, displacedVertices;
    private Vector3[] vertexVelocities;
    private MeshCollider meshCollider; // Reference to the MeshCollider


    public float springForce = 50f;
    public float damping = 10f;
    public float colliderUpdateInterval = 0.01f; // Time in seconds between collider updates
    private float colliderUpdateTimer; // Timer to track when to update the collider
    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>(); // Get the MeshCollider component
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        vertexVelocities = new Vector3[originalVertices.Length];

        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }

        colliderUpdateTimer = 0f; // Initialize the timer
    }

    public void AddDeformingForce(Vector3 point, float force, bool isInflate)
    {
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force, isInflate);
        }
    }

    private void AddForceToVertex(int i, Vector3 point, float force, bool isInflate)
    {
        Vector3 pointToVertex = displacedVertices[i] - point;
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocity = attenuatedForce * Time.deltaTime;
        if (isInflate)
        {
            vertexVelocities[i] += pointToVertex.normalized * velocity;
        }
        else
        {
            vertexVelocities[i] -= pointToVertex.normalized * velocity;
        }
    }

    void Update()
    {
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }

        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();

        // Update the timer and check if it's time to update the collision mesh
        colliderUpdateTimer += Time.deltaTime;
        if (colliderUpdateTimer >= colliderUpdateInterval)
        {
            UpdateCollisionMesh();
            colliderUpdateTimer = 0f; // Reset the timer
        }
    }

    private void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        Vector3 displacement = displacedVertices[i] - originalVertices[i];
        velocity -= displacement * springForce * Time.deltaTime;
        velocity *= 1f - damping * Time.deltaTime;
        vertexVelocities[i] = velocity;
        displacedVertices[i] += velocity * Time.deltaTime;
    }

    private void UpdateCollisionMesh()
    {
        // Assign the updated mesh to the MeshCollider
        meshCollider.sharedMesh = null; // Clear the existing collision mesh
        meshCollider.sharedMesh = deformingMesh; // Assign the updated mesh
    }
}
