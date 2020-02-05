using UnityEngine;
using System;
using System.IO;

public partial class DatasManager : MonoBehaviour
{

    // The path path of the folder where all the datas will be stored in
    private string _BASE_PATH = "";
    public const string GAME_DATAS_FOLDER = "GameDatas/";

    private static DatasManager s_instance;

    public static DatasManager Instance
    {
        get { return s_instance; }
    }

    public void Awake()
    {
        if (s_instance != null)
        {
            return;
        }


        DontDestroyOnLoad(gameObject);

        s_instance = this;
        _BASE_PATH = Application.persistentDataPath + "/";
    }

    // -----------------------------------------------------------------------------------------
    // SongSave
    // -----------------------------------------------------------------------------------------

    public MiniCubeSave cubeSaveContainer;

    // The folder where all the song datas are stored in
    public const string CUBE_SAVE_FILE = "MiniCube";
    public const string EXTENSION = ".json";

    //private SaveDatasContainer datasToSave = new SaveDatasContainer();

    public void LoadMiniCube()
    {
        string uniqueJSONFile = CUBE_SAVE_FILE + GameManager.Instance.numberCube + EXTENSION;
        cubeSaveContainer = _Deserialize<MiniCubeSave>(GAME_DATAS_FOLDER, uniqueJSONFile);
    }

    public void SaveMiniCube()
    {
        if (cubeSaveContainer != null)
        {
            string uniqueJSONFile = CUBE_SAVE_FILE + GameManager.Instance.numberCube + EXTENSION;
            _Serialize(cubeSaveContainer, GAME_DATAS_FOLDER, uniqueJSONFile);
        }
    }

    // -----------------------------------------------------------------------------------------
    // Serialize information
    // -----------------------------------------------------------------------------------------

    // Create an object from a JSON
    // If the JSON doesn't exist, serialize the object first and create a JSON with all it's base attribute.   
    private T _Deserialize<T>(string folderName, string fileName, bool manageStore = true) where T : new()
    {
        string finalPath = _BASE_PATH + folderName + fileName;
        T obj = default(T);

        if (File.Exists(finalPath))
        {
            // The file exist, we deserialize it
            string json = _ReadTextFile(finalPath);
            try
            {
                obj = JsonUtility.FromJson<T>(json);

            }
            catch (Exception e)
            {
                Debug.LogError("DatasManager: Exception " + e);
            }
        }

        if (obj == null)
        {
            // The file doesn't exist, or the foramt is bad
            // We create a blank object and serialize it
            obj = new T();
            _Serialize(obj, folderName, fileName);
        }

        return obj;
    }

    // Serialize an object to save it into the JSON
    private void _Serialize(object obj, string folderName, string fileName)
    {
#if UNITY_EDITOR
            bool prettyPrint = true;
#else
        bool prettyPrint = false;
#endif
        string json = JsonUtility.ToJson(obj, prettyPrint);
        _Serialize(json, folderName, fileName);
    }

    private void _Serialize(string json, string folderName, string fileName)
    {
        // If a folder is set, check if it exist, if not, create it.
        if (!string.IsNullOrEmpty(folderName))
        {
            if (!File.Exists(_BASE_PATH + folderName))
            {
                Directory.CreateDirectory(_BASE_PATH + folderName);
            }
        }

        string finalPath = _BASE_PATH + folderName + fileName;
        _WriteTextFile(finalPath, json);
    }

    // -----------------------------------------------------------------------------------------
    // Download logic
    // -----------------------------------------------------------------------------------------
    private string _ReadTextFile(string fileNameFullPath)
    {
        // Check if the file exist at the path
        if (File.Exists(fileNameFullPath))
        {
            try
            {
                Debug.Log("<color=yellow>Reading Text File: " + fileNameFullPath + "</color>");
                string text = null;

                // Open a stream, and read the file
                using (FileStream file = new FileStream(fileNameFullPath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(file))
                    {

                        text = sr.ReadToEnd();
                        // When the file is read, close the stream
                        sr.Close();
                    }

                    // Then close the file
                    file.Close();
                }
                // And return the full string
                return text;

            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        return null;
    }

    private void _WriteTextFile(string fileNameFullPath, string text)
    {
        try
        {
            // Open a stream at the indicated path
            //Debug.Log("<color=yellow>Writing Text File: " + fileNameFullPath + "</color>");
            using (FileStream file = new FileStream(fileNameFullPath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(file))
                {

                    // Write inside the stream, then close it
                    sw.Write(text);
                    sw.Close();
                }

                // Close the file
                file.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}