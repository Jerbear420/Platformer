using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;

public class SaveData : MonoBehaviour
{

    private GameData gameData;
    public int CurrentScore { get { return gameData._score; } }
    private string CurrentName { get { return gameData._name; } }
    private float CurrentTimePlayed { get { return gameData._timePlayed; } }
    private string saveFile;
    private FileStream dataStream;
    byte[] savedKey = { 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15, 0x16, 0x15 };

    void Awake()
    {
        saveFile = Application.persistentDataPath + "/save.json";


        gameData = new GameData(0, "ASDF", 0f);
    }
    void Start()
    {

        //1 CreateSave();
        ReadSave();
    }

    public void ReadSave()
    {
        // Does the file exist?
        if (File.Exists(saveFile))
        {
            // Create FileStream for opening files.
            dataStream = new FileStream(saveFile, FileMode.Open);

            // Create new AES instance.
            Aes oAes = Aes.Create();

            // Create an array of correct size based on AES IV.
            byte[] outputIV = new byte[oAes.IV.Length];

            // Read the IV from the file.
            dataStream.Read(outputIV, 0, outputIV.Length);

            // Create CryptoStream, wrapping FileStream
            CryptoStream oStream = new CryptoStream(
                   dataStream,
                   oAes.CreateDecryptor(savedKey, outputIV),
                   CryptoStreamMode.Read);

            // Create a StreamReader, wrapping CryptoStream
            StreamReader reader = new StreamReader(oStream);

            // Read the entire file into a String value.
            string text = reader.ReadToEnd();

            // Deserialize the JSON data 
            //  into a pattern matching the GameData class.
            gameData = JsonUtility.FromJson<GameData>(text);
            reader.Close();
            oStream.Close();
            dataStream.Close();
            Debug.Log("loaded from file");
            Debug.Log(gameData);
            Debug.Log(text);
        }
    }

    public void writeFile()
    {
        // Create new AES instance.
        Aes iAes = Aes.Create();

        // Create a FileStream for creating files.
        dataStream = new FileStream(saveFile, FileMode.Create);

        // Save the new generated IV.
        byte[] inputIV = iAes.IV;

        // Write the IV to the FileStream unencrypted.
        dataStream.Write(inputIV, 0, inputIV.Length);

        // Create CryptoStream, wrapping FileStream.
        CryptoStream iStream = new CryptoStream(
                dataStream,
                iAes.CreateEncryptor(savedKey, iAes.IV),
                CryptoStreamMode.Write);

        // Create StreamWriter, wrapping CryptoStream.
        StreamWriter sWriter = new StreamWriter(iStream);

        // Serialize the object into JSON and save string.
        Debug.Log("Saving data, score is...");
        Debug.Log(CurrentScore);
        gameData._score = CurrentScore;
        string jsonString = JsonUtility.ToJson(gameData);

        // Write to the innermost stream (which will encrypt).
        sWriter.Write(jsonString);

        // Close StreamWriter.
        sWriter.Close();

        // Close CryptoStream.
        iStream.Close();

        // Close FileStream.
        dataStream.Close();
    }

    public void AddScore(int points)
    {
        gameData._score += points;
    }

}
