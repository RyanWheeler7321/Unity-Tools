using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Singleton instance of SaveManager
    public static SaveManager instance;

    // Save file name and type
    private const string FILE_NAME = "/data.datatype";

    // Game data to be saved/loaded
    public GameData gameData;

    private void Awake()
    {
        // Ensure only one instance of SaveManager exists
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGame();
    }

    private void Update()
    {
        // Update the game time if gameData exists
        if (gameData != null)
        {
            gameData.gameTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Returns the full file path for the save file.
    /// </summary>
    private string GetFilePath()
    {
        return Application.persistentDataPath + FILE_NAME;
    }

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
        public List<string> mainData; // Holds the names of triggered events
        public int mainStoryProgress;
        public Data[] dataUnits; // Custom data to hold any specific information
    }

    /// <summary>
    /// Checks if a specific key is present in the game data.
    /// </summary>
    public bool Check(string key)
    {
        return gameData.mainData.Contains(key);
    }

    /// <summary>
    /// Creates default game data if no save file exists.
    /// </summary>
    private void CreateDefaultData()
    {
        gameData = new GameData
        {
            gameTime = 0f,
            mainData = new List<string>(),
            mainStoryProgress = 0,
            dataUnits = new Data[10]
        };

        // Initialize default values for each data unit
        for (int i = 0; i < gameData.dataUnits.Length; i++)
        {
            gameData.dataUnits[i] = new Data
            {
                example1 = 0f,
                example2 = false
            };
        }

        Debug.Log("Blank File Created. Saving...");
        SaveGame();
    }

    /// <summary>
    /// Saves the current game data to a file.
    /// </summary>
    public void SaveGame()
    {
        try
        {
            using (FileStream stream = new FileStream(GetFilePath(), FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, gameData);
                Debug.Log("Saved Data Successfully");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error Saving Data: " + e.Message);
        }
    }

    /// <summary>
    /// Loads the game data from a file if it exists.
    /// </summary>
    public void LoadGame()
    {
        if (File.Exists(GetFilePath()))
        {
            try
            {
                using (FileStream stream = new FileStream(GetFilePath(), FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    gameData = (GameData)bf.Deserialize(stream);
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

    /// <summary>
    /// Deletes the save file if it exists.
    /// </summary>
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
