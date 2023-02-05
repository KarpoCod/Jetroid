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
    public Vector3Int BlockWorldPos;
    public Vector3 BlockCenter;
    public Vector2Int UptdChunk;



    void Start()
    {
        seed = (int)(Time.realtimeSinceStartup * 1000000);
        Cam = Camera.main;

        Debug.Log(PPos);
        PlayerChunk = new Vector2Int((int)(PPos.x) / ChunkRenderer.chunkWide, (int)PPos.y / ChunkRenderer.chunkWide);
        updateChunks();

       
        Debug.Log(PlayerChunk);
        Vector3 cord = new Vector3(-1, -1, -1);
        for (int y = PlayerChunk.y - ChunkSpawnRad;  y < PlayerChunk.y + ChunkSpawnRad; y++)
        {
            for(int x = PlayerChunk.x - ChunkSpawnRad;  x < PlayerChunk.x + ChunkSpawnRad; x++)
            {
                SpawnChunk = ChunkDatas[new Vector2Int(x, y)];
                cord = SpawnChunk.Chunk.setSpawn(x, y);
                if (cord != new Vector3(-1, -1, -1))
                {
                    cord += new Vector3(x * ChunkRenderer.chunkWide, y * ChunkRenderer.chunkWide, 0); 
                    break;
                }
            }
            if (cord != new Vector3(-1, -1, -1)) break;
        }
        CurrentChunk = new Vector2((int)cord.x / ChunkRenderer.chunkWide,(int) cord.y / ChunkRenderer.chunkWide);
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
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - PPos;


        Ray2D ray = new Ray2D(PPos, mouse);
        RaycastHit2D hitInfo = Physics2D.Raycast(PPos, mouse, 10, LayerMask.GetMask("FG"));
        Vector2 hitP = new Vector2(hitInfo.point.x - PPos.x, hitInfo.point.y - PPos.y);
        
        if (hitInfo != false)
        {
            Debug.DrawRay(PPos, hitP, Color.yellow);
            if (Input.GetButtonDown("Fire1"))
            {
                BlockCenter = hitInfo.point;
                BlockWorldPos = new Vector3Int(0, 0, 0);
                UptdChunk = new Vector2Int(0, 0);
                for (int x = -ChunkRenderer.size; x <= ChunkRenderer.size; x++)
                {
                    for (int y = -ChunkRenderer.size; y <= ChunkRenderer.size; y++)
                    {
                        BlockWorldPos = Vector3Int.FloorToInt(BlockCenter + new Vector3(0.5f, 0.5f, 0.5f)) + new Vector3Int(x, y, 0);
                        if (BlockWorldPos.x < 0) BlockWorldPos.x -= ChunkRenderer.chunkWide - 1;
                        if (BlockWorldPos.y < 0) BlockWorldPos.y -= ChunkRenderer.chunkWide - 1;

                        UptdChunk = new Vector2Int(BlockWorldPos.x / ChunkRenderer.chunkWide, BlockWorldPos.y / ChunkRenderer.chunkWide);

                        if (BlockWorldPos.x < 0) BlockWorldPos.x--;
                        if (BlockWorldPos.y < 0) BlockWorldPos.y--;
                        //Debug.Log(BlockCenter);
                        //Debug.Log(BlockWorldPos);
                        //Debug.Log(UptdChunk);
                        ChunkDatas[UptdChunk].Chunk.DeleteBlock(new Vector3Int(mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide), 0));
                    }
                }
            }
            if (Input.GetButtonDown("Fire2"))
            {
                if(!Physics2D.Raycast(PPos, mouse, 3, LayerMask.GetMask("FG")))
                {
                    BlockCenter = hitInfo.point;
                    BlockWorldPos = new Vector3Int(0, 0, 0);
                    UptdChunk = new Vector2Int(0, 0);
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
                            ChunkDatas[UptdChunk].Chunk.SetBlock(new Vector3Int(mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide), 0), BlockType.damagedStone);
                        }
                    }
                } 
            }
        }
        else
        {
            Debug.DrawRay(PPos, mouse, Color.red);
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
                    continue;
                }     
            }
        }
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
            res = ChunkRenderer.chunkWide - ((-a-1) % b)-1 ;
        }
        return res;
    }
        
}
 