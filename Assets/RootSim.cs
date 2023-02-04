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

        for(int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
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

    void FixedUpdate()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {

                switch (currentMap[x,y].type)
                {
                    case Type.Sky:
                    {
                        texture2D.SetPixel(x,y,skyColor);
                        break;
                    }
                    case Type.Dirt:
                    {
                        texture2D.SetPixel(x,y,dirtColor);
                        break;
                    }
                    case Type.Rock:
                    {
                        texture2D.SetPixel(x,y,rockColor);
                        break;
                    }
                    case Type.Water:
                    {
                        texture2D.SetPixel(x,y,waterColor);
                        break;
                    }
                    case Type.Nutrient:
                    {
                        texture2D.SetPixel(x,y,nutrientColor);
                        break;
                    }
                    case Type.Root:
                    {
                        texture2D.SetPixel(x,y,rootColor);
                        break;
                    }
                    case Type.RootNutrient:
                    {
                        texture2D.SetPixel(x,y,rootNutrientColor);
                        break;
                    }
                    case Type.RootWater:
                    {
                        texture2D.SetPixel(x,y,rootWaterColor);
                        break;
                    }
                }
                
            }
        }
        texture2D.Apply();
    }
}
