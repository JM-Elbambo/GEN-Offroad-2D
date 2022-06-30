using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Bonus : MonoBehaviour
{
    [SerializeField] int[] values;
    [SerializeField] Sprite[] sprites;

    SpriteRenderer spriteRenderer;
    int value;
    bool isCollected;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RandomizeValue()
    {
        float rand = Random.value;
        int index;
        if (rand <= 0.6f) index = 0;
        else if (rand <= 0.9f) index = 1;
        else index = 2;
        SetValueIndex(index);
    }

    void SetValueIndex(int index)
    {
        value = values[index];
        spriteRenderer.sprite = sprites[index];
    }

    public int Collect()
    {
        if (isCollected)
        {
            return 0;
        }
        else
        {
            isCollected = true;
            return value;
        }
    }
}
