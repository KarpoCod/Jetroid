using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Teraingen : MonoBehaviour
{
    public static BlockType[,] GenerateTaerrain(int xOffset, int yOffset, int seed)
    {
        int chunkWide = ChunkRenderer.chunkWide;
        int mount = 80;
        //int grass = 70;
        //int dungeons = 60;

        var result = new BlockType[chunkWide, chunkWide];
        for (int x=0; x < chunkWide; x++)
        {
            for(int y=0; y < chunkWide; y++)
            {
                float hight = (2f * Mathf.PerlinNoise((x+xOffset+seed) / 3201f, 0) 
                    + 0.5f * Mathf.PerlinNoise((x + xOffset + seed) / 104f, 0) 
                    + 0.25f * Mathf.PerlinNoise((x + xOffset + seed) / 15f, 0))/ 2.75f * 120f;

                float DanRate = (Mathf.PerlinNoise((x + xOffset + seed * 3) / 11f, (y + yOffset + seed * 7) / 9f)
                   + 0.5f * Mathf.PerlinNoise((x + xOffset + seed * 2) / 14f, (y + yOffset + seed * 6) / 9f)
                   + 0.25f * Mathf.PerlinNoise((x + xOffset + seed * 7) / 6f, (y + yOffset + seed * 9) / 14f)) / 1.75f * 100f;
                float grassRate = Mathf.PerlinNoise((x + xOffset + seed* 4) / 5f, (y + yOffset + seed) / 5f);
                float dirtRate = Mathf.PerlinNoise((x + xOffset + seed) / 3f, (y + yOffset + seed * 3) / 7f);
                float Bioms = Mathf.PerlinNoise((x + xOffset + seed*3) * 5f, (y + yOffset + seed*2) * 5f);


                //max height
                if (hight + 8 < y + yOffset || y + yOffset > 12800 || (DanRate > 75 || DanRate > grassRate * 125) && hight + 63 > y + yOffset)
                {
                    result[x, y] = BlockType.bgAir;
                }
                //mountains
                else if (y + yOffset > mount && y + yOffset < 12800)
                {
                    if (dirtRate < 0.5)
                    {
                        result[x, y] = BlockType.stone;
                    }
                    else if (grassRate > 0.75)
                    {
                        result[x, y] = BlockType.grass;
                    }
                    else
                    {
                        result[x, y] = BlockType.dirt;
                    }

                }
                //grass
                else if (y + yOffset + 4 > hight && hight - grassRate*120 > 10 || dirtRate > 0.5 && grassRate > 0.7)
                {
                    result[x, y] = BlockType.grass;

                }
                else if (dirtRate < 0.4)
                {
                    result[x, y] = BlockType.stone;
                }
                else if (dirtRate > 0.4)
                {
                    result[x, y] = BlockType.dirt;
                }
                else
                {
                    result[x, y] = BlockType.stone;
                }
                

                
            }
        }
        return result;
    }

    public static BlockType[,] GenerateBG(int xOffset, int yOffset, int seed)
    {
        int chunkWide = ChunkRenderer.chunkWide;
        var result = new BlockType[chunkWide, chunkWide];
        for (int x = 0; x < chunkWide; x++)
        {
            for (int y = 0; y < chunkWide; y++)
            {
                float hight = Mathf.PerlinNoise((x + xOffset + seed * 3) / 11f, (y + yOffset + seed * 7) / 9f)
                    + 0.5f * Mathf.PerlinNoise((x + xOffset + seed * 2) / 14f, (y + yOffset + seed * 6) / 9f)
                    + 0.25f * Mathf.PerlinNoise((x + xOffset + seed * 7) / 6f, (y + yOffset + seed * 9) / 14f);
                float bgRate = 0.8f + 0.4f * Mathf.PerlinNoise((x + xOffset + seed * 3) / 11f, (y + yOffset + seed * 7) / 9f);
                float Bioms = Mathf.PerlinNoise((x + xOffset + seed * 3) * 5f, (y + yOffset + seed * 2) * 5f);

                if (hight > bgRate)
                {
                    result[x, y] = BlockType.bgAir;
                }
                else
                {
                    result[x, y] = BlockType.bgDirt;
                }
            }
        }
        return result;
    }
}
