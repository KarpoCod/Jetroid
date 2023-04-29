using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;


public class LoadManager : MonoBehaviour
{
    public GameObject player;
    public BlockInfo[] blTyp;
    public WorldGen WorldPrefab;
    public GameObject PlayerPrefab;
    private int count = 1;
    private string fToSave = " ";
    public WorldGen world;
    private Vector3 PlayerPos;
    public CameraFolow Cam;
    public GameObject LoadMan;

    public void Save_World()
    {
        //(Dictionary<Vector2Int, ChunkData> ChunkDatas, Vector2 PlayerPos
        Dictionary<Vector2Int, ChunkData> ChunkDatas = world.ChunkDatas;
        PlayerPos = player.transform.position;
        fToSave = PlayerPos.x + "p" + PlayerPos.y + "p" + world.seed + "p";
        foreach (KeyValuePair<Vector2Int, ChunkData> ReadCh in ChunkDatas)
        {
            fToSave += 'c' + ReadCh.Key.x + '/' + ReadCh.Key.y + '/';
            //Debug.Log(fToSave);
            foreach (int bl in ReadCh.Value.Blocks)
            {
                //if (fToSave.Length == 0) { return; }
                if (fToSave.Substring(fToSave.Length - 2) == (bl / 10).ToString() + (bl % 10).ToString()) { count++; }
                else if (fToSave.Substring(fToSave.Length - 2) != (bl / 10).ToString() + (bl % 10).ToString() && count != 1)
                {
                    fToSave = fToSave.Remove(fToSave.Length - 6, 6);
                    fToSave += 'n' + (count / 100).ToString() + (count % 100 / 10).ToString() + (count % 10).ToString() + (bl / 10).ToString() + (bl % 10).ToString();
                    count = 1;
                }
                else { fToSave += "n" + "001" + (bl / 10).ToString() + (bl % 10).ToString(); }
            }
            if (count != 1)
            {
                int bl = int.Parse(fToSave.Substring(fToSave.Length - 2));
                fToSave = fToSave.Remove(fToSave.Length - 6, 6);
                fToSave += 'n' + (count / 100).ToString() + (count % 100 / 10).ToString() + (count % 10).ToString() + (bl / 10).ToString() + (bl % 10).ToString();
                count = 1;
            }
        }
      
        BinaryFormatter form = new BinaryFormatter();
        string path = Application.persistentDataPath + "WorldSave.dat";
        FileStream stream = new FileStream(path, FileMode.Create);
        form.Serialize(stream, fToSave);
        stream.Close();
    }
    

    public void LoadWorld()
    {
        string path = Application.persistentDataPath + "WorldSave.dat";
        
        if (File.Exists(path))
        {
            

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            string data = (string)formatter.Deserialize(stream);
            stream.Close();

            Destroy(world.World);
            world = Instantiate(WorldPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            world.World.SetActive(true);
            //world.Player = player;
            // Загрузка данных о положении игрока
            string[] positionData = data.Split('p');
            Debug.Log("{" + positionData[0] + "}" + positionData[1] + "/" + positionData[2]);
            float posX = float.Parse(positionData[0]);
            float posY = float.Parse(positionData[1]);
            int seed = int.Parse(positionData[2]);
            //player = Instantiate(PlayerPrefab, new Vector3(posX, posY, 0f), Quaternion.identity, transform);

            if (player == null) { Instantiate(PlayerPrefab, new Vector3(posX, posY, 0f), Quaternion.identity); }
            world.seed = seed;
            

            /// Извлечение данных из строки сохранения
            Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();

            string[] chunks = data.Split('c');
            int i = 0;
            foreach (string ch in chunks)
            {
                Debug.Log(ch);
                if (i == 0) { i++; continue; } /// Пропустить первый блок
                string[] parts = ch.Split('/');
                
                Debug.Log("(" + parts[0] + ")  " + parts[1]);
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);
                var chunkData = new ChunkData();
                int xpos = x * ChunkRenderer.chunkWide;
                int ypos = y * ChunkRenderer.chunkWide;

                
                string[] blockData = (parts[2].Substring(1)).Split('n');
                //int[] ChunkBlocks = new int[ChunkRenderer.chunkWide * ChunkRenderer.chunkWide];
                int con = 0;
                foreach (string blockDat in blockData)
                {
                    Debug.Log(blockDat.Length);
                    if (blockDat.Length > 4)
                    {
                        Debug.Log("lox");
                        int count = int.Parse(blockDat.Substring(0, 3));
                        int blockID = int.Parse(blockDat.Substring(2, 2));
                        for (int k = 0; k < count; k++)
                        {
                            //ChunkBlocks[con] = blockID;

                            chunkData.Blocks[con % ChunkRenderer.chunkWide, con / ChunkRenderer.chunkWide] =  BlockType.damagedStone;
                            con++;
                        }
                    }
                }

                chunkData.BgBlocks = Teraingen.GenerateBG(xpos, ypos, seed);
                world.ChunkDatas.Add(new Vector2Int(x, y), chunkData);

                var chunk = Instantiate(world.ChunkPrefab, new Vector3(xpos, ypos, 0), Quaternion.identity, transform);
                chunk.ChunkData = chunkData;
                chunkData.Chunk = chunk;
                chunkData.seed = world.seed;
                chunk.ParentWorld = world;

                //world.LoadChunk(ChunkDatas[new Vector2Int(x, y)]);
            }

            

            

            Cam.target = player;
            world.Player = player;
            player.transform.position = new Vector3(posX, posY, 0f);
            player.SetActive(true);
            world.gen_world();
            world.ready = true;

            //world.ChunkDatas = ChunkDatas;
        }
    }

    void exit()
    {
        world.destroy();
        Destroy(world.World);
        player.SetActive(false);
        //Destroy(world);
        //Destroy(player);
        //Cam.target = LoadMan;
    }

    void create()
    {
        if (world != null) { exit(); }

        world = Instantiate(WorldPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        if (player == null) { player = Instantiate(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity); }
        else {  player.SetActive(true); }
        
        world.Player = player;
        world.create_world();
        world.ready = true;
        Cam.target = player;    
    }
}
