using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyAITests
{
    private GameObject enemyObject;
    private GameObject playerObject;
    private EnemyAI enemyAI;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Set up enemy
        enemyObject = new GameObject("Enemy");
        enemyAI = enemyObject.AddComponent<EnemyAI>();
        enemyAI.patrolPoints = new Transform[] { enemyObject.transform };  // Dummy patrol point
        enemyAI.chaseDistance = 10f;
        enemyAI.attackDistance = 2f;
        // Set up player
        playerObject = new GameObject("Player");
        playerObject.tag = "Player";
        playerObject.transform.position = enemyObject.transform.position + Vector3.right * 15f; // Start far away

        yield return null;
    }
    [UnityTest]
    public IEnumerator EnemyStartsInPatrol()
    {
        yield return new WaitForSeconds(0.1f);
        Assert.AreEqual(EnemyState.Patrol, enemyAI.currentState);
    }

    [UnityTest]
    public IEnumerator EnemyTransitionsToChase()
    {
        // Move player within chase distance
        playerObject.transform.position = enemyObject.transform.position + Vector3.right * 5f;

        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(EnemyState.Chase, enemyAI.currentState);
    }
    [UnityTest]
    public IEnumerator EnemyTransitionsToAttack()
    {
        // Move player within attack distance
        playerObject.transform.position = enemyObject.transform.position + Vector3.right * 1f;
        yield return new WaitForSeconds(0.2f);
        Assert.AreEqual(EnemyState.Attack, enemyAI.currentState);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(enemyObject);
        Object.DestroyImmediate(playerObject);
    }
}
