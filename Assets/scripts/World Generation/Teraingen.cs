using System;
using System.Collections;
using System.Collections.Generic;
using static Teraingen;

public class Teraingen
{
    public int BaseHight;
    public int seed;


    public BiomInfo[] bioms;
    

    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }
    TerrainMath TerrainMath;

    public Teraingen(BiomInfo[] bms, int s = 0, int bh = 0)
    {
        seed = s;
        BaseHight = bh;
        bioms = bms;
        TerrainMath = new TerrainMath(bioms, BaseHight);
        TerrainMath.INIT();
    }

    public void Generate(int seed, int width, int hight)
    {

    }

    public int GetFirstAir(int xOffset, int seed)
    {
        return TerrainMath.GetFirstAir(xOffset, seed);
    }

    public BlockType[,] GenerateTerrain(int xOffset, int yOffset, int seed)
    {
        return TerrainMath.GenerateTerrain(xOffset, yOffset, seed); 
    }

    public BlockType[,] GenerateBG(int xOffset, int yOffset, int seed)
    {
        return TerrainMath.GenerateBG(xOffset, yOffset, seed);
    }

}

class TerrainMath{
    private FastNoiseLite[][] HoctaveNoises;
    private FastNoiseLite[][] DoctaveNoises;
    private FastNoiseLite[][] AoctaveNoises;

    private BiomInfo[] bioms;
    private int BaseHight;


    public TerrainMath(BiomInfo[] bioms, int BaseHight)
    {
        this.bioms = bioms;
        this.BaseHight = BaseHight;
    }

    public void INIT()
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
        float hight = GetHight(xOffset, 0, seed, biom.HOctaves, HoctaveNoises[biom.index]);
        return (int)Math.Ceiling(hight + 1);
    }

    public BlockType[,] GenerateTerrain(int xOffset, int yOffset, int seed)
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
                float B = GetHight((x + xOffset + seed * 3), (y + yOffset + seed * 2), 547, biom.DOctaves, DoctaveNoises[biom.index]) + 1;

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

    public BiomInfo SetBiom(int xOffset, int seed)
    {
        FastNoiseLite noise = new FastNoiseLite();
        noise.SetSeed(seed);
        noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        noise.SetFrequency(0.03f);
        int res = (int)Math.Ceiling(MathF.Abs(noise.GetNoise(xOffset, 0))) * bioms.Length;
        if (res == bioms.Length) { res--; }
        return bioms[res];
    }
}