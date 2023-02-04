using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class root_handler : MonoBehaviour
{
    Texture2D texture;
    Sprite sprite;
    void Start()
    {
        texture = new Texture2D(Screen.width, Screen.height);
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        GetComponent<SpriteRenderer>().sprite = sprite;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, Color.black);
            }
        }
        texture.Apply();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            draw_pix(500, 800, 500, 1000, Color.cyan);
        }
    }
    void draw_pix(int x_start, int x_end, int y_start, int y_end, Color col)
    {

        int x = x_start;
        int y = y_start;

        while (x != x_end && y != y_end)
        {
            texture.SetPixel(x, y, col);
            if (x_end > x)
            {
                x += 1;
            }
            else if (x_end < x)
            {
                x += -1;
            }


            if (y_end > y)
            {
                y += 1;
            }
            else if (y_end < y)
            {
                y += -1;
            }

        }
        texture.Apply();
    }
}


