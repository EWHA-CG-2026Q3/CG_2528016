using UnityEngine;

public class S09_ShearDiamond : MonoBehaviour
{
    [SerializeField] private MeshFilter originalFilter;
    [SerializeField] private MeshFilter shearedFilter;
    [SerializeField] private float k = 1.4f;

    // 0=아래 꼭짓점, 1~4=중간 사각형, 5=위 꼭짓점
    private Vector3[] baseVertices = new Vector3[]
    {
        new Vector3(0.5f, 0f, 0.5f),
        new Vector3(0f, 0.5f, 0f),
        new Vector3(1f, 0.5f, 0f),
        new Vector3(1f, 0.5f, 1f),
        new Vector3(0f, 0.5f, 1f),
        new Vector3(0.5f, 1f, 0.5f),
    };

    private int[] triangles = new int[]
    {
        5,1,2, 5,2,3, 5,3,4, 5,4,1,
        0,2,1, 0,3,2, 0,4,3, 0,1,4
    };

    void Start()
    {
        // 원본: 변환 없이 그대로
        originalFilter.mesh = BuildMesh(baseVertices);

        // 기울인 버전: shear 행렬 적용
        Vector3[] sheared = ApplyShear(baseVertices, k);
        shearedFilter.mesh = BuildMesh(sheared);

        Debug.Log($"k = {k} / 꼭대기 정점: {baseVertices[5].ToString("F2")} → {sheared[5].ToString("F2")}");
    }

    private Mesh BuildMesh(Vector3[] verts)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    private Vector3[] ApplyShear(Vector3[] vertices, float k)
    {
        float[,] H = ShearMatrixRaw(k);
        Vector3[] result = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector4 h = ToHomogeneous(vertices[i]);
            h = MultiplyMatrixVectorRaw(H, h);
            result[i] = FromHomogeneous(h);
        }
        return result;
    }

    // 핵심: 변환 후 e2 = (k, 1, 0, 0) 을 2열에 넣은 행렬
    float[,] ShearMatrixRaw(float k)
    {
        return new float[,] {
            { 1f, k,  0f, 0f },
            { 0f, 1f, 0f, 0f },
            { 0f, 0f, 1f, 0f },
            { 0f, 0f, 0f, 1f }
        };
    }

    Vector4 ToHomogeneous(Vector3 v)
    {
        return new Vector4(v.x, v.y, v.z, 1f);
    }

    Vector3 FromHomogeneous(Vector4 h)
    {
        return new Vector3(h.x, h.y, h.z);
    }

    Vector4 MultiplyMatrixVectorRaw(float[,] M, Vector4 v)
    {
        float[] input = { v.x, v.y, v.z, v.w };
        float[] result = new float[4];
        for (int row = 0; row < 4; row++)
            for (int col = 0; col < 4; col++)
                result[row] += M[row, col] * input[col];
        return new Vector4(result[0], result[1], result[2], result[3]);
    }
}