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
    public GameObject World;
    public ChunkData SpawnChunk;
    private ChunkData CheckChunk;
    private Vector2 CurrentChunk;
    private Camera Cam;
    public Vector3 PPos;
    public Vector2Int PlayerChunk;
    public int seed;
    public Vector3Int BlockWorldPos;
    public Vector3 BlockCenter;
    public Vector2Int UptdChunk;
    public bool ready = false;
    public Teraingen Teraingen;




    public void gen_world()
    {
        PPos = Player.transform.position;

        PlayerChunk = new Vector2Int((int)(PPos.x) / ChunkRenderer.chunkWide, (int)PPos.y / ChunkRenderer.chunkWide);
        foreach (KeyValuePair<Vector2Int, ChunkData> ReadCh in ChunkDatas)
        {
            int x = ReadCh.Key.x;
            int y = ReadCh.Key.y;
            var chunkData = new ChunkData();
            int xpos = x * ChunkRenderer.chunkWide;
            int ypos = y * ChunkRenderer.chunkWide;
            chunkData.Blocks = ChunkDatas[new Vector2Int(x, y)].Blocks;
            chunkData.BgBlocks = Teraingen.GenerateBG(xpos, ypos, seed);

        }
            StartCoroutine(updateChunks());

        
    }

    public void create_world()
    {
        seed = (int)(Time.realtimeSinceStartup * 1000000 % 10000);
        Cam = Camera.main;

        StartCoroutine(Spawn());
        gen_world();
    }

    void Update()
    {
        if (ready)
        {
            PPos = Player.transform.position;
            PlayerChunk = new Vector2Int((int)(PPos.x) / ChunkRenderer.chunkWide, (int)PPos.y / ChunkRenderer.chunkWide);
            if (CurrentChunk != PlayerChunk)
            {
                CurrentChunk = PlayerChunk;
                StartCoroutine(updateChunks());
            }
            CheckInput();
        }
        
    }

	IEnumerator Spawn()
	{
		yield return StartCoroutine(updateChunks());
		Vector3 cord = new Vector3(-1, -1, -1);
        
        SpawnChunk = ChunkDatas[new Vector2Int(PlayerChunk.x, PlayerChunk.y)];
        cord = SpawnChunk.Chunk.setSpawn(PlayerChunk.x, PlayerChunk.y);
                
		
        CurrentChunk = new Vector2((int)cord.x / ChunkRenderer.chunkWide,(int) cord.y / ChunkRenderer.chunkWide);
        Player.transform.position = cord;

		yield return null;
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
                        ChunkDatas[UptdChunk].Chunk.DeleteBlock(new Vector3Int(mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide), 0));
                        ChunkDatas[UptdChunk].Blocks[mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide)] = BlockType.bgAir;
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
                            ChunkDatas[UptdChunk].Blocks[mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide)] = BlockType.damagedStone;
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

    public IEnumerator updateChunks()
    {
        PlayerChunk = new Vector2Int((int)(PPos.x) / ChunkRenderer.chunkWide, (int)PPos.y / ChunkRenderer.chunkWide);
        for (int x = -ChunkSpawnRad + PlayerChunk.x; x <= ChunkSpawnRad + PlayerChunk.x; x++)
        {
            for (int y = -ChunkSpawnRad + PlayerChunk.y; y <= ChunkSpawnRad + PlayerChunk.y; y++)
            {
                StartCoroutine(updtChunk(x, y));
                yield return null;
            }
        }
    }

    public IEnumerator updtChunk(int x, int y)
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
        }
        yield return null;
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
        
    public void destroy()
    {
        foreach (KeyValuePair<Vector2Int, ChunkData> ReadCh in ChunkDatas)
        {
            Destroy(ReadCh.Value.Chunk);
        }
    }
}
 