using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class PlayerData
{
    public int health;
    public int level;
    public bool[] unlockedAbilities;
    // Fügen Sie hier weitere zu speichernde Daten hinzu
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        PlayerData data = new PlayerData();
        // Füllen Sie hier die PlayerData mit aktuellen Spielwerten

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Spiel gespeichert");
    }

    public PlayerData LoadGame()
    {
        string path = Application.persistentDataPath + "/player.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            Debug.Log("Spiel geladen");
            return data;
        }
        else
        {
            Debug.LogError("Speicherdatei nicht gefunden in " + path);
            return null;
        }
    }
}

public class AutoSave : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Players"))
        {
            SaveManager.instance.SaveGame();
        }
    }
}
/*
public class SceneManager : MonoBehaviour
{
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        SaveManager.instance.SaveGame();
    }
}*/