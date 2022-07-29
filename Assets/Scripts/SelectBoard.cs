using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBoard : MonoBehaviour
{
    [SerializeField] Matchmaking matchMaking;

    // Select deffrent boards

    public void OnOsloClick()
    {
        PassData.BoardType = "Oslo";
        matchMaking.FindMatch();
    }

    public void OnTokyoClick()
    {
        PassData.BoardType = "Tokyo";
        matchMaking.FindMatch();
    }

    public void OnBostonClick()
    {
        PassData.BoardType = "Boston";
        matchMaking.FindMatch();
    }

    public void OnLondonClick()
    {
        PassData.BoardType = "London";
        matchMaking.FindMatch();
    }

    public void OnParisClick()
    {
        PassData.BoardType = "Paris";
        matchMaking.FindMatch();
    }

    public void OnNewyorkClick()
    {
        PassData.BoardType = "Newyork";
        matchMaking.FindMatch();
    }
}
