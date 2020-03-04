using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelEditor : EditorWindow 
{
    #region Variables
    public int levelNumber; 
    private int gridSize;
    //private string saveLevelPath = "/Prefabs/Level/";
    private string saveLevelPath = "/";
    private string levelName = "Level";
    private bool isInEditMode;
    private bool isHideGrid = false;

    private GameObject parentObject;
    private GameObject testObject;

    private int numberOfHearts = 0;
    private int numberOfMoves;

    private Vector2 timSP;
    private Vector2 annieSP;
    private bool spawnTim;
    private bool spawnAnnie;

    //Boss Level Stuff
    private bool isBossLevel;
    private ShootDirection platformLocation;

    #region Enemies
    //Moving Enemy
    private bool _spawnMoving;
    //Values
    private Vector2 _movingEnemySP;
    private ShootDirection _movingEnemyPath;
    private List<ShootDirection> _movingEnemyPaths= new List<ShootDirection>();
    private List<MovingEnemyInfo> _movingEnemyInfo = new List<MovingEnemyInfo>();

    //Shooter Enemy
    private bool _spawnShooting;
    //Values
    private int _shooterPosition;
    private ShootDirection _shooterDirection;
    private float _shooterFireRate;
    private float _shooterProjectileSpeed;

    [SerializeField] private List<ShooterInfo> _shooterEnemyInfos = new List<ShooterInfo>();
    
    //Exploading Enemy
    private bool _spawnExploding;
    //Values
    [SerializeField] private List<Vector2> _exploadingEnemyInfos = new List<Vector2>();
    private Vector2Int _exploadingEnemyInfo = Vector2Int.zero;
    #endregion

    #endregion

    #region Resources
    [Header("Sprites")]
    private Sprite platformNormalSprite;
    private Sprite platformBreakableSprite;
    private Sprite platformHaySprite;
    private Sprite platformSpikeSprite;
    private Sprite platformSlideLeftSprite;
    private Sprite platformSlideRightSprite;
    private Sprite platformSlimeSprite;
    private Sprite platformDoubleSprite;
    private Sprite platformPortalSprite;

    //Pickable Items
    private Sprite coinSprite;
    private Sprite heartSprite;
    private Sprite weaponSprite;
    private Sprite plusThreeMovesSprite;
    private Sprite plusFiveMovesSprite;
    private Sprite reverseMovesSprite;

    [Header("Textures")]
    private Texture platformNormalTexture;
    private Texture platformBreakableTexture;
    private Texture platformHayTexture;
    private Texture platformSpikeTexture;
    private Texture platformSlideLeftTexture;
    private Texture platformSlideRightTexture;
    private Texture platformSlimeTexture;
    private Texture platformDoubleTexture;
    private Texture platformPortalTexture;

    //Pickable Items
    private Texture coinTexture;
    private Texture heartTexture;
    private Texture weaponTexture;
    private Texture plusThreeMovesTexture;
    private Texture plusFiveMovesTexture;
    private Texture reverseMovesTexture;

    private Texture defaultTexture;

    //Prefabs
    private GameObject timPrefab;
    private GameObject anniePrefab;

    //Enemy prefabs
    private GameObject movingEnemyPrefab;
    private GameObject explodingEnemyPrefab;
    private GameObject shootingEnemyPrefab;

    //Particle prefabs
    private GameObject heartPickUpEffect;
    private GameObject plusThreePickUpEffect;
    private GameObject plusFivePickUpEffect;
    private GameObject hayLandingEffect;
    #endregion

    #region Drop Down Variables
    private PlatformType selectedPlatformType;
    private PickableType selectedPickableType;
    #endregion

    private Dictionary<int, PlatformInfo> dictionary = new Dictionary<int, PlatformInfo>();

    private void Awake() 
    {
        
        Debug.Log("Loading Assets");
        
        //Debug.LogWarning("LevelEditor.cs Add the number of the level => load all levels from folder and +1 the value so when create new level it continues the level number for prefabs");
        #region Loading Assets
        
        testObject = Resources.Load<GameObject>("Prefabs/Grid Cell");
        timPrefab = Resources.Load<GameObject>("Prefabs/Characters/Tim");
        anniePrefab = Resources.Load<GameObject>("Prefabs/Characters/Annie");
        //Sprites
        platformNormalSprite = Resources.Load<Sprite>("Sprites/Platforms/NormalPlatform");
        platformBreakableSprite = Resources.Load<Sprite>("Sprites/Platforms/BreakablePlatform");
        platformHaySprite = Resources.Load<Sprite>("Sprites/Platforms/HayPlatform");
        platformSpikeSprite = Resources.Load<Sprite>("Sprites/Platforms/SpikePlatform");
        platformSlideLeftSprite = Resources.Load<Sprite>("Sprites/Platforms/SlideLeftPlatform");
        platformSlideRightSprite = Resources.Load<Sprite>("Sprites/Platforms/SlideRightPlatform");
        platformSlimeSprite = Resources.Load<Sprite>("Sprites/Platforms/SlimePlatform");
        platformPortalSprite = Resources.Load<Sprite>("Sprites/Platforms/PortalPlatform");

        coinSprite = Resources.Load<Sprite>("Sprites/Pickables/Coin");
        heartSprite = Resources.Load<Sprite>("Sprites/Pickables/Heart"); //Heart 1
        weaponSprite = Resources.Load<Sprite>("Sprites/Pickables/RandomBox");
        plusFiveMovesSprite = Resources.Load<Sprite>("Sprites/Pickables/plus5");
        plusThreeMovesSprite = Resources.Load<Sprite>("Sprites/Pickables/plus3");
        reverseMovesSprite = Resources.Load<Sprite>("Sprites/Pickables/reverseMove");

        //Texture
        platformNormalTexture = Resources.Load<Texture>("Sprites/Platforms/NormalPlatform");
        platformBreakableTexture = Resources.Load<Texture>("Sprites/Platforms/BreakablePlatform");
        platformHayTexture = Resources.Load<Texture>("Sprites/Platforms/HayPlatform");
        platformSpikeTexture = Resources.Load<Texture>("Sprites/Platforms/SpikePlatform");
        platformSlideLeftTexture = Resources.Load<Texture>("Sprites/Platforms/SlideLeftPlatform");
        platformSlideRightTexture = Resources.Load<Texture>("Sprites/Platforms/SlideRightPlatform");
        platformSlimeTexture = Resources.Load<Texture>("Sprites/Platforms/SlimePlatform");
        platformPortalTexture = Resources.Load<Texture>("Sprites/Platforms/PortalPlatform");
        platformDoubleTexture = Resources.Load<Texture>("Sprites/Platforms/DoublePlatform");

        coinTexture = Resources.Load<Texture>("Sprites/Pickables/Coin");
        heartTexture = Resources.Load<Texture>("Sprites/Pickables/Heart 1");
        weaponTexture = Resources.Load<Texture>("Sprites/Pickables/RandomBox");
        plusFiveMovesTexture = Resources.Load<Texture>("Sprites/Pickables/plus5");
        plusThreeMovesTexture = Resources.Load<Texture>("Sprites/Pickables/plus3");
        reverseMovesTexture = Resources.Load<Texture>("Sprites/Pickables/reverseMove");

        defaultTexture = Resources.Load<Texture>("Sprites/White1x1");

        movingEnemyPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Moving Enemy");
        explodingEnemyPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Explosive Enemy");
        shootingEnemyPrefab = Resources.Load<GameObject>("Prefabs/Enemies/Shooter");


        heartPickUpEffect = Resources.Load<GameObject>("Particles/Pickable/Heart Burst");
        plusThreePickUpEffect = Resources.Load<GameObject>("Particles/Pickable/Plus3Moves Burst");
        plusFivePickUpEffect = Resources.Load<GameObject>("Particles/Pickable/Plus5Moves Burst");
        hayLandingEffect = Resources.Load<GameObject>("Particles/Hay Landing Particle");
        #endregion

        #region Default values
        gridSize = 5;

        levelNumber = Directory.GetFiles(@"Assets/Prefabs/Level", "*.prefab").Length + 1;
        selectedPlatformType = PlatformType.NONE;
        selectedPickableType = PickableType.NONE;
        numberOfHearts = 0;
        numberOfMoves = 0;
        spawnTim = true;
        #endregion

    }

    [MenuItem("Window/Custom Windows/LevelEditor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Generator");
    }

    Vector2 scrollPosition = new Vector2(750, 750);

    [SerializeField] private GameObject levelToLoad;
    private void OnGUI() 
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, true, true, GUILayout.Width(650), GUILayout.Height(900));
    
        #region Input Fields
        gridSize = EditorGUILayout.IntField("Enter Grid Size" ,gridSize);
        //saveLevelPath = EditorGUILayout.TextField(new GUIContent("Enter Path to Save", "Do not add Assets infront its already added when saving") ,saveLevelPath);
        levelName = EditorGUILayout.TextField("Enter the Prefab name", levelName);

        selectedPlatformType = (PlatformType) EditorGUILayout.EnumPopup("Select Platform Type ", selectedPlatformType);
        selectedPickableType = (PickableType) EditorGUILayout.EnumPopup("Select Pickable Type ", selectedPickableType);
        CheckHearts();
        GUILayout.Label($"Hearts To Collect: { numberOfHearts.ToString() }");

        isBossLevel = EditorGUILayout.Toggle(new GUIContent("Boss Level?", "Is Boss level or not, based on this toogle level will be setup with different settings."), isBossLevel);
        if (!isBossLevel)
        {
            EditorGUILayout.Space();
            numberOfMoves = EditorGUILayout.IntField("Enter number of moves", numberOfMoves);

            spawnTim = EditorGUILayout.Toggle("Spawn Tim?", spawnTim);
            timSP = EditorGUILayout.Vector2Field("Tim Spawn Position", timSP);

            spawnAnnie = EditorGUILayout.Toggle("Spawn Annie?", spawnAnnie);
            annieSP = EditorGUILayout.Vector2Field("Annie Spawn Position", annieSP);

            EditorGUILayout.Space();
        }
        else
        {
            platformLocation = (ShootDirection) EditorGUILayout.EnumPopup(new GUIContent("Platform location direction", "Make sure to set this field. Down is when boss is spawned up on screen so that means platform will be down."), platformLocation);
        }

        levelToLoad = EditorGUILayout.ObjectField("Choose Prefab Level to Edit", levelToLoad, typeof(GameObject), false) as GameObject;
        #endregion

        #region Buttons
        GUILayout.BeginHorizontal();

        GUI.color = Color.cyan;
        if(GUILayout.Button("Load Level to Edit")) TestLoadPrefab("Level 1");

        GUI.color = Color.yellow;
        //Spawn Level Button
        if (GUILayout.Button("Spawn Level"))
        {
            if(!isBossLevel && ( numberOfHearts <= 0 || numberOfMoves <= 0))
            {
                Debug.LogError("LEVEL EDITOR- Cannot make level without atleast 1 Heart/Move!");
                return;
            }
            if (dictionary.Count <= 0)
            {
                Debug.LogError("LEVEL EDITOR- Cannot make empty level, place atleast one platform!");
                return;
            }
            saveLevelPath = EditorUtility.GetAssetPath(Selection.activeObject) + "/";
            if (saveLevelPath == "/")
            {
                Debug.LogError("LEVEL EDITOR-You didn't select folder where you want to save prefab in project window!");
                return;
            }
            SpawnLevel();
        }

        GUI.color = Color.red;
        //Print the dictionary => What fields we filled up in a grid
        if(GUILayout.Button("Debug"))
        {
            /*for (int i = 0; i < _exploadingEnemyInfos.Count; i++)
            {
                Debug.Log(_exploadingEnemyInfos[i]);
            }*/

            for (int i = 0; i < _movingEnemyInfo.Count; i++)
            {
                Debug.Log(_movingEnemyInfo[i]);
            }
            /*foreach (KeyValuePair<int, PlatformInfo> kvp in dictionary)
            {
                Debug.Log(kvp.Value.ToString());
            }*/
        }
        
        GUI.color = Color.green;
        //Clears out the dictionary
        if(GUILayout.Button("Reset Grid")) dictionary.Clear();
        GUI.color = Color.white;

        GUILayout.EndHorizontal();
        #endregion

        //Creates the grid
        isHideGrid = EditorGUILayout.Toggle(new GUIContent("Hide Grid", "Hide/Show the grid."), isHideGrid);
        if(!isHideGrid) CreateGrid(gridSize, 25, 25, 50, 400, "Level Grid", 25);
        else
        {
            //Display enemy info for editing 
            DisplayEnemyInfo();
        }

        GUILayout.EndScrollView();
    }

    private void CreateGrid(int gridSize, float buttonSize, float spaceBetween, float posX, float posY, string boxName, float boxOffset)
    {
        float initialPosY = posY;
        float initialPosX = posX;
        float boxSize2 = 10+ buttonSize*3 ; // *2

        float boxSize = gridSize * boxSize2 + (gridSize - 1) * spaceBetween/2;

        int buttonIndex = 1;
        GUI.Box(new Rect(posX - boxOffset, posY - boxOffset, boxSize + boxOffset*2, boxSize + boxOffset*2), boxName);

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GUI.Box(new Rect(initialPosX, initialPosY , boxSize2, boxSize2), "Platform " + buttonIndex);

                //Pickable Button
                if(dictionary.ContainsKey(buttonIndex))
                {
                    if (GUI.Button(new Rect(initialPosX, initialPosY + boxSize2 / 2 - 20, buttonSize*3, buttonSize), GetPickableTexture(dictionary[buttonIndex].GetPickableType())))
                    {
                        if (selectedPickableType == PickableType.NONE && dictionary[buttonIndex].GetPlatformType() == PlatformType.NONE)  dictionary.Remove(buttonIndex);
                        else dictionary[buttonIndex].ChangePickable(selectedPickableType);
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(initialPosX, initialPosY + boxSize2 / 2 - 20, buttonSize*3, buttonSize), GetDefaultTexture()))
                        dictionary[buttonIndex] = new PlatformInfo(i, j, PlatformType.NONE, selectedPickableType);                 
                }

                //Platform Button
                if(dictionary.ContainsKey(buttonIndex))
                {
                    if(GUI.Button(new Rect(initialPosX + boxSize2 / 2 - 41, initialPosY + boxSize2 / 2 + 5, buttonSize * 3, buttonSize), GetPlatformTexture(dictionary[buttonIndex].GetPlatformType()) ))
                    {

                        if (selectedPlatformType == PlatformType.NONE && dictionary[buttonIndex].GetPickableType() == PickableType.NONE) dictionary.Remove(buttonIndex);
                        else dictionary[buttonIndex].ChangePlatform(selectedPlatformType);
                    }
                }
                else
                {                  
                    if (GUI.Button(new Rect(initialPosX + boxSize2 / 2 - 41, initialPosY + boxSize2 / 2 + 5, buttonSize * 3, buttonSize), GetDefaultTexture()))
                        dictionary[buttonIndex] = new PlatformInfo(i, j, selectedPlatformType, PickableType.NONE);
                }

                initialPosX += buttonSize + spaceBetween + boxSize2 / 2;
                buttonIndex++;
            }
            initialPosY += buttonSize + spaceBetween + boxSize2 / 2 ;
            initialPosX = posX;
        }

        GUI.color = Color.white;
    }

    private void DisplayEnemyInfo()
    {
        GUILayout.Box("Moving Enemy Panel");
        _spawnMoving = EditorGUILayout.Toggle(new GUIContent("Spawn Moving?", "Enable/Disable spawning moving enemies"), _spawnMoving);
        if(_spawnMoving)
        {

            EditorGUILayout.LabelField($"Moving Enemies Added:{_movingEnemyInfo.Count}");
            _movingEnemySP = EditorGUILayout.Vector2Field("Spawn Position", _movingEnemySP);
            _movingEnemyPath = (ShootDirection)EditorGUILayout.EnumPopup(new GUIContent("Move Direction", "Indicates in which direction should enemy go"), _movingEnemyPath);

            GUILayout.BeginVertical();
            for (int i = 0; i < _movingEnemyPaths.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Path[{i}]:{_movingEnemyPaths[i]}");

                _movingEnemyPaths[i] = (ShootDirection)EditorGUILayout.EnumPopup(new GUIContent("Edit Direction:", "You can Edit Path Direction"), _movingEnemyPaths[i]);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();

            #region First Column Vertical
                GUILayout.BeginVertical();
                //First Column Vertical
                    //Add Path Button
                    GUI.color = Color.cyan;
                    if (GUILayout.Button("Add path"))
                    {
                        _movingEnemyPaths.Add(_movingEnemyPath);
                    }
                    GUI.color = Color.white;

                    GUI.color = Color.yellow;
                    //Add Enemy Button
                    if (GUILayout.Button("Add Enemy"))
                    {
                        List<ShootDirection> tempList = new List<ShootDirection>(_movingEnemyPaths);

                        _movingEnemyInfo.Add(new MovingEnemyInfo(_movingEnemySP, tempList));
                        _movingEnemyPaths.Clear();
                    }
                    GUI.color = Color.white;
                GUILayout.EndVertical();
            #endregion

            #region Second Column Vertical
            GUILayout.BeginVertical();
                    //Second Column Vertical
                    GUI.color = Color.red;
                    //Removes the last added Enemy in
                    if (GUILayout.Button("Remove Last"))
                            {
                                if (_movingEnemyInfo.Count > 0) _movingEnemyInfo.Remove(_movingEnemyInfo[_movingEnemyInfo.Count - 1]);
                            }
                    GUI.color = Color.white;
                    if(GUILayout.Button("Reset Values"))
                    {
                        ResetMovingValues();
                    }
                    //Remove Last Path Button
                    if (GUILayout.Button("Clear Last Path"))
                    {
                        if (_movingEnemyPaths.Count > 0) _movingEnemyPaths.Remove(_movingEnemyPaths[_movingEnemyPaths.Count - 1]);
                    }
                    //Clears All Paths
                    if (GUILayout.Button("Clear All Paths"))
                            {
                                _movingEnemyPaths.Clear();
                            }
                    //Clears all Enemies
                    if (GUILayout.Button("Clear All Enemies"))
                                {
                                    _movingEnemyInfo.Clear();
                                }
                GUILayout.EndVertical();
            #endregion

            GUILayout.EndHorizontal();
        }

        GUILayout.Box("Exploading Enemy Panel");
        _spawnExploding = EditorGUILayout.Toggle(new GUIContent("Spawn Exploading?", "Enable/Disable spawning exploding enemies"), _spawnExploding);
        if (_spawnExploding)
        {
            EditorGUILayout.LabelField($"Exploading Enemies Added:{_exploadingEnemyInfos.Count}");

            _exploadingEnemyInfo = EditorGUILayout.Vector2IntField("", _exploadingEnemyInfo);

            GUILayout.BeginHorizontal();

            #region First Column Vertical
                GUILayout.BeginVertical();
                    GUI.color = Color.yellow;
                    if (GUILayout.Button("Add Enemy"))
                    {
                        _exploadingEnemyInfos.Add(_exploadingEnemyInfo);
                    }
                    GUI.color = Color.white;
                GUILayout.EndVertical();
            #endregion

            #region Second Column Vertical
            GUILayout.BeginVertical();

                    GUI.color = Color.red;
                    if (GUILayout.Button("Remove Last"))
                    {
                        _exploadingEnemyInfos.Remove(_exploadingEnemyInfos[_exploadingEnemyInfos.Count - 1]);
                    }
                    GUI.color = Color.white;
                    if (GUILayout.Button("Reset Value"))
                    {
                         ResetExploadingValues();
                    }
                    if (GUILayout.Button("Clear All Enemies"))
                    {
                        _exploadingEnemyInfos.Clear();
                    }
                GUILayout.EndVertical();
            #endregion

            GUILayout.EndHorizontal();
        }

        GUILayout.Box("Shooting Enemy Panel");
        _spawnShooting = EditorGUILayout.Toggle(new GUIContent("Spawn Shooting?", "Enable/Disable spawning shooting enemies"), _spawnShooting);
        if (_spawnShooting)
        {
            EditorGUILayout.LabelField($"Shooter Enemies Added: {_shooterEnemyInfos.Count}");

            _shooterDirection = (ShootDirection) EditorGUILayout.EnumPopup(new GUIContent("Projectile direction", "Towards which direction bullets will go(if its Left) that means enemy will be placed on right side of the screen"), _shooterDirection);
            _shooterPosition = EditorGUILayout.IntField("Spawn position", _shooterPosition);
            _shooterFireRate = EditorGUILayout.FloatField("Fire Rate", _shooterFireRate);
            _shooterProjectileSpeed = EditorGUILayout.FloatField("Projectile Speed", _shooterProjectileSpeed);
       
            GUILayout.BeginHorizontal();

            #region First Column Vertical
                GUILayout.BeginVertical();
                    GUI.color = Color.yellow;
                    if (GUILayout.Button("Add Enemy"))
                    {
                        ShooterInfo _shooterInfo = new ShooterInfo(_shooterDirection, _shooterFireRate, _shooterProjectileSpeed, _shooterPosition);

                        _shooterEnemyInfos.Add(_shooterInfo);
                        ResetShooterValues();
                    }
                    GUI.color = Color.white;
                GUILayout.EndVertical();
            #endregion

            #region First Column Vertical

            GUILayout.BeginVertical();
                    GUI.color = Color.red;
                    if (GUILayout.Button("Remove Last"))
                    {
                        if (_shooterEnemyInfos.Count > 0) _shooterEnemyInfos.Remove(_shooterEnemyInfos[_shooterEnemyInfos.Count - 1]);
                    }
                    GUI.color = Color.white;
                    if (GUILayout.Button("Reset Values"))
                    {
                        ResetShooterValues();
                    }
                    if (GUILayout.Button("Clear All Enemies"))
                    {
                        _shooterEnemyInfos.Clear();
                    }
                GUILayout.EndVertical();
            #endregion

            GUILayout.EndHorizontal();
        }
    }

    #region Reset Values
    private void ResetVariables()
    {
        _movingEnemyInfo.Clear();
        _movingEnemyPaths.Clear();
        _exploadingEnemyInfos.Clear();
        _shooterEnemyInfos.Clear();

        ResetShooterValues();
        ResetExploadingValues();
        ResetMovingValues();

    }

    private void ResetShooterValues()
    {
        _shooterDirection = ShootDirection.DOWN;
        _shooterFireRate = _shooterProjectileSpeed = _shooterPosition = 0;
    }

    private void ResetExploadingValues()
    {
        _exploadingEnemyInfo = Vector2Int.zero;
    }

    private void ResetMovingValues()
    {
        _movingEnemySP = Vector2.zero;
        _movingEnemyPath = ShootDirection.DOWN;
    }
    #endregion

    private void SpawnLevel()
    {
        parentObject = new GameObject();
        parentObject.name = levelName; 

        parentObject.transform.position = Vector3.zero;
        //Adding Script
        parentObject.AddComponent<Level>();
        parentObject.GetComponent<Level>().SpawnPlayers = true;

        parentObject.transform.position = Vector3.zero;
        List<GameObject> tempList = new List<GameObject>();

        for (int i = 0; i < Mathf.Pow(gridSize, 2); i++)
        {
            GameObject gm = Instantiate(testObject, Vector3.zero, Quaternion.identity, parentObject.transform);

            if(dictionary.ContainsKey(i+1))
            {
                PlatformType platformType = dictionary[i+1].GetPlatformType();
                if(platformType != PlatformType.NONE)
                {

                    if (isBossLevel) 
                    {
                        Rigidbody2D _rb = gm.gameObject.transform.GetChild(1).gameObject.AddComponent<Rigidbody2D>();
                        _rb.bodyType = RigidbodyType2D.Kinematic;
                        _rb.freezeRotation = true;
                    }
                    gm.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = GetPlatformSprite(platformType);
                    gm.gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    AttachPlatformScript(platformType, gm.gameObject.transform.GetChild(1).gameObject);
                }
                else gm.gameObject.transform.GetChild(1).gameObject.SetActive(false);

                PickableType pickableType = dictionary[i + 1].GetPickableType();
                if (pickableType != PickableType.NONE)
                {
                    gm.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetPickableSprite(pickableType);
                    gm.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    AttachInteractableScript(pickableType, gm.gameObject.transform.GetChild(0).gameObject);
                }
                else gm.gameObject.transform.GetChild(0).gameObject.SetActive(false);

            }
            else
            {
                gm.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                gm.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }

            tempList.Add(gm);
        }

        GameObject[] prefabs;
        Vector2[] positions;

        positions = new Vector2[] { timSP, annieSP };

        if (spawnAnnie) prefabs = new GameObject[] { movingEnemyPrefab, explodingEnemyPrefab, shootingEnemyPrefab, timPrefab, anniePrefab };
        else prefabs = new GameObject[] { movingEnemyPrefab, explodingEnemyPrefab, shootingEnemyPrefab, timPrefab };

        if(isBossLevel)
        {
            //Trigger Level To Spawn grid cells and position them
            parentObject.GetComponent<Level>().SetUpBossLevel(tempList, gridSize, SaveAndDestroyPrefab);
            //Invoke saving prefab and destroy
        }
        else
        {
            
            parentObject.GetComponent<Level>().SetUpLevel(tempList, numberOfHearts, numberOfMoves, gridSize, prefabs, positions, _exploadingEnemyInfos, _shooterEnemyInfos, _movingEnemyInfo);
            SaveAndDestroyPrefab();
        }
        parentObject = null;
        ResetVariables();
    }

    #region Helper Functions
    private void SaveAndDestroyPrefab()
    {
        PrefabUtility.SaveAsPrefabAsset(parentObject, saveLevelPath + parentObject.name + ".prefab");
        DestroyImmediate(parentObject);
    }

    private void AttachInteractableScript(PickableType pickableType, GameObject go)
    {
        Moves script;
        go.GetComponent<BoxCollider2D>().size = Vector2.one;
        go.AddComponent<Scalable>();
        switch (pickableType)
        {
            case PickableType.HEART:
                go.AddComponent<Heart>().SetUpPowerUp(heartPickUpEffect);
                break;
            case PickableType.COIN:
                go.AddComponent<Coin>();
                break;
            case PickableType.WEAPON:
                go.AddComponent<RandomWeapon>();
                break;

            case PickableType.MOVE_3: 
                script = go.AddComponent<Moves>();
                script.SetUpPowerUp(3, plusThreePickUpEffect);
                break;
            case PickableType.MOVE_5:
                 script = go.AddComponent<Moves>();
                script.SetUpPowerUp(5, plusFivePickUpEffect);
                break;

            case PickableType.MOVE_REVERSE: 
                go.AddComponent<Reverse>();
                break;
            default:
                break;
        }
        if(go.GetComponent<Interactable>()) go.GetComponent<Interactable>().type = pickableType;
    }

    private void AttachPlatformScript(PlatformType platformType, GameObject go)
    {
        BasePlatform script;
        SlidePlatform scriptSlide;
        go.layer = 8;
        go.GetComponent<BoxCollider2D>().size = new Vector2(1, 0.15f);
        switch (platformType)
        {
            case PlatformType.NORMAL:
                script = go.AddComponent<BasePlatform>();
                if(isBossLevel)script.SetUpPlatformForBoss(platformLocation);
                break;
            case PlatformType.HAY:
                script = go.AddComponent<HayPlatform>();
                go.GetComponent<HayPlatform>().SetUpParticle(hayLandingEffect);

                if (isBossLevel) script.SetUpPlatformForBoss(platformLocation);
                break;
            case PlatformType.SPIKE:
                script = go.AddComponent<SpikePlatform>();
                if (isBossLevel) script.SetUpPlatformForBoss(platformLocation);
                break;
            case PlatformType.BREAKABLE:
                go.tag = "Platform";
                BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.size = new Vector2(1, 0.5f);

                script = go.AddComponent<BreakablePlatform>();
                script.SetUpTriggerCollider(collider);
                if (isBossLevel) script.SetUpPlatformForBoss(platformLocation);
                break;

            /*case PlatformType.PORTAL:
                script = go.AddComponent<PortalPlatform>();
                go.GetComponent<SpriteRenderer>().sprite = platformPortalSprite;
                //Set Up Portal Sprite
                break;
            */
            case PlatformType.SLIDE_LEFT:
                scriptSlide = go.AddComponent<SlidePlatform>();
                scriptSlide.SetUpSlide(false);
                if (isBossLevel) scriptSlide.SetUpPlatformForBoss(platformLocation);
                break;
            case PlatformType.SLIDE_RIGHT:
                scriptSlide = go.AddComponent<SlidePlatform>();
                scriptSlide.SetUpSlide(true);
                if (isBossLevel) scriptSlide.SetUpPlatformForBoss(platformLocation);
                break;
            case PlatformType.SLIME:
                script = go.AddComponent<SlimePlatform>();
                if (isBossLevel) script.SetUpPlatformForBoss(platformLocation);
                break;
            case PlatformType.DOUBLE:
                script = go.AddComponent<DoublePlatform>();
                break;

            default:
                break;
        }
        go.GetComponent<BasePlatform>().type = platformType;
    }

    private Sprite GetPlatformSprite(PlatformType platformType)
    {
        switch(platformType)
        {
            case PlatformType.NORMAL: return platformNormalSprite;
            case PlatformType.BREAKABLE: return platformBreakableSprite;
            case PlatformType.HAY: return platformHaySprite;
            //case PlatformType.PORTAL: return platformPortalSprite;

            case PlatformType.SLIDE_LEFT: return platformSlideLeftSprite;
            case PlatformType.SLIDE_RIGHT: return platformSlideRightSprite;
            case PlatformType.SPIKE: return platformSpikeSprite;
            case PlatformType.DOUBLE: return platformNormalSprite;
            case PlatformType.SLIME: return platformSlimeSprite;
            default: return null;
        }
    }

    private Sprite GetPickableSprite(PickableType pickableType)
    {
        switch(pickableType)
        {
            case PickableType.COIN: return coinSprite;
            case PickableType.HEART: return heartSprite;
            case PickableType.WEAPON: return weaponSprite;
            case PickableType.MOVE_3: return plusThreeMovesSprite;
            case PickableType.MOVE_5: return plusFiveMovesSprite;
            case PickableType.MOVE_REVERSE:  return reverseMovesSprite;
            default: return null;
        }
    }

    private Texture GetPlatformTexture(PlatformType platformType)
    {
        switch(platformType)
        {
            case PlatformType.NORMAL: return platformNormalTexture;
            case PlatformType.BREAKABLE: return platformBreakableTexture;
            case PlatformType.HAY: return platformHayTexture;
            //case PlatformType.PORTAL: return platformPortalTexture;

            case PlatformType.SLIDE_LEFT: return platformSlideLeftTexture;
            case PlatformType.SLIDE_RIGHT: return platformSlideRightTexture;
            case PlatformType.SPIKE: return platformSpikeTexture;
            case PlatformType.DOUBLE: return platformDoubleTexture;
            case PlatformType.SLIME: return platformSlimeTexture;

            case PlatformType.NONE: return defaultTexture;
            default: return defaultTexture;
        }
    }

    private Texture GetPickableTexture(PickableType pickableType)
    {
        switch(pickableType)
        {
            case PickableType.COIN: return coinTexture;
            case PickableType.HEART: return heartTexture;
            case PickableType.WEAPON: return weaponTexture;

            case PickableType.MOVE_3: return plusThreeMovesTexture;
            case PickableType.MOVE_5: return plusFiveMovesTexture;
            case PickableType.MOVE_REVERSE: return reverseMovesTexture;

            default: return defaultTexture;
        }
    }

    private Texture GetDefaultTexture()
    {
        return defaultTexture;
    }

    private void CheckHearts()
    {
        numberOfHearts = 0;
        foreach(KeyValuePair<int, PlatformInfo> kvp in dictionary)
        {
            if (kvp.Value.GetPickableType() == PickableType.HEART) numberOfHearts++;
        }
    }
    
    private int[] levelNumbersToCheck;

    private void TestLoadPrefab(string prefabName)
    {
        if (!levelToLoad) return;

        Debug.Log(levelToLoad.gameObject.name);

        GameObject go = levelToLoad;
        Level script = go.GetComponent<Level>();
        //Load Values
        gridSize = (int) Mathf.Sqrt(go.transform.childCount);
        dictionary.Clear();

        Debug.Log(go.transform.childCount);
        for(int i = 0; i < go.transform.childCount; i++)
        {
            dictionary.Add(i + 1, new PlatformInfo( i / gridSize, i % gridSize, GetPlatformTypeFromGO(go.transform.GetChild(i).GetChild(1).gameObject ), GetPickableTypeFromGO(go.transform.GetChild(i).GetChild(0).gameObject ) ) );
        }

    }

    private PickableType GetPickableTypeFromGO(GameObject GO)
    {
        if(!GO.activeSelf) return PickableType.NONE;
        else
        {
            if (GO.GetComponent<Interactable>())
            {
                switch (GO.GetComponent<Interactable>().type)
                {
                    case PickableType.MOVE_5:
                        return PickableType.MOVE_5;

                    case PickableType.MOVE_3:
                        return PickableType.MOVE_3;

                    case PickableType.MOVE_REVERSE:
                        return PickableType.MOVE_REVERSE;

                    case PickableType.HEART:
                        return PickableType.HEART;

                    case PickableType.COIN:
                        return PickableType.COIN;

                    case PickableType.WEAPON:
                        return PickableType.WEAPON;

                    case PickableType.NONE:
                        return PickableType.NONE;
                    default:
                        return PickableType.NONE;
                }
            }
            else return PickableType.MOVE_REVERSE;

        }
    }

    private PlatformType GetPlatformTypeFromGO(GameObject GO)
    {
        if (!GO.activeSelf) return PlatformType.NONE;
        else
        {
            switch (GO.GetComponent<BasePlatform>().type)
            {
                case PlatformType.NORMAL:
                    return PlatformType.NORMAL;

                case PlatformType.HAY:
                    return PlatformType.HAY;

                case PlatformType.SPIKE:
                    return PlatformType.SPIKE;

                case PlatformType.BREAKABLE:
                    return PlatformType.BREAKABLE;

                case PlatformType.SLIDE_LEFT:
                    return PlatformType.SLIDE_LEFT;

                case PlatformType.SLIDE_RIGHT:
                    return PlatformType.SLIDE_RIGHT;

                case PlatformType.SLIME:
                    return PlatformType.SLIME;

                case PlatformType.DOUBLE:
                    return PlatformType.DOUBLE;

                case PlatformType.NONE:
                    return PlatformType.NONE;
                default:
                    return PlatformType.NONE;
            }
        }
    }

    #endregion
}

public class PlatformInfo 
{
    private int posX;
    private int posY;
    private PlatformType platformType;
    private PickableType pickableType;  

    public PlatformInfo( int posX, int posY, PlatformType platformType, PickableType pickableType)
    {
        this.posX = posY;
        this.posY = posX;
        this.platformType = platformType;
        this.pickableType = pickableType;
    }
    
    public float GetPositionX() => posX;
    public float GetPositionY() => posY;

    public void ChangePlatform(PlatformType newPlatformType)
    {
        this.platformType = newPlatformType;
    }
    public void ChangePickable(PickableType newPickableType)
    {
        this.pickableType = newPickableType;
    }
    public PlatformType GetPlatformType()
    {
        return platformType;
    }
    public PickableType GetPickableType()
    {
        return pickableType;
    }
    public Vector3 GetPosition(bool isPickable)
    {
        if(!isPickable) return new Vector3(posX, -posY, 0);
        else return new Vector3(posX, -posY + 0.5f, 0);
    }

    public override string ToString()
    {
        return "( " + posX  + "," + posY + " ) Is Type : " + platformType.ToString() + " and Has picklable: " + pickableType.ToString();
    }
}
