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
    [SerializeField] UserProfile userProfile;
    [SerializeField] GameObject NoEnoughCoinPanel;


    // Start is called before the first frame update
    public async void Start()
    {
        

        isocket = PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchmakerMatched += match => mainThread.Enqueue(() => OnREceivedMatchmakerMatched(match));
        isocket.ReceivedMatchPresence += m => mainThread.Enqueue(() => OnRecivedMatchPresence(m ));

    }

    public async void FindMatch(string BoardName)
    {

        PassData.BoardType = BoardName;

        int wallet = Math.Abs(PassData.WalletMoney);
        int boardPrice = Math.Abs(PassData.BoardPrice);

 
        if (wallet >= boardPrice)
        {
        Debug.Log("Finding match");
        SearchingPanel.SetActive(true);

        var properites = new Dictionary<string, string>() {
          {"board", BoardName}
         };
        var query = "+properties.board:"+BoardName;

        var matchmakingTickets = await isocket.AddMatchmakerAsync(query, 2, 2, properites);

        ticket = matchmakingTickets.Ticket;

        }
        else
        {
            StartCoroutine(NoMoneyPanelTimer());
            Debug.Log("you dont have enough money");
        }

    }

    IEnumerator NoMoneyPanelTimer()
    {
        NoEnoughCoinPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        NoEnoughCoinPanel.SetActive(false);
    }
 
 
    public async void RemoveTicket()
    {
 
        await isocket.RemoveMatchmakerAsync(ticket);
        SearchingPanel.SetActive(false);
        
    }

    private async void OnREceivedMatchmakerMatched(IMatchmakerMatched matchmakerMatched)
    {
        var users = matchmakerMatched.Users;

  
       
        
        foreach(var u in users)
        {

            var ids = new[] { u.Presence.UserId};
            var result = await PassData.iClient.GetUsersAsync(PassData.isession, ids);

            foreach (var user in result.Users)
            {
               
            }
        }



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

           

            SceneManager.LoadScene("GameScene");


            if (presence.UserId != match.Self.UserId)
            {
                PassData.OtherUserId = presence.UserId;
                PassData.otherUsername = presence.Username;
                
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
                PassData.otherUsername = user.Username;
            }




        }

    }
 



}
