using UnityEngine;

[CreateAssetMenu(menuName = "Biom")]
public class BiomInfo : ScriptableObject
{
    public int index;
    public string name;
    public int mount;
    public int grass;
    public int dungeons;
    public Teraingen.NoiseOctaveSettings[] HOctaves;
    public Teraingen.NoiseOctaveSettings[] DOctaves;
    public Teraingen.NoiseOctaveSettings[] AOctaves;

}