using UnityEngine;
using UnityEngine.UI;

public class S07_DepthTest2 : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;

    // 삼각형 1: 평평함(z 전부 동일, 상대적으로 가까움)
    [SerializeField] private Vector3 vertexA1 = new Vector3(40, 40, 0.2f);
    [SerializeField] private Vector3 vertexB1 = new Vector3(220, 40, 0.2f);
    [SerializeField] private Vector3 vertexC1 = new Vector3(130, 220, 0.2f);
    [SerializeField] private Color color1 = new Color(1f, 0.3f, 0.3f, 1f);

    // 삼각형 2: 기울어짐(z가 꼭짓점마다 다름) - 삼각형 1과 겹치면서 부분마다 승부가 갈림
    [SerializeField] private Vector3 vertexA2 = new Vector3(60, 180, 0.9f);
    [SerializeField] private Vector3 vertexB2 = new Vector3(240, 180, 0.2f);
    [SerializeField] private Vector3 vertexC2 = new Vector3(150, 20, 0.5f);
    [SerializeField] private Color color2 = new Color(0.3f, 0.5f, 1f, 1f);

    // 삼각형 3: 추가된 삼각형, 픽셀 단위 비교가 필요한 상황을 만듦
    [SerializeField] private Vector3 vertexA3 = new Vector3(20, 100, 0.5f);
    [SerializeField] private Vector3 vertexB3 = new Vector3(150, 250, 0.1f);
    [SerializeField] private Vector3 vertexC3 = new Vector3(230, 90, 0.8f);
    [SerializeField] private Color color3 = new Color(0.3f, 1f, 0.4f, 1f);

    [SerializeField] private Color backgroundColor = Color.black;

    private Texture2D canvasTexture;
    private RawImage targetImage;
    private float[,] depthBuffer;

    void Start()
    {
        targetImage = GetComponent<RawImage>();
        canvasTexture = new Texture2D(canvasWidth, canvasHeight);
        canvasTexture.filterMode = FilterMode.Point;

        depthBuffer = new float[canvasWidth, canvasHeight];
        for (int x = 0; x < canvasWidth; x++)
            for (int y = 0; y < canvasHeight; y++)
                depthBuffer[x, y] = float.MaxValue;

        FillBackground(backgroundColor);

        // 일부러 순서를 뒤섞어서 그림 -> depth test가 그리는 순서와 무관하게 동작함
        DrawTriangle(vertexA2, vertexB2, vertexC2, color2);
        DrawTriangle(vertexA3, vertexB3, vertexC3, color3);
        DrawTriangle(vertexA1, vertexB1, vertexC1, color1);

        canvasTexture.Apply();
        targetImage.texture = canvasTexture;
    }

    private void FillBackground(Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
            for (int y = 0; y < canvasHeight; y++)
                canvasTexture.SetPixel(x, y, color);
    }

    private bool GetBarycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c, out float w1, out float w2, out float w3)
    {
        float denom = a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y);
        w1 = (p.x * (b.y - c.y) + b.x * (c.y - p.y) + c.x * (p.y - b.y)) / denom;
        w2 = (a.x * (p.y - c.y) + p.x * (c.y - a.y) + c.x * (a.y - p.y)) / denom;
        w3 = 1f - w1 - w2;
        return w1 >= 0f && w2 >= 0f && w3 >= 0f;
    }

    private void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                float w1, w2, w3;
                bool isInside = GetBarycentric(p, a, b, c, out w1, out w2, out w3);

                if (isInside)
                {
                    // TODO 1: barycentric 가중치로 z값 보간
                    float interpolatedZ = w1 * a.z + w2 * b.z + w3 * c.z;

                    // TODO 2: depth buffer보다 더 가까울 때만 색/깊이 갱신
                    if (interpolatedZ < depthBuffer[x, y])
                    {
                        canvasTexture.SetPixel(x, y, color);
                        depthBuffer[x, y] = interpolatedZ;
                    }
                }
            }
        }
    }
}