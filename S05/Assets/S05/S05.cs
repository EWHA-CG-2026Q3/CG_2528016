using UnityEngine;
using UnityEngine.UI;

public class S05_MyMeshRenderer : MonoBehaviour
{
    [SerializeField] private int canvasWidth = 256;
    [SerializeField] private int canvasHeight = 256;
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 1f);

    [Header("무늬 실습 ")]
    [SerializeField] private int patternSize = 16;
    [SerializeField] private Color colorA = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color colorB = new Color(0.3f, 0.5f, 0.8f, 1f);

    private Texture2D canvasTexture;
    private RawImage targetImage;

    void Start()
    {
        targetImage = GetComponent<RawImage>();

        canvasTexture = new Texture2D(canvasWidth, canvasHeight);
        canvasTexture.filterMode = FilterMode.Point;

        // 줄무늬
        // FillVerticalStripes(patternSize, colorA, colorB);

        // 체스판
        FillCheckerboard(patternSize, colorA, colorB);

        canvasTexture.Apply();
        targetImage.texture = canvasTexture;
    }

    // 줄무늬: x좌표를 width로 나눈 몫이 짝수/홀수인지로 색 결정
    private void FillVerticalStripes(int width, Color colorA, Color colorB)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            bool isColorA = (x / width) % 2 == 0;
            Color stripeColor = isColorA ? colorA : colorB;

            for (int y = 0; y < canvasHeight; y++)
                canvasTexture.SetPixel(x, y, stripeColor);
        }
    }

    // 체스판: x, y 몫의 합이 짝수/홀수인지로 색 결정
    private void FillCheckerboard(int size, Color colorA, Color colorB)
    {
        for (int x = 0; x < canvasWidth; x++)
        {
            for (int y = 0; y < canvasHeight; y++)
            {
                int checkSum = (x / size) + (y / size);
                Color squareColor = (checkSum % 2 == 0) ? colorA : colorB;
                canvasTexture.SetPixel(x, y, squareColor);
            }
        }
    }
}