using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class GameControllerTests {

    [Test]
    public void GameControllerTestsSimplePasses() {
        // Use the Assert class to test conditions.
    }

    [Test]
    public void Ctor_BasicInstantiation_ReturnsNonNull()
    {
        GameObject subjectGameControllerObject = Resources.Load("GameController") as GameObject;
        GameController subjectGameControllerScript = subjectGameControllerObject.GetComponent<GameController>();

        Assert.That(subjectGameControllerScript != null);
    }

    /*
    [Test]
    public void AddBlock_FullRow_AddsBlockSuccessfully()
    {
        GameObject subjectGameControllerObject = Resources.Load("GameController") as GameObject;
        GameController subjectGameControllerScript = subjectGameControllerObject.GetComponent<GameController>();

        System.Random random = new System.Random();
        foreach(int valu
        random.Next(0, 4);
        Assert.That();
    }
    */

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator GameControllerTestsWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }

    
}
