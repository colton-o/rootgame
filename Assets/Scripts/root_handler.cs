using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class root_handler : MonoBehaviour
{
    const int PIXEL_THRESHOLD = 25;
    const int Dir_distance = 25;
    const int grad_min = 0;
    const int grad_max = 5;
    const int min_length = 3;
    const int max_length = 7;
    Texture2D texture;
    Sprite sprite;
    Vector2 root_start, root_end;
    LineRenderer drag;

    bool can_root;


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
        if (can_root)
        {
            drag.startColor = Color.green;
            drag.endColor = Color.blue;
        }
        else
        {
            drag.startColor = Color.red;
            drag.endColor = Color.yellow;
        }

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

            if (root_end.y >= root_start.y)
                can_root = false;
            else
                can_root = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (can_root)
            {
                StartCoroutine(draw_root(Mathf.RoundToInt(root_start.x),
                                Mathf.RoundToInt(root_end.x),
                                Mathf.RoundToInt(root_start.y),
                                Mathf.RoundToInt(root_end.y),
                                Color.cyan));
            }



            drag.SetPosition(0, Vector3.zero);
            drag.SetPosition(1, Vector3.zero);
        }
    }
    IEnumerator draw_root(int x_start, int x_end, int y_start, int y_end, Color col)
    {

        Vector2 dir = new Vector2(0, 0);
        Vector2 gradient = new Vector2(0, 0);

        Debug.Log(new Vector4(x_start, x_end, y_start, y_end));

        int x = x_start;
        int y = y_start;
        //main draw loop
        while (Mathf.Abs(x - x_end) > PIXEL_THRESHOLD || Mathf.Abs(y - y_end) > PIXEL_THRESHOLD)
        {
            int loop_save = 99999999;

            //pick length
            int length = Random.Range(min_length, max_length);

            //set direction and gradient
            if (y_end > y)
            {
                dir.y = Random.Range(0, 2);
                gradient.y = Random.Range(grad_min, grad_max);
            }
            else if (y_end < y)
            {
                dir.y = Random.Range(-1, 1);
                gradient.y = Random.Range(grad_min, grad_max);
            }
            else
            {
                dir.y = Random.Range(-1, 2);
                gradient.y = Random.Range(grad_min, grad_max);
            }


            if (x_end > x)
            {
                dir.x = Random.Range(0, 2);
                gradient.x = Random.Range(grad_min, grad_max);
            }
            else if (x_end < x)
            {
                dir.x = Random.Range(-1, 1);
                gradient.x = Random.Range(grad_min, grad_max);
            }
            else
            {
                dir.x = Random.Range(-1, 2);
                gradient.x = Random.Range(grad_min, grad_max);
            }


            int grad_dir = Random.Range(0, 2);
            Debug.Log("GRAD =" + grad_dir);

            if (grad_dir == 1 || Mathf.Abs(y - y_end) < Dir_distance)
                gradient.y = 1;
            if (grad_dir == 0 || Mathf.Abs(x - x_end) < Dir_distance)
                gradient.x = 1;


            //run through length
            while (length != 0)
            {
                if (Mathf.Abs(x - x_end) < Dir_distance && Mathf.Abs(y - y_end) < Dir_distance)
                    break;
                //per gradient render
                for (int gx = 0; gx <= gradient.x; gx++)
                {
                    x += (int)dir.x;
                    Debug.Log(dir);
                    pixel_set(x, y, col);
                    texture.Apply();
                    yield return new WaitForSeconds(0.0000001f);
                }
                for (int gy = 0; gy <= gradient.y; gy++)
                {

                    y += (int)dir.y;
                    pixel_set(x, y, col);
                    texture.Apply();
                    yield return new WaitForSeconds(0.0000001f);
                }
                length--;
            }
            loop_save--;
            if (loop_save <= 0)
            {
                Debug.Log("omg you have infinate loop");
            }
        }

        Debug.Log("BREAK \n" + "x = " + x + "x end =" + x_end + "y = " + y + "y end = " + y_end);
    }

    void pixel_set(int x, int y, Color col)
    {
        texture.SetPixel(x, y, col);
        texture.SetPixel(x + 1, y, col);
        texture.SetPixel(x - 1, y, col);
        texture.SetPixel(x, y + 1, col);
        texture.SetPixel(x, y - 1, col);
    }

}




