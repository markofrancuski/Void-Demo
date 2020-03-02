using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using System;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    [SerializeField] private List<GameObject> objects;
    private float tempX;
    private float tempY;

    [Header("Level Information")]
    #region Level Information
    public bool ToogleLevelInfo = true;
    [HidePropertyDrawer("ToogleLevelInfo", true)]
    public int maxMoves;
    [HidePropertyDrawer("ToogleLevelInfo", true)]
    public int heartsToCollect;
    [HidePropertyDrawer("ToogleLevelInfo", true)]
    [SerializeField] private int _gridSize;
    public int GridSize { get { return _gridSize; } }
    [HidePropertyDrawer("ToogleLevelInfo", true)]
    public GameObject timObject;
    [HidePropertyDrawer("ToogleLevelInfo", true)]
    public GameObject annieObject;
    [HidePropertyDrawer("ToogleLevelInfo", true)]
    public Vector2 timSP;
    [HidePropertyDrawer("ToogleLevelInfo", true)]
    public Vector2 annieSP;
    #endregion

    [Header("Debug Stuff")]
    #region Debug Stuff
    public bool ToogleDebugStuff = true;
    /*public float worldSpriteWidth;
    public float worldScreenHeight;
    public float worldScreenWidth;
    public Vector3 newScale;
    */
    [HidePropertyDrawer("ToogleDebugStuff", true)]
    public bool isTestLevel;
    [HidePropertyDrawer("ToogleDebugStuff", true)]
    public bool SpawnPlayers;
    [HidePropertyDrawer("ToogleDebugStuff", true)]
    public bool isDoubleLevel;
    #endregion

    [Header("Enemy Settings")]
    #region Enemy Stuff
    [SerializeField] private GameObject explosivePrefab;
    [SerializeField] private GameObject shootingPrefab;
    [SerializeField] private GameObject movingPrefab;
    public Vector2[] explosiveEnemyPosition;
    public Vector2[] movingEnemyPosition;

    [SerializeField] private ShooterInfo[] shooterInfo;
    #endregion

    #region Unity Functions

    private void Start()
    {      

        StartLevel();

        if (!isTestLevel && !isDoubleLevel)
        {
            Globals.Instance.levelGO = gameObject;
            LevelManager.Instance.HeartToCollect = heartsToCollect;
            LevelManager.Instance.MaxMoves = maxMoves;
            LevelManager.Instance.Players = 0;
        }

        int spawnIndex = 0;
        if (SpawnPlayers)
        {
            //Spawn Tim
            if (timObject != null)
            {
                //Gets the index for arrayList where to place the player 
                spawnIndex = ((int)timSP.x + ((int)timSP.y * _gridSize));
                if (timSP.y < 1) spawnIndex += (int)timSP.y;

                //Clones the player prefab
                GameObject playerGO = Instantiate(timObject);
                playerGO.transform.position = gameObject.transform.GetChild(spawnIndex).transform.position;
                LevelManager.Instance.Players++;
            }
            //Spawn Annie
            if (annieObject != null)
            {
                //Gets the index for arrayList where to place the player 
                spawnIndex = (int)( annieSP.y * _gridSize + annieSP.x);
                //Clones the player prefab
                GameObject playerGO = Instantiate(annieObject);
                playerGO.transform.position = gameObject.transform.GetChild(spawnIndex).transform.position;
                LevelManager.Instance.Players++;
            }
        }

        if (explosiveEnemyPosition.Length > 0) SpawnExplosive();
        if (movingEnemyPosition.Length > 0) SpawnMoving();
        if (shooterInfo.Length > 0) SpawnShooters();
    }

    /*private void OnDisable() 
  {       
      //TerrainManager.instance.spawnedLevels.Remove(gameObject);
      //LevelManager.OnResetLevelEventHandler -= ResetLevel;
  }*/


    private void OnDestroy()
    {
        LevelManager.ResetLevel -= StartLevel;
    }

    private void OnEnable()
    {
        //LevelManager.OnResetLevelEventHandler += ResetLevel;
        //StartLevel();
        LevelManager.ResetLevel += StartLevel;
        //LevelManager.Instance.maxMoves = maxMoves;
        //LevelManager.Instance.heartToCollect = heartsToCollect;
    }

    private void OnDisable()
    {
        
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
        TerrainManager.Instance.CurrentSpawned--;
    }
    #endregion

    /*private void FixedUpdate() 
    {
         if(TerrainManager.instance.isStarted) 
        {
            nextPos = transform.position + new Vector3(0, movePosY, 0);  
            Tween.Position(transform , transform.position, nextPos, speed, 0 );

            if(transform.position.y <= disablePoint) 
            {
                 gameObject.SetActive(false);
                 //TerrainManager.instance.spawnedLevels.Remove(gameObject);
                 //Destroy(gameObject);
            }    
        }
        
    }*/

    /// <summary>
    /// This method stores information about level. Gets called when creating prefabs in LevelEditor.
    /// </summary>
    /// <param name="newList"> List of grid spaces</param>
    /// <param name="hearts"> Number of Hearts to collect in this level </param>
    /// <param name="moves"> Number of Moves player can have </param>
    public void SetUpLevel(List<GameObject> newList, int hearts, int moves, int gridSize, GameObject[] prefabs, Vector2[] playerSP)
    {
        objects = newList;
        heartsToCollect = hearts;
        maxMoves = moves;
        _gridSize = gridSize;

        movingPrefab = prefabs[0];
        explosivePrefab = prefabs[1];
        shootingPrefab = prefabs[2];

        timObject = prefabs[3];
        if (prefabs.Length >= 5)
        {          
            annieObject = prefabs[4];
        }
        timSP = playerSP[0];
        annieSP = playerSP[1];

    }

    public void SetUpBossLevel(List<GameObject> objects, int gridSize, UnityAction callback)
    {
        this.objects = objects;
        _gridSize = gridSize;
        SpawnPlayers = false;
        SpawnGrids();
        callback();
    }

    private void SpawnGrids()
    {
        //Get the sprite width in world space units
        float worldSpriteWidth = objects[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        //Get the screen height & width in world space units
        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = (worldScreenHeight / Screen.height) * Screen.width;
        //Initialize new scale to the current scale
        Vector3 newScale = transform.localScale;

        //Divide screen width by sprite width, set to X axis scale
        newScale.x = worldScreenWidth / worldSpriteWidth * (1f / _gridSize); //0.2f; 
        newScale.y = worldScreenHeight / 1f * (1f / _gridSize);//0.2f;
        newScale.y = newScale.y - 0.4f; // Reduce the Height of the grid size by 0.4f
                                        //0.2f  = 5 grid space

        //Position of the starting grid cell
        float posX = 0;//_gridSize);// * 2);
        float posY = 0;///_gridSize;// * 2;

        float multiplyCoeficient = 0;
        //if (_gridSize > 1) multiplyCoeficient = _gridSize * 0.25f;
        switch (_gridSize)
        {
            case 2: multiplyCoeficient = 0.5f; break;
            case 3: multiplyCoeficient = 1; break;
            case 4: multiplyCoeficient = 1.5f; break;
            case 5: multiplyCoeficient = 2f; break;
            default:
                break;
        }

        posX = -(newScale.x * multiplyCoeficient);
        posY = newScale.y * multiplyCoeficient;

        tempX = posX;
        tempY = posY;

        int gridCell = _gridSize;
        //First Cells platform and pickable
        GameObject pickableGO = objects[0].transform.GetChild(0).transform.gameObject;
        GameObject platformGO = objects[0].transform.GetChild(1).transform.gameObject;
        float pickablePos = 0;
        switch (_gridSize)
        {
            case 2:
                pickablePos = -0.104f;
                break;
            case 3:
                pickablePos = -0.015f;
                break;
            case 4:
                pickablePos = 0.05f;
                break;
            case 5:
                pickablePos = 0.05f;
                break;

            default:
                break;
        }

        #region Same Size Code
        if (pickableGO.activeInHierarchy)
        {
            pickableGO.transform.parent = null;
        }

        if (platformGO.activeInHierarchy)
        {
            platformGO.transform.parent = null;
        }

        #endregion

        //spritePickable.Scale.x *= scale.y;
        //Scale Sprites 
        #region Scaling Sprites Code

        /*if(pickableGO.activeInHierarchy)
        {
            Vector3 pickableScale = pickableGO.transform.localScale;
            pickableScale.x *= (newScale.y/2);
            pickableScale.y *= (newScale.x/2);
            pickableGO.transform.localScale = pickableScale;
        }
        
        
        if(platformGO.activeInHierarchy)
        {
            Vector3 platformScale = platformGO.transform.localScale;
            //platformScale.x *= newScale.x;
            platformScale.y *= (newScale.y/2);
            platformGO.transform.localScale = platformScale;
        }
        */

        #endregion

        //Setting the first cells position and scale
        objects[0].transform.localScale = newScale;
        objects[0].transform.localPosition = new Vector3(tempX, tempY, 1); //localPosition

        #region Same Size Code

        if (pickableGO.activeInHierarchy)
        {
            pickableGO.transform.SetParent(objects[0].transform);
            pickableGO.transform.localPosition = new Vector3(0, pickablePos, 0);
        }

        if (platformGO.activeInHierarchy)
        {
            platformGO.transform.SetParent(objects[0].transform);
            platformGO.transform.localPosition = new Vector3(0, -0.2f, 0);
            Vector3 childScale = platformGO.transform.localScale;
            childScale.x = 0.75f;
            platformGO.transform.localScale = childScale; // Set the platform Scale.X = 1 (width of the parent). Scale.Y will be scaled based on the parent 
        }

        #endregion

        tempX += newScale.x;

        for (int i = 1; i < Mathf.Pow(_gridSize, 2); i++)
        {
            //Getting the childs of the Grid => Pickable, Platform
            pickableGO = objects[i].transform.GetChild(0).transform.gameObject;
            platformGO = objects[i].transform.GetChild(1).transform.gameObject;

            #region Same Size Code
            if (pickableGO.activeInHierarchy)
            {
                pickableGO.transform.parent = null;
            }

            if (platformGO.activeInHierarchy)
            {
                platformGO.transform.parent = null;
            }
            #endregion

            //Setting the position of a grid cell and scaling it even;
            objects[i].transform.localScale = newScale;
            objects[i].transform.localPosition = new Vector3(tempX, tempY, 1);  //localPosition

            #region Scaling Sprites Code
            // Scale Pickable for the half the scale parent
            /*if(pickableGO.activeInHierarchy)
            {
                Vector3 pickableScale = pickableGO.transform.localScale;
                pickableScale.x *= (newScale.y);
                pickableScale.y *= (newScale.x);
                pickableGO.transform.localScale = pickableScale;
            }
            */
            #endregion

            #region Same Size Code
            if (pickableGO.activeInHierarchy)
            {
                pickableGO.transform.SetParent(objects[i].transform);
                pickableGO.transform.localPosition = new Vector3(0, pickablePos, 0);
            }

            if (platformGO.activeInHierarchy)
            {
                platformGO.transform.SetParent(objects[i].transform);
                platformGO.transform.localPosition = new Vector3(0, -0.2f, 0);
                Vector3 childScale = platformGO.transform.localScale;
                childScale.x = 0.75f;
                platformGO.transform.localScale = childScale; // If the parent new scale.x is < 1f => Set the child to the witdh of the parent(x = 1); 
            }
            #endregion

            if (i + 1 == gridCell)
            {
                tempX = posX;
                tempY -= newScale.y;
                gridCell += _gridSize;
            }
            else
            {

                tempX += newScale.x;
            }
        }

        //Set up enemies
    }

    /// <summary>
    /// Calculates the device screen size, scales and positions sprites accordingly
    /// </summary>
    public void StartLevel()
    {

        //Get the sprite width in world space units
        float worldSpriteWidth = objects[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;

        //Get the screen height & width in world space units
        float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        float worldScreenWidth = (worldScreenHeight / Screen.height) * Screen.width;
        //Initialize new scale to the current scale
        Vector3 newScale = transform.localScale;

        //Divide screen width by sprite width, set to X axis scale
        newScale.x = worldScreenWidth / worldSpriteWidth * (1f / _gridSize); //0.2f; 
        newScale.y = worldScreenHeight / 1f * (1f / _gridSize);//0.2f;
        newScale.y = newScale.y - 0.4f; // Reduce the Height of the grid size by 0.4f
        //0.2f  = 5 grid space

        if (!isTestLevel)
        {
            Globals.Instance.movePaceHorizontal = newScale.x;
            Globals.Instance.movePaceVertical = newScale.y;

            Globals.Instance.upVector = new Vector3(0, newScale.y, 0);
            Globals.Instance.downVector = new Vector3(0, -newScale.y, 0);
            Globals.Instance.rightVector = new Vector3(newScale.x, 0, 0);
            Globals.Instance.leftVector = new Vector3(-newScale.x, 0, 0);

            //Set the boundary size
            //Globals.Instance.horizontalBoundary = (newScale.x * 5f) / 2;
            //Globals.Instance.verticalBoundary = Camera.main.orthographicSize;
        }

        //Position of the starting grid cell
        float posX = 0;//_gridSize);// * 2);
        float posY = 0;///_gridSize;// * 2;

        float multiplyCoeficient = 0;
        //if (_gridSize > 1) multiplyCoeficient = _gridSize * 0.25f;
        switch (_gridSize)
        {
            case 2: multiplyCoeficient = 0.5f; break;
            case 3: multiplyCoeficient = 1; break;
            case 4: multiplyCoeficient = 1.5f; break;
            case 5: multiplyCoeficient = 2f; break;
            default:
                break;
        }

        posX = -(newScale.x * multiplyCoeficient);
        posY = newScale.y * multiplyCoeficient;

        tempX = posX;
        tempY = posY;

        int gridCell = _gridSize;
        //First Cells platform and pickable
        GameObject pickableGO = objects[0].transform.GetChild(0).transform.gameObject;
        GameObject platformGO = objects[0].transform.GetChild(1).transform.gameObject;
        float pickablePos = 0;
        switch (_gridSize)
        {
            case 2:
                pickablePos = -0.104f;
                break;
            case 3:
                pickablePos = -0.015f;
                break;
            case 4:
                pickablePos = 0.05f;
                break;
            case 5:
                pickablePos = 0.05f;
                break;

            default:
                break;
        }

        #region Same Size Code
        if (pickableGO.activeInHierarchy)
        {
            pickableGO.transform.parent = null;
        }

        if (platformGO.activeInHierarchy)
        {
            platformGO.transform.parent = null;
        }

        #endregion

        //spritePickable.Scale.x *= scale.y;
        //Scale Sprites 
        #region Scaling Sprites Code

        /*if(pickableGO.activeInHierarchy)
        {
            Vector3 pickableScale = pickableGO.transform.localScale;
            pickableScale.x *= (newScale.y/2);
            pickableScale.y *= (newScale.x/2);
            pickableGO.transform.localScale = pickableScale;
        }
        
        
        if(platformGO.activeInHierarchy)
        {
            Vector3 platformScale = platformGO.transform.localScale;
            //platformScale.x *= newScale.x;
            platformScale.y *= (newScale.y/2);
            platformGO.transform.localScale = platformScale;
        }
        */

        #endregion

        //Setting the first cells position and scale
        objects[0].transform.localScale = newScale;
        objects[0].transform.localPosition = new Vector3(tempX, tempY, 1); //localPosition

        #region Same Size Code

        if (pickableGO.activeInHierarchy)
        {
            pickableGO.transform.SetParent(objects[0].transform);
            pickableGO.transform.localPosition = new Vector3(0, pickablePos, 0);
        }

        if (platformGO.activeInHierarchy)
        {
            platformGO.transform.SetParent(objects[0].transform);
            platformGO.transform.localPosition = new Vector3(0, -0.2f, 0);
            Vector3 childScale = platformGO.transform.localScale;
            childScale.x = 0.75f;
            platformGO.transform.localScale = childScale; // Set the platform Scale.X = 1 (width of the parent). Scale.Y will be scaled based on the parent 
        }

        #endregion

        tempX += newScale.x;

        for (int i = 1; i < Mathf.Pow(_gridSize, 2); i++)
        {
            //Getting the childs of the Grid => Pickable, Platform
            pickableGO = objects[i].transform.GetChild(0).transform.gameObject;
            platformGO = objects[i].transform.GetChild(1).transform.gameObject;

            #region Same Size Code
            if (pickableGO.activeInHierarchy)
            {
                pickableGO.transform.parent = null;
            }

            if (platformGO.activeInHierarchy)
            {
                platformGO.transform.parent = null;
            }
            #endregion

            //Setting the position of a grid cell and scaling it even;
            objects[i].transform.localScale = newScale;
            objects[i].transform.localPosition = new Vector3(tempX, tempY, 1);  //localPosition

            #region Scaling Sprites Code
            // Scale Pickable for the half the scale parent
            /*if(pickableGO.activeInHierarchy)
            {
                Vector3 pickableScale = pickableGO.transform.localScale;
                pickableScale.x *= (newScale.y);
                pickableScale.y *= (newScale.x);
                pickableGO.transform.localScale = pickableScale;
            }
            */
            #endregion

            #region Same Size Code
            if (pickableGO.activeInHierarchy)
            {
                pickableGO.transform.SetParent(objects[i].transform);
                pickableGO.transform.localPosition = new Vector3(0, pickablePos, 0);
            }

            if (platformGO.activeInHierarchy)
            {
                platformGO.transform.SetParent(objects[i].transform);
                platformGO.transform.localPosition = new Vector3(0, -0.2f, 0);
                Vector3 childScale = platformGO.transform.localScale;
                childScale.x = 0.75f;
                platformGO.transform.localScale = childScale; // If the parent new scale.x is < 1f => Set the child to the witdh of the parent(x = 1); 
            }
            #endregion

            if (i + 1 == gridCell)
            {
                tempX = posX;
                tempY -= newScale.y;
                gridCell += _gridSize;
            }
            else
            {

                tempX += newScale.x;
            }
        }
        //Set up enemies

    }

    void SpawnShooters()
    {
        print("Spawning shooters");
        for (int i = 0; i < shooterInfo.Length; i++)
        {
            GameObject go = Instantiate(shootingPrefab);
            shooterInfo[i].level = this;
            go.GetComponent<ShooterEnemy>().SetUpShooter(shooterInfo[i]);
        }
    }
  
    void SpawnExplosive()
    {
        for (int i = 0; i < explosiveEnemyPosition.Length; i++)
        {
            GameObject go = Instantiate(explosivePrefab, transform.GetChild(TransformPositionToIndex(explosiveEnemyPosition[i])).position, Quaternion.identity);

        }
    }

    void SpawnMoving()
    {
        for (int i = 0; i < movingEnemyPosition.Length; i++)
        {
            GameObject go = Instantiate(movingPrefab, transform.GetChild(TransformPositionToIndex(movingEnemyPosition[i])).position, Quaternion.identity);
        }
    }

    int TransformPositionToIndex(Vector2 position) { return (int) ((position.y* _gridSize) + position.x);}

    Vector3 TransformIndexToPosition(ShootDirection dir, int spawnPos)
    {
        Vector3 position = Vector3.zero;

        switch (dir)
        {
            case ShootDirection.UP:
                position.x = transform.GetChild(spawnPos).position.x;
                position.y = 5f;
                break;
            case ShootDirection.DOWN:
                position.x = transform.GetChild(spawnPos).position.x;
                position.y = -5f;
                break;
            case ShootDirection.RIGHT:
                position.y = transform.GetChild(spawnPos + _gridSize).position.x;
                position.x = 2.8f;
                break;
            case ShootDirection.LEFT:
                position.y = transform.GetChild(spawnPos).position.x;
                position.x = -2.8f;
                break;
            default:
                break;
        }
        return new Vector3(0, 0, 0);
    }
}

[Serializable]
public class ShooterInfo
{
    public Level level;
    public ShootDirection shootDirection;
    public float fireRate;
    [SerializeField] private int spawnPos;
    public float projectileSpeed = 150f;

    public Vector3 pos 
    {
        get { return TransformIndexToPosition(level, shootDirection, spawnPos); }
    }

    Vector3 TransformIndexToPosition(Level level ,ShootDirection dir, int spawnPos)
    {
        Vector3 position = Vector3.zero;

        switch (dir)
        {
            case ShootDirection.UP:
                position.x = level.gameObject.transform.GetChild(spawnPos).position.x;
                position.y = -5f;
                break;
            case ShootDirection.DOWN:
                position.x = level.gameObject.transform.GetChild(spawnPos).position.x;
                position.y = 5f;
                break;
            case ShootDirection.RIGHT:             
                if(spawnPos > 0) position.y = level.gameObject.transform.GetChild(spawnPos + level.GridSize).position.y;
                else position.y = level.gameObject.transform.GetChild(spawnPos).position.y;
                position.x = -2.8f;
                break;
            case ShootDirection.LEFT:
                if (spawnPos > 0) position.y = level.gameObject.transform.GetChild(spawnPos + level.GridSize).position.y;
                else position.y = level.gameObject.transform.GetChild(spawnPos).position.y;
                position.x = 2.8f;
                break;
            default:
                break;
        }

        return position;
    }
}