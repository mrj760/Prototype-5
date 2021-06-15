using System.IO;
using UnityEngine;

[System.Serializable]
public class DataManager : MonoBehaviour
{

    private class Data
    {
        public int easyHighScore = 0;
        public int mediumHighScore = 0;
        public int hardHighScore = 0;
        public string easyName = "";
        public string mediumName = "";
        public string hardName = "";
        public float volume = 1f;
    }

    private static Data instance;

    private static string path;
    
    private void Awake()
    {
        path = Application.persistentDataPath + "/cookie-meat-data.json";
        
        Load();
        if (instance is null)
        {
            instance = new Data();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Load()
    {
        Data data = LoadData();
        if (data is null) return;
        instance = new Data
        {
            easyHighScore = data.easyHighScore,
            mediumHighScore = data.mediumHighScore,
            hardHighScore = data.hardHighScore,
            easyName = data.easyName,
            mediumName = data.mediumName,
            hardName = data.hardName,
            volume = data.volume
        };
        Debug.Log("Loaded");
    }

    public static void UpdateVolume(float volume)
    {
        instance.volume = volume;
    }
    public static float GetVolume()
    {
        return instance.volume;
    }
    public static void UpdateScore(GameManager.Difficulty diff, int score, string name)
    {
        switch (diff)
        {
            case GameManager.Difficulty.Easy:
                instance.easyHighScore = score;
                instance.easyName = name;
                break;
            case GameManager.Difficulty.Medium:
                instance.mediumHighScore = score;
                instance.mediumName = name;
                break;
            case GameManager.Difficulty.Hard:
                instance.hardHighScore = score;
                instance.hardName = name;
                break;
            default:
                Debug.Log("Invalid Difficulty to save score");
                break;
        }
    }
    public static int GetHighScore(GameManager.Difficulty diff)
    {
        switch (diff)
        {
            case GameManager.Difficulty.Easy:
                return instance.easyHighScore;
            case GameManager.Difficulty.Medium:
                return instance.mediumHighScore;
            case GameManager.Difficulty.Hard:
                return instance.hardHighScore;
            default:
                Debug.Log("Invalid Difficulty: DataManager -> GetHighScore()");
                return -1;
        }
    }
    
    public static string GetHighScoreHolder(GameManager.Difficulty diff)
    {
        switch (diff)
        {
            case GameManager.Difficulty.Easy:
                return instance.easyName;
            case GameManager.Difficulty.Medium:
                return instance.mediumName;
            case GameManager.Difficulty.Hard:
                return instance.hardName;
            default:
                Debug.Log("Invalid Difficulty: DataManager -> GetHighScore()");
                return "null";
        }
    }
    public static void Save()
    {
        if (instance == null) return;
        Debug.Log("Saving");
        string json = JsonUtility.ToJson(instance);
        File.WriteAllText(path, json);
    }
    private static Data LoadData()
    {
        if (File.Exists(path))
        {
            Debug.Log("Loading");
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<Data>(json);
        }

        return null;
    }
}
