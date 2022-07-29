using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Nakama;
using Nakama.TinyJson;
using System.Linq;
using System;

public class UserProfile : MonoBehaviour
{


    IClient client;
    ISession session;
    ISocket isocket;

  

    //USER DATAS
    int levelValue = 1;
    int wins = 0;


    //USER TEXTFRIENDS
    [SerializeField] Text LevelText;
    [SerializeField] Text TimeText;
    [SerializeField] Text Username;
    [SerializeField] RawImage ProfileImage;
    [SerializeField] RawImage ProfilePanel;
    [SerializeField] GameObject UserPanel;
    [SerializeField] Text CoinUserPanelText;
    [SerializeField] Text CoinText;
    [SerializeField] Text WinText;
 

    // Start is called before the first frame update
    void Start()
    {
        client = PassData.iClient;
        session = PassData.isession;

        StartCoroutine(GetTexture());

        // WriteData();

        ReadData();

        getUserProfile();

        //AddLeaderboard();

       // Wallet();

        // rpc();



 
      
        
    

 

        }

 

    public async void rpc()
    {
        var rpcid = "users";
        var pokemonInfo = await client.RpcAsync(session, rpcid);
        Debug.Log(pokemonInfo.Payload);
    }


    public async void WriteData(int levelvalue, int winsvalue)
    {

        var Datas = new PlayerDataObj
        {
            Level = levelvalue.ToString(),
            wins = winsvalue.ToString()
        };

        var Sendata = await client.WriteStorageObjectsAsync(session, new[] {
        new WriteStorageObject
  {
      Collection = "UserData",
      Key = "Data",
      Value = JsonWriter.ToJson(Datas),


  }
});

        Debug.Log(string.Join(",\n  ", Sendata));

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
 

            levelValue = int.Parse(datas.Level);
        }
        else
        {
            WriteData(levelValue, wins);


        }


    }

    public async void Wallet()
    {
        var account = await client.GetAccountAsync(session);
        var wallet = JsonParser.FromJson<Dictionary<string, int>>(account.Wallet);


        foreach (var currency in wallet.Values)
        {
            CoinText.text = currency.ToString();
            CoinUserPanelText.text = currency.ToString();

        }
    }

    public async void AddLeaderboard()
    {
        const string leaderboardId = "level1";
        const long score = 200L;
        var r = await client.WriteLeaderboardRecordAsync(session, leaderboardId, score);
        System.Console.WriteLine("New record for '{0}' score '{1}'", r.Username, r.Score);
    }



    public void Update()
    {
        LevelText.text = levelValue.ToString();
        WinText.text = wins.ToString();
    }


    public async void getUserProfile()
    {
        var account = await client.GetAccountAsync(session);
        var user = account.User;

        var ids = new[] { "071bb808-118f-40e6-a1ea-744584c69c91", "09121f55-5c50-4b94-9698-dd42e5bd4f32" };
        var result = await client.GetUsersAsync(session, ids);

        foreach (var u in result.Users)
        {
            Debug.Log(u.Id +" "+u.Online);
            System.Console.WriteLine("User id '{0}' username '{1}'", u.Id, u.Online);
        }


        //time created
        string date = user.CreateTime.ToString().Substring(0, 10);
        TimeText.text = date;

        //Username
        Username.text = user.Username;
    }

    public void OpenProfilePanel()
    {
        UserPanel.SetActive(true);
    }

    public void CloseProfilePanel()
    {
        UserPanel.SetActive(false);
    }

 

 

        IEnumerator GetTexture()
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(PassData.ImageURL);
 
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

                ProfileImage.texture = myTexture;
                ProfilePanel.texture = myTexture;
            }

        }


    }


