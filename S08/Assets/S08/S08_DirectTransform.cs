using UnityEngine;

public enum DemoMode
{
    TranslateThenRotate,
    RotateThenTranslate,
    TranslateThenScale,
    ScaleThenTranslate
}

public class S08_DirectTransform : MonoBehaviour
{
    [Header("비교할 두 순서")]
    [SerializeField] private DemoMode demoModeA = DemoMode.TranslateThenScale;
    [SerializeField] private DemoMode demoModeB = DemoMode.ScaleThenTranslate;

    [Header("적용할 변환 값")]
    [SerializeField] private Vector3 translation = new Vector3(2f, 0f, 0f);
    [SerializeField] private Vector3 scale = new Vector3(2f, 1f, 1f);
    [SerializeField] private float rotationAngle = 90f;

    [Header("렌더링 대상")]
    [SerializeField] private MeshFilter meshFilterA;
    [SerializeField] private MeshFilter meshFilterB;

    // 다이아몬드 기본 정점 (0=아래 꼭짓점, 1~4=중간 사각형, 5=위 꼭짓점)
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
        5,1,2, 5,2,3, 5,3,4, 5,4,1, // 위쪽 4면
        0,2,1, 0,3,2, 0,4,3, 0,1,4  // 아래쪽 4면
    };

    void Start()
    {
        ApplyDemo(meshFilterA, demoModeA);
        ApplyDemo(meshFilterB, demoModeB);
    }

    private void ApplyDemo(MeshFilter targetFilter, DemoMode mode)
    {
        Vector3[] result = baseVertices;

        switch (mode)
        {
            case DemoMode.TranslateThenRotate:
                result = ApplyTranslation(result, translation);
                result = ApplyRotation(result, rotationAngle);
                break;
            case DemoMode.RotateThenTranslate:
                result = ApplyRotation(result, rotationAngle);
                result = ApplyTranslation(result, translation);
                break;

            // TODO 1: 이동 → 스케일
            case DemoMode.TranslateThenScale:
                result = ApplyTranslation(result, translation);
                result = ApplyScale(result, scale);
                break;

            // TODO 2: 스케일 → 이동
            case DemoMode.ScaleThenTranslate:
                result = ApplyScale(result, scale);
                result = ApplyTranslation(result, translation);
                break;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = result;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        targetFilter.mesh = mesh;
    }

    public Vector3[] ApplyTranslation(Vector3[] baseVertices, Vector3 t)
    {
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
            verts[i] = baseVertices[i] + t;
        return verts;
    }

    public Vector3[] ApplyScale(Vector3[] baseVertices, Vector3 s)
    {
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            verts[i] = new Vector3(
                baseVertices[i].x * s.x,
                baseVertices[i].y * s.y,
                baseVertices[i].z * s.z);
        }
        return verts;
    }

    public Vector3[] ApplyRotation(Vector3[] baseVertices, float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        Vector3[] verts = new Vector3[baseVertices.Length];
        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 v = baseVertices[i];
            float newX = v.x * c - v.y * s;
            float newY = v.x * s + v.y * c;
            verts[i] = new Vector3(newX, newY, v.z);
        }
        return verts;
    }
}