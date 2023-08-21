using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;

public class WorldGen : MonoBehaviour
{
    private ConcurrentQueue <ChunkData> generatedResults = new ConcurrentQueue<ChunkData>();
    private ConcurrentQueue<ChunkData> generatedResultsPrin = new ConcurrentQueue<ChunkData>();

    public int ChunkSpawnRad = 3;
    public float buildCD = 0.2f;
    public float digCD = 0.15f;
    private float CD = 1f;
    public Boolean CanBuild = true;

    public Vector2 offset = new Vector2(0, 13);
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();

    public ChunkRenderer ChunkPrefab;
    //public GameObject Cursor;
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
        //Teraingen.Generate();
        PPos = Player.transform.position;

        if (PPos.x < 0) PPos.x -= 1;
        if (PPos.y < 0) PPos.y -= 1;
        PlayerChunk = new Vector2Int((int)Math.Floor(PPos.x / ChunkRenderer.chunkWide), (int)Math.Floor(PPos.y / ChunkRenderer.chunkWide));
        foreach (KeyValuePair<Vector2Int, ChunkData> ReadCh in ChunkDatas)
        {
            Task.Factory.StartNew(() =>
            {
                int x = ReadCh.Key.x;
                int y = ReadCh.Key.y;
                var chunkData = new ChunkData();
                int xpos = x * ChunkRenderer.chunkWide;
                int ypos = y * ChunkRenderer.chunkWide;
                chunkData.Blocks = ChunkDatas[new Vector2Int(x, y)].Blocks;
                chunkData.BgBlocks = Teraingen.GenerateBG(xpos, ypos, seed);
            });
            

        }
        updateChunks(PlayerChunk);
        Player.transform.position = PPos;
        
    }

    public void create_world()
    {
        seed = (int)(Time.realtimeSinceStartup * 1000000 % 10000);
        Cam = Camera.main;

        gen_world();
        Player.transform.position = new Vector3(Player.transform.position.x, Teraingen.GetFirstAir((int)Math.Floor(Player.transform.position.x), seed), 0);
    }

    void Update()
    {
        if (ready)
        {
            PPos = Player.transform.position;
            if (PPos.x < 0) PPos.x-=1;
            if (PPos.y < 0) PPos.y-=1;
            PlayerChunk = new Vector2Int((int)Math.Floor(PPos.x / ChunkRenderer.chunkWide), (int)Math.Floor(PPos.y / ChunkRenderer.chunkWide));
            if (CurrentChunk != PlayerChunk)
            {
                CurrentChunk = PlayerChunk;
                updateChunks(PlayerChunk);

            }
            if (CanBuild) CheckInput();

            while (generatedResultsPrin.TryDequeue(out var chunkDat))
            {
                if (Mathf.Abs(chunkDat.coords.x - PlayerChunk.x) <= ChunkSpawnRad || Mathf.Abs(chunkDat.coords.y - PlayerChunk.y) <= ChunkSpawnRad)
                {
                    if (ChunkDatas[chunkDat.coords] == null) createChunkPrin(chunkDat);
                }
                
            }

            if (generatedResults.TryDequeue(out var chunkData))
            {
                if (Mathf.Abs(chunkData.coords.x - PlayerChunk.x) <= ChunkSpawnRad || Mathf.Abs(chunkData.coords.y - PlayerChunk.y) <= ChunkSpawnRad)
                {
                    createChunkPrin(chunkData);
                }
            }

            if (CD < 1) CD += Time.deltaTime;
        }
        
    }

    public void createChunkPrin(ChunkData chunkData)
    {
        ChunkDatas[new Vector2Int(chunkData.coords.x, chunkData.coords.y)] = chunkData;
        var chunk = Instantiate(ChunkPrefab, new Vector3(chunkData.coords.x * ChunkRenderer.chunkWide, chunkData.coords.y * ChunkRenderer.chunkWide, 0), Quaternion.identity, transform);
        chunk.ChunkData = chunkData;
        chunkData.Chunk = chunk;
        chunkData.seed = seed;
        chunk.pos = new Vector2Int(chunkData.coords.x, chunkData.coords.y);
        chunk.ParentWorld = this;
    }


    void CheckInput()
    {
       
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - PPos;


        Ray2D ray = new Ray2D(PPos, mouse);
        RaycastHit2D hitInfo = Physics2D.Raycast(PPos, mouse, 10, LayerMask.GetMask("FG"));
        Vector2 hitP = new Vector2(hitInfo.point.x - PPos.x, hitInfo.point.y - PPos.y);
        
        if (hitInfo != false)
        {
            //Cursor.SetActive(true);
            //Cursor.transform.position = Vector3Int.FloorToInt(hitInfo.point);
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
            //Cursor.SetActive(false);
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


    public void updateChunks(Vector2Int PChunk)
    {
        updtChunkPrin(PChunk.x, PChunk.y);
        /*for (int x = 0; x <= ChunkSpawnRad; x++)
        {
            for (int y = 0; y <= ChunkSpawnRad; y++)
            {
                if (x == 0 && y == 0) yield return null;
                updtChunk(x + PChunk.x, y + PChunk.y);
                updtChunk(x + PChunk.x, -y + PChunk.y);
                updtChunk(-x + PChunk.x, y + PChunk.y);
                updtChunk(-x + PChunk.x, -y + PChunk.y);

                yield return null;
            }
        }*/
        bool prin = true;
        for (int i = 3; i <= ChunkSpawnRad*2 + 1; i += 2)
        {
            if ((i - 1) / 2 > ChunkSpawnRad / 2) prin = false;
            int x = -(i - 1) / 2;
            int y = -x;
            for (int xi = 0; xi < i; xi++)
            {
                updtChunk(x + xi + PChunk.x, y + PChunk.y, prin);
            }
            
            x += i-1;
            for (int yi = 0; yi < i; yi++)
            {
                updtChunk(x + PChunk.x, y - yi + PChunk.y, prin);
            }
          
            y -= i-1;
            for (int xi = 0; xi < i; xi++)
            {
                updtChunk(x - xi + PChunk.x, y + PChunk.y, prin);
            }
            x -= i-1;
            for (int yi = 0; yi < i; yi++)
            {
                updtChunk(x + PChunk.x, y + yi + PChunk.y, prin);
            }

        }
    }

    void updtChunk(int x, int y, bool prin)
    {
        if (!ChunkDatas.ContainsKey(new Vector2Int(x, y)))
        {
            ChunkDatas.Add(new Vector2Int(x, y), null);
            Task.Factory.StartNew(() =>
            {
                
                var chunkData = new ChunkData();
                    int xpos = x * ChunkRenderer.chunkWide;
                    int ypos = y * ChunkRenderer.chunkWide;
                    chunkData.coords = new Vector2Int(x, y);
                    chunkData.Blocks = Teraingen.GenerateTaerrain(xpos, ypos, seed);
                    chunkData.BgBlocks = Teraingen.GenerateBG(xpos, ypos, seed);
                    if (prin) generatedResultsPrin.Enqueue(chunkData);
                    else generatedResults.Enqueue(chunkData);             
            });

            /*var chunkData = new ChunkData();
            int xpos = x * ChunkRenderer.chunkWide;
            int ypos = y * ChunkRenderer.chunkWide;
            chunkData.coords = new Vector2Int(x, y);
            chunkData.Blocks = Teraingen.GenerateTaerrain(xpos, ypos, seed);
            chunkData.BgBlocks = Teraingen.GenerateBG(xpos, ypos, seed);
            ChunkDatas.Add(new Vector2Int(x, y), chunkData);
            var chunk = Instantiate(ChunkPrefab, new Vector3(xpos, ypos, 0), Quaternion.identity, transform);
            chunk.ChunkData = chunkData;
            chunkData.Chunk = chunk;
            chunkData.seed = seed;
            chunk.pos = new Vector2Int(x, y);
            chunk.ParentWorld = this;*/


        }
        else if (ChunkDatas[new Vector2Int(x, y)] != null)
        {
            if (!ChunkDatas[new Vector2Int(x, y)].Chunk.gameObject.active) ChunkDatas[new Vector2Int(x, y)].Chunk.gameObject.SetActive(true);
        }
    }

    void updtChunkPrin(int x, int y)
    {
        if (!ChunkDatas.ContainsKey(new Vector2Int(x, y)) || ChunkDatas[new Vector2Int(x,y)] == null)
        {
            var chunkData = new ChunkData();
            int xpos = x * ChunkRenderer.chunkWide;
            int ypos = y * ChunkRenderer.chunkWide;
            chunkData.coords = new Vector2Int(x, y);
            chunkData.Blocks = Teraingen.GenerateTaerrain(xpos, ypos, seed);
            chunkData.BgBlocks = Teraingen.GenerateBG(xpos, ypos, seed);
            if (!ChunkDatas.ContainsKey(new Vector2Int(x, y))) ChunkDatas.Add(new Vector2Int(x, y), chunkData);
            else ChunkDatas[new Vector2Int(x, y)] = chunkData;
            var chunk = Instantiate(ChunkPrefab, new Vector3(xpos, ypos, 0), Quaternion.identity, transform);
            chunk.ChunkData = chunkData;
            chunkData.Chunk = chunk;
            chunkData.seed = seed;
            chunk.pos = new Vector2Int(x, y);
            chunk.ParentWorld = this;
        }
        else if (ChunkDatas[new Vector2Int(x, y)] != null)
        {
            if (!ChunkDatas[new Vector2Int(x, y)].Chunk.gameObject.active) ChunkDatas[new Vector2Int(x, y)].Chunk.gameObject.SetActive(true);
        }
    }

        /*public IEnumerator updtChunk(int x, int y)
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
        }*/

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
 