using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private PathPoints mainUnitPathPoint;
    [SerializeField] private PathPoints singlePathToPlayerTown;
    [SerializeField] private EnemySpawnerBehaviours spawnPattern;
    
    private EnemySpawnBehaviour currentBehaviour;
    private List<UnitPathPatrolling> currentSpawnedEnemies = new List<UnitPathPatrolling>();

    private float timer;
    private float behaviourTimer;
    private int currentBehaviourIndex;
    private bool spawnActive;

    private void Start() 
    {
        currentBehaviour = spawnPattern.spawnBehaviours[0];
    }

    private void Update() 
    {

        //Wait for the activate spawn.

        if(!spawnActive)
        {
            timer += Time.deltaTime;
            if(timer >= spawnPattern.secUntilSpawnStart)
            {
                spawnActive = true;
                timer = 0;
            }
        }
        
        //Spawn Active.
        else    
        {
            timer += Time.deltaTime;
            behaviourTimer += Time.deltaTime;

            if(timer >= currentBehaviour.spawnSec)
            {
                SpawnEnemy();
            }

            //Also check the timer for currentBehaviour if it's time finished skip the next behaviour.
            
            if(behaviourTimer >= currentBehaviour.secActiveTime)
            {
                behaviourTimer = 0;
                timer = 0;
                //Skip to next behaviour.
                ChangeTheNextBehaviour();
            }

        }
    }

    private void SpawnEnemy()
    {
        //Set standart path for the enemy units.
        GameObject enemy = Instantiate(currentBehaviour.enemyPrefab,spawnPosition.position,Quaternion.identity);
        UnitPathPatrolling pathPatroller = enemy.GetComponent<UnitPathPatrolling>();
        pathPatroller.PathPoints = mainUnitPathPoint;
        pathPatroller.StartPatrolling(false);
        timer = 0;
        currentSpawnedEnemies.Add(pathPatroller);   //Add this enemy to the list.
        CheckEnemyCountAndActivateThem();
    }

    private void CheckEnemyCountAndActivateThem()
    {
        //Check the current count of spawned enemies if it's reached to spawnReadyCount activate and send them to attack player and clear list.
        if(currentSpawnedEnemies.Count >= currentBehaviour.spawnReadyCount)
        {
            //Send them to attack to player.
            for(int i=0; i < currentSpawnedEnemies.Count;i++)
            {
                if(currentSpawnedEnemies[i] == null) continue;
                currentSpawnedEnemies[i].alwaysInterruptable = true;
                currentSpawnedEnemies[i].PathPoints = singlePathToPlayerTown;
                currentSpawnedEnemies[i].StartPatrolling(false);                                        
            }

            //Clear list.
            currentSpawnedEnemies.Clear();
        }
    }

    private void ChangeTheNextBehaviour()
    {
        currentBehaviourIndex ++;
        if(currentBehaviourIndex >= spawnPattern.spawnBehaviours.Length)
            return;     //If we'r already in last behaviour dont change it just return.
        //Else
        currentBehaviour = spawnPattern.spawnBehaviours[currentBehaviourIndex];
    }
}
