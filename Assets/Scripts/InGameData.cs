using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using System.Linq;

public class InGameData : MonoBehaviour
{

    [SerializeField] Text LocalUsername;
    [SerializeField] Text RemoteUsername;
    [SerializeField] Text LevelText;
    [SerializeField] GameManager gameManager;
    IClient client;
    ISession session;
    

    // Start is called before the first frame update
    void Start()
    {
        client = PassData.iClient;
        session = PassData.isession;
        LocalUsername.text = PassData.Match.Self.Username;
        RemoteUsername.text = PassData.otherUsername;


    }

    public void Update()
    {
           ReadData();
    }


    public async void ReadData()
    {
        var result = await client.ReadStorageObjectsAsync(session, new[] {
        new StorageObjectId {
        Collection = "UserData",
        Key = "Data",
        UserId = session.UserId
  }
});
        if (result.Objects.Any())
        {
            var storageObject = result.Objects.First();
            var datas = JsonParser.FromJson<PlayerDataObj>(storageObject.Value);
            LevelText.text = datas.Level;
            var state = MatchDataJson.SetLevel(datas.Level);
            gameManager.SendMatchState(OpCodes.Player_Level, state);

        }

    }
    }
