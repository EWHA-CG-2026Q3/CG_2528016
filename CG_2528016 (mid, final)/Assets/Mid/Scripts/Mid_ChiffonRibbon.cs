using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_ChiffonRibbon : MonoBehaviour
{
    [SerializeField] private float radius = 0.25f;         // 기둥 반지름보다 조금 크게
    [SerializeField] private float height = 2.5f;
    [SerializeField] private float turns = 3f;             // 기둥을 감는 횟수
    [SerializeField] private float ribbonWidth = 0.15f;    // 리본 폭
    [SerializeField, Range(10, 200)] private int segments = 80;
    [SerializeField] private float flutter = 0.03f;        // 바깥으로 살랑이는 정도
    [SerializeField] private float flutterSpeed = 2f;
    [SerializeField] private Material material;

    private Mesh mesh;
    private Vector3[] verts;
    private int builtSegments = -1;

    void OnEnable() { Build(); }
    void OnDisable() { Cleanup(); }

    void Update()
    {
        if (builtSegments != segments) Build();
        UpdateVertices(Application.isPlaying ? Time.time : 0f);
    }

    void Cleanup()
    {
        if (mesh != null)
        {
            if (Application.isPlaying) Destroy(mesh); else DestroyImmediate(mesh);
        }
        builtSegments = -1;
    }

    void Build()
    {
        Cleanup();
        verts = new Vector3[2 * (segments + 1)];   // 위쪽 점과 아래쪽 점이 한 쌍
        List<int> t = new List<int>();
        for (int k = 0; k < segments; k++)
        {
            int top = 2 * k, bottom = 2 * k + 1, nextTop = 2 * k + 2, nextBottom = 2 * k + 3;
            t.Add(top); t.Add(nextTop); t.Add(bottom);
            t.Add(nextTop); t.Add(nextBottom); t.Add(bottom);
        }
        mesh = new Mesh { name = "ChiffonRibbon", hideFlags = HideFlags.HideAndDontSave };
        mesh.vertices = verts;
        mesh.triangles = t.ToArray();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        if (material != null) GetComponent<MeshRenderer>().sharedMaterial = material;
        builtSegments = segments;
    }

    void UpdateVertices(float time)
    {
        for (int k = 0; k <= segments; k++)
        {
            float t = (float)k / segments;
            float angle = turns * 2f * Mathf.PI * t;                                 // 나선: 돌면서 올라감
            float r = radius + flutter * Mathf.Sin(time * flutterSpeed + t * 12f);   // 살랑임
            Vector3 center = new Vector3(r * Mathf.Cos(angle), height * t, r * Mathf.Sin(angle));
            verts[2 * k] = center + new Vector3(0f, ribbonWidth * 0.5f, 0f);
            verts[2 * k + 1] = center - new Vector3(0f, ribbonWidth * 0.5f, 0f);
        }
        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}