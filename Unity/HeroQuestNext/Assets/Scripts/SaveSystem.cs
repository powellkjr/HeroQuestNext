using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_ROOT = Application.dataPath + "/SavedData/";
    private const string SAVE_EXTENSION = ".txt";
    //private const string SAVE_EXTENSION = ".data";
    private const string DEFAULT_SAVE_NAME = "save_";


    public static void Initialize()
    {
        if(!Directory.Exists(SAVE_ROOT))
        {
            Directory.CreateDirectory(SAVE_ROOT);
        }
    }

    public static void Save(
        string inJSONString,
        string inFilePath = "",
        string inFileName = DEFAULT_SAVE_NAME,
        bool bOverWrite = false)
    {
        Initialize();
        string strTempName = SAVE_ROOT + inFilePath + inFileName;
        if (!Directory.Exists(SAVE_ROOT + inFilePath))
        {
            Directory.CreateDirectory(SAVE_ROOT + inFilePath);
        }
        if (inFileName == DEFAULT_SAVE_NAME)
        {
            int iSaveNumber = 1;

            if (!bOverWrite)
            {
                while (File.Exists(strTempName + iSaveNumber + SAVE_EXTENSION))
                {
                    iSaveNumber++;
                }
                strTempName = strTempName + iSaveNumber + SAVE_EXTENSION;
            }
        }
        File.WriteAllText(strTempName, inJSONString);
    }


    public static string Load(
        string inFilePath = "",
        string inExtention = SAVE_EXTENSION)
    {
        Initialize();
            if(File.Exists(SAVE_ROOT + inFilePath + SAVE_EXTENSION))
        {
            string strJSON = File.ReadAllText(SAVE_ROOT + inFilePath + SAVE_EXTENSION);
            return strJSON;
        }
            else
        { return null; }
    }
    public static string LoadMostRecent(
        string inFilePath ="", 
        string inExtention = SAVE_EXTENSION)
    {
        Initialize();
        DirectoryInfo dDirectoryInfo = new DirectoryInfo(SAVE_ROOT + inFilePath);
        FileInfo[] fSaveFiles = dDirectoryInfo.GetFiles("*" + inExtention);
        FileInfo fMostRecentFile = null;

        foreach (FileInfo aFileInfo in fSaveFiles)
        {
            if (fMostRecentFile==null)
            {
                fMostRecentFile = aFileInfo;
            }
            else
            {
                if(aFileInfo.LastWriteTime>fMostRecentFile.LastWriteTime)
                {
                    fMostRecentFile = aFileInfo;
                }
            }
        }
        if(fMostRecentFile != null)
        {
            string strSaveString = File.ReadAllText(fMostRecentFile.FullName);
            return strSaveString;
        }
        else
        { 
            return null; 
        }
    }

    public static void SaveObject(string inFileName, string inFilePath,object oSaveObject, bool bOverWrite)
    {
        Initialize();
        string sJSON = JsonUtility.ToJson(oSaveObject);
        Save(sJSON, inFilePath, inFileName, bOverWrite);
    }

    public static TSaveObject LoadMostRecentObject<TSaveObject>(string inFilePath ="")
    {
        Initialize();
        string strJSON = LoadMostRecent(inFilePath);
        if (strJSON != null)
        {
            TSaveObject oSaveObject = JsonUtility.FromJson<TSaveObject>(strJSON);
            return oSaveObject;
        }
        else
        {
            return default(TSaveObject); 
        }
    }

    public static TSaveObject LoadObject<TSaveObject>(
    string inFilePath = "",
    string inExtention = SAVE_EXTENSION)
    {
        Initialize();
        string strJSON = Load(inFilePath,inExtention);
        if (strJSON != null)
        {
            TSaveObject oSaveObject = JsonUtility.FromJson<TSaveObject>(strJSON);
            return oSaveObject;
        }
        else
        {
            return default(TSaveObject);
        }
    }
}

