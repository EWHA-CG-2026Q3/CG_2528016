using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S04_CustomDiamondMesh : MonoBehaviour
{
    void Start()
    {
        // 정점 6개 - 위/아래 꼭짓점 2개 + 가운데 4개
    
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0.5f, 0f, 0.5f), // 0: 아래
            new Vector3(0.5f, 1f, 0.5f), // 1: 위
            new Vector3(0f,   0.5f, 0f), // 2: 앞왼
            new Vector3(1f,   0.5f, 0f), // 3: 앞오른
            new Vector3(1f,   0.5f, 1f), // 4: 뒤오른
            new Vector3(0f,   0.5f, 1f), // 5: 뒤왼
        };

        int[] triangles = new int[]
        {
            // 위
            1, 2, 3,
            1, 3, 4,
            1, 4, 5,
            1, 5, 2,

            // 아래
            0, 3, 2,
            0, 4, 3,
            0, 5, 4,
            0, 2, 5,
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }
}