using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_FloatingJewel : MonoBehaviour
{
    [SerializeField] private float size = 0.4f;
    [SerializeField] private float spinSpeed = 60f;   // 초당 회전 각도
    [SerializeField] private float bobHeight = 0.1f;  // 위아래 움직임 폭
    [SerializeField] private float bobSpeed = 1.5f;
    [SerializeField] private Material material;

    // S04 다이아몬드: 0=아래, 1~4=가운데 사각형, 5=위
    private readonly Vector3[] baseVertices =
    {
        new Vector3(0.5f, 0f, 0.5f), new Vector3(0f, 0.5f, 0f),
        new Vector3(1f, 0.5f, 0f),   new Vector3(1f, 0.5f, 1f),
        new Vector3(0f, 0.5f, 1f),   new Vector3(0.5f, 1f, 0.5f)
    };
    private readonly int[] baseTriangles =
    {
        5,1,2, 5,2,3, 5,3,4, 5,4,1,
        0,2,1, 0,3,2, 0,4,3, 0,1,4
    };

    private Mesh mesh;
    private Vector3[] facetVerts = new Vector3[24];

    void OnEnable()
    {
        mesh = new Mesh { name = "Jewel", hideFlags = HideFlags.HideAndDontSave };
        Animate(0f);
        int[] tris = new int[24];
        for (int i = 0; i < 24; i++) tris[i] = i;   // 면마다 정점을 따로 써서 각지게 반짝임
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        if (material != null) GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    void OnDisable()
    {
        if (mesh != null)
        {
            if (Application.isPlaying) Destroy(mesh); else DestroyImmediate(mesh);
        }
    }

    void Update()
    {
        Animate(Application.isPlaying ? Time.time : 0f);
    }

    void Animate(float t)
    {
        float angle = spinSpeed * t;
        float bob = Mathf.Sin(t * bobSpeed) * bobHeight;

        // v' = 이동 · 회전 · 크기 · (중심 맞추기) · v
        float[,] M = Mul(Translation(0f, bob, 0f),
                     Mul(RotationY(angle),
                     Mul(Scale(size),
                         Translation(-0.5f, -0.5f, -0.5f))));

        Vector3[] moved = new Vector3[6];
        for (int i = 0; i < 6; i++) moved[i] = Apply(M, baseVertices[i]);
        for (int k = 0; k < 24; k++) facetVerts[k] = moved[baseTriangles[k]];

        mesh.vertices = facetVerts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    float[,] Translation(float x, float y, float z)
    {
        return new float[,] { {1,0,0,x}, {0,1,0,y}, {0,0,1,z}, {0,0,0,1} };
    }

    float[,] Scale(float s)
    {
        return new float[,] { {s,0,0,0}, {0,s,0,0}, {0,0,s,0}, {0,0,0,1} };
    }

    float[,] RotationY(float deg)
    {
        float r = deg * Mathf.Deg2Rad;
        float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new float[,] { {c,0,s,0}, {0,1,0,0}, {-s,0,c,0}, {0,0,0,1} };
    }

    float[,] Mul(float[,] A, float[,] B)
    {
        float[,] R = new float[4, 4];
        for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
            {
                float sum = 0f;
                for (int k = 0; k < 4; k++) sum += A[r, k] * B[k, c];
                R[r, c] = sum;
            }
        return R;
    }

    Vector3 Apply(float[,] M, Vector3 p)
    {
        return new Vector3(
            M[0,0]*p.x + M[0,1]*p.y + M[0,2]*p.z + M[0,3],
            M[1,0]*p.x + M[1,1]*p.y + M[1,2]*p.z + M[1,3],
            M[2,0]*p.x + M[2,1]*p.y + M[2,2]*p.z + M[2,3]);
    }
}