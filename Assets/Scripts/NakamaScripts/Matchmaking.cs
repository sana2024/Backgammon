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

 

    [SerializeField] GameObject SearchingPanel;
    [SerializeField] JoinChannel joinChannel;


    // Start is called before the first frame update
    public async void Start()
    {
 

        isocket = PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchmakerMatched += match => mainThread.Enqueue(() => OnREceivedMatchmakerMatched(match));
        isocket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnRecivedMatchPresence(m ));

    }

    public async void FindMatch()
    {
        Debug.Log("Finding match");
        SearchingPanel.SetActive(true);

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
        PlayerPrefs.SetString("matchID",currentMatch.Id);

        PlayerPrefs.SetString("MatchSelf", currentMatch.Self.UserId);
        foreach (var presence in match.Presences)
        {
            Debug.Log("we Joined A match");

            joinChannel.onJoin(match.Id);

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
            joinChannel.onJoin(PlayerPrefs.GetString("matchID"));
            SceneManager.LoadScene("GameScene");
 
            var UserId = PlayerPrefs.GetString("MatchSelf");

            if (user.UserId !=  UserId)
            {
                PassData.OtherUserId = user.UserId;
            }




        }

    }



}
