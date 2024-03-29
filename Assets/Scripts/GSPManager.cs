﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;


public class GSPManager : MonoBehaviour
{
    #if UNITY_ANDROID

    private PlayGamesClientConfiguration clientConfigration;
    public string username;
    public string email;
    public string password;
    public bool signedin= false;

    [SerializeField] NakamaLogin nakamaLogin;
 

    // Start is called before the first frame update
    void Start()
    {
        
   }

internal void CongigureGPGS(){

    clientConfigration= new PlayGamesClientConfiguration.Builder().Build();
}

internal void SingintoGPGS(SignInInteractivity Interactivity,PlayGamesClientConfiguration configuration)
{
        nakamaLogin.LoadingPanel.SetActive(true);

        configuration =clientConfigration;
PlayGamesPlatform.InitializeInstance(configuration);
PlayGamesPlatform.Activate();
PlayGamesPlatform.Instance.Authenticate(Interactivity,(code)=>{
 
if(code==SignInStatus.Success){

   Debug.Log("Successfully Authenticated");
        signedin = true;

        username = Social.localUser.userName;
        email = Social.localUser.userName + "@gmail.com";
        password = Social.localUser.id;

        var avatarUrl = "https://play-lh.googleusercontent.com/szHQCpMAb0MikYIhvNG1MlruXFUggd6DJHXkMPG1H4lJPB7Lee_BkODfwxpQazxfO9mA";

        nakamaLogin.EmailLogin(email, password,username , avatarUrl );


}

else{

    Debug.Log("Failed to Authenticate");
 
}
});

}


public void GoogleSigin(){

        CongigureGPGS();
        SingintoGPGS(SignInInteractivity.CanPromptOnce, clientConfigration);
    }
#endif

}

