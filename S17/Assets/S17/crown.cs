using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Mid_Crown : MonoBehaviour
{
    [SerializeField, Range(3, 16)] private int spikes = 6;     // 뾰족한 끝 개수
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float bandHeight = 0.25f;
    [SerializeField] private float spikeHeight = 0.35f;
    [SerializeField] private Material material;

    private Mesh mesh;
    private string builtKey;

    void OnEnable() { Build(); }
    void OnDisable() { Cleanup(); }
    void Update() { if (builtKey != MakeKey()) Build(); }

    string MakeKey() { return spikes + "_" + radius + "_" + bandHeight + "_" + spikeHeight; }

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
        int n = spikes;
        // 정점: [아랫고리 n][윗고리 n][뾰족한 끝 n]
        Vector3[] v = new Vector3[3 * n];
        for (int i = 0; i < n; i++)
        {
            float a = 2f * Mathf.PI * i / n;               // 고리 위의 각도
            float mid = 2f * Mathf.PI * (i + 0.5f) / n;    // 두 점 사이 각도 (뾰족한 끝)
            v[i] = new Vector3(radius * Mathf.Cos(a), 0f, radius * Mathf.Sin(a));
            v[n + i] = new Vector3(radius * Mathf.Cos(a), bandHeight, radius * Mathf.Sin(a));
            v[2 * n + i] = new Vector3(radius * Mathf.Cos(mid), bandHeight + spikeHeight, radius * Mathf.Sin(mid));
        }

        List<int> t = new List<int>();
        for (int i = 0; i < n; i++)
        {
            int j = (i + 1) % n;
            t.Add(i); t.Add(n + j); t.Add(n + i);        // 띠: 사각형 = 삼각형 2개
            t.Add(i); t.Add(j); t.Add(n + j);
            t.Add(n + i); t.Add(n + j); t.Add(2 * n + i); // 뾰족한 끝: 삼각형 1개
        }

        mesh = new Mesh { name = "Crown", hideFlags = HideFlags.HideAndDontSave };
        mesh.vertices = v;
        mesh.triangles = t.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        if (material != null) GetComponent<MeshRenderer>().sharedMaterial = material;
        builtKey = MakeKey();
    }
}