using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class S07_Clipping : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;

    // 왼쪽과 위쪽 경계를 동시에 넘어가도록 설계한 삼각형
    [SerializeField] private Vector2 p0 = new Vector2(-40, 150);
    [SerializeField] private Vector2 p1 = new Vector2(120, 300);
    [SerializeField] private Vector2 p2 = new Vector2(250, 60);

    [SerializeField] private Color fillColor = new Color(1f, 0.6f, 0.2f, 1f);
    [SerializeField] private Color backgroundColor = Color.black;

    private Texture2D canvasTexture;
    private RawImage targetImage;

    void Start()
    {
        targetImage = GetComponent<RawImage>();
        canvasTexture = new Texture2D(canvasWidth, canvasHeight);
        canvasTexture.filterMode = FilterMode.Point;

        FillBackground(backgroundColor);

        List<Vector2> polygon = new List<Vector2> { p0, p1, p2 };
        List<Vector2> clipped = ClipPolygon(polygon);

        DrawFilledPolygon(clipped, fillColor);

        canvasTexture.Apply();
        targetImage.texture = canvasTexture;
    }

    private void FillBackground(Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
            for (int y = 0; y < canvasHeight; y++)
                canvasTexture.SetPixel(x, y, color);
    }

    private List<Vector2> ClipPolygon(List<Vector2> polygon)
    {
        List<Vector2> result = polygon;
        result = ClipLeft(result);
        result = ClipRight(result);
        result = ClipBottom(result);
        result = ClipTop(result);
        return result;
    }

    // 예시 부분 (x >= 0)
    private List<Vector2> ClipLeft(List<Vector2> polygon)
    {
        List<Vector2> output = new List<Vector2>();
        int n = polygon.Count;
        for (int i = 0; i < n; i++)
        {
            Vector2 current = polygon[i];
            Vector2 previous = polygon[(i - 1 + n) % n];

            bool currentInside = current.x >= 0f;
            bool previousInside = previous.x >= 0f;

            if (currentInside)
            {
                if (!previousInside) output.Add(IntersectLeft(previous, current));
                output.Add(current);
            }
            else if (previousInside)
            {
                output.Add(IntersectLeft(previous, current));
            }
        }
        return output;
    }

    private Vector2 IntersectLeft(Vector2 a, Vector2 b)
    {
        float t = (0f - a.x) / (b.x - a.x);
        float y = a.y + t * (b.y - a.y);
        return new Vector2(0f, y);
    }

    // TODO 1: ClipLeft와 거울 구조 (x <= canvasWidth)
    private List<Vector2> ClipRight(List<Vector2> polygon)
    {
        List<Vector2> output = new List<Vector2>();
        int n = polygon.Count;
        for (int i = 0; i < n; i++)
        {
            Vector2 current = polygon[i];
            Vector2 previous = polygon[(i - 1 + n) % n];

            bool currentInside = current.x <= canvasWidth;
            bool previousInside = previous.x <= canvasWidth;

            if (currentInside)
            {
                if (!previousInside) output.Add(IntersectRight(previous, current));
                output.Add(current);
            }
            else if (previousInside)
            {
                output.Add(IntersectRight(previous, current));
            }
        }
        return output;
    }

    private Vector2 IntersectRight(Vector2 a, Vector2 b)
    {
        float t = (canvasWidth - a.x) / (b.x - a.x);
        float y = a.y + t * (b.y - a.y);
        return new Vector2(canvasWidth, y);
    }

    // TODO 2: y >= 0
    private List<Vector2> ClipBottom(List<Vector2> polygon)
    {
        List<Vector2> output = new List<Vector2>();
        int n = polygon.Count;
        for (int i = 0; i < n; i++)
        {
            Vector2 current = polygon[i];
            Vector2 previous = polygon[(i - 1 + n) % n];

            bool currentInside = current.y >= 0f;
            bool previousInside = previous.y >= 0f;

            if (currentInside)
            {
                if (!previousInside) output.Add(IntersectBottom(previous, current));
                output.Add(current);
            }
            else if (previousInside)
            {
                output.Add(IntersectBottom(previous, current));
            }
        }
        return output;
    }

    private Vector2 IntersectBottom(Vector2 a, Vector2 b)
    {
        float t = (0f - a.y) / (b.y - a.y);
        float x = a.x + t * (b.x - a.x);
        return new Vector2(x, 0f);
    }

    // TODO 3: y <= canvasHeight
    private List<Vector2> ClipTop(List<Vector2> polygon)
    {
        List<Vector2> output = new List<Vector2>();
        int n = polygon.Count;
        for (int i = 0; i < n; i++)
        {
            Vector2 current = polygon[i];
            Vector2 previous = polygon[(i - 1 + n) % n];

            bool currentInside = current.y <= canvasHeight;
            bool previousInside = previous.y <= canvasHeight;

            if (currentInside)
            {
                if (!previousInside) output.Add(IntersectTop(previous, current));
                output.Add(current);
            }
            else if (previousInside)
            {
                output.Add(IntersectTop(previous, current));
            }
        }
        return output;
    }

    private Vector2 IntersectTop(Vector2 a, Vector2 b)
    {
        float t = (canvasHeight - a.y) / (b.y - a.y);
        float x = a.x + t * (b.x - a.x);
        return new Vector2(x, canvasHeight);
    }

    private void DrawFilledPolygon(List<Vector2> polygon, Color color)
    {
        if (polygon.Count < 3) return;

        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                if (IsInsideConvexPolygon(p, polygon))
                    canvasTexture.SetPixel(x, y, color);
            }
        }
    }

    // 다각형이 convex하다는 전제로, 모든 변에 대해 같은 부호이면 내부
    private bool IsInsideConvexPolygon(Vector2 p, List<Vector2> polygon)
    {
        int n = polygon.Count;
        bool? positive = null;
        for (int i = 0; i < n; i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[(i + 1) % n];
            float cross = (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
            if (Mathf.Approximately(cross, 0f)) continue;

            bool isPositive = cross > 0f;
            if (positive == null) positive = isPositive;
            else if (positive != isPositive) return false;
        }
        return true;
    }
}