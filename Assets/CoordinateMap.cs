using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class CoordinateMap : MonoBehaviour
{
    private Sprite sprite;
    private SpriteRenderer spriteRenderer;
    public int radius;
    float _width, _height;
    Sprite newSprite;
    Texture2D newTexture;
    PolygonCollider2D polygonCollider2D;
    private void Start()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        sprite = GetComponent<SpriteRenderer>().sprite;
        spriteRenderer = GetComponent<SpriteRenderer>();
        _width = sprite.bounds.size.x * sprite.pixelsPerUnit;
        _height = sprite.bounds.size.y * sprite.pixelsPerUnit;
        //Copying original
        newTexture = new Texture2D(sprite.texture.width, sprite.texture.height);
        newTexture.SetPixels(sprite.texture.GetPixels());
        newTexture.Apply();
        newSprite = Sprite.Create(newTexture, new Rect(0f, 0f, newTexture.width, newTexture.height), new Vector2(.5f, .5f), sprite.pixelsPerUnit); ;
        newSprite.name = sprite.name + " Copy";
        spriteRenderer.sprite = newSprite;
    }

    public Vector2 TextureSpaceCoord(Vector3 worldPos)
    {
        float ppu = sprite.pixelsPerUnit;

        // Local position on the sprite in pixels.
        Vector2 localPos = transform.InverseTransformPoint(worldPos) * ppu;

        // When the sprite is part of an atlas, the rect defines its offset on the texture.
        // When the sprite is not part of an atlas, the rect is the same as the texture (x = 0, y = 0, width = tex.width, ...)
        var texSpacePivot = new Vector2(sprite.rect.x, sprite.rect.y) + sprite.pivot;
        Vector2 texSpaceCoord = texSpacePivot + localPos;

        return texSpaceCoord;
    }

    public Vector2 TextureSpaceUV(Vector3 worldPos)
    {
        Texture2D tex = newSprite.texture;
        Vector2 texSpaceCoord = TextureSpaceCoord(worldPos);

        // Pixels to UV(0-1) conversion.
        Vector2 uvs = texSpaceCoord;
        uvs.x /= tex.width;
        uvs.y /= tex.height;


        return uvs;
    }
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseCoord = Mouse.current.position.ReadValue();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mouseCoord);

            Vector2 coords = TextureSpaceCoord(worldPos);
            StartCoroutine(ChangePixels((int)coords.x, (int)coords.y, radius, new Color(0f, 0f, 0f, 0f)));
        }
    }
    IEnumerator ChangePixels(int centreX, int centreY, int diameter, Color color)
    {
        int radius = diameter / 2;
        int a;
        int b;

        for (int y = centreY - (diameter / 2); y < centreY + (diameter / 2); y++)
        {
            for (int x = centreX - (diameter / 2); x < centreX + (diameter / 2); x++)
            {
                if (x < 0 || y < 0 || x >= _width || y >= _height) continue;
                a = x - centreX;
                b = y - centreY;

                if ((a * a) + (b * b) <= (radius * radius))
                {
                    newSprite.texture.SetPixel(x, y, color);
                }
            }
        }
        newSprite.texture.Apply();
        yield return null;
        StartCoroutine(Trace());
    }
    float tolerance = 0f;
    uint gapLength = 0;
    float product = 0;


    IEnumerator Trace()
    {
        ContourTracer tracer = new ContourTracer();
        Texture2D targetTex = newSprite.texture;
        tracer.Trace(targetTex, new Vector2(.5f, .5f), newSprite.pixelsPerUnit, gapLength, product);

        List<Vector2> path = new List<Vector2>();
        List<Vector2> points = new List<Vector2>();

        if (polygonCollider2D == null) polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();

        polygonCollider2D.pathCount = tracer.pathCount;
        for (int i = 0; i < polygonCollider2D.pathCount; i++)
        {
            tracer.GetPath(i, ref path);
            LineUtility.Simplify(path, tolerance, points);
            if (points.Count < 3)
            {
                polygonCollider2D.pathCount--;
                i--;
            }
            else
            {
                polygonCollider2D.SetPath(i, points);
            }
            yield return null;
        }
    }
}
