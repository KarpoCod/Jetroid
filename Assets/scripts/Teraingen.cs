using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Teraingen : MonoBehaviour
{

    public int mount = 80;
    public int grass = 60;
    public int dungeons = -40;

    public int BaseHight = 0;
    public NoiseOctaveSettings[] HOctaves;
    public NoiseOctaveSettings[] DOctaves;
    public NoiseOctaveSettings[] AOctaves;

    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f; 
        public float Amplitude = 1 ;
    }

    public FastNoiseLite[] HoctaveNoises;
    public FastNoiseLite[] DoctaveNoises;
    public FastNoiseLite[] AoctaveNoises;

    public void Awake()
    {
        HoctaveNoises = new FastNoiseLite[HOctaves.Length];
        DoctaveNoises = new FastNoiseLite[DOctaves.Length];
        AoctaveNoises = new FastNoiseLite[AOctaves.Length];
        for (int i = 0; i < HOctaves.Length; i++)
        {
            HoctaveNoises[i] = new FastNoiseLite();
            HoctaveNoises[i].SetNoiseType(HOctaves[i].NoiseType);
            HoctaveNoises[i].SetFrequency(HOctaves[i].Frequency);
        }
        for (int i = 0; i < DOctaves.Length; i++)
        {
            DoctaveNoises[i] = new FastNoiseLite();
            DoctaveNoises[i].SetNoiseType(DOctaves[i].NoiseType);
            DoctaveNoises[i].SetFrequency(DOctaves[i].Frequency);
        }
        for (int i = 0; i < AOctaves.Length; i++)
        {
            AoctaveNoises[i] = new FastNoiseLite();
            AoctaveNoises[i].SetNoiseType(AOctaves[i].NoiseType);
            AoctaveNoises[i].SetFrequency(AOctaves[i].Frequency);
        }
    }

    public BlockType[,] GenerateTaerrain(int xOffset, int yOffset, int seed)
    {

        int chunkWide = ChunkRenderer.chunkWide;
        

        var result = new BlockType[chunkWide, chunkWide];

        for (int x=0; x < chunkWide; x++)
        {
            for(int y=0; y < chunkWide; y++)
            {
                float hight = GetHight(x + xOffset, 0, seed, HOctaves, HoctaveNoises);
                float danRate = GetHight(x + xOffset, y + yOffset, seed, DOctaves, DoctaveNoises);
                float grassRate = GetHight(x + xOffset, y + yOffset, seed, AOctaves, AoctaveNoises);

                if (hight < y + yOffset)
                {
                    result[x, y] = BlockType.bgAir;
                }
                else if(danRate < dungeons)
                {
                    result[x, y] = BlockType.bgAir;
                }
                else if (danRate < dungeons && grassRate < dungeons + 5 || grassRate > grass)
                {
                    result[x, y] = BlockType.grass;
                }
                else
                {
                    result[x, y] = BlockType.stone;
                }

                /*float hight = (2f * Mathf.PerlinNoise((x+xOffset+seed) / 3201f, 0) 
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
                */

                
            }
        }
        return result;
    }

    public BlockType[,] GenerateBG(int xOffset, int yOffset, int seed)
    {
        int chunkWide = ChunkRenderer.chunkWide;
        var result = new BlockType[chunkWide, chunkWide];
        for (int x = 0; x < chunkWide; x++)
        {
            for (int y = 0; y < chunkWide; y++)
            {
                float hight = GetHight(x + xOffset, 0, seed, HOctaves, HoctaveNoises);
                float bgRate = GetHight(x + xOffset, y + yOffset, seed, DOctaves, DoctaveNoises);
                float B = Mathf.PerlinNoise((x + xOffset + seed * 3) * 5f, (y + yOffset + seed * 2) * 5f);

                if (bgRate < dungeons - 15 * B && hight > y + yOffset)
                {
                    result[x, y] = BlockType.bgDirt;
                }
                else
                {
                    result[x, y] = BlockType.bgAir;
                }
            }
        }
        return result;
    }

     float GetHight(float x, float y, int seed, NoiseOctaveSettings[] Octaves, FastNoiseLite[] octaveNoises)
    {

        float result = BaseHight;

        for(int i = 0; i < Octaves.Length; i++)
        {
            octaveNoises[i].SetSeed(seed);
            float noise = octaveNoises[i].GetNoise(x, y);
            result += noise * Octaves[i].Amplitude / 2;
        }
        return result;
    }
}
