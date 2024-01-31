using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Teraingen : MonoBehaviour
{
    public BiomInfo[] bioms;
    public int BaseHight = 0;

    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }

    private FastNoiseLite[][] HoctaveNoises;
    private FastNoiseLite[][] DoctaveNoises;
    private FastNoiseLite[][] AoctaveNoises;

    public void Awake()
    {
        HoctaveNoises = new FastNoiseLite[bioms.Length][];
        DoctaveNoises = new FastNoiseLite[bioms.Length][];
        AoctaveNoises = new FastNoiseLite[bioms.Length][];
        foreach (BiomInfo biom in bioms)
        {
            HoctaveNoises[biom.index] = new FastNoiseLite[biom.HOctaves.Length];
            DoctaveNoises[biom.index] = new FastNoiseLite[biom.DOctaves.Length];
            AoctaveNoises[biom.index] = new FastNoiseLite[biom.AOctaves.Length];
            for (int i = 0; i < biom.HOctaves.Length; i++)
            {
                HoctaveNoises[biom.index][i] = new FastNoiseLite();
                HoctaveNoises[biom.index][i].SetNoiseType(biom.HOctaves[i].NoiseType);
                HoctaveNoises[biom.index][i].SetFrequency(biom.HOctaves[i].Frequency);
            }
            for (int i = 0; i < biom.DOctaves.Length; i++)
            {
                DoctaveNoises[biom.index][i] = new FastNoiseLite();
                DoctaveNoises[biom.index][i].SetNoiseType(biom.DOctaves[i].NoiseType);
                DoctaveNoises[biom.index][i].SetFrequency(biom.DOctaves[i].Frequency);
            }
            for (int i = 0; i < biom.AOctaves.Length; i++)
            {
                AoctaveNoises[biom.index][i] = new FastNoiseLite();
                AoctaveNoises[biom.index][i].SetNoiseType(biom.AOctaves[i].NoiseType);
                AoctaveNoises[biom.index][i].SetFrequency(biom.AOctaves[i].Frequency);
            }
        }
    }

    public int GetFirstAir(int xOffset, int seed)
    {
        BiomInfo biom = SetBiom(xOffset, seed);
        for (int i = 10; i == 0; i--) { Debug.Log(i + " to start"); }
        float hight = GetHight(xOffset, 0, seed, biom.HOctaves, HoctaveNoises[biom.index]);
        return (int)Math.Ceiling(hight + 1);
    }

    public BlockType[,] GenerateTaerrain(int xOffset, int yOffset, int seed)
    {
        BiomInfo biom = SetBiom(xOffset, seed);
        int chunkWide = ChunkRenderer.chunkWide;
        var result = new BlockType[chunkWide, chunkWide];
            for (int x = 0; x < chunkWide; x++)
            {
                float hight = GetHight(x + xOffset, 0, seed, biom.HOctaves, HoctaveNoises[biom.index]);
                for (int y = 0; y < chunkWide; y++)
                {

                    
                    float danRate = GetHight(x + xOffset, y + yOffset, seed, biom.DOctaves, DoctaveNoises[biom.index]);
                    float grassRate = GetHight(x + xOffset, y + yOffset, seed, biom.AOctaves, AoctaveNoises[biom.index]);

                    if (hight < y + yOffset)
                    {
                        result[x, y] = BlockType.bgAir;
                    }
                    else if (danRate < biom.dungeonsRate)
                    {
                        result[x, y] = BlockType.bgAir;
                    }
                    else if (danRate < biom.dungeonsRate && grassRate < biom.dungeonsRate + 5 || grassRate > biom.grassRate)
                    {
                        result[x, y] = biom.grass;
                    }
                    else
                    {
                        result[x, y] = biom.stone;
                    }
                }
            }
        return result;
    }

    public BlockType[,] GenerateBG(int xOffset, int yOffset, int seed)
    {
        BiomInfo biom = SetBiom(xOffset, seed);
        int chunkWide = ChunkRenderer.chunkWide;
        var result = new BlockType[chunkWide, chunkWide];
            for (int x = 0; x < chunkWide; x++)
            {
                float hight = GetHight(x + xOffset, 0, seed, biom.HOctaves, HoctaveNoises[biom.index]);
                for (int y = 0; y < chunkWide; y++)
                {

                    
                    float bgRate = GetHight(x + xOffset, y + yOffset, seed, biom.DOctaves, DoctaveNoises[biom.index]);
                    float B = Mathf.PerlinNoise((x + xOffset + seed * 3) * 5f, (y + yOffset + seed * 2) * 5f) + 1;

                    if (bgRate > biom.dungeonsRate - 17 * B && hight > y + yOffset)
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
            for (int i = 0; i < Octaves.Length; i++)
            {
                    try
                    {
                        octaveNoises[i].SetSeed(seed);
                        float noise = octaveNoises[i].GetNoise(x, y);
                        result += noise * Octaves[i].Amplitude / 2;
                    }
                    catch { }
            }     
        return result;
     }

    public BiomInfo SetBiom(int xOffset,int seed)
    {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetSeed(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        noise.SetFrequency(0.03f);
        int res = (int)Math.Ceiling(MathF.Abs(noise.GetNoise(xOffset, 0))) * bioms.Length;
        if (res == bioms.Length) { res--; }
        Debug.Log(res);
        return bioms[res];
    }
}
