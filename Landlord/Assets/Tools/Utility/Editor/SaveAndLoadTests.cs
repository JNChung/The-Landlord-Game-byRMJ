using NUnit.Framework;
using RonTools;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoadTextTests
{
    SaveAndLoadForIO saveAndLoad;
    [SetUp]
    public void SetUp()
    {
        saveAndLoad = new SaveAndLoadForIO("TestDatas/myData.txt");
    }
    [TearDown]
    public void TearDown()
    {
        //recover
        Directory.Delete(saveAndLoad.FullPath, true);
    }
    [Test]
    public void SaveAndLoadConstructorTest()
    {
        bool directorExists = Directory.Exists(saveAndLoad.FullPath);
        Assert.IsTrue(directorExists);
    }
    [Test]
    public void SaveTextAndLoadTest()
    {
        saveAndLoad.SaveText("Test1");
        string actual = saveAndLoad.LoadText();

        Assert.AreEqual("Test1", actual);
    }
}
public class SaveAndLoadJsonTests
{
    public class RpgData
    {
        public string Name;
        public string Description;
        public int Hp;
    }
    SaveAndLoadForIO saveAndLoad;
    [SetUp]
    public void SetUp()
    {
        saveAndLoad = new SaveAndLoadForIO("TestDatas/myJson.txt");
    }
    [TearDown]
    public void TearDown()
    {
        //recover
        Directory.Delete(saveAndLoad.FullPath, true);
    }
    [Test]
    public void SaveAndLoadConstructorTest()
    {
        bool directorExists = Directory.Exists(saveAndLoad.FullPath);
        Assert.IsTrue(directorExists);
    }
    [Test]
    public void SaveTextAndLoadTest()
    {
        var data = new RpgData { Name = "tt", Description = "haha", Hp = 100 };

        saveAndLoad.SaveJson<RpgData>(data);
        RpgData actual = saveAndLoad.LoadJson<RpgData>();

        Assert.AreEqual("tt", actual.Name);
        Assert.AreEqual("haha", actual.Description);
        Assert.AreEqual(100, actual.Hp);
    }
}
