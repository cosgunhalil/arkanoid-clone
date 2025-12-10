#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;

public class LevelEditor : EditorWindow
{

    private static LevelEditor window;
    private string newLevelName = String.Empty;
    private Vector2 scrollPosition = Vector2.zero;
    private List<string> savedLevelNames = new List<string>();
    private LevelData levelData;

    [MenuItem("Tools/LevelEditor")]
    public static void CreateWindow()
    {
        window = (LevelEditor)GetWindow(typeof(LevelEditor));
        window.titleContent = new GUIContent("Level Editor");
    }

    void OnGUI()
    {
        if (window == null)
        {
            CreateWindow();
            savedLevelNames = new List<string>();
        }

        newLevelName = GUI.TextField(new Rect(10, 10, position.width, 20), newLevelName, 25);

        if (GUI.Button(new Rect(10, 40, position.width, 20), "Save"))
        {
            Save(newLevelName);
        }

        if (GUI.Button(new Rect(10, 70, position.width, 20), "LoadLevels"))
        {
            CreateLevelButtons();
        }

        GUILayout.BeginArea(new Rect(10, 110, position.width, position.height));
        for (int i = 0; i < savedLevelNames.Count; i++)
        {
            if (GUILayout.Button(savedLevelNames[i]))
            {
                LoadDataFromJson(savedLevelNames[i]);
            }
        }
        GUILayout.EndArea();

    }

    public void CreateLevelButtons()
    {
        savedLevelNames = new List<string>();

        savedLevelNames = GetLevelNames();

        if (GUI.Button(new Rect(10, 90, position.width, 20), "Level1"))
        {
            Debug.Log("Test is completed");
        }
    }

    private void Save(string levelName)
    {
        //TODO: handle
        //var itemsToSave = FindObjectsOfType<LevelItem>();

        //string path = Application.dataPath + "/Resources/" + levelName + ".txt";
        //var data = SerializeMapData(itemsToSave);

        //using (FileStream fs = new FileStream(path, FileMode.Create))
        //{
        //    using (StreamWriter writer = new StreamWriter(fs))
        //    {

        //        writer.Write(data);
        //    }

        //}

        //AssetDatabase.Refresh();

    }

    // TODO: Add Level Item and Level Data
    //private string SerializeMapData(LevelItem[] itemsToSave)
    //{
    //    LevelData levelData = new LevelData();

    //    SetCameraRectSizes(levelData);

    //    foreach (var item in itemsToSave)
    //    {
    //        LevelItemData levelItemData = new LevelItemData();
    //        levelItemData.Type = item.Type;
    //        levelItemData.Size = item.transform.localScale;
    //        levelItemData.Position = item.transform.position;
    //        levelItemData.Rotation = item.transform.eulerAngles;
    //        levelData.LevelItems.Add(levelItemData);
    //    }

    //    var data = JsonUtility.ToJson(levelData);

    //    return data;
    //}

    public void LoadDataFromJson(string fileName)
    {
        string path = Application.dataPath + "/Resources/" + fileName;
        var data = ReadDataFromText(path);
        var levelData = JsonUtility.FromJson<LevelData>(data);
        LoadScene(levelData);
    }

    private void LoadScene(LevelData levelData)
    {
        ClearScene();

        //TODO: add load scene functionality
        //foreach (var levelItem in levelData.LevelItems)
        //{
        //    var levelItemObject = GenerateLevelItem(levelItem.Type);
        //    var levelItemObjectData = levelItemObject.GetComponent<LevelItem>();
        //    levelItemObjectData.transform.localScale = levelItem.Size;
        //    levelItemObjectData.transform.position = levelItem.Position;
        //    levelItemObjectData.transform.eulerAngles = levelItem.Rotation;

        //}
    }

    public string ReadDataFromText(string path)
    {
        string data = null;
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    data = reader.ReadToEnd();
                }
            }

        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }

        return data;
    }

    private void ClearScene()
    {
        //TODO: handle
        //var levelItems = GameObject.FindObjectsOfType<LevelItem>();

        //foreach (var rect in levelItems)
        //{
        //    DestroyImmediate(rect.gameObject);
        //}
    }

    private List<string> GetLevelNames()
    {
        List<string> levelNames = new List<string>();

        string partialName = string.Empty;

        DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Application.dataPath + "/Resources");
        FileSystemInfo[] filesAndDirs = hdDirectoryInWhichToSearch.GetFileSystemInfos("*" + partialName + "*.txt");

        foreach (FileSystemInfo foundFile in filesAndDirs)
        {
            string fullName = foundFile.Name;
            levelNames.Add(fullName);
        }

        return levelNames;
    }
}

#endif
