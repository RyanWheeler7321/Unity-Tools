using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Save : MonoBehaviour
{
    public static Save x;
    private void Awake()
    {
        if (x != null)
        {
            Destroy(gameObject);
            return;
        }
        x = this;
        LoadGame();
    }

    private void Update()
    {
        if (data != null)
        {
            data.gameTime += time.deltaTime;
        }
    }

    public GameData data;

    //You can use any file name and type you want
    const string fileName = "/data.datatype";

    private string GetFilePath()
    {
        return Application.persistentDataPath + fileName;
    }

    public Color[] shirtColors;

    [System.Serializable]
    public class Data
    {
        public float example1;
        public bool example2;
    }

    [System.Serializable]
    public class GameData
    {
        public float gameTime;
        public List<string> mainData; // Any boolean trigger
        public int mainProgress;

        //Custom data here, you can make your own data structures for this
        public Data[] datas;
    }

    public bool Check(string key)
    {
        return data.mainData.Contains(key);
    }


    private void CreateDefaultData()
    {
        data = new GameData
        {
            gameTime = 0f,
            mainData = new List<string>(),
            mainStoryProgress = 0,
            dataUnits = new Data[10];
        };

        // Initialize default values for each world
        for (int i = 0; i < 10; i++)
        {
            data.worlds[i] = new World
            {
                example1 = 0f;
                example2 = false;
            };
        }

        Debug.Log("Blank File Created. Saving...");
        SaveGame();
    }

    public void SaveGame()
    {
        try
        {
            using (FileStream stream = new FileStream(GetFilePath(), FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, data);
                Debug.Log("Saved Data Successfully");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error Saving Data: " + e.Message);
        }
    }


    public void LoadGame()
    {
        if (File.Exists(GetFilePath()))
        {
            //Debug.Log("Save Data Found");
            try
            {
                using (FileStream stream = new FileStream(GetFilePath(), FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    data = (GameData)bf.Deserialize(stream);
                    Debug.Log("Data found and Loaded");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Data found but Error Loading Data: " + e.Message);
            }
        }
        else
        {
            Debug.Log("No Save data found, creating blank file...");
            CreateDefaultData();
        }
    }

    public void DeleteFile()
    {
        if (File.Exists(GetFilePath()))
        {
            try
            {
                File.Delete(GetFilePath());
                Debug.Log("Save file deleted.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error Deleting File: " + e.Message);
            }
        }
    }
}
