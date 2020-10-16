using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_ROOT = Application.dataPath + "/SavedData/";
    private const string SAVE_EXTENSION = ".data";
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
        string inFileName = DEFAULT_SAVE_NAME)
    {
        string strTempName = SAVE_ROOT + inFilePath + inFileName;
        if (inFileName == DEFAULT_SAVE_NAME)
        {
            int iSaveNumber = 1;
                
            while (File.Exists(strTempName + iSaveNumber + SAVE_EXTENSION))
            {
                iSaveNumber++;
            }
        strTempName = strTempName + iSaveNumber + SAVE_EXTENSION;
        }

    File.WriteAllText(strTempName, inJSONString);
    }

    public static string Load(
        string inFilePath ="", 
        string inExtention=SAVE_EXTENSION)
    {
        DirectoryInfo dDirectoryInfo = new DirectoryInfo(SAVE_ROOT + inFilePath);
        FileInfo[] fSaveFiles = dDirectoryInfo.GetFiles(inFilePath + "*" + SAVE_EXTENSION);
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
}

