using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FriendSystem : MonoBehaviour
{

    [SerializeField] GameObject FriendPanel;
    [SerializeField] GameObject FacebookFriendListPanel;
    [SerializeField] GameObject GameFriendListPanel;
    [SerializeField] Button SwitchListButton;
    [SerializeField] Sprite FacebookOnSprite;
    [SerializeField] Sprite GameFriendOnSprite;
    [SerializeField] GameObject NoUserText;
    [SerializeField] InputField FriendName;
    [SerializeField] GameObject UserFound;
    [SerializeField] Text FoundUserName;
    [SerializeField] RawImage FoundUserAvatar;

    

    bool FacebookOn = false;
    bool GameFriendsOn = true;

    //Nakama Variables
    IClient iclient;
    ISession isession;
    ISocket isocket;


    string addFriendName;

    // Start is called before the first frame update
    void Start()
    {
        iclient = PassData.iClient;
        isession = PassData.isession;
        isocket = PassData.isocket;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnFriendButtonClicked()
    {
        FriendPanel.SetActive(true);
    }

    public void OnCloseButtonClicked()
    {
        FriendPanel.SetActive(false);
    }

    public void OnSwitchedButtonClicked()
    {
        FacebookOn = !FacebookOn;
        GameFriendsOn = !GameFriendsOn;

        if(FacebookOn == true && GameFriendsOn == false)
        {
            FacebookFriendListPanel.SetActive(true);
            GameFriendListPanel.SetActive(false);
            SwitchListButton.image.sprite = FacebookOnSprite;
        }
        if(FacebookOn == false && GameFriendsOn == true)
        {
            FacebookFriendListPanel.SetActive(false);
            GameFriendListPanel.SetActive(true);
            SwitchListButton.image.sprite = GameFriendOnSprite;
        }
    }

    public async void AddFriend()
    {
        var usernames = new[] { addFriendName };
        await iclient.AddFriendsAsync(isession, null, usernames);
    }

    public async void FindFriend()
    {
        var name = FriendName.text;
        var username = new[] { name };
        var result = await iclient.GetUsersAsync(isession, null ,username );

        Debug.Log(result);
        Debug.Log(result.Users);

        if(result.ToString() == "Users: [], ")
        {
            NoUserText.SetActive(true);
        }
        else
        {
        foreach (var u in result.Users)
        {
            Debug.Log(u.DisplayName + " " + u.Online +" "+u.AvatarUrl);

            Debug.Log(result.Users.Equals(u));

                UserFound.SetActive(true);
                FoundUserName.text = u.DisplayName;
                addFriendName = u.DisplayName;
                StartCoroutine(GetTexture(u.AvatarUrl));
        }
        }
 
        

         

    }

    IEnumerator GetTexture(string uri)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            FoundUserAvatar.texture = myTexture;
            
        }

    }

}
