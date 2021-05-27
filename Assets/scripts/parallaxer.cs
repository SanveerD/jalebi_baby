using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxer : MonoBehaviour
{
    class PoolObject {
        public Transform transform;
        public bool inUse;
        public float shiftSpeed;
        public PoolObject(Transform t) { transform = t; }
        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }
    }
    
    [System.Serializable]
    public struct YSpawnRange
    {
        public float min;
        public float max;
    }
    [System.Serializable]
    public struct VarSpeed
    {
        public float min;
        public float max;
    }

    public GameObject Prefab;
    public int poolSize;
    public float spawnRate;
    public bool leveledDifficulty;
    public bool falling;

    public YSpawnRange ySpawnRange;
    public VarSpeed varSpeed;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate; // particle prewarm
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;

    // Private Variables
    float currVarMin;
    float currVarMax;
    float currSpawnRate;

    float speed;
    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjects;
    GameManager game;

    void Awake()
    {
        Configure();
        currVarMin = varSpeed.min;
        currVarMax = varSpeed.max;
        currSpawnRate = spawnRate;
    }

    void Start()
    {
        game = GameManager.Instance;
    }

    void OnEnable() {
        currVarMin = varSpeed.min;
        currVarMax = varSpeed.max;
        currSpawnRate = spawnRate;

        GameManager.OnGameStarted += OnGameStarted;
        PlayerMovement.OnLevelUp += OnLevelUp;
    }

    void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        PlayerMovement.OnLevelUp -= OnLevelUp;
        // Reset spawn settings
    }

    void OnGameStarted()
    {

        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Update()
    {
       // if (game.GameOver) return;
        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > currSpawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(Prefab) as GameObject;
            go.SetActive(true);
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one *1000;
            poolObjects[i] = new PoolObject(t);
            poolObjects[i].shiftSpeed = Random.Range(currVarMin, currVarMax);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Spawn()
    {
        if (!falling)
        {
            Transform t = GetPoolObject();
            if (t == null) return; // if true, this indicate sthat poolSize is too  small
            Vector3 pos = Vector3.zero;
            pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
            pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
            t.position = pos;
            t.gameObject.SetActive(true);
        }

        if (falling)
        {
            Transform t = GetPoolObject();
            if (t == null) return; // if true, this indicate sthat poolSize is too  small
            Vector3 pos = Vector3.zero;
            pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
            pos.y = defaultSpawnPos.y;
            t.position = pos;
            t.gameObject.SetActive(true);
        }
    }

    void SpawnImmediate()
    {
        if (!falling)
        {
            Transform t = GetPoolObject();
            if (t == null) return; // if true, this indicate sthat poolSize is too  small
            Vector3 pos = Vector3.zero;
            pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
            pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
            t.position = pos;
            Spawn();
        }

        if (falling)
        {
            Transform t = GetPoolObject();
            if (t == null) return; // if true, this indicate sthat poolSize is too  small
            Vector3 pos = Vector3.zero;
            pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
            pos.y = immediateSpawnPos.y;
            t.position = pos;
            Spawn();
        }
    }

    void Shift()
    {
        if (!falling)
        {
            for (int i = 0; i < poolObjects.Length; i++)
            {
                speed = poolObjects[i].shiftSpeed;
                poolObjects[i].transform.localPosition += -Vector3.right * speed * Time.deltaTime;
                CheckDisposeObject(poolObjects[i]);
            }
        }

        if (falling)
        {
            for (int i = 0; i < poolObjects.Length; i++)
            {
                speed = poolObjects[i].shiftSpeed;
                poolObjects[i].transform.localPosition += Vector3.down * speed * Time.deltaTime;
                CheckDisposeObject(poolObjects[i]);
            }
        }
    }

    void CheckDisposeObject(PoolObject poolObject) {
        if (poolObject.transform.position.x < -(defaultSpawnPos.x * Camera.main.aspect) / targetAspect - 5)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                poolObjects[i].shiftSpeed = Random.Range(currVarMin, currVarMax);
                return poolObjects[i].transform;
            }
        }
        return null;
    }

    // Increase speed and spawn rates on levelup
    void OnLevelUp()
    {
        if (leveledDifficulty)
        {
            currVarMin += 0.65f;
            currVarMax += 0.65f;

            if (currSpawnRate > 1)
            {
                currSpawnRate -= 0.1f;
            }
            else
            {
                currSpawnRate = 1;
            }
        }


    }
}
