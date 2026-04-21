using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Linq;

public class LevelGenerationTests
{
    private GameObject generatorObject;
    private LevelGeneratorObstacles generator;
    private GameObject customObstaclePrefab;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        generatorObject = new GameObject("LevelGenerator");
        generator = generatorObject.AddComponent<LevelGeneratorObstacles>();


        var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.name = "TestPlatform";
        generator.platformPrefab = platform;

        generator.startPoint = new GameObject("StartPoint").transform;


        customObstaclePrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        customObstaclePrefab.name = "TestObstacle";
        generator.obstaclePrefabs = new System.Collections.Generic.List<GameObject> { customObstaclePrefab };

        var final = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        final.name = "TestFinal";
        generator.finalPrefab = final;

        yield return null;
    }

    [UnityTest]
    public IEnumerator GeneratesCorrectPlatformCount()
    {
        generator.totalObstaclesToPlace = 3;
        generator.platformsBeforeObstacle = 5;
        generator.finalPlatformsCount = 3;

        yield return generator.StartCoroutine(generator.GenerateLevel());

        var platforms = GameObject.FindObjectsOfType<Transform>()
            .Where(t => t.name.Contains("TestPlatform")).ToList();

        Assert.GreaterOrEqual(platforms.Count, generator.totalObstaclesToPlace * generator.platformsBeforeObstacle);
        yield return null;
    }

    [UnityTest]
    public IEnumerator GeneratesObstaclesAndFinal()
    {
        generator.totalObstaclesToPlace = 4;
        generator.platformsBeforeObstacle = 3;
        generator.finalPlatformsCount = 2;

        yield return generator.StartCoroutine(generator.GenerateLevel());

        var obstacles = GameObject.FindObjectsOfType<Transform>()
            .Where(t => t.name.Contains("TestObstacle")).ToList();

        var finalFlag = GameObject.FindObjectsOfType<Transform>()
            .FirstOrDefault(t => t.name.Contains("TestFinal"));

        Assert.AreEqual(5, obstacles.Count);
        Assert.IsNotNull(finalFlag);
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var obj in Object.FindObjectsOfType<GameObject>())
        {
            Object.DestroyImmediate(obj);
        }
    }
}
