using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RootSim : MonoBehaviour
{

    [SerializeField] private RawImage image;
    [SerializeField] private Vector2Int mapSize;

    [SerializeField] private Color skyColor;
    [SerializeField] private Color dirtColor;
    [SerializeField] private Color rockColor;
    [SerializeField] private Color waterColor;
    [SerializeField] private Color nutrientColor;
    [SerializeField] private Color rootColor;
    [SerializeField] private Color rootWaterColor;
    [SerializeField] private Color rootNutrientColor;
    [SerializeField] private Color seedColor;
    [SerializeField] private Color growthColor;
    [SerializeField] private Color rotColor;

    private cell[,] currentMap;
    private cell[,] previousMap;
    private Texture2D texture2D;

    [SerializeField] private int water;
    [SerializeField] private int nutrient;

    [SerializeField] private LineRenderer drag;
    const int PIXEL_THRESHOLD = 5;
    Vector2Int root_start, root_end;
    const int grad_min = 1;
    const int grad_max = 5;

    Vector2Int seedPosition = new Vector2Int(64, 80);
    
    bool can_root, clicked_root;
    public GameObject gameOver;

    struct cell
    {
        public Type type;
        public int distanceFromSeed;
    }

    enum Type
    {
        Sky,
        Dirt,
        Rock,
        Water,
        Nutrient,
        Root,
        RootWater,
        RootNutrient,
        Seed,
        Growth,
        Rot
    }
    void Start()
    {
        gameOver.SetActive(false);
        currentMap = new cell[mapSize.x, mapSize.y];
        previousMap = new cell[mapSize.x, mapSize.y];
        texture2D = new Texture2D(mapSize.x, mapSize.y);
        texture2D.filterMode = FilterMode.Point;
        image.texture = texture2D;

        InitializeMap();
    }

    private void InitializeMap()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (x == 0 || x == mapSize.x - 1 || y == mapSize.y - 1)
                {
                    currentMap[x, y].type = Type.Sky;
                }
                else if (y == 0)
                {
                    currentMap[x, y].type = Type.Rock;
                }
                else
                {
                    if (y > mapSize.y * 0.75f)
                    {
                        currentMap[x, y].type = Type.Sky;
                    }
                    else
                    {
                        switch (Random.value)
                        {
                            case < 0.9f:
                                {
                                    if (y < mapSize.y * 0.1f)
                                    {
                                        if (Random.value < 0.5f)
                                        {
                                            currentMap[x, y].type = Type.Water;
                                        }
                                        else
                                        {
                                            currentMap[x, y].type = Type.Dirt;
                                        }
                                    }
                                    else
                                    {
                                        currentMap[x, y].type = Type.Dirt;
                                    }
                                    currentMap[x, y].distanceFromSeed = 10000;
                                    break;
                                }
                            case < 0.99f:
                                {
                                    if (y > mapSize.y * 0.5f)
                                    {
                                        if (Random.value > 0.25f)
                                        {
                                            currentMap[x, y].type = Type.Dirt;
                                        }
                                        else
                                        {
                                            currentMap[x, y].type = Type.Rock;
                                        }
                                    }
                                    else
                                    {
                                        currentMap[x, y].type = Type.Rock;
                                    }
                                    currentMap[x, y].distanceFromSeed = 10000;
                                    break;
                                }
                            case < 1f:
                                {
                                    currentMap[x, y].type = Type.Water;
                                    currentMap[x, y].distanceFromSeed = 10000;
                                    break;
                                }
                            case >= 1f:
                                {
                                    currentMap[x, y].type = Type.Nutrient;
                                    currentMap[x, y].distanceFromSeed = 10000;
                                    break;
                                }
                        }

                    }
                }

            }
        }

        
        for (int distance = 1; distance < 10; distance++)
        {
            int x = seedPosition.x + distance / 2;
            int y = seedPosition.y - (distance + distance % 2) / 2;

            currentMap[x, y].type = Type.Root;
            currentMap[x, y].distanceFromSeed = distance;
        }
        for (int distance = 1; distance < 10; distance++)
        {
            int x = seedPosition.x - distance / 2;
            int y = seedPosition.y - (distance + distance % 2) / 2;

            currentMap[x, y].type = Type.Root;
            currentMap[x, y].distanceFromSeed = distance;
        }

        currentMap[seedPosition.x, seedPosition.y].type = Type.Seed;
        currentMap[seedPosition.x, seedPosition.y].distanceFromSeed = 0;

    }

    private void Update()
    {
        if (currentMap[seedPosition.x - 1, seedPosition.y - 1].type != Type.Root && currentMap[seedPosition.x + 1, seedPosition.y - 1].type != Type.Root && currentMap[seedPosition.x, seedPosition.y - 1].type != Type.Root)
        {
            gameOver.SetActive(true);
        }
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
        Vector2Int mapPosition = ScreenToMapPosition(Input.mousePosition);
        if (Input.GetButtonDown("Fire1"))
        {
            root_start = mapPosition;
            drag.SetPosition(0, new Vector3(mouse_world.x, mouse_world.y, -2));
            if (currentMap[mapPosition.x, mapPosition.y].type == Type.Root)
            {
                clicked_root = true;
            }
        }
        if (Input.GetButton("Fire1"))
        {
            drag.SetPosition(1, new Vector3(mouse_world.x, mouse_world.y, -2));
            root_end = mapPosition;
            if (clicked_root)
            {
                if (root_end.y < root_start.y && Vector2.Distance(root_end, root_start) < 50)
                    can_root = true;
                else
                    can_root = false;
            }

        }
        if (Input.GetButtonUp("Fire1"))
        {

            if (can_root)
                StartCoroutine(draw_root(root_start.x,
                root_end.x,
                root_start.y,
                root_end.y));

            drag.SetPosition(0, Vector3.zero);
            drag.SetPosition(1, Vector3.zero);

            clicked_root = false;
            can_root = false;
        }

        if (Input.GetButton("Fire2"))
        {
            if (currentMap[mapPosition.x, mapPosition.y].type == Type.Root)
            {
                currentMap[mapPosition.x, mapPosition.y].type = Type.Dirt;
            }

        }
    }

    void FixedUpdate()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                previousMap[x, y].type = currentMap[x, y].type;
                previousMap[x, y].distanceFromSeed = currentMap[x, y].distanceFromSeed;
            }
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                SimulateCell(x, y);
                PaintPixel(x, y);
            }
        }
        texture2D.Apply();
    }

    private void SimulateCell(int x, int y)
    {
        if (x > 0 && x < mapSize.x - 1 && y > 0 && y < mapSize.y - 1)
        {
            switch (currentMap[x, y].type)
            {
                case Type.Sky:
                    {
                        if (Random.value < 0.0001f)
                        {
                            currentMap[x, y].type = Type.Water;
                        }
                        else if (previousMap[x, y + 1].type == Type.Water)
                        {
                            currentMap[x, y].type = Type.Water;
                        }
                        else
                        {
                            currentMap[x, y].type = previousMap[x, y].type;
                        }
                        break;
                    }
                case Type.Dirt:
                    {
                        if (Random.value < 0.00001f)
                        {
                            currentMap[x, y].type = Type.Rot;
                        }
                        else if (previousMap[x + 1, y].type == Type.Rot || previousMap[x - 1, y].type == Type.Rot ||
                                 previousMap[x, y + 1].type == Type.Rot || previousMap[x, y - 1].type == Type.Rot)
                        {
                            if (Random.value < 0.01f)
                            {
                                currentMap[x, y].type = Type.Rot;
                            }
                        }
                        else if (previousMap[x + 1, y].type == Type.Nutrient || previousMap[x - 1, y].type == Type.Nutrient ||
                                 previousMap[x, y + 1].type == Type.Nutrient || previousMap[x, y - 1].type == Type.Nutrient)
                        {
                            if (Random.value < 0.001f)
                            {
                                currentMap[x, y].type = Type.Nutrient;
                            }
                        }
                        else if (previousMap[x, y + 1].type == Type.Water)
                        {
                            currentMap[x, y].type = Type.Water;
                        }
                        else if (Random.value < 0.0001f)
                        {
                            currentMap[x, y].type = Type.Nutrient;
                        }

                        break;
                    }
                case Type.Rock:
                    {
                        currentMap[x, y].type = previousMap[x, y].type;
                        break;
                    }
                case Type.Water:
                    {
                        if (previousMap[x + 1, y].type == Type.Root || previousMap[x - 1, y].type == Type.Root ||
                            previousMap[x, y + 1].type == Type.Root || previousMap[x, y - 1].type == Type.Root)
                        {
                            currentMap[x, y].type = Type.Dirt;
                        }
                        else if (previousMap[x, y - 1].type == Type.Dirt)
                        {
                            currentMap[x, y].type = Type.Dirt;
                        }
                        else if (previousMap[x, y - 1].type == Type.Sky)
                        {
                            currentMap[x, y].type = Type.Sky;
                        }
                        else
                        {
                            currentMap[x, y].type = previousMap[x, y].type;
                        }

                        break;
                    }
                case Type.Nutrient:
                    {
                        if (previousMap[x + 1, y].type == Type.Root || previousMap[x - 1, y].type == Type.Root ||
                            previousMap[x, y + 1].type == Type.Root || previousMap[x, y - 1].type == Type.Root)
                        {
                            currentMap[x, y].type = Type.Dirt;
                        }
                        else if (previousMap[x, y + 1].type == Type.Water)
                        {
                            currentMap[x, y].type = Type.Water;
                        }
                        else
                        {
                            currentMap[x, y].type = previousMap[x, y].type;
                        }

                        break;
                    }
                case Type.Root:
                    {
                        if (previousMap[x + 1, y].type == Type.Rot || previousMap[x - 1, y].type == Type.Rot ||
                            previousMap[x, y + 1].type == Type.Rot || previousMap[x, y - 1].type == Type.Rot)
                        {
                            if (Random.value < 0.5f)
                            {
                                currentMap[x, y].type = Type.Rot;
                                currentMap[x, y].distanceFromSeed = 10000;
                            }
                        }
                        else if ((previousMap[x + 1, y].type == Type.RootNutrient &&
                                  previousMap[x + 1, y].distanceFromSeed > previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x - 1, y].type == Type.RootNutrient &&
                                  previousMap[x - 1, y].distanceFromSeed > previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x, y + 1].type == Type.RootNutrient &&
                                  previousMap[x, y + 1].distanceFromSeed > previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x, y - 1].type == Type.RootNutrient &&
                                  previousMap[x, y - 1].distanceFromSeed > previousMap[x, y].distanceFromSeed))
                        {
                            currentMap[x, y].type = Type.RootNutrient;
                        }
                        else if ((previousMap[x + 1, y].type == Type.RootWater &&
                                  previousMap[x + 1, y].distanceFromSeed > previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x - 1, y].type == Type.RootWater && previousMap[x - 1, y].distanceFromSeed >
                                     previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x, y + 1].type == Type.RootWater && previousMap[x, y + 1].distanceFromSeed >
                                     previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x, y - 1].type == Type.RootWater && previousMap[x, y - 1].distanceFromSeed >
                                     previousMap[x, y].distanceFromSeed))
                        {
                            currentMap[x, y].type = Type.RootWater;
                        }
                        else if (previousMap[x + 1, y].type == Type.Nutrient ||
                                 previousMap[x - 1, y].type == Type.Nutrient ||
                                 previousMap[x, y + 1].type == Type.Nutrient || previousMap[x, y - 1].type == Type.Nutrient)
                        {
                            currentMap[x, y].type = Type.RootNutrient;
                        }
                        else if (previousMap[x + 1, y].type == Type.Water || previousMap[x - 1, y].type == Type.Water ||
                                 previousMap[x, y + 1].type == Type.Water || previousMap[x, y - 1].type == Type.Water)
                        {
                            currentMap[x, y].type = Type.RootWater;
                        }
                        else if (!(previousMap[x + 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed ||
                                previousMap[x - 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed ||
                                previousMap[x, y + 1].distanceFromSeed < previousMap[x, y].distanceFromSeed ||
                                previousMap[x, y - 1].distanceFromSeed < previousMap[x, y].distanceFromSeed))
                        {
                            if (Random.value < 0.5f)
                            {
                                currentMap[x, y].type = Type.Dirt;
                                currentMap[x, y].distanceFromSeed = 10000;
                            }
                        }
                        else
                        {
                            currentMap[x, y].type = previousMap[x, y].type;
                        }

                        break;
                    }
                case Type.RootNutrient:
                    {
                        if (previousMap[x + 1, y].type == Type.Rot || previousMap[x - 1, y].type == Type.Rot ||
                            previousMap[x, y + 1].type == Type.Rot || previousMap[x, y - 1].type == Type.Rot)
                        {
                            currentMap[x, y].type = Type.Rot;
                            currentMap[x, y].distanceFromSeed = 10000;
                        }
                        else if (previousMap[x + 1, y].type == Type.Seed || previousMap[x - 1, y].type == Type.Seed ||
                                previousMap[x, y + 1].type == Type.Seed || previousMap[x, y - 1].type == Type.Seed)
                        {
                            currentMap[x, y].type = Type.Root;
                        }
                        else if ((previousMap[x + 1, y].type == Type.Root &&
                                  previousMap[x + 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x - 1, y].type == Type.Root && previousMap[x - 1, y].distanceFromSeed <
                                     previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x, y + 1].type == Type.Root && previousMap[x, y + 1].distanceFromSeed <
                                     previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x, y - 1].type == Type.Root && previousMap[x, y - 1].distanceFromSeed <
                                     previousMap[x, y].distanceFromSeed))
                        {
                            currentMap[x, y].type = Type.Root;
                        }
                        else if (!(previousMap[x + 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed ||
                                   previousMap[x - 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed ||
                                   previousMap[x, y + 1].distanceFromSeed < previousMap[x, y].distanceFromSeed ||
                                   previousMap[x, y - 1].distanceFromSeed < previousMap[x, y].distanceFromSeed))
                        {
                            if (Random.value < 0.5f)
                            {
                                currentMap[x, y].type = Type.Dirt;
                                currentMap[x, y].distanceFromSeed = 10000;
                            }
                        }
                        else
                        {
                            currentMap[x, y].type = previousMap[x, y].type;
                        }

                        break;
                    }
                case Type.RootWater:
                    {
                        if (previousMap[x + 1, y].type == Type.Rot || previousMap[x - 1, y].type == Type.Rot ||
                            previousMap[x, y + 1].type == Type.Rot || previousMap[x, y - 1].type == Type.Rot)
                        {
                            currentMap[x, y].type = Type.Rot;
                            currentMap[x, y].distanceFromSeed = 10000;
                        }
                        else if (previousMap[x + 1, y].type == Type.Seed || previousMap[x - 1, y].type == Type.Seed ||
                                previousMap[x, y + 1].type == Type.Seed || previousMap[x, y - 1].type == Type.Seed)
                        {
                            currentMap[x, y].type = Type.Root;
                        }
                        else if ((previousMap[x + 1, y].type == Type.Root &&
                                  previousMap[x + 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x - 1, y].type == Type.Root && previousMap[x - 1, y].distanceFromSeed <
                                     previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x, y + 1].type == Type.Root && previousMap[x, y + 1].distanceFromSeed <
                                     previousMap[x, y].distanceFromSeed) ||
                                 (previousMap[x, y - 1].type == Type.Root && previousMap[x, y - 1].distanceFromSeed <
                                     previousMap[x, y].distanceFromSeed))
                        {
                            currentMap[x, y].type = Type.Root;
                        }
                        else if (!(previousMap[x + 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed ||
                                   previousMap[x - 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed ||
                                   previousMap[x, y + 1].distanceFromSeed < previousMap[x, y].distanceFromSeed ||
                                   previousMap[x, y - 1].distanceFromSeed < previousMap[x, y].distanceFromSeed))
                        {
                            if (Random.value < 0.5f)
                            {
                                currentMap[x, y].type = Type.Dirt;
                                currentMap[x, y].distanceFromSeed = 10000;
                            }
                        }
                        else
                        {
                            currentMap[x, y].type = previousMap[x, y].type;
                            currentMap[x, y].distanceFromSeed = previousMap[x, y].distanceFromSeed;
                        }

                        break;
                    }
                case Type.Seed:
                    {
                        if (previousMap[x + 1, y].type == Type.RootWater || previousMap[x - 1, y].type == Type.RootWater ||
                            previousMap[x, y + 1].type == Type.RootWater || previousMap[x, y - 1].type == Type.RootWater)
                        {
                            water++;
                        }

                        if (previousMap[x + 1, y].type == Type.RootNutrient ||
                            previousMap[x - 1, y].type == Type.RootNutrient ||
                            previousMap[x, y + 1].type == Type.RootNutrient ||
                            previousMap[x, y - 1].type == Type.RootNutrient)
                        {
                            nutrient++;
                        }

                        break;
                    }
                case Type.Rot:
                    {
                        if (Random.value < 0.1f)
                        {
                            currentMap[x, y].type = Type.Dirt;
                        }

                        break;
                    }
            }

            if ((previousMap[x, y - 1].type == Type.Growth || previousMap[x, y - 1].type == Type.Seed)
                && previousMap[x, y].type != Type.Growth && water > 0 && nutrient > 0)
            {
                currentMap[x, y].type = Type.Growth;
                water--;
                nutrient--;
            }
        }

    }

    private void PaintPixel(int x, int y)
    {
        switch (currentMap[x, y].type)
        {
            case Type.Sky:
                {
                    texture2D.SetPixel(x, y, skyColor);
                    break;
                }
            case Type.Dirt:
                {
                    texture2D.SetPixel(x, y, dirtColor * ((float)y / (float)mapSize.y + 0.75f));
                    break;
                }
            case Type.Rock:
                {
                    texture2D.SetPixel(x, y, rockColor);
                    break;
                }
            case Type.Water:
                {
                    texture2D.SetPixel(x, y, waterColor);
                    break;
                }
            case Type.Nutrient:
                {
                    texture2D.SetPixel(x, y, nutrientColor);
                    break;
                }
            case Type.Root:
                {
                    texture2D.SetPixel(x, y, rootColor * (1f - currentMap[x,y].distanceFromSeed / 200f));
                    break;
                }
            case Type.RootNutrient:
                {
                    texture2D.SetPixel(x, y, rootNutrientColor);
                    break;
                }
            case Type.RootWater:
                {
                    texture2D.SetPixel(x, y, rootWaterColor);
                    break;
                }
            case Type.Seed:
                {
                    texture2D.SetPixel(x, y, seedColor);
                    break;
                }
            case Type.Growth:
                {
                    texture2D.SetPixel(x, y, growthColor);
                    break;
                }
            case Type.Rot:
                {
                    texture2D.SetPixel(x, y, rotColor);
                    break;
                }
        }
    }

    private Vector2Int ScreenToMapPosition(Vector2 screenPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(screenPosition.x / Screen.width * mapSize.x), Mathf.RoundToInt(screenPosition.y / Screen.height * mapSize.y));
    }

    IEnumerator draw_root(int x_start, int x_end, int y_start, int y_end)
    {
        int loopnum = 0;

        Vector2 dir = new Vector2(0, 0);
        Vector2 gradient = new Vector2(0, 0);

        int x = x_start;
        int y = y_start;
        //main draw loop
        while (currentMap[x_end, y_end].type != Type.Root)
        {

            //pick length
            int length = Random.Range(1, 3);

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
                dir.x = Random.Range(-1, 0);
                gradient.x = Random.Range(grad_min, grad_max);
            }
            else
            {
                dir.x = Random.Range(-1, 2);
                gradient.x = Random.Range(grad_min, grad_max);
            }


            int grad_dir = Random.Range(0, 2);
            if (grad_dir == 1)
                gradient.y = 1;
            else
                gradient.x = 1;


            int distanceToSeed = currentMap[x, y].distanceFromSeed;

            //run through length
            while (length != 0)
            {
                //if (Mathf.Abs(x - x_end) < Dir_distance || Mathf.Abs(y - y_end) < Dir_distance)
                // break;
                //per gradient render
                for (int gx = 0; gx <= gradient.x; gx++)
                {
                    x += (int)dir.x;
                    if (x >= 0 && x < mapSize.x)
                    {
                        distanceToSeed++;
                        if (currentMap[x, y].type == Type.Root || currentMap[x, y].type == Type.RootNutrient || currentMap[x, y].type == Type.RootWater)
                        {
                            if (currentMap[x, y].distanceFromSeed > distanceToSeed)
                            {
                                currentMap[x, y].distanceFromSeed = distanceToSeed;
                            }
                            else
                            {
                                distanceToSeed = currentMap[x, y].distanceFromSeed;
                            }
                        }
                        else
                        {
                            currentMap[x, y].type = Type.Root;
                            currentMap[x, y].distanceFromSeed = distanceToSeed;
                        }
                    }
                    yield return new WaitForFixedUpdate();
                }
                distanceToSeed++;
                for (int gy = 0; gy <= gradient.y; gy++)
                {
                    y += (int)dir.y;
                    if (y >= 0 && y < mapSize.y)
                    {
                        distanceToSeed++;
                        if (currentMap[x, y].type == Type.Root || currentMap[x, y].type == Type.RootNutrient || currentMap[x, y].type == Type.RootWater)
                        {    
                            if (currentMap[x, y].distanceFromSeed > distanceToSeed)
                            {
                                currentMap[x, y].distanceFromSeed = distanceToSeed;
                            }
                            else
                            {
                                distanceToSeed = currentMap[x, y].distanceFromSeed;
                            }
                        }
                        else
                        {
                            currentMap[x, y].type = Type.Root;
                            currentMap[x, y].distanceFromSeed = distanceToSeed;
                        }
                    }
                    yield return new WaitForFixedUpdate();
                }
                length--;
            }
            loopnum += 1;

            if (loopnum > 10000)
            {
                Debug.Log("Oof looks like we got some infinate loop action happening :(");
                break;
            }
        }
    }
}
