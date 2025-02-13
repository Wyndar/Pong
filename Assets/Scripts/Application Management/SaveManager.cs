using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class SaveManager:MonoBehaviour
{
    //saves card ID to json for decks
    //public void SaveIDToJson(string path, List<string> data)
    //{
    //    string json = JsonConvert.SerializeObject(data);
    //    File.WriteAllText(Application.dataPath + path, json);
    //}

    ////loads playerdata from json
    //public Player LoadPlayerFromJson(string path)
    //{
    //    string json = File.ReadAllText(Application.dataPath + path);
    //    Player data = JsonConvert.DeserializeObject<Player>(json, new JsonSerializerSettings()
    //    {
    //        ContractResolver = new PrivateResolver(),
    //        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
    //    });
    //    return data;
    //}

    //loads a list of powerups from the db
    public List<PowerUpData> LoadPowerUps(string path)
    {
        var json = Resources.Load<TextAsset>(path);
        List<PowerUpData> data = JsonConvert.DeserializeObject<List<PowerUpData>>(json.text, new JsonSerializerSettings()
        {
            ContractResolver = new PrivateResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        });
        return data;
    }
}