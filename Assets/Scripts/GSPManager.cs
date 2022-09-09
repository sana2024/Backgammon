using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

public class GSPManager : MonoBehaviour
{

    private PlayGamesClientConfiguration clientConfigration;
    public Text status;
    public Text Description;
 



    // Start is called before the first frame update
    void Start()
    {
        
        CongigureGPGS();
        SingintoGPGS(SignInInteractivity.CanPromptOnce,clientConfigration);    }

internal void CongigureGPGS(){

    clientConfigration= new PlayGamesClientConfiguration.Builder().Build();
}

internal void SingintoGPGS(SignInInteractivity Interactivity,PlayGamesClientConfiguration configuration)
{

configuration=clientConfigration;
PlayGamesPlatform.InitializeInstance(configuration);
PlayGamesPlatform.Activate();
PlayGamesPlatform.Instance.Authenticate(Interactivity,(code)=>{
status.text="Athenticating...";
if(code==SignInStatus.Success){

    status.text="Successfully Authenticated";
    Description.text="hello"+Social.localUser.userName +"You have an Id of"+Social.localUser.id;
}

else{

     status.text="Failed to Authenticate";
    Description.text="Failed to Authenticate, reason for failure is "+ code;
}
});

}


public void BasicSigninBtn(){

    SingintoGPGS(SignInInteractivity.CanPromptAlways,clientConfigration);
}


}

