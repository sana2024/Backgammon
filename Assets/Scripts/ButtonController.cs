using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{

    [SerializeField] GameObject undoButton;
    [SerializeField] GameObject rollButton;
    [SerializeField] GameObject doneButton;


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


}
