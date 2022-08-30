 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.UI;
using UnityEngine.Networking;
using Nakama.TinyJson;

public class Notifications : MonoBehaviour
{
    IClient client;
    ISession session;
    ISocket isocket;

    [SerializeField] Text NotificationAmountText;
    [SerializeField] Text username;
    [SerializeField] RawImage userAvatar;
    [SerializeField] GameObject NotificationPanel;
    [SerializeField] GameObject notificationPrefab;
    [SerializeField]  Transform notificationParent;
    [SerializeField] FriendSystem friendsSystem;
    [SerializeField] GameObject ChallangePanel;
 
 
#if UNITY_IOS
    [SerializeField] mobileNotificationIos IosNotification;

#endif


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
        Application.runInBackground = true;
        switch (notification.Code)
        {
            case -2: 
 
                NotificationAmount++;
                NotificationAmountText.text = NotificationAmount.ToString();

#if UNITY_IOS
                IosNotification.OnClick("Friend Request" , notification.Subject);

#endif
 
                break;

            case 102:

                Debug.Log("sender id " + notification.SenderId + "subject " + notification.Subject);
                break;

        }

    }

    public async void AcceptRequest(string UserId)
    {
 
            var id = new[] { UserId };
            await client.AddFriendsAsync(session, id, null);
        
    }

    public async void ListNotifications()
    {
            if (NotificationPanel.active)
            {

        foreach (Transform item in notificationParent)
        {
            Destroy(item.gameObject);
        }
 
        var result = await client.ListNotificationsAsync(session);


        foreach (var n in result.Notifications)
        {

                GameObject Notification = Instantiate(notificationPrefab, notificationParent);
                Notification.transform.parent = notificationParent.transform;
                Text Message = GameObject.Find("NotMessage").GetComponent<Text>();
                Button AcceptButton = GameObject.Find("AcceptButton").GetComponent<Button>();

                AcceptButton.onClick.AddListener(() => AcceptRequest(n.SenderId));

                if (n.Subject.Contains("accepted your friend request"))
                {
                    AcceptButton.gameObject.SetActive(false);

                }
                SenderProfile(n.SenderId);
                Message.text = n.Subject;
            }
        }
    }

    public async void RemoveNotification(string NotificationIds , GameObject notification)
    {
        var notificationIds = new[] { NotificationIds };
        await client.DeleteNotificationsAsync(session, notificationIds);
        notification.SetActive(false);
    }

 

    public async void SenderProfile(string SenderId)
    {
        var id = new[] { SenderId };
        var result = await client.GetUsersAsync(session, id, null);

        foreach( var user in result.Users)
        {
            SenderUsername = user.Username;
        }
    }


    IEnumerator GetTexture(string uri , RawImage rawImage)
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
            rawImage.texture = myTexture;

        }

    }

    public void OnNotificationClicked()
    {
        NotificationPanel.SetActive(true);
        ListNotifications();
    }

    public void OnCancelClicked()
    {
        NotificationPanel.SetActive(false);
    }
 

}



    
