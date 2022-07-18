using System.Collections;
using System.Collections.Generic;
using Nakama;

public class PassData
{
    public static ISocket isocket;
    public static IMatch Match;
    public static IClient iClient;
    public static ISession isession;
    public static int DiceId;

    //-----------
    //user ID
    //-----------

    public static string UserIDState;
    public static string OtherUserId;
    public static string otherUsername;
    public static IUserPresence hostPresence;
    public static IUserPresence SecondPresence;



    //-----------
    //user profile
    //-----------

    public static string Username;
    public static string ImageURL;

}
