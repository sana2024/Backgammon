using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using System.Linq;

public class UserProfile : MonoBehaviour
{
    [SerializeField] RawImage ProfileImage;

    IClient client;
    ISession session;

    //USER DATAS
    int levelValue= 2;


    //USER TEXTFRIENDS
    [SerializeField] Text LevelText;

    

    // Start is called before the first frame update
    void Start()
    {
        client = PassData.iClient;
        session = PassData.isession;
 
        StartCoroutine(GetTexture());

        WriteLevel();

        ReadLevel();
 
    }
 

    public async void WriteLevel()
    {

        var levels = new PlayerLevelObj
        {
            Level = levelValue.ToString()
        };

        var levelID = await client.WriteStorageObjectsAsync(session, new[] {
        new WriteStorageObject
  {
      Collection = "Levels",
      Key = "level",
      Value = JsonWriter.ToJson(levels),
       
     
  }
});

        Debug.Log( string.Join(",\n  ", levelID));
        
    }

    public async void ReadLevel()
    {
        var result = await client.ReadStorageObjectsAsync(session, new[] {
        new StorageObjectId {
        Collection = "Levels",
        Key = "level",
        UserId = session.UserId
  }
});

        Debug.Log(string.Join(",\n  ", result.Objects));



        if (result.Objects.Any())
        {
            var storageObject = result.Objects.First();
            var level = JsonParser.FromJson<PlayerLevelObj>(storageObject.Value);
            Debug.Log(level.Level);

            levelValue =  int.Parse(level.Level);
        }


}

    public class PlayerLevelObj
    {
        public string Level;
      
    }

    public void Update()
    {
        LevelText.text = levelValue.ToString();
    }




    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(PassData.ImageURL);
        Debug.Log("image url " + www);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            ProfileImage.texture = myTexture;
        }

    }
}
