using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnBehaviour",menuName ="EnemySpawnBehaviours/SpawnBehaviourPattern",order = 1)]
public class EnemySpawnerBehaviours : ScriptableObject
{
    
    public float secUntilSpawnStart;
    public EnemySpawnBehaviour[] spawnBehaviours;

}

[System.Serializable]
public class EnemySpawnBehaviour
{

    public GameObject enemyPrefab;
    public float spawnSec;
    public int spawnReadyCount;
    public float secActiveTime;

}




