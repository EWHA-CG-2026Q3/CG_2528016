using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_LoopMesh : MonoBehaviour
{
    [SerializeField, Range(3, 32)] private int sides = 8;     // 몇 각형인지
    [SerializeField] private float bottomRadius = 0.5f;
    [SerializeField] private float topRadius = 0.5f;          // 0이면 원뿔
    [SerializeField] private float height = 1f;
    [SerializeField] private Material material;

    private Mesh mesh;
    private string builtKey;

    void OnEnable() { Build(); }
    void OnDisable() { Cleanup(); }
    void Update() { if (builtKey != MakeKey()) Build(); }

    string MakeKey() { return sides + "_" + bottomRadius + "_" + topRadius + "_" + height; }

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
        int n = sides;
        bool hasTop = topRadius > 0.0001f;

        // 정점 배치: [아랫고리 n][윗고리 n][아랫뚜껑 중심 1 + 고리 n][윗뚜껑 중심 1 + 고리 n]
        int bottomRing = 0, topRing = n;
        int bottomCenter = 2 * n, bottomCapRing = 2 * n + 1;
        int topCenter = 3 * n + 1, topCapRing = 3 * n + 2;
        Vector3[] v = new Vector3[4 * n + 2];

        v[bottomCenter] = new Vector3(0f, 0f, 0f);
        v[topCenter] = new Vector3(0f, height, 0f);

        // 반복문으로 원 위의 점 n개를 만듦: 각도 = 360° / n * i
        for (int i = 0; i < n; i++)
        {
            float angle = 2f * Mathf.PI * i / n;
            float c = Mathf.Cos(angle), s = Mathf.Sin(angle);
            v[bottomRing + i] = new Vector3(bottomRadius * c, 0f, bottomRadius * s);
            v[topRing + i] = new Vector3(topRadius * c, height, topRadius * s);
            v[bottomCapRing + i] = v[bottomRing + i];
            v[topCapRing + i] = v[topRing + i];
        }

        List<int> t = new List<int>();
        for (int i = 0; i < n; i++)
        {
            int j = (i + 1) % n;
            // 옆면: 사각형 하나 = 삼각형 2개
            t.Add(bottomRing + i); t.Add(topRing + j); t.Add(topRing + i);
            t.Add(bottomRing + i); t.Add(bottomRing + j); t.Add(topRing + j);
            // 아랫뚜껑
            t.Add(bottomCenter); t.Add(bottomCapRing + j); t.Add(bottomCapRing + i);
            // 윗뚜껑 (원뿔이면 없음)
            if (hasTop) { t.Add(topCenter); t.Add(topCapRing + i); t.Add(topCapRing + j); }
        }

        mesh = new Mesh { name = "LoopMesh", hideFlags = HideFlags.HideAndDontSave };
        mesh.vertices = v;
        mesh.triangles = t.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        if (material != null) GetComponent<MeshRenderer>().sharedMaterial = material;
        builtKey = MakeKey();
    }
}