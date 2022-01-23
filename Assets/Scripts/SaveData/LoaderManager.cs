using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoaderManager : MonoBehaviour
{
    public static LoaderManager Instance { get; private set; }
    
    public static Data Data { get; private set; }
    
    private static string FilePath => Application.persistentDataPath + "/save.data";
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if(File.Exists(FilePath))
        {
            // File exists 
            FileStream dataStream = new FileStream(FilePath, FileMode.Open);

            BinaryFormatter converter = new BinaryFormatter();
            Data saveData = converter.Deserialize(dataStream) as Data;

            dataStream.Close();
            Data = saveData;
        }
        else
        {
            Data = new Data();
        }
    }

    public void LoadMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    private void SaveFile()
    {
        FileStream dataStream = new FileStream(FilePath, FileMode.Create);

        BinaryFormatter converter = new BinaryFormatter();
        converter.Serialize(dataStream, Data);

        dataStream.Close();
    }

    public void SaveMaxScore(int score)
    {
        Data.maxScore = score;
        SaveFile();
    }

    public void SaveLanguage(int language)
    {
        Data.language = language;
        SaveFile();
    }

    public void SaveVolume(float volume, int sliderNumber)
    {
        switch (sliderNumber)
        {
            case 0:
                Data.masterVolume = volume;
                break;
            case 1:
                Data.musicVolume = volume;
                break;
            case 2:
                Data.sfxVolume = volume;
                break;
        }
        
        SaveFile();
    }

    public void StartNewGame()
    {
        LoadGameScene();
    }

    public static void ResetScore()
    {
        if (!File.Exists(FilePath)) return;
            
        // File exists 
        FileStream dataStream = new FileStream(FilePath, FileMode.Open);

        BinaryFormatter converter = new BinaryFormatter();
        Data saveData = converter.Deserialize(dataStream) as Data;

        dataStream.Close();
        
        saveData.maxScore = 0;
        
        dataStream = new FileStream(FilePath, FileMode.Create);
        converter.Serialize(dataStream, saveData);

        dataStream.Close();
    }

    public static void RemoveData()
    {
        if (!File.Exists(FilePath)) return;
        
        File.Delete(FilePath);
    }
}
