using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class WorldGen : MonoBehaviour
{
    //списки для работы с потоками
    private ConcurrentQueue <ChunkData> generatedResults = new ConcurrentQueue<ChunkData>();
    private ConcurrentQueue<ChunkData> generatedResultsForced = new ConcurrentQueue<ChunkData>();

    //настройки взаимодействия с миром

    [SerializeField]private float buildCD = 0.2f;
    [SerializeField]private float digCD = 0.15f;
    private float CD = 1f;
    public Boolean CanBuild = true;
    public GameObject Player;

    //параметры мира
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();

    public int ChunkSpawnRad = 3;
    public ChunkRenderer ChunkPrefab;
    public GameObject World;
    public int seed;
    public Teraingen Teraingen;

    public bool ready = false;

    //"локальные" параметры
    private ChunkData SpawnChunk;
    private ChunkData CheckChunk;

    public Vector2Int CurrentChunk;
    private Vector2Int PlayerChunk;

    private Camera Cam;
    private Vector3 PPos;

    private Vector3Int BlockWorldPos;
    private Vector3 BlockCenter;
    private Vector2Int UptdChunk;


    public void gen_world()//дозагрузка мира сгенерированного\загруженного другими скриптами
    {
        PPos = Player.transform.position;

        if (PPos.x < 0) PPos.x -= 1;
        if (PPos.y < 0) PPos.y -= 1;
        PlayerChunk = new Vector2Int((int)Math.Floor(PPos.x / ChunkRenderer.chunkWide), (int)Math.Floor(PPos.y / ChunkRenderer.chunkWide));
        foreach (KeyValuePair<Vector2Int, ChunkData> ReadCh in ChunkDatas)//прогрузка каждого отдельного чанка из загруженного массива
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
        Player.transform.position = new Vector3(Player.transform.position.x, Teraingen.GetFirstAir((int)Math.Floor(Player.transform.position.x), seed), 0);//перемещение игрока в первое возможное для спавна пространство
    }

    void Update()
    {
        if (ready)
        {
            PPos = Player.transform.position;
            PlayerChunk = new Vector2Int((int)Math.Floor(((PPos.x > 0) ? PPos.x : PPos.x - 1) / ChunkRenderer.chunkWide), (int)Math.Floor(((PPos.y > 0) ? PPos.y : PPos.y - 1) / ChunkRenderer.chunkWide));//нахождение координат чанка в котором находится игрок в массиве чанков
            if (CurrentChunk != PlayerChunk)
            {
                CurrentChunk = PlayerChunk;
                updateChunks(PlayerChunk);
            }

            if (CanBuild) CheckInput();

            while (generatedResultsForced.TryDequeue(out var chunkDat))//попытка достать с других потоков свежесгенерированные чанки(с большим приоритетом)
            {
                    if (ChunkDatas[chunkDat.coords] == null) createChunkForced(chunkDat);                   
            }

            for (int i = 0; i < 4;  i++) //попытка достать с других потоков свежесгенерированные чанки(с меньшим приоритетом)
            {
                if (generatedResults.TryDequeue(out var chunkData))
                {
                    if (Mathf.Abs(chunkData.coords.x - PlayerChunk.x) <= ChunkSpawnRad || Mathf.Abs(chunkData.coords.y - PlayerChunk.y) <= ChunkSpawnRad)
                    {
                        createChunkForced(chunkData);
                    }
                    else { i--; }
                }
            }
            if (CD < 1) CD += Time.deltaTime;
        }
        
    }

    public void createChunkForced(ChunkData chunkData)//инициализация отдельного чанка
    {
        ChunkDatas[new Vector2Int(chunkData.coords.x, chunkData.coords.y)] = chunkData;
        var chunk = Instantiate(ChunkPrefab, new Vector3(chunkData.coords.x * ChunkRenderer.chunkWide, chunkData.coords.y * ChunkRenderer.chunkWide, 0), Quaternion.identity, transform);
        chunk.name = (chunkData.coords.x + " " + chunkData.coords.y);
        chunk.ChunkData = chunkData;
        chunkData.Chunk = chunk;
    }


    void CheckInput()//проверка нажатий и попадания по блокам
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
        Vector2 PPos2 = new Vector2(PPos.x, PPos.y);
        Vector2 mouse2 = new Vector2(mouse.x, mouse.y);
        RaycastHit2D hitInfo = Physics2D.Raycast(PPos2, mouse2, 10, LayerMask.GetMask("FG"));
        Vector2 hitP = new Vector2(hitInfo.point.x, hitInfo.point.y);


        if (hitInfo != false)//проверка попадания в блок
        {

            if (Input.GetButton("Fire1") && CD > digCD)//вскапывания области (2*chunkRenderer.size Х 2*chunkRenderer.size)
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
            else if (Input.GetButton("Fire2") && CD > buildCD && Mathf.Abs(hitInfo.point.x) > 0.5 && Mathf.Abs(hitInfo.point.y) > 0.5)//постройка блоками в области (2*chunkRenderer.size Х 2*chunkRenderer.size)
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
            Debug.DrawRay(PPos, mouse, Color.red);
        }
    }

    public void SetBlock(BlockType bt, Vector3Int BlockWorldPos)
    {
        if (BlockWorldPos.x < 0) BlockWorldPos.x -= ChunkRenderer.chunkWide - 1;
        if (BlockWorldPos.y < 0) BlockWorldPos.y -= ChunkRenderer.chunkWide - 1;

        UptdChunk = new Vector2Int(BlockWorldPos.x / ChunkRenderer.chunkWide, BlockWorldPos.y / ChunkRenderer.chunkWide);//координаты чанка в массиве

        if (BlockWorldPos.x < 0) BlockWorldPos.x--;
        if (BlockWorldPos.y < 0) BlockWorldPos.y--;
        Vector2Int BlockChunkPos = new Vector2Int(mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide));//координаты блока внутри чанка
        ChunkDatas[UptdChunk].Chunk.SetBlock(new Vector3Int(BlockChunkPos.x, BlockChunkPos.y, 0), bt);//установка блока в чанке
        ChunkDatas[UptdChunk].Blocks[BlockChunkPos.x, BlockChunkPos.y] = bt;//задание блока в чанке
    }

    public void DeleteBlock(Vector3Int BlockWorldPos)
    {
        if (BlockWorldPos.x < 0) BlockWorldPos.x -= ChunkRenderer.chunkWide - 1;
        if (BlockWorldPos.y < 0) BlockWorldPos.y -= ChunkRenderer.chunkWide - 1;

        UptdChunk = new Vector2Int(BlockWorldPos.x / ChunkRenderer.chunkWide, BlockWorldPos.y / ChunkRenderer.chunkWide);//координаты чанка в массиве

        if (BlockWorldPos.x < 0) BlockWorldPos.x--;
        if (BlockWorldPos.y < 0) BlockWorldPos.y--;
        Vector2Int BlockChunkPos = new Vector2Int(mod(BlockWorldPos.x, ChunkRenderer.chunkWide), mod(BlockWorldPos.y, ChunkRenderer.chunkWide));//координаты блока внутри чанка
        ChunkDatas[UptdChunk].Chunk.DeleteBlock(new Vector3Int(BlockChunkPos.x, BlockChunkPos.y, 0));
        ChunkDatas[UptdChunk].Blocks[BlockChunkPos.x, BlockChunkPos.y] = BlockType.bgAir;//затирание данных о блоке в чанке
    }

    public void updateChunks(Vector2Int PChunk)//обновление чанков по спирали в области  ChunkSpawnRad*2 + 1
    {
        updtChunk(PChunk.x, PChunk.y, true);
        bool forced = true;
        for (int i = 3; i <= ChunkSpawnRad*2 + 1; i += 2)
        {
            if ((i - 1) / 2 > ChunkSpawnRad / 2) forced = false;
            int x = -(i - 1) / 2;
            int y = -x;
            for (int xi = 0; xi < i; xi++)
            {
                updtChunk(x + xi + PChunk.x, y + PChunk.y, forced);
            }
            
            x += i-1;
            for (int yi = 0; yi < i; yi++)
            {
                updtChunk(x + PChunk.x, y - yi + PChunk.y, forced);
            }
          
            y -= i-1;
            for (int xi = 0; xi < i; xi++)
            {
                updtChunk(x - xi + PChunk.x, y + PChunk.y, forced);
            }
            x -= i-1;
            for (int yi = 0; yi < i; yi++)
            {
                updtChunk(x + PChunk.x, y + yi + PChunk.y, forced);
            }
        }
    }

    void updtChunk(int x, int y, bool forced)//включение чанка из неактивного положения\генерация нового при отсутствии
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
                    chunkData.Blocks = Teraingen.GenerateTerrain(xpos, ypos, seed);
                    chunkData.BgBlocks = Teraingen.GenerateBG(xpos, ypos, seed);
                    chunkData.seed = seed;
                    chunkData.ParentWorld = this;
                    if (forced) generatedResultsForced.Enqueue(chunkData);
                    else generatedResults.Enqueue(chunkData);             
            });

        }
        else if (ChunkDatas[new Vector2Int(x, y)] != null)
        {
            if (!ChunkDatas[new Vector2Int(x, y)].Chunk.gameObject.activeSelf) ChunkDatas[new Vector2Int(x, y)].Chunk.gameObject.SetActive(true);
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
        
    public void destroy()
    {
        foreach (KeyValuePair<Vector2Int, ChunkData> ReadCh in ChunkDatas)
        {
            Destroy(ReadCh.Value.Chunk);
        }
    }
}
 