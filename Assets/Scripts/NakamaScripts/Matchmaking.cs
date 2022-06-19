using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class Matchmaking : MonoBehaviour
{
    [SerializeField] NakamaConnection Nconnect;
    ISocket isocket;
    IClient iclient;
    ISession isession;
    private string ticket;
    static IMatch currentMatch;
    string OtherId;

    IUserPresence hostPresence;
    IUserPresence SecondPresence;

    [SerializeField] GameObject Loggedin;
    string s;


    // Start is called before the first frame update
    public async void Start()
    {
        iclient = Nconnect.client();
        isession = await iclient.AuthenticateDeviceAsync(SystemInfo.deviceUniqueIdentifier);
        isocket = iclient.NewSocket();
        await isocket.ConnectAsync(isession, true);
        Loggedin.SetActive(true);
        Debug.Log(isession + " " + isocket);

        PassData.isocket = isocket;

        PlayerPrefs.SetString("Username", isession.Username);
        PlayerPrefs.SetString("UserID", isession.UserId);

        isocket = PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchmakerMatched += match => mainThread.Enqueue(() => OnREceivedMatchmakerMatched(match));
        isocket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnRecivedMatchPresence(m ));

    }

    public async void FindMatch()
    {
        Debug.Log("Finding match");

        var matchmakingTickets = await isocket.AddMatchmakerAsync("*", 2, 2);

        ticket = matchmakingTickets.Ticket;
    }

    private async void OnREceivedMatchmakerMatched(IMatchmakerMatched matchmakerMatched)
    {

        var match = await isocket.JoinMatchAsync(matchmakerMatched);

        hostPresence = matchmakerMatched.Users.OrderBy(x => x.Presence.SessionId).First().Presence;
        SecondPresence = matchmakerMatched.Users.OrderBy(x => x.Presence.SessionId).Last().Presence;

        PassData.hostPresence = hostPresence;
        PassData.SecondPresence = SecondPresence;

        Debug.Log("Our Match ID: " + match.Self.SessionId);
        currentMatch = match;
        PassData.Match = currentMatch;

        PlayerPrefs.SetString("MatchSelf", currentMatch.Self.UserId);
        foreach (var presence in match.Presences)
        {
            Debug.Log("we Joined A match");



            SceneManager.LoadScene("GameScene");


            if (presence.UserId != match.Self.UserId)
            {
                PassData.OtherUserId = presence.UserId;
            }


        }

    }

    private void OnRecivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        foreach (var user in matchPresenceEvent.Joins)
        {

            SceneManager.LoadScene("GameScene");
 
            var UserId = PlayerPrefs.GetString("MatchSelf");

            if (user.UserId !=  UserId)
            {
                PassData.OtherUserId = user.UserId;
            }




        }

    }



}
