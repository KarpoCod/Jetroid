using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{
    public int ChunkSpawnRad = 3;
    public Vector2 offset = new Vector2(0, 13);
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
    public ChunkRenderer ChunkPrefab;
    public GameObject Player;
    public ChunkData SpawnChunk;
    private ChunkData CheckChunk;
    private Vector2 CurrentChunk;
    private Camera Cam;
    public Vector3 PPos = new Vector3(0, 0, 0);
    public Vector2Int PlayerChunk;
    public int seed;


    void Start()
    {
        seed = (int)(Time.realtimeSinceStartup * 1000000);
        Cam = Camera.main;

        Debug.Log(seed);
        PlayerChunk = new Vector2Int((int)(PPos.x) / ChunkRenderer.chunkWide, (int)PPos.y / ChunkRenderer.chunkWide);
        updateChunks();

       
        Debug.Log(PlayerChunk);
        Vector3 cord = new Vector3(0, 0, 0);
        bool CanSpawn = false;
        for (int y = PlayerChunk.y - ChunkSpawnRad;  y < PlayerChunk.y + ChunkSpawnRad; y++)
        {
            for(int x = PlayerChunk.x - ChunkSpawnRad;  x < PlayerChunk.x + ChunkSpawnRad; x++)
            {
                SpawnChunk = ChunkDatas[new Vector2Int(x, y)];
                cord = SpawnChunk.Chunk.setSpawn(x, y);
                CanSpawn = (cord != new Vector3(-1, -1, -1));
            }
        }
        CurrentChunk = cord;
        Player.transform.position = cord;
    }


    void Update()
    {
        PPos = Player.transform.position;
        PlayerChunk = new Vector2Int((int) (PPos.x) / ChunkRenderer.chunkWide, (int)PPos.y / ChunkRenderer.chunkWide);
        if (CurrentChunk != PlayerChunk)
        {
            CurrentChunk = PlayerChunk;
            updateChunks();
        }

        CheckInput();
    }


    void CheckInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - PPos;


            Ray2D ray = new Ray2D(PPos, mouse);
            Debug.DrawRay(PPos, mouse, Color.yellow);
            RaycastHit2D hitInfo = Physics2D.Raycast(PPos, mouse, 10, LayerMask.GetMask("FG"));
            if (hitInfo != false)
            {
            Vector3 BlockCenter = hitInfo.point;
            Vector3Int BlockWorldPos = new Vector3Int(0, 0, 0);
            Vector2Int UptdChunk = new Vector2Int(0, 0);
                for (int x = -ChunkRenderer.size; x <= ChunkRenderer.size; x++)
                {
                    for (int y = -ChunkRenderer.size; y <= ChunkRenderer.size; y++)
                    {
                        BlockWorldPos = Vector3Int.FloorToInt(BlockCenter) + new Vector3Int(x, y, 0);
                        if (BlockWorldPos.x < 0) BlockWorldPos.x -= ChunkRenderer.chunkWide - 1;
                        if (BlockWorldPos.y < 0) BlockWorldPos.y -= ChunkRenderer.chunkWide - 1;

                        UptdChunk = new Vector2Int(BlockWorldPos.x / ChunkRenderer.chunkWide, BlockWorldPos.y / ChunkRenderer.chunkWide);

                        if (BlockWorldPos.x < 0) BlockWorldPos.x--;
                        if (BlockWorldPos.y < 0) BlockWorldPos.y--;
                        Debug.Log(BlockCenter);
                        Debug.Log(BlockWorldPos);
                        Debug.Log(UptdChunk);
                        ChunkDatas[UptdChunk].Chunk.DeleteBlock(new Vector3Int(mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide), 0));
                    }
                }

                


            }

        }
    }

    public void updateChunks()
    {
        PlayerChunk = new Vector2Int((int)(PPos.x) / ChunkRenderer.chunkWide, (int)PPos.y / ChunkRenderer.chunkWide);
        for (int x = -ChunkSpawnRad + PlayerChunk.x; x <= ChunkSpawnRad + PlayerChunk.x; x++)
        {
            for (int y = -ChunkSpawnRad + PlayerChunk.y; y <= ChunkSpawnRad + PlayerChunk.y; y++)
            {
                try
                {
                    var chunkData = new ChunkData();
                    int xpos = x * ChunkRenderer.chunkWide;
                    int ypos = y * ChunkRenderer.chunkWide;
                    chunkData.Blocks = Teraingen.GenerateTaerrain(xpos, ypos, seed);
                    chunkData.BgBlocks = Teraingen.GenerateBG(xpos, ypos, seed);
                    ChunkDatas.Add(new Vector2Int(x, y), chunkData);

                    var chunk = Instantiate(ChunkPrefab, new Vector3(xpos, ypos, 0), Quaternion.identity, transform);
                    chunk.ChunkData = chunkData;
                    chunkData.Chunk = chunk;
                    chunkData.seed = seed;
                    chunk.ParentWorld = this;
                }
                catch
                {
                    Debug.LogWarning("chunk is already created!");
                }     
            }
        }
        Debug.LogWarning("chunks updated");
    }

    int mod(int a, int b)
    {
        int res;
        if (a >= 0)
        {
            res = a % b;
        }
        else
        {
            res = ChunkRenderer.chunkWide - (-a % b) ;
        }
        return res;
    }
        
}
 