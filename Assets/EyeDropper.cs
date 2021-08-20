using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class EyeDropper : MonoBehaviour {
    public CoordinateMap mapper;

    private SpriteRenderer spriteRenderer;
    private Sprite spriteToEyedrop;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteToEyedrop = mapper.GetComponent<SpriteRenderer>().sprite;
    }

    private void Update() {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            // NOTE: if your objects aren't at zPos = 0, you'll have to adjust for that.
            Vector2 mouseCoord = Mouse.current.position.ReadValue();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mouseCoord);

            Vector2 coords = mapper.TextureSpaceCoord(worldPos);
            //Vector2 coords = mapper.TextureSpaceUV(worldPos);

            Color pixel = spriteToEyedrop.texture.GetPixel((int)coords.x, (int)coords.y);
            //Color pixel = sprite.texture.GetPixelBilinear(coords.x, coords.y);

            spriteRenderer.color = pixel;
        }
    }
}
