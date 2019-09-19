using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    //[SerializeField]
    //int generatedHeight;

    [SerializeField, Range(5, 15)]
    float distanceBetweenWalls;

    [SerializeField, Range(2f, 5f)]
    float distanceBetweenSpikes;

    //[SerializeField, Range(1, 100)]
    //int obstacleDensity;

    [SerializeField, Range(1, 100)]
    int platformDensity;

    [SerializeField]
    Transform playerTransform;

    [SerializeField]
    GameObject wallPrefab;

    [SerializeField]
    GameObject spikePrefab;

    [SerializeField]
    GameObject floatyPrefab;

    [SerializeField]
    GameObject platformPrefab;

    [SerializeField]
    GameObject bacteriaPrefab;  

    [SerializeField]
    GameObject tapeWormPrefab;

    [SerializeField]
    GameObject menu;

    public static LevelGeneration ActiveLevel;

    // half the height of object (calculated from SpriteRenderer atm)
    float verticalExtentWall;
    float verticalExtentSpikes;

    // offset between tiles
    Vector3 verticalOffset;

    // positions where to spawn next walls
    Vector3 posLeft;
    Vector3 posRight;

    // how many different spikes can a wall have, given the distance between them
    int possibleObstacles;

    // running counter
    int curTileNr;

    bool[] obstacleSpawnableLeft; // unused
    bool[] obstacleSpawnableRight; // unused
    bool[] platformSpawnable;

    [SerializeField, Range(0, 2)]
    int level;

    int levelsCleared = 0;

    [SerializeField, Range(10, 50)]
    int[] minTiles;

    [SerializeField, Range(0, 100)]
    int[] obstacleDensity;

    Queue<GameObject> tiles;

    GameObject levelParent;

    bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        tiles = new Queue<GameObject>();
    }

    private void Init()
    {
        ControllerScript.Instance.ResetLevels(); // delete all other levels

        ActiveLevel = this;

        levelParent = new GameObject("Level " + (level + 1));
        levelParent.transform.SetPositionAndRotation(transform.position, Quaternion.identity);

        Vector3 start = transform.position;

        verticalExtentWall = wallPrefab.GetComponent<SpriteRenderer>().bounds.extents.y;
        verticalOffset = new Vector3(0, verticalExtentWall * 2f);
        Debug.Log("Vertical offset: " + verticalOffset);

        posLeft = start + new Vector3(-distanceBetweenWalls / 2f, verticalExtentWall);
        posRight = start + new Vector3(distanceBetweenWalls / 2f, verticalExtentWall);

        possibleObstacles = Mathf.FloorToInt(verticalExtentWall * 2f / distanceBetweenSpikes);
        Debug.Log("Possible obstacles: " + possibleObstacles);
        verticalExtentSpikes = spikePrefab.GetComponentInChildren<SpriteRenderer>().bounds.extents.y;

        curTileNr = 0;

        obstacleSpawnableLeft = new bool[possibleObstacles];
        obstacleSpawnableRight = new bool[possibleObstacles];
        platformSpawnable = new bool[possibleObstacles];

        // initialize everything with true;
        for (int i = 0; i < possibleObstacles; i++)
        {
            obstacleSpawnableLeft[i] = true;
            obstacleSpawnableRight[i] = true;
            platformSpawnable[i] = true;
        }

        // spawn 3 Tiles in advance
        SpawnNewTile();
        SpawnNewTile();
        SpawnNewTile();

        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ControllerScript.Instance.GameOver)
            return;

        if (!IsActive())
            return;

        if(!initialized)
        {
            Init();
        } 

        Vector3 lowestTilePos = tiles.Peek().transform.position;
        //Debug.Log("Position: " + lowestTilePos);

        if (playerTransform.position.y > lowestTilePos.y + verticalExtentWall * 4 &&
            Mathf.Abs(playerTransform.position.x - lowestTilePos.x) < distanceBetweenWalls)
        {
            GameObject lowestTile = tiles.Dequeue();
            Destroy(lowestTile);          
            SpawnNewTile();
        }
    }

    void SpawnNewTile()
    {           
        // used keep track of platforms that reach into new tile
        bool platformSpawnableOnNextTile0 = true;
        bool platformSpawnableOnNextTile1 = true;

        // create parent           
        var parent = new GameObject("Tile " + curTileNr);
        parent.transform.SetPositionAndRotation(posLeft - new Vector3(0, verticalExtentWall, 0), Quaternion.identity);
        parent.transform.SetParent(levelParent.transform);
        tiles.Enqueue(parent);

        if(curTileNr >= minTiles[level + levelsCleared])
        {
            curTileNr -= minTiles[level + levelsCleared];
            levelsCleared++;
            Debug.Log("Level Cleared!");

            ControllerScript.Instance.LevelUnlocked(level + levelsCleared);

            if(level + levelsCleared > 2)
            {
                Debug.Log("You beat the game!");
                levelsCleared = 2 - level;
            }
        }

        // create walls
        var left = Instantiate(wallPrefab, posLeft, Quaternion.identity, parent.transform);
        var right = Instantiate(wallPrefab, posRight, Quaternion.Euler(0, 180, 0), parent.transform);
      
        for (int j = 0; j < possibleObstacles; j++)
        {
            if (curTileNr == 0 && j < 3) // Player spawn
                continue;

            bool spawn = Random.Range(0, 100) < obstacleDensity[level + levelsCleared];

            if (spawn)
            {
                bool spawnLeft = Random.Range(0, 2) == 0;

                GameObject spawnedObject = null;
                Vector3 spikePosLeft = GetSpikePosition(posLeft, j);

                GameObject prefabToSpawn = null;

                // spawn spike/floaty 
                if (Random.Range(0, 4) > 0)
                {
                    if (Random.Range(0, 2) > 0) // spawn normal spike
                    {
                        prefabToSpawn = spikePrefab;
                    }
                    else // spwan floaty
                    {
                        prefabToSpawn = floatyPrefab;
                    }
                }
                else // spawn enemy
                {
                    switch(level + levelsCleared)
                    {
                        case 0:
                            prefabToSpawn = spikePrefab;
                            break;

                        case 1:
                            prefabToSpawn = bacteriaPrefab;
                            break;

                        case 2:
                            if(Random.Range(0, 2) > 0)
                            {
                                prefabToSpawn = bacteriaPrefab;
                            }
                            else
                            {
                                prefabToSpawn = tapeWormPrefab;
                            }
                            break;
                    }
                }

                /*
                int obstacleID = Random.Range(0, level + 2);

                switch(obstacleID)
                {
                    // wall spike
                    case 0:
                        prefabToSpawn = spikePrefab;
                        break;

                    // floaty
                    case 1:
                        prefabToSpawn = floatyPrefab;
                        break;
                    
                    // bacteria
                    case 2:
                        prefabToSpawn = bacteriaPrefab;
                        break;

                    // tapeworm
                    case 3:
                        prefabToSpawn = tapeWormPrefab;
                        break;

                    default:
                        Debug.LogError("Unkown obstacle ID!");
                        prefabToSpawn = spikePrefab;
                        break;
                }
                */

                /*
                bool spawnBacteria = Random.Range(0, 7) == 0;

                if(spawnBacteria)
                {
                    prefabToSpawn = bacteriaPrefab;
                    bool spawnFloaty = Random.Range(0, 2) == 0;

                    if(spawnFloaty)
                    {
                        prefabToSpawn = floatyPrefab;

                        bool spawnTapeworm = Random.Range(0, 2) == 0;

                        if(spawnTapeworm)
                        {
                            prefabToSpawn = tapeWormPrefab;
                        }
                    }
                }
                */

                

                if (spawnLeft)
                {
                    spawnedObject = Instantiate(prefabToSpawn, spikePosLeft, Quaternion.identity, parent.transform);                   
                }
                else
                {
                    spawnedObject = Instantiate(prefabToSpawn, GetSpikePosition(posRight, j), Quaternion.Euler(0, 180f, 0), parent.transform);
                }

                bool spawnPlatform = false;

                if (platformSpawnable[j])
                {
                    spawnPlatform = Random.Range(0, 100) < platformDensity;
                }

                if (spawnPlatform)
                {
                    var platform = Instantiate(platformPrefab, new Vector3(spikePosLeft.x + distanceBetweenWalls / 2, spikePosLeft.y, 0f), Quaternion.identity, parent.transform);
                    platformSpawnable[j] = false;

                    if (j + 1 < possibleObstacles)
                        platformSpawnable[j + 1] = false;
                    else
                        platformSpawnableOnNextTile0 = false;

                    if (j + 2 < possibleObstacles)
                        platformSpawnable[j + 2] = false;
                    else
                        platformSpawnableOnNextTile1 = false;

                }
            }
        }

        posLeft += verticalOffset;
        posRight += verticalOffset;
        curTileNr++;

        // reinitialize everything with true;
        for (int i = 0; i < possibleObstacles; i++)
        {
            obstacleSpawnableLeft[i] = true;
            obstacleSpawnableRight[i] = true;
            platformSpawnable[i] = true;
        }

        // block spawn areas on next tile if platforms from this tile reach into it
        if (!platformSpawnableOnNextTile0)
            platformSpawnable[0] = false;

        if(!platformSpawnableOnNextTile1)
            platformSpawnable[1] = false;
    }

    /// <summary>
    /// returns random element of an array
    /// </summary>
    /// <typeparam name="T">Type of elements in array</typeparam>
    /// <param name="list">list to take from</param>
    /// <returns></returns>
    public static T TakeRandomElement<T>(T[] list)
    {
        return list[Random.Range(0, list.Length)];
    }

    /// <summary>
    /// returns position of spikes dependent on heightIndex
    /// </summary>
    /// <param name="wallPos">Position of wall to spawn on</param>
    /// <param name="heightIndex">Index of spike position (from bottom, 
    /// needs to be smaller than max. spikeCount!)</param>
    /// <returns></returns>
    Vector3 GetSpikePosition(Vector3 wallPos, int heightIndex)
    {
        if(heightIndex >= possibleObstacles)
        {
            throw new System.ArgumentOutOfRangeException("heightIndex", "Index needs to be smaller than max. spike count!");
        }

        return wallPos + new Vector3(0, -verticalExtentWall + heightIndex * distanceBetweenSpikes + distanceBetweenSpikes / 2f);
    }

    public void ResetCompletely()
    {       
        Destroy(levelParent);
        tiles.Clear();
        initialized = false;
        levelsCleared = 0;
    }

    public bool IsActive()
    {
        return Mathf.Abs(playerTransform.position.x - transform.position.x) < distanceBetweenWalls;
    }

    public void RepositionMenu()
    {
        menu.transform.position = new Vector3(transform.position.x, tiles.Peek().transform.position.y);
    }
}
