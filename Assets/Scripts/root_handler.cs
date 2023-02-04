using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class root_handler : MonoBehaviour
{
   void Start()
    {
        Texture2D texture = new Texture2D(128, 128);
        Sprite sprite = Sprite.Create(texture,new Rect(0,0,128,128), Vector2.zero);
        GetComponent<SpriteRenderer>().sprite = sprite;
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Color color = ((x & y) != 0 ? Color.magenta : Color.black);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
    }
    
}


     