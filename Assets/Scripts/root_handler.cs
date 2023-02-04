using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class root_handler : MonoBehaviour
{
    Texture2D texture;
    Sprite sprite;
    Vector2 root_start, root_end;
    LineRenderer drag;

    enum Type
    {
        Sky,
        Dirt,
        Rock,
        Water,
        Nutrient,
        Root,
        RootWater,
        RootNutrient
    }

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
        drag = GameObject.Find("Drag_indicator").GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3 mouse_world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetButtonDown("Fire1"))
        {
            root_start = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            drag.SetPosition(0, new Vector3(mouse_world.x, mouse_world.y, -2));
        }
        if (Input.GetButton("Fire1"))
        {
            drag.SetPosition(1, new Vector3(mouse_world.x, mouse_world.y, -2));
            root_end = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        }
        if (Input.GetButtonUp("Fire1"))
        {

            draw_root
            (
                Mathf.RoundToInt(root_start.x),
                Mathf.RoundToInt(root_end.x),
                Mathf.RoundToInt(root_start.y),
                Mathf.RoundToInt(root_end.y),
                Color.cyan
            );

            drag.SetPosition(0, Vector3.zero);
            drag.SetPosition(1, Vector3.zero);
        }
    }
    void draw_root(int x_start, int x_end, int y_start, int y_end, Color col)
    {
        int loopnum = 0;

        Vector2 dir = new Vector2(0, 0);
        Vector2 gradient = new Vector2(0, 0);

        Debug.Log(new Vector4(x_start, x_end, y_start, y_end));

        int x = x_start;
        int y = y_start;
        //main draw loop
        while (x != x_end && y != y_end)
        {
            //pick length
            int length = Random.Range(5, 10);

            //set direction and gradient
            if (y_end > y)
            {
                dir.y = Random.Range(0, 1);
                gradient.y = Random.Range(1, 5);
            }
            else if (y_end < y)
            {
                dir.y = Random.Range(-1, 0);
                gradient.y = Random.Range(1, 5);
            }
            else
                y = 0;

            if (x_end > x)
            {
                Debug.Log(x);
                Debug.Log(x_end);
                Debug.Log("X is larger");
                dir.x = Random.Range(0, 1);
                gradient.x = Random.Range(1, 5);
            }
            else if (x_end < x)
            {
                dir.x = Random.Range(-1, 0);
                gradient.x = Random.Range(1, 5);
            }
            else
                x = 0;

            int grad_dir = Random.Range(0, 1);
            if (grad_dir == 1)
                gradient.y = 1;
            else
                gradient.x = 1;


            //run through length
            while (length != 0)
            {
                for (int gx = 0; gx <= gradient.x; gx++)
                {
                    if (dir.x != 0)
                        Debug.Log(dir.x);

                    x += Mathf.RoundToInt(dir.x);
                    texture.SetPixel(x, y, col);
                }

                for (int gy = 0; gy <= gradient.y; gy++)
                {
                    y += Mathf.RoundToInt(dir.y);
                    texture.SetPixel(x, y, col);
                }
                length--;
            }
            loopnum += 1;

            if (loopnum > 10000)
                break;
        }
        texture.Apply();
    }
}


