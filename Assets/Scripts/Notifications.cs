using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Notifications : MonoBehaviour
{
    IClient client;
    ISession session;
    ISocket isocket;

    [SerializeField] Text NotificationAmountText;
    [SerializeField] Text username;
    [SerializeField] RawImage userAvatar;
    [SerializeField] GameObject NotificationPanel;
    [SerializeField] GameObject NotificationDialog;


    int NotificationAmount = 0;
    string SenderUsername ="";

    // Start is called before the first frame update
    void Start()
    {
        client = PassData.iClient;
        session = PassData.isession;


        isocket = PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();

 
        isocket.ReceivedNotification += notification => mainThread.Enqueue(() => onRecivedNotification(notification));
    }


    private void onRecivedNotification(IApiNotification notification)
    {
        Debug.Log("hello");
        switch (notification.Code)
        {
            case -2:
                Debug.Log("recived");
                NotificationDialog.SetActive(true);
                NotificationAmount++;
                NotificationAmountText.text = NotificationAmount.ToString();
                SenderProfile(notification.SenderId);



                break;

        }

    }

    public async void AcceptRequst()
    {
            Debug.Log(SenderUsername);
            var usernames = new[] { SenderUsername };
            await client.AddFriendsAsync(session, null, usernames);
         
    }

    public async void SenderProfile(string SenderId)
    {
        var id = new[] { SenderId };
        var result = await client.GetUsersAsync(session, id, null);

        foreach( var user in result.Users)
        {
            username.text = user.Username;
            SenderUsername = user.Username;
            Debug.Log(SenderUsername);
            GetTexture(user.AvatarUrl);
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
            userAvatar.texture = myTexture;

        }

    }

    public void OnNotificationClicked()
    {
        NotificationPanel.SetActive(true);
    }

    public void OnCancelClicked()
    {
        NotificationPanel.SetActive(false);
    }

}



    
