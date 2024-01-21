using UnityEngine;

[CreateAssetMenu(menuName = "Biom")]
public class BiomInfo : ScriptableObject
{
    public int index;
    public int mountRate;
    public int grassRate;
    public int dungeonsRate;
    public BlockType stone;
    public BlockType grass;
    public BlockType dirt;
    public BlockType special;
    public Teraingen.NoiseOctaveSettings[] HOctaves;
    public Teraingen.NoiseOctaveSettings[] DOctaves;
    public Teraingen.NoiseOctaveSettings[] AOctaves;

}