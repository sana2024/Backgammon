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

    public static string SetDicePos(Vector2 Pos)
    {
        var values = new Dictionary<string, string>
        {
            { "Dice_Pos_X", Pos.x.ToString()},
            { "Dice_Pos_Y", Pos.y.ToString()}
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







}
