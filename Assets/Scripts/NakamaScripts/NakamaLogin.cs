using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using UnityEngine.SceneManagement;
public class NakamaLogin : MonoBehaviour
{
    [SerializeField] NakamaConnection Nconnect;
    private IClient iclient;
    private ISession isession;
    private ISocket isocket;
 


    public async void OnLogin()
    {
        iclient = Nconnect.client();
        isession = await iclient.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
        isocket = iclient.NewSocket();
        await isocket.ConnectAsync(isession, true);
        Debug.Log(isession + " " + isocket);

        PassData.isocket = isocket;

        PlayerPrefs.SetString("Username", isession.Username);
        PlayerPrefs.SetString("UserID", isession.UserId);
 

        SceneManager.LoadScene("Matchmaking");
    }
}
