using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGen : MonoBehaviour
{
    public int ChunkSpawnRad = 3;
    public float buildCD = 0.2f;
    public float digCD = 0.15f;
    private float CD = 1f;

    public Vector2 offset = new Vector2(0, 13);
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();

    public ChunkRenderer ChunkPrefab;
    public GameObject Cursor;
    public GameObject Player;
    public GameObject World;
    public int seed;
    public Teraingen Teraingen;

    public bool ready = false;

    public ChunkData SpawnChunk;
    private ChunkData CheckChunk;

    public Vector2Int CurrentChunk;
    public Vector2Int PlayerChunk;

    private Camera Cam;
    public Vector3 PPos;

    public Vector3Int BlockWorldPos;
    public Vector3 BlockCenter;
    public Vector2Int UptdChunk;


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
        PlayerChunk = new Vector2Int((int)(PPos.x) / ChunkRenderer.chunkWide, (int)PPos.y / ChunkRenderer.chunkWide);
        StartCoroutine(updateChunks(PlayerChunk));

        
    }

    public void create_world()
    {
        seed = (int)(Time.realtimeSinceStartup * 1000000 % 10000);
        Cam = Camera.main;

        Spawn();
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
                //DeactChunks();
                //updtChunkPrin(PlayerChunk.x, PlayerChunk.y);
                StartCoroutine(updateChunks(PlayerChunk));

            }
            CheckInput();
            if (CD < 1) CD += Time.deltaTime;
        }
        
    }

	void Spawn()
	{
        PlayerChunk = new Vector2Int((int)(PPos.x) / ChunkRenderer.chunkWide, (int)PPos.y / ChunkRenderer.chunkWide);
        StartCoroutine(updateChunks(PlayerChunk));
		Vector3 cord = new Vector3(-1, -1, -1);
        
        SpawnChunk = ChunkDatas[new Vector2Int(PlayerChunk.x, PlayerChunk.y)];
        cord = SpawnChunk.Chunk.setSpawn(PlayerChunk.x, PlayerChunk.y);
                
		
        CurrentChunk = new Vector2Int((int)cord.x / ChunkRenderer.chunkWide,(int) cord.y / ChunkRenderer.chunkWide);
        Player.transform.position = cord;

	}

    void CheckInput()
    {
       
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - PPos;


        Ray2D ray = new Ray2D(PPos, mouse);
        RaycastHit2D hitInfo = Physics2D.Raycast(PPos, mouse, 10, LayerMask.GetMask("FG"));
        Vector2 hitP = new Vector2(hitInfo.point.x - PPos.x, hitInfo.point.y - PPos.y);
        
        if (hitInfo != false)
        {
            Cursor.SetActive(true);
            Cursor.transform.position = Vector3Int.FloorToInt(hitInfo.point);
            Debug.DrawRay(PPos, hitP, Color.yellow);
            if (Input.GetButton("Fire1") && CD > digCD)
            {
                CD = 0;
                BlockCenter = hitInfo.point;
                for (int x = -ChunkRenderer.size; x <= ChunkRenderer.size; x++)
                {
                    for (int y = -ChunkRenderer.size; y <= ChunkRenderer.size; y++)
                    {
                        BlockWorldPos = Vector3Int.FloorToInt(BlockCenter) + new Vector3Int(x, y, 0);
                        DeleteBlock(BlockWorldPos);
                    }
                }
            }
            else if (Input.GetButton("Fire2") && CD > buildCD && Mathf.Abs(hitP.x) > 0.5 && Mathf.Abs(hitP.y) > 0.5)
            {
                CD = 0;
                if(!Physics2D.Raycast(PPos, mouse, 3, LayerMask.GetMask("FG")))
                {
                    BlockCenter = hitInfo.point;

                    for (int x = -ChunkRenderer.size; x <= ChunkRenderer.size; x++)
                    {
                        for (int y = -ChunkRenderer.size; y <= ChunkRenderer.size; y++)
                        {
                            BlockWorldPos = Vector3Int.FloorToInt(BlockCenter) + new Vector3Int(x, y, 0);
                            SetBlock(BlockType.damagedStone, BlockWorldPos);
                        }
                    }
                } 
            }
        }
        else
        {
            Cursor.SetActive(false);
            Debug.DrawRay(PPos, mouse, Color.red);
        }
    }

    public void SetBlock(BlockType bt, Vector3Int BlockWorldPos)
    {
        if (BlockWorldPos.x < 0) BlockWorldPos.x -= ChunkRenderer.chunkWide - 1;
        if (BlockWorldPos.y < 0) BlockWorldPos.y -= ChunkRenderer.chunkWide - 1;

        UptdChunk = new Vector2Int(BlockWorldPos.x / ChunkRenderer.chunkWide, BlockWorldPos.y / ChunkRenderer.chunkWide);

        if (BlockWorldPos.x < 0) BlockWorldPos.x--;
        if (BlockWorldPos.y < 0) BlockWorldPos.y--;
        ChunkDatas[UptdChunk].Chunk.SetBlock(new Vector3Int(mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide), 0), bt);
        ChunkDatas[UptdChunk].Blocks[mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide)] = bt;
    }

    public void DeleteBlock(Vector3Int BlockWorldPos)
    {
        
        if (BlockWorldPos.x < 0) BlockWorldPos.x -= ChunkRenderer.chunkWide - 1;
        if (BlockWorldPos.y < 0) BlockWorldPos.y -= ChunkRenderer.chunkWide - 1;

        UptdChunk = new Vector2Int(BlockWorldPos.x / ChunkRenderer.chunkWide, BlockWorldPos.y / ChunkRenderer.chunkWide);

        if (BlockWorldPos.x < 0) BlockWorldPos.x--;
        if (BlockWorldPos.y < 0) BlockWorldPos.y--;
        ChunkDatas[UptdChunk].Chunk.DeleteBlock(new Vector3Int(mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide), 0));
        ChunkDatas[UptdChunk].Blocks[mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide)] = BlockType.bgAir;
    }


    public IEnumerator updateChunks(Vector2Int PChunk)
    {
        StartCoroutine(updtChunk(PChunk.x, PChunk.y));
        for (int x = 0; x <= ChunkSpawnRad; x++)
        {
            for (int y = 0; y <= ChunkSpawnRad; y++)
            {
                if (x == 0 && y == 0) yield return null;
                StartCoroutine(updtChunk(x + PChunk.x, y + PChunk.y));
                StartCoroutine(updtChunk(x + PChunk.x, -y + PChunk.y));
                StartCoroutine(updtChunk(-x + PChunk.x, y + PChunk.y));
                StartCoroutine(updtChunk(-x + PChunk.x, -y + PChunk.y));

                yield return null;
            }
        }

    }

    void updtChunkPrin(int x, int y)
    {
        if (!ChunkDatas.ContainsKey(new Vector2Int(x, y)))
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
    }

    public IEnumerator updtChunk(int x, int y)
    {
            if (!ChunkDatas.ContainsKey(new Vector2Int(x, y)))
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
                chunk.pos = new Vector2Int(x, y);
                chunk.ParentWorld = this;

            }
            else if (!ChunkDatas[new Vector2Int(x, y)].Chunk.gameObject.active)
            {
                ChunkDatas[new Vector2Int(x, y)].Chunk.gameObject.SetActive(true);
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
 