using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S12_StillLifeMesh : MonoBehaviour
{
    public enum Shape { Pentagon, Diamond, Pyramid }
    [SerializeField] private Shape shape = Shape.Diamond;
    [SerializeField] private Color color = Color.white;

    void Start()
    {
        Mesh mesh = new Mesh();
        switch (shape)
        {
            case Shape.Pentagon: // S03: 정점 5개, 삼각형 3개
                mesh.vertices = new Vector3[] {
                    new Vector3(0f, 1f, 0f), new Vector3(0.95f, 0.31f, 0f),
                    new Vector3(0.59f, -0.81f, 0f), new Vector3(-0.59f, -0.81f, 0f),
                    new Vector3(-0.95f, 0.31f, 0f) };
                mesh.triangles = new int[] { 0,1,2, 0,2,3, 0,3,4 };
                break;
            case Shape.Diamond: // S04: 정점 6개, 삼각형 8개
                mesh.vertices = new Vector3[] {
                    new Vector3(0.5f, 0f, 0.5f), new Vector3(0f, 0.5f, 0f),
                    new Vector3(1f, 0.5f, 0f),   new Vector3(1f, 0.5f, 1f),
                    new Vector3(0f, 0.5f, 1f),   new Vector3(0.5f, 1f, 0.5f) };
                mesh.triangles = new int[] { 5,1,2, 5,2,3, 5,3,4, 5,4,1,
                                             0,2,1, 0,3,2, 0,4,3, 0,1,4 };
                break;
            case Shape.Pyramid: // 정점 5개, 삼각형 6개 (바닥 2 + 옆면 4)
                mesh.vertices = new Vector3[] {
                    new Vector3(-0.5f, 0f, -0.5f), new Vector3(0.5f, 0f, -0.5f),
                    new Vector3(0.5f, 0f, 0.5f),   new Vector3(-0.5f, 0f, 0.5f),
                    new Vector3(0f, 1f, 0f) };
                mesh.triangles = new int[] { 0,2,1, 0,3,2, 0,1,4, 1,2,4, 2,3,4, 3,0,4 };
                break;
        }
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", color);
        GetComponent<MeshRenderer>().sharedMaterial = mat;
    }
}