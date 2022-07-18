using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Nakama;

public class ButtonController : MonoBehaviour
{

    //in game action buttons
    [SerializeField] GameObject undoButton;
    [SerializeField] GameObject rollButton;
    [SerializeField] GameObject doneButton;

    //Menu button
    [SerializeField] GameObject menuPanel;

 
  
    // enable buttons 
    public void EnableUndoButton()
    {
        undoButton.SetActive(true);
    }

    public void EnableRollButton()
    {
        rollButton.SetActive(true);
    }

    public void EnableDoneButton()
    {
        doneButton.SetActive(true);
    }


    // disable buttons
    public void DissableRollButton()
    {
        rollButton.SetActive(false);
    }

    public void DisableUndoButton()
    {
        undoButton.SetActive(false);
    }

    public void DissableDoneButton()
    {
        doneButton.SetActive(false);
    }

    public void OnMenubuttonClicked()
    {
        menuPanel.SetActive(true);
    }

    public void OnMenuCancleClicked()
    {
        menuPanel.SetActive(false);
    }

    public async void OnLeaveClicked()
    {

        await PassData.isocket.LeaveMatchAsync(PassData.Match.Id);
        SceneManager.LoadScene("Menu");
        Debug.Log("player left game");
 
    }


}
