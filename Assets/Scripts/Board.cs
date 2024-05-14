using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int _x;
    public int _y;

    public SpriteRenderer spriteRenderer;
    public Color valid;
    public Color invalid;

    public void SetHighlight(bool active, bool canDrop)
    {
        spriteRenderer.gameObject.SetActive(active);
        spriteRenderer.color = canDrop ? valid : invalid;
    }
}
