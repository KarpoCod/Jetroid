 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using UnityEngine.SceneManagement;


public class LoadManager : MonoBehaviour
{
    public GameObject player;
    public BlockInfo[] blTyp;
    public WorldGen WorldPrefab;
    public GameObject PlayerPrefab;
    private string fToSave = " ";
    public WorldGen world;
    private Vector3 PlayerPos;
    public CameraFolow Cam;
    public GameObject LoadMan;
    public GameObject Canvas;
    public bool menu = false;


    private void Awake()
    {
        int op = DataHold.WorldOperation;
        switch(op)
        {
            case 0:
                Save_World();
                break;
            case 1:
                LoadWorld();
                break;
            case 2:
                create();
                break;
            case 3:
                destroy();
                break;
            case 4:
                exit();
                break;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Pause();

        }

    }

    public void Pause()
    {
        if (menu)
        {
            Canvas.gameObject.SetActive(false);
            Time.timeScale = 1;
            menu = false;
            world.CanBuild = true;
        }
        else
        {
            Canvas.gameObject.SetActive(true);
            Time.timeScale = 0;
            menu = true;
            world.CanBuild = false;
        }
    }

    public void Save_World()
    {
        //(Dictionary<Vector2Int, ChunkData> ChunkDatas, Vector2 PlayerPos
        Dictionary<Vector2Int, ChunkData> ChunkDatas = world.ChunkDatas;
        PlayerPos = player.transform.position;
        fToSave = PlayerPos.x + "p" + PlayerPos.y + "p" + world.seed + "p";
        foreach (KeyValuePair<Vector2Int, ChunkData> ReadCh in ChunkDatas)
        {            
            if (ReadCh.Value != null)
            {
                string chToSave = "c" + ReadCh.Key.x + "/" + ReadCh.Key.y + "/";

                //int count = 1;
                foreach (int bl in ReadCh.Value.Blocks)
                {
                    /*//if (fToSave.Length == 0) { return; }
                    if (chToSave.Substring(chToSave.Length - 2) == (bl / 10).ToString() + (bl % 10).ToString()) { count++; }
                    else if (chToSave.Substring(chToSave.Length - 2) != (bl / 10).ToString() + (bl % 10).ToString() && count > 1)
                    {
                        chToSave = chToSave.Remove(chToSave.Length - 6, 6);
                        chToSave += "n" + (count / 100).ToString() + (count % 100 / 10).ToString() + (count % 10).ToString() + (bl / 10).ToString() + (bl % 10).ToString();
                        count = 1;
                    }
                    else { chToSave += "n" + "001" + (bl / 10).ToString() + (bl % 10).ToString(); }*/

                    chToSave += "n" + (bl / 10).ToString() + (bl % 10).ToString();

                }
                /*if (count != 1)
                {
                    int bl = int.Parse(chToSave.Substring(chToSave.Length - 2));
                    chToSave = chToSave.Remove(chToSave.Length - 6, 6);
                    chToSave += "n" + (count / 100).ToString() + ((count % 100) / 10).ToString() + (count % 10).ToString() + (bl / 10).ToString() + (bl % 10).ToString();
                    count = 1;
                }*/
                fToSave += chToSave;
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
        Time.timeScale = 1;
        if(world != null) { destroy(); }
        string path = Application.persistentDataPath + "WorldSave.dat";
        
        if (File.Exists(path))
        {
            

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            string data = (string)formatter.Deserialize(stream);
            stream.Close();


            world.destroy();
            Destroy(world.World);

            world = Instantiate(WorldPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

            world.World.SetActive(true);
            world.Player = player;
            // Загрузка данных о положении игрока
            string[] positionData = data.Split('p');
            float posX = float.Parse(positionData[0]);
            float posY = float.Parse(positionData[1]);
            int seed = int.Parse(positionData[2]);
            //player = Instantiate(PlayerPrefab, new Vector3(posX, posY, 0f), Quaternion.identity, transform);

            if (player == null) { Instantiate(PlayerPrefab, new Vector3(posX, posY, 0f), Quaternion.identity); }
            else { player.transform.position = new Vector3(posX, posY, 0f); }
            world.seed = seed;
            

            /// Извлечение данных из строки сохранения
            Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
            int i = 0;
            string[] chunks = positionData[3].Split('c');
            foreach (string ch in chunks)
            {
                if (i == 0) { i++; continue; }
                string[] parts = ch.Split('/');
                
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);
                var chunkData = new ChunkData();
                int xpos = x * ChunkRenderer.chunkWide;
                int ypos = y * ChunkRenderer.chunkWide;

                
                string[] blockData = (parts[2].Substring(1)).Split('n');
                BlockType[,] ChunkBlocks = new BlockType[ChunkRenderer.chunkWide, ChunkRenderer.chunkWide];
                int con = 0;
                foreach (string blockDat in blockData)
                {
                    /*if (blockDat.Length > 4)
                    {
                        int count = int.Parse(blockDat.Substring(0, 3));
                        int blockID = int.Parse(blockDat.Substring(3, 2));
                        for (int k = 0; k < count; k++)
                        {
                            //ChunkBlocks[con] = blockID;

                            ChunkBlocks[con % ChunkRenderer.chunkWide, con / ChunkRenderer.chunkWide] = (blTyp.FirstOrDefault(b => b.ID == blockID)).BT;
                            con++;
                        }


                    }*/

                    int blockID = int.Parse(blockDat);
                    ChunkBlocks[(con / ChunkRenderer.chunkWide), (con % ChunkRenderer.chunkWide)] = (blTyp.FirstOrDefault(b => b.ID == blockID)).BT;
                    con++;
                }
                chunkData.Blocks = ChunkBlocks;

                chunkData.BgBlocks = world.Teraingen.GenerateBG(xpos, ypos, seed);
                world.ChunkDatas.Add(new Vector2Int(x, y), chunkData);

                var chunk = Instantiate(world.ChunkPrefab, new Vector3(xpos, ypos, 0), Quaternion.identity, world.World.transform);
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

    public void create()
    {
        Time.timeScale = 1;
        if (world != null) { destroy(); }

        world = Instantiate(WorldPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);

        if (player == null) { player = Instantiate(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity); }
        else {  player.SetActive(true); }
        
        world.Player = player;
        world.create_world();
        world.ready = true;
        Cam.target = player;
    }

    public void destroy()
    {
        Time.timeScale = 1;
        world.destroy();
        Destroy(world.World);
        player.SetActive(false);
        //Destroy(world);
        //Destroy(player);
        //Cam.target = LoadMan;
    }

    public void exit()
    {
        Save_World();
        Time.timeScale = 1;
        SceneManager.LoadScene("Game Menu");
    }

}
