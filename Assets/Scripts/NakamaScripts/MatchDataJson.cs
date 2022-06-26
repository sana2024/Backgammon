using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nakama.TinyJson;

public class MatchDataJson
{
    public static string SetPeicePos(int PeiceID , Transform transform)
    {
        var values = new Dictionary<string, string>
        {
            { "PeiceID", PeiceID.ToString() },
            { "Pos_x", transform.position.x.ToString() },
            { "pos_y", transform.position.y.ToString() }
        };

        return values.ToJson();
    }

 

    public static string SetDicePos(Vector2 pos)
    {
        var values = new Dictionary<string, string>
        {
            { "Pos_X", pos.x.ToString()},
            { "Pos_Y", pos.y.ToString()}
 
        };

        return values.ToJson();
    }

    public static string SetDiceSprite(int DiceId ,int Index)
    {
        var values = new Dictionary<string, string>
        {
            { "Dice_Id",  DiceId.ToString() },
            { "Dice_sprite_index", Index.ToString() }
        };

        return values.ToJson();
    }

    public static string SetDiceVisability(string visability)
    {
        var values = new Dictionary<string, string>
        {
            { "Dice_sprite_index", visability }
        };

        return values.ToJson();
    }

    public static string SetCurrentPlayer(string currentPlayer)
    {
        var values = new Dictionary<string, string>
        {
            { "Current_Player" , currentPlayer}
     
        };

        return values.ToJson();
    }

    public static string SetUndo(string peiceID,  string from, string to , string steps, string actionType)
    {
        var values = new Dictionary<string, string>
        {
            {"PeiceID" , peiceID },
            {"From" , from },
            {"To" , to },
            {"Steps" , steps },
            {"ActionType" , actionType },

        };

        return values.ToJson();
    }









}
