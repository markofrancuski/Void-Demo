using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

public class TerrainManager : Singleton<TerrainManager>
{
    [SerializeField] private LevelManager levelManager_Instance;

    [SerializeField] private List<GameObject> _upPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> _downPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> _leftPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> _rightPrefabs = new List<GameObject>();

    [SerializeField] private List<GameObject> _spawnPrefabs;

    [SerializeField] private Vector3 currentPosition = Vector3.zero;
    [SerializeField] private Transform parentTerrain;
    [SerializeField] private float cameraSize;

    [SerializeField] private int maxSpawned;
    public int CurrentSpawned;
    public List<GameObject> spawnedLevels;

    private void Start() 
    {
        cameraSize = Camera.main.orthographicSize*2;

        switch (LevelManager.Instance.bossScript.direction)
        {
            case ShootDirection.DOWN:
                _spawnPrefabs = _downPrefabs;
                break;
            case ShootDirection.LEFT:
                _spawnPrefabs = _leftPrefabs;
                break;
            case ShootDirection.UP:
                _spawnPrefabs = _upPrefabs;
                break;
            case ShootDirection.RIGHT:
                _spawnPrefabs = _rightPrefabs;
                break;
        }
    }

    private void OnEnable()
    {
        Boss.BossChangedDirection += ChangePrefabs;
    }
    private void OnDisable()
    {
        Boss.BossChangedDirection -= ChangePrefabs;
    }

    private void FixedUpdate()
    {
        if (CurrentSpawned < maxSpawned) SpawnLevel();
    }

    private Vector3 GetNextSpawnPosition()
    {
        Vector3 vector = Vector3.zero;

        switch (LevelManager.Instance.bossScript.direction)
        {
            case ShootDirection.DOWN:
                vector.x = -(cameraSize/2f) - 0.622f;
                return vector;
            case ShootDirection.LEFT:
                vector.y = -8;
                return vector;
            case ShootDirection.UP:
                vector.x = (cameraSize/ 2f) + 0.622f;
                return vector;
            case ShootDirection.RIGHT:
                vector.y = 8;
                return vector;
        }
        return vector;
    }

    public void SpawnLevel()
    {
        //if (!isStart) currentPosition = spawnedLevels[spawnedLevels.Count - 1].transform.position + new Vector3(0, cameraSize - Time.deltaTime, 0); // -0.03f => Time.deltaTime
        GameObject go;

        go = Instantiate(_spawnPrefabs[Random.Range(0, _spawnPrefabs.Count)]);
        //Check which one to spawn (Up/Down/Right/Left)
        go.transform.position = currentPosition;
        go.transform.SetParent(parentTerrain);

        spawnedLevels.Add(go);

        currentPosition += GetNextSpawnPosition();
        CurrentSpawned++;

        //spawnedLevels.Add(go);

        /*for (int i = 0; i < prefabs.Count; i++)
        {
            if(!isStart) currentPosition = spawnedLevels[spawnedLevels.Count-1].transform.position + new Vector3(0,cameraSize - Time.deltaTime, 0); // -0.03f => Time.deltaTime
            GameObject go = Instantiate(prefabs[i]);
            go.transform.SetParent(parentTerrain);
            go.transform.position = currentPosition;

            currentPosition += new Vector3(0, cameraSize, 0);
            spawnedLevels.Add(go);
        }
        */
    }

    private void ChangePrefabs()
    {
        for (int i = 0; i < spawnedLevels.Count; i++)
        {
            Destroy(spawnedLevels[i]);
        }
        spawnedLevels.Clear();

        Debug.Log("Changing prefabs");

        //if (!InputManager.Instance.isControllable) return;

        switch (LevelManager.Instance.bossScript.direction)
        {
            case ShootDirection.DOWN:
                _spawnPrefabs = _downPrefabs;
                break;
            case ShootDirection.LEFT:
                _spawnPrefabs = _leftPrefabs;
                break;
            case ShootDirection.UP:
                _spawnPrefabs = _upPrefabs;
                break;
            case ShootDirection.RIGHT:
                _spawnPrefabs = _rightPrefabs;
                break;
        }

        CurrentSpawned = 0;
        currentPosition = Vector3.zero;
    }

}