using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NakamaLogin : MonoBehaviour
{
    //--------------
    // Nakama Connections and sessions
    //--------------

    [SerializeField] NakamaConnection Nconnect;
    private IClient iclient;
    private ISession isession;
    private ISocket isocket;


    //-------------
    // UI
    //-------------

    [SerializeField] Button PlatformBtn;
    [SerializeField] Sprite GoogleIcon;
    [SerializeField] Sprite GameCenterIcon;
    [SerializeField] Sprite EditorIcon;


    private void Start()
    {




#if UNITY_ANDROID

        PlatformBtn.image.sprite = GoogleIcon;
        PlatformBtn.onClick.AddListener(OnGoogleLogin);

#endif

#if UNITY_IOS

        PlatformBtn.image.sprite = GameCenterIcon;
        PlatformBtn.onClick.AddListener(OnGameCenterLogin);

#endif

#if UNITY_EDITOR

        PlatformBtn.image.sprite = EditorIcon;

#endif


    }



    public async void OnGuestLogin()
    {
        iclient = Nconnect.client();
        isession = await iclient.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
        isocket = iclient.NewSocket();
        await isocket.ConnectAsync(isession, true);
        Debug.Log(isession + " " + isocket);



        string displayName = "Player " + Random.RandomRange(0, 5000);
        string username = displayName;
        const string avatarUrl = "https://i.pinimg.com/564x/86/50/bf/8650bf253abad3936206478befcf7f50.jpg";
        await iclient.UpdateAccountAsync(isession, username, displayName, avatarUrl, null, null);

        PassData.isocket = isocket;
        PassData.Username = username;
        PassData.ImageURL = avatarUrl;

 

        ChangeScene();
    }

    public async void OnGoogleLogin()
    {
        const string playerIdToken = "...";
        var session = await iclient.AuthenticateGoogleAsync(playerIdToken);
        System.Console.WriteLine("New user: {0}, {1}", session.Created, session);
 

        const string avatarUrl = "https://play-lh.googleusercontent.com/szHQCpMAb0MikYIhvNG1MlruXFUggd6DJHXkMPG1H4lJPB7Lee_BkODfwxpQazxfO9mA";
        await iclient.UpdateAccountAsync(isession, null, null, avatarUrl, null, null);
 

        ChangeScene();
    }

    public async void OnGameCenterLogin()
    {
        var bundleId = "...";
        var playerId = "...";
        var publicKeyUrl = "...";
        var salt = "...";
        var signature = "...";
        var timestamp = "...";
        var session = await iclient.AuthenticateGameCenterAsync(bundleId, playerId,
            publicKeyUrl, salt, signature, timestamp);
        System.Console.WriteLine("New user: {0}, {1}", session.Created, session);


        const string avatarUrl = "https://www.iphonelife.com/sites/iphonelife.com/files/u31936/Game_Center.png";
        await iclient.UpdateAccountAsync(isession, null, null, avatarUrl, null, null);

        PassData.isocket = isocket;

        ChangeScene();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("Menu");
    }



}
