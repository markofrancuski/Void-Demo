using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelEditor : EditorWindow 
{
    public int levelNumber; 
    private int gridSize;
    //private string saveLevelPath = "/Prefabs/Level/";
    private string saveLevelPath = "/";
    private string levelName = "Level";
    private bool isInEditMode;

    //private Vector3 spaceBetweenPlatforms;
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

    [SerializeField] private Texture defaultTexture;

    [SerializeField] private GameObject timPrefab;
    [SerializeField] private GameObject anniePrefab;

    private GameObject movingEnemyPrefab;
    private GameObject explodingEnemyPrefab;
    private GameObject shootingEnemyPrefab;

    #endregion

    #region Drop Down Variables
    /* private bool isPlatformClick = false;
     private bool isPickableClick = false;
     private int selectedPlatformIndex;*/
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
        heartSprite = Resources.Load<Sprite>("Sprites/Pickables/Heart 1");
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

        #endregion

        gridSize = 5;

        levelNumber = Directory.GetFiles(@"Assets/Prefabs/Level", "*.prefab").Length + 1;
        selectedPlatformType = PlatformType.NONE;
        selectedPickableType = PickableType.NONE;
        //spaceBetweenPlatforms = new Vector3(1 , 1, 0);
        numberOfHearts = 0;
        numberOfMoves = 0;
        spawnTim = true;
    }

    [MenuItem("Window/Custom Windows/LevelEditor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Generator");
    }

    [SerializeField] private GameObject levelToLoad;
    private void OnGUI() 
    {

        #region Input Fields
        //Counts the number of level prefabs and displays it.
        //levelNumber = Directory.GetFiles(@"Assets" + saveLevelPath, "*.prefab").Length + 1;
        //GUILayout.Label($"Next Number of the Level is: { levelNumber }");
        gridSize = EditorGUILayout.IntField("Enter Grid Size" ,gridSize);
        //saveLevelPath = EditorGUILayout.TextField(new GUIContent("Enter Path to Save", "Do not add Assets infront its already added when saving") ,saveLevelPath);
        levelName = EditorGUILayout.TextField("Enter the Prefab name", levelName);
        //spaceBetweenPlatforms = EditorGUILayout.Vector3Field("Enter the desired space between platforms",  spaceBetweenPlatforms);

        selectedPlatformType = (PlatformType) EditorGUILayout.EnumPopup("Select Platform Type ", selectedPlatformType);
        selectedPickableType = (PickableType) EditorGUILayout.EnumPopup("Select Pickable Type ", selectedPickableType);
        CheckHearts();
        GUILayout.Label($"Hearts To Collect: { numberOfHearts.ToString() }");

        isBossLevel = EditorGUILayout.Toggle(new GUIContent("Boss Level", "Is Boss level or not, based on this toogle level will be setup with different settings."), isBossLevel);
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
        GUI.color = Color.cyan;
        if(GUILayout.Button("Load Level to Edit"))
        {
            //CheckLevelNumber();
            //CheckMissingLevelNumber();
            TestLoadPrefab("Level 1");
        }

        GUI.color = Color.yellow;
        //Spawn Level Button
        if (GUILayout.Button("Spawn Level"))
        {
            if (dictionary.Count <= 0) { Debug.LogError("LEVEL EDITOR- Cannot make empty level, place atleast one platform"); return; }
            saveLevelPath = EditorUtility.GetAssetPath(Selection.activeObject) + "/";
            if (saveLevelPath == "/") { Debug.LogError("LEVEL EDITOR-You didn't select folder where you want to save prefab in project window!"); return; }
            SpawnLevel();
        }

        GUI.color = Color.red;
        //Print the dictionary => What fields we filled up in a grid
        if(GUILayout.Button("Debug Clicked Platforms"))
        {
            foreach (KeyValuePair<int, PlatformInfo> kvp in dictionary)
            {
                Debug.Log(kvp.Value.ToString());
            }
        }

        GUI.color = Color.green;
        //Clears out the dictionary
        if(GUILayout.Button("Reset Grid"))
        {
            dictionary.Clear();
        }
        GUI.color = Color.white;

        #endregion

        //Creates the grid
        CreateGrid(gridSize, 25, 25, 50, 400, "Level Grid", 25);
        #region Deprecated
        // Shows the drop down menu to select the platform type
        /*if(isPlatformClick)
        {      
            selectedPlatformType = dictionary[selectedPlatformIndex].GetPlatformType();

            GUI.Box(new Rect(150, 400, 200, 50), "Platform Selection");
            selectedPlatformType = (PlatformType) EditorGUI.EnumPopup(new Rect(150, 420, 200, 50),"Select Platform", selectedPlatformType);

            //selectedPlatformType = (PlatformType) EditorGUILayout.EnumPopup("Select Platform", selectedPlatformType);

            if (dictionary[selectedPlatformIndex].GetPlatformType() != selectedPlatformType)
            {
                dictionary[selectedPlatformIndex].ChangePlatform(selectedPlatformType);

                isPlatformClick = false;
                //Debug.Log("Changed: " + selectedPlatformType);
                //Debug.Log(selectedPlatformIndex);   
            }
        }

        // Shows the drop down menu to selcet the pickable type
        if(isPickableClick)
        {
            selectedPickableType = dictionary[selectedPlatformIndex].GetPickableType();

            GUI.Box(new Rect(150, 400, 200, 50), "Pickable Selection");
            selectedPickableType = (PickableType) EditorGUI.EnumPopup(new Rect(150, 420, 200, 50),"Select Pickable", selectedPickableType);

            //selectedPickableType = (PickableType) EditorGUILayout.EnumPopup("Select Pickable", selectedPickableType);

            if(dictionary[selectedPlatformIndex].GetPickableType() != selectedPickableType)
            {
                dictionary[selectedPlatformIndex].ChangePickable(selectedPickableType);
                isPickableClick = false;
            }
        }
        */
        #endregion
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
                        //if(isPlatformClick) isPlatformClick = false;

                        //selectedPlatformIndex = buttonIndex;
                        //dictionary[buttonIndex].ChangePickable(selectedPickableType);
                        //isPickableClick = true;

                        if (selectedPickableType == PickableType.NONE && dictionary[buttonIndex].GetPlatformType() == PlatformType.NONE)
                        {
                            dictionary.Remove(buttonIndex);
                        }
                        else
                        {
                            dictionary[buttonIndex].ChangePickable(selectedPickableType);
                        }
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(initialPosX, initialPosY + boxSize2 / 2 - 20, buttonSize*3, buttonSize), GetDefaultTexture()))
                    {
                        dictionary[buttonIndex] = new PlatformInfo(i, j, PlatformType.NONE, selectedPickableType);
                        //selectedPlatformIndex = buttonIndex;
                        //isPickableClick = true;
                    }
                }

                //Platform Button
                if(dictionary.ContainsKey(buttonIndex))
                {
                    if(GUI.Button(new Rect(initialPosX + boxSize2 / 2 - 41, initialPosY + boxSize2 / 2 + 5, buttonSize * 3, buttonSize), GetPlatformTexture(dictionary[buttonIndex].GetPlatformType()) ))
                    {
                        //if(isPickableClick) isPickableClick = false;
                        
                        //selectedPlatformIndex = buttonIndex;
                        //dictionary[buttonIndex].ChangePlatform(selectedPlatformType);
                        //isPlatformClick = true;

                        if (selectedPlatformType == PlatformType.NONE && dictionary[buttonIndex].GetPickableType() == PickableType.NONE)
                        {
                            dictionary.Remove(buttonIndex);
                        }
                        else
                        {
                            dictionary[buttonIndex].ChangePlatform(selectedPlatformType);
                        }
                    }
                }
                else
                {
                   
                    if (GUI.Button(new Rect(initialPosX + boxSize2 / 2 - 41, initialPosY + boxSize2 / 2 + 5, buttonSize * 3, buttonSize), GetDefaultTexture()))
                    {

                        dictionary[buttonIndex] = new PlatformInfo(i, j, selectedPlatformType, PickableType.NONE);
                        //isPlatformClick = true;
                        //selectedPlatformIndex = buttonIndex;
                    }
                }

                initialPosX += buttonSize + spaceBetween + boxSize2 / 2;
                buttonIndex++;
            }
            initialPosY += buttonSize + spaceBetween + boxSize2 / 2 ;
            initialPosX = posX;
        }
        GUI.color = Color.white;

    }

    private void SpawnLevel()
    {

        parentObject = new GameObject();
        parentObject.name = levelName; //"Level " + levelNumber;

        parentObject.transform.position = Vector3.zero;

        #region Deprecated
        /*foreach(KeyValuePair<int, PlatformInfo> kvp in dictionary)
        {
            GameObject pickable = resource.GetPickablePrefab(kvp.Value.GetPickableType());
            GameObject platform = resource.GetPlatformPrefab(kvp.Value.GetPlatformType());
            Vector3 offset = new Vector3(0, 0 ,0);
            
            //Adds offset based on the position of the platform
            if(kvp.Value.GetPositionX() > 0) offset += new Vector3(spaceBetweenPlatforms.x*kvp.Value.GetPositionX(), 0, 0);
            if(kvp.Value.GetPositionY() > 0) offset += new Vector3(0, -spaceBetweenPlatforms.y*kvp.Value.GetPositionY(), 0);

            if(platform != null) Instantiate(platform, kvp.Value.GetPosition(false)+ offset, Quaternion.identity, parentObject.transform);
            if(pickable != null) Instantiate(pickable, kvp.Value.GetPosition(true)+ offset, Quaternion.identity, parentObject.transform);         
        }

        PrefabUtility.SaveAsPrefabAsset(parentObject, saveLevelPath + parentObject.name+ ".prefab");
        GameObject.DestroyImmediate(parentObject);
        */
        #endregion

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
                    //gm.gameObject.tag = "Platform";


                }
                else
                {
                    gm.gameObject.transform.GetChild(1).gameObject.SetActive(false);
                }
                PickableType pickableType = dictionary[i + 1].GetPickableType();
                if (pickableType != PickableType.NONE)
                {
                    gm.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = GetPickableSprite(pickableType);
                    gm.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    AttachInteractableScript(pickableType, gm.gameObject.transform.GetChild(0).gameObject);

                }
                else
                {
                    gm.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                }
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

        //Default path if non is provided
        /*if (saveLevelPath == "") saveLevelPath = "Prefabs/";
        else if (saveLevelPath[saveLevelPath.Length - 1] != '/') saveLevelPath += "/";*/

        if(isBossLevel)
        {
            //Trigger Level To Spawn grid cells and position them
            parentObject.GetComponent<Level>().SetUpBossLevel(tempList, gridSize, SaveAndDestroyPrefab);
            //Invoke saving prefab and destroy
        }
        else
        {
            parentObject.GetComponent<Level>().SetUpLevel(tempList, numberOfHearts, numberOfMoves, gridSize, prefabs, positions);
            SaveAndDestroyPrefab();
        }
        parentObject = null;
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
                go.AddComponent<Heart>();
                break;
            case PickableType.COIN:
                go.AddComponent<Coin>();
                break;
            case PickableType.WEAPON:
                go.AddComponent<RandomWeapon>();
                break;

            case PickableType.MOVE_3: 
                script = go.AddComponent<Moves>();
                script.SetUpPowerUp(3);
                break;
            case PickableType.MOVE_5:
                 script = go.AddComponent<Moves>();
                script.SetUpPowerUp(5);
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
        go.GetComponent<BoxCollider2D>().size = new Vector2(1, 0.5f);
        switch (platformType)
        {
            case PlatformType.NORMAL:
                script = go.AddComponent<BasePlatform>();
                if(isBossLevel)script.SetUpPlatformForBoss(platformLocation);
                break;
            case PlatformType.HAY:
                script = go.AddComponent<HayPlatform>();
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

   /* private void CheckMissingLevelNumber()
    {
        
        Debug.Log(Application.dataPath + saveLevelPath);
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/" + saveLevelPath);
        FileInfo[] info = dir.GetFiles("*.prefab");

        int[] levelNumbersToCheck;
        levelNumbersToCheck = new int[info.Length];
        int index = 0;

        foreach (FileInfo f in info)
        {
            Debug.Log(f.Name);
            Debug.Log(f.Name.Split('.')[1]);

            int.TryParse(f.Name.Split('.')[1].Split(' ')[1], out levelNumbersToCheck[index]);
            index++;

        }

        for (int i = 0; i < levelNumbersToCheck.Length; i++)
        {
            Debug.Log(levelNumbersToCheck[i]);
        }
    }*/

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
