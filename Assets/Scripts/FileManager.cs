using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Urg;

public class FileManager : MonoBehaviour
{
    [SerializeField] DebugRenderer debugRenderer;
    DirectoryInfo di;
    string folderPath;
    ConfigApp config;
    // Start is called before the first frame update
    void Start()
    {
        folderPath = Application.dataPath + "/../config";
        if (!File.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        
        if (!File.Exists(folderPath + "/config.json"))
        {
            SaveCalibrationData();
        }


        LoadConfig();

        void LoadConfig()
        {
            string jsonStringRead = File.ReadAllText(folderPath + "/config.json");
            config = JsonUtility.FromJson<ConfigApp>(jsonStringRead);


            debugRenderer.SetMaxDistance(config.maxDis);
            debugRenderer.SetScreenWidth(config.width);
            debugRenderer.SetScreenHeight(config.height);
            debugRenderer.SetOffsetX(config.offsetX);
            debugRenderer.SetOffsetY(config.offsetY);
            debugRenderer.flipX = config.flipX;
            debugRenderer.flipY = config.flipY;
        }



    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SaveCalibrationData();
        }
    }

    void SaveCalibrationData()
    {
        Debug.Log("saved");
        ConfigApp configApp = new ConfigApp();
        configApp.directory = folderPath;
        configApp.flipX = debugRenderer.flipX;
        configApp.flipY = debugRenderer.flipY;
        configApp.maxDis = debugRenderer.maxDis;
        configApp.width = debugRenderer.xSize;
        configApp.height = debugRenderer.ySize;
        configApp.offsetX = debugRenderer.xOffset;
        configApp.offsetY = debugRenderer.yOffset;
        string jsonString = JsonUtility.ToJson(configApp);
        File.WriteAllText(folderPath + "/config.json", jsonString);
    }



    [SerializeField]
    public class ConfigApp
    {
        public string directory;
        public bool flipX;
        public bool flipY;
        public float maxDis;
        public float width;
        public float height;
        public float offsetX;
        public float offsetY;
    }

}
