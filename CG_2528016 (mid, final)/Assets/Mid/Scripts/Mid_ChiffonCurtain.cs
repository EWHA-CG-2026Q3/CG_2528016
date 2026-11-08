using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_ChiffonCurtain : MonoBehaviour
{
    [SerializeField] private float width = 2f;
    [SerializeField] private float height = 2.5f;
    [SerializeField, Range(4, 60)] private int columns = 30;   // 가로 칸 수
    [SerializeField, Range(2, 40)] private int rows = 16;      // 세로 칸 수
    [SerializeField] private float foldCount = 6f;             // 주름 개수
    [SerializeField] private float foldDepth = 0.08f;          // 주름 깊이
    [SerializeField] private float windStrength = 0.05f;       // 바람에 흔들리는 정도
    [SerializeField] private float windSpeed = 1.2f;
    [SerializeField] private Material material;

    private Mesh mesh;
    private Vector3[] verts;
    private string builtKey;

    void OnEnable() { Build(); }
    void OnDisable() { Cleanup(); }

    void Update()
    {
        if (builtKey != columns + "_" + rows) Build();
        UpdateVertices(Application.isPlaying ? Time.time : 0f);
    }

    void Cleanup()
    {
        if (mesh != null)
        {
            if (Application.isPlaying) Destroy(mesh); else DestroyImmediate(mesh);
        }
        builtKey = null;
    }

    void Build()
    {
        Cleanup();
        int cols = columns, rws = rows;
        verts = new Vector3[(cols + 1) * (rws + 1)];
        Vector2[] uv = new Vector2[verts.Length];
        for (int j = 0; j <= rws; j++)
            for (int i = 0; i <= cols; i++)
                uv[j * (cols + 1) + i] = new Vector2((float)i / cols, 1f - (float)j / rws);

        // 칸마다 사각형 하나 = 삼각형 2개
        List<int> t = new List<int>();
        for (int j = 0; j < rws; j++)
            for (int i = 0; i < cols; i++)
            {
                int a = j * (cols + 1) + i;   // 왼쪽 위
                int b = a + 1;                // 오른쪽 위
                int c = a + (cols + 1);       // 왼쪽 아래
                int d = c + 1;                // 오른쪽 아래
                t.Add(a); t.Add(b); t.Add(c);
                t.Add(b); t.Add(d); t.Add(c);
            }

        mesh = new Mesh { name = "ChiffonCurtain", hideFlags = HideFlags.HideAndDontSave };
        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.triangles = t.ToArray();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        if (material != null) GetComponent<MeshRenderer>().sharedMaterial = material;
        builtKey = cols + "_" + rws;
    }

    void UpdateVertices(float time)
    {
        int cols = columns, rws = rows;
        for (int j = 0; j <= rws; j++)
        {
            float v = (float)j / rws;                   // 0 = 윗단, 1 = 아랫단
            for (int i = 0; i <= cols; i++)
            {
                float u = (float)i / cols;
                float x = (u - 0.5f) * width;
                float y = -v * height;
                float fold = foldDepth * Mathf.Sin(foldCount * 2f * Mathf.PI * u);        // 주름
                float wind = windStrength * v * Mathf.Sin(time * windSpeed + u * 4f);     // 아랫단일수록 크게
                verts[j * (cols + 1) + i] = new Vector3(x, y, fold + wind);
            }
        }
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}