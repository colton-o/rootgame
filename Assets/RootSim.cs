using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private cell[,] currentMap;
    private cell[,] previousMap;
    private Texture2D texture2D;

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
        RootNutrient
    }
    void Start()
    {
        currentMap = new cell[mapSize.x,mapSize.y];
        previousMap = new cell[mapSize.x,mapSize.y];
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
                if (x == 0 || y == 0 || x == mapSize.x - 1 || y == mapSize.y - 1)
                {
                    currentMap[x, y].type = Type.Sky;
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
                            case < 0.75f:
                            {
                                currentMap[x, y].type = Type.Dirt;
                                break;
                            }
                            case < 0.8f:
                            {
                                currentMap[x, y].type = Type.Rock;
                                break;
                            }
                            case < 0.9f:
                            {
                                currentMap[x, y].type = Type.Water;
                                break;
                            }
                            case <= 1f:
                            {
                                currentMap[x, y].type = Type.Nutrient;
                                break;
                            }
                        }

                    }
                }

            }
        }

        Vector2Int seedPoint = new Vector2Int(64, 64);
        for (int distance = 0; distance < 50; distance++)
        {
            int x = seedPoint.x + distance/2;
            int y = seedPoint.y - (distance + distance % 2) /2;

            currentMap[x, y].type = Type.Root;
            currentMap[x, y].distanceFromSeed = distance;
        }
        for (int distance = 0; distance < 70; distance++)
        {
            int x = seedPoint.x - distance/2;
            int y = seedPoint.y - (distance + distance % 2) /2;

            currentMap[x, y].type = Type.Root;
            currentMap[x, y].distanceFromSeed = distance;
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
                SimulateCell(x,y);
                PaintPixel(x,y);
            }
        }
        texture2D.Apply();
    }

    private void SimulateCell(int x, int y)
    {
        switch (currentMap[x, y].type)
        {
            case Type.Sky:
            {
                currentMap[x, y].type = previousMap[x, y].type;
                break;
            }
            case Type.Dirt:
            {
                if (previousMap[x, y + 1].type == Type.Water)
                {
                    currentMap[x, y].type = Type.Water;
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
                else
                {
                    currentMap[x, y].type = previousMap[x, y].type;
                }
                break;
            }
            case Type.Root:
            {
                if ((previousMap[x + 1, y].type == Type.RootNutrient && previousMap[x + 1, y].distanceFromSeed > previousMap[x, y].distanceFromSeed) ||
                    (previousMap[x - 1, y].type == Type.RootNutrient && previousMap[x - 1, y].distanceFromSeed > previousMap[x, y].distanceFromSeed) ||
                    (previousMap[x, y + 1].type == Type.RootNutrient && previousMap[x, y + 1].distanceFromSeed > previousMap[x, y].distanceFromSeed) ||
                    (previousMap[x, y - 1].type == Type.RootNutrient && previousMap[x, y - 1].distanceFromSeed > previousMap[x, y].distanceFromSeed))
                {
                    currentMap[x, y].type = Type.RootNutrient;
                }
                else if((previousMap[x + 1, y].type == Type.RootWater && previousMap[x + 1, y].distanceFromSeed > previousMap[x, y].distanceFromSeed) ||
                        (previousMap[x - 1, y].type == Type.RootWater && previousMap[x - 1, y].distanceFromSeed > previousMap[x, y].distanceFromSeed) ||
                        (previousMap[x, y + 1].type == Type.RootWater && previousMap[x, y + 1].distanceFromSeed > previousMap[x, y].distanceFromSeed) ||
                        (previousMap[x, y - 1].type == Type.RootWater && previousMap[x, y - 1].distanceFromSeed > previousMap[x, y].distanceFromSeed))
                {
                    currentMap[x, y].type = Type.RootWater;
                }
                else if (previousMap[x + 1, y].type == Type.Nutrient || previousMap[x - 1, y].type == Type.Nutrient ||
                         previousMap[x, y + 1].type == Type.Nutrient || previousMap[x, y - 1].type == Type.Nutrient)
                {
                    currentMap[x, y].type = Type.RootNutrient;
                }
                else if (previousMap[x + 1, y].type == Type.Water || previousMap[x - 1, y].type == Type.Water ||
                         previousMap[x, y + 1].type == Type.Water || previousMap[x, y - 1].type == Type.Water)
                {
                    currentMap[x, y].type = Type.RootWater;
                }
                else
                {
                    currentMap[x, y].type = previousMap[x, y].type;
                }
                break;
            }
            case Type.RootNutrient:
            {
                if ((previousMap[x + 1, y].type == Type.Root && previousMap[x + 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed) ||
                    (previousMap[x - 1, y].type == Type.Root  && previousMap[x - 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed) ||
                    (previousMap[x, y + 1].type == Type.Root  && previousMap[x, y+ 1].distanceFromSeed < previousMap[x, y].distanceFromSeed) ||
                    (previousMap[x, y - 1].type == Type.Root  && previousMap[x, y - 1].distanceFromSeed < previousMap[x, y].distanceFromSeed))
                {
                    currentMap[x, y].type = Type.Root;
                }
                else
                {
                    currentMap[x, y].type = previousMap[x, y].type;
                }
                break;
            }
            case Type.RootWater:
            {
                if ((previousMap[x + 1, y].type == Type.Root && previousMap[x + 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed) ||
                    (previousMap[x - 1, y].type == Type.Root  && previousMap[x - 1, y].distanceFromSeed < previousMap[x, y].distanceFromSeed) ||
                    (previousMap[x, y + 1].type == Type.Root  && previousMap[x, y+ 1].distanceFromSeed < previousMap[x, y].distanceFromSeed) ||
                    (previousMap[x, y - 1].type == Type.Root  && previousMap[x, y - 1].distanceFromSeed < previousMap[x, y].distanceFromSeed))
                {
                    currentMap[x, y].type = Type.Root;
                }
                else
                {
                    currentMap[x, y].type = previousMap[x, y].type;
                }
                break;
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
                texture2D.SetPixel(x, y, dirtColor);
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
                texture2D.SetPixel(x, y, rootColor);
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
        }
    }
}
