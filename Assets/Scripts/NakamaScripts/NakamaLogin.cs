using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Nakama.Ninja.WebSockets;
 

public class NakamaLogin : MonoBehaviour
{
    //--------------
    // Nakama Connections and sessions
    //--------------

    [SerializeField] NakamaConnection Nconnect;
    public IClient iclient;
    public ISession isession;
    public ISocket isocket;


    //-------------
    // UI
    //-------------

    [SerializeField] Button PlatformBtn;
    [SerializeField] Sprite GoogleIcon;
    [SerializeField] Sprite GameCenterIcon;
    [SerializeField] Sprite EditorIcon;
    [SerializeField] GameObject LoadingPanel;
 
    [SerializeField] GameObject ConnectionPanel;
    [SerializeField] GameObject DiceRotate;

    private void Start()
    {


 

#if UNITY_ANDROID
        

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

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
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ConnectionPanel.SetActive(true);
        }
        else
        {



            ConnectionPanel.SetActive(false);
            string displayName = "";
            string username = "";
            string avatarUrl = "";

            var vars = new Dictionary<string, string>();
            vars["key"] = "value";
            vars["key2"] = "value2";

            iclient = Nconnect.client();
            LoadingPanel.SetActive(true);
 
            isession = await iclient.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier, create: true);
            isession = await iclient.SessionRefreshAsync(isession);

            var keepAliveIntervalSec = 10;
            isocket = Socket.From(iclient, new WebSocketAdapter(keepAliveIntervalSec));
            await isocket.ConnectAsync(isession, true , keepAliveIntervalSec);



            if (isession.Created)
            {
                displayName = "Player" + Random.RandomRange(0, 5000);
                username = displayName;
                avatarUrl = "https://i.pinimg.com/564x/bc/7f/80/bc7f8058b40eaf9118e762830db84e3e.jpg";
                await iclient.UpdateAccountAsync(isession, username, displayName, avatarUrl, null, null);

                PassData.isocket = isocket;
                PassData.Username = username;
                PassData.MyURL = avatarUrl;
                PassData.iClient = iclient;
                PassData.isession = isession;
                PassData.ImageURL = avatarUrl;
            }
            else
            {
                var account = await iclient.GetAccountAsync(isession);
                var user = account.User;
                displayName = user.DisplayName;
                username = displayName;
                avatarUrl = "https://i.pinimg.com/564x/86/50/bf/8650bf253abad3936206478befcf7f50.jpg";
                await iclient.UpdateAccountAsync(isession, username, displayName, avatarUrl, null, null);

                PassData.isocket = isocket;
                PassData.Username = username;
                PassData.MyURL = avatarUrl;
                PassData.iClient = iclient;
                PassData.isession = isession;
            }

            ChangeScene();
            LoadingPanel.SetActive(false);
 

        }
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
        PassData.iClient = iclient;
        PassData.isession = isession;

        ChangeScene();
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("Menu");
    }


    public void Update()
    {


        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            ConnectionPanel.SetActive(false);
        }


        var speed = 3;

 
           DiceRotate.transform.Rotate(Vector3.forward * speed);
        

    }





}
