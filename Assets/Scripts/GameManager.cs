using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using Nakama;
using Nakama.TinyJson;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameEndScreen;

    
    //-----------------
    //Player properties
    //-----------------
    public Player playerWhite;
    public Player playerBlack;
    public Player currentPlayer;
    public Player turnPlayer;
    public Player playerWonRound;


    //-----------------
    //UI elements
    //-----------------
    public Button undoButton;
    public Button nextTurnButton;
    public Button rollButton;
    public Image firstDiceValueImage;
    public Image secondDiceValueImage;
    [SerializeField] ButtonController buttonController; 

    //rounds
    private const int ROUND_LIMIT = 3;
    private int currentRound = 1;

    //nakama components
    ISocket isocket;

    //Bourd
    [SerializeField] GameObject Board;
    [SerializeField] ResizeSlots resizeSlots;
    [SerializeField] GameObject CameraBackground;
   

    #region Unity API

    private void Awake()
    {
        if (instance == null)
            instance = this;

        playerWhite = new Player { id = 0, pieceType = PieceType.White , UserId = PassData.hostPresence.UserId};
        playerBlack = new Player { id = 1, pieceType = PieceType.Black , UserId = PassData.SecondPresence.UserId };
       

//        gameEndScreen.transform.Find(UI_BUTTON_NEXT_ROUND).GetComponent<Button>().onClick.AddListener(OnNextRoundButtonClick);
        nextTurnButton.onClick.AddListener(OnNextTurnButtonClick);
        undoButton.onClick.AddListener(UndoPiece);
        rollButton.onClick.AddListener(RollDices);


        if(PassData.Match.Self.UserId == playerBlack.UserId)
        {
            resizeSlots.rotate();
        }

 
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;


        isocket = PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));

        HideGameEndScreen();
 
        currentPlayer = playerWhite;
        turnPlayer = currentPlayer;
        HideDiceValues();

        if (currentPlayer.UserId == PassData.Match.Self.UserId)
        {
            buttonController.EnableRollButton();

        }
    }

    public async void SendMatchState(long opCode, string state)
    {
        await isocket.SendMatchStateAsync(PassData.Match.Id, opCode, state);
    }


    private async Task OnReceivedMatchState(IMatchState matchState)
    {
        var state = matchState.State.Length > 0 ? System.Text.Encoding.UTF8.GetString(matchState.State).FromJson<Dictionary<string, string>>() : null;

        switch (matchState.OpCode)
        {
            case 6:

 
                 buttonController.EnableRollButton();

               

                Debug.Log(state["Current_Player"]);

                if(state["Current_Player"] == "Black")
                {
                    currentPlayer = playerBlack;
                    turnPlayer = currentPlayer;

                    Debug.Log(currentPlayer.id + " " + currentPlayer.UserId + " " + currentPlayer.pieceType);

                }

                if (state["Current_Player"] == "White")
                {
                    currentPlayer = playerWhite;
                    turnPlayer = currentPlayer;

                    Debug.Log(currentPlayer.id + " " + currentPlayer.UserId + " " + currentPlayer.pieceType);
                }



                break;

            case 7:
                
                GameObject pieceOb = GameObject.Find(state["PeiceID"]);
                Piece piece = pieceOb.GetComponent<Piece>();

                GameObject from = GameObject.Find(state["From"]);
                Slot FromSlot = from.GetComponent<Slot>();

                FromSlot.pieces.Remove(piece);

                GameObject to = GameObject.Find(state["To"]);
                Slot ToSlot = to.GetComponent<Slot>();

                int steps = int.Parse(state["Steps"]);

                MoveActionTypes ActionType = (MoveActionTypes)Enum.Parse(typeof(MoveActionTypes), state["ActionType"]);

               currentPlayer.movesPlayed.Add(new Move
                {
                    piece = piece,
                    from = FromSlot,
                    to = ToSlot,
                    step = steps,
                    action = ActionType,
                });


                var lastMove = currentPlayer.movesPlayed.Last();

                
                    ConvertPieceOutside.instance.FromOutToSlot(lastMove.piece);
                     piece.IncreaseColliderRadius();
                

                lastMove.piece.PlaceOn(lastMove.from);

                // undo hit action
                if ((lastMove.action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
                {
                    var enemyBar = Slot.GetBar(Piece.GetEnemyType(lastMove.piece.pieceType));
                    var enemyPiece = enemyBar.pieces.Last();
                    enemyPiece.PlaceOn(lastMove.to);
                }

                currentPlayer.movesPlayed.Remove(lastMove);

                


                break;


             


            case 9:

                if (state["Camera_Background"] == "True"){

                    CameraBackground.SetActive(true);
                }


                if (state["Camera_Background"] == "False")
                {

                    CameraBackground.SetActive(false);
                }

                break;


            case 10:

              int time=  int.Parse(state["Timer"]);
                Debug.Log(time);

                break;

        }


    }

 

    private void Update()
    {
        if (DiceController.instance.animationStarted && !DiceController.instance.animationFinished)
        {
            ShowDiceValues();
        }


 
    }

    #endregion

    #region UI
    private const string UI_TEXT_ROUND = "RoundText";
    private const string UI_PANEL_SCORE = "GameScore";
    private const string UI_PANEL_SCORE_PLAYER_WHITE = "PlayerWhite";
    private const string UI_PANEL_SCORE_PLAYER_BLACK = "PlayerBlack";
    private const string UI_TEXT_SCORE = "Score";
    private const string UI_BUTTON_NEXT_ROUND = "NextRoundButton";

    private void OnNextTurnButtonClick()
    {
        if (!turnPlayer.rolledDice)
        {
            Debug.Log("you have to roll the dice");
            return;
        }

        if (turnPlayer.IsMoveLeft())
        {
            Debug.Log("You have to move");
            return;
        }

        NextTurn();
    }

    private void OnNextRoundButtonClick()
    {
        if (IsAnyPlayerWon())
            SceneManager.LoadScene(Constants.SCENE_WHO_IS_FIRST);

        // increment current round
        currentRound++;

        // reset players
        Player.ResetForNextRound(playerWhite);
        Player.ResetForNextRound(playerBlack);

        // who wins the round starts first
        turnPlayer = playerWonRound;
        playerWonRound = null;
        currentPlayer = turnPlayer;

        RestartBoard();
        HideGameEndScreen();
    }

    private void UpdateGameEndScreen()
    {
        // get ui elements
        var roundText = gameEndScreen.transform.Find(UI_TEXT_ROUND).GetComponent<Text>();
        var playerWhiteScoreText = gameEndScreen.transform.Find(UI_PANEL_SCORE).Find(UI_PANEL_SCORE_PLAYER_WHITE).Find(UI_TEXT_SCORE).GetComponent<Text>();
        var playerBlackScoreText = gameEndScreen.transform.Find(UI_PANEL_SCORE).Find(UI_PANEL_SCORE_PLAYER_BLACK).Find(UI_TEXT_SCORE).GetComponent<Text>();

        // update ui elements
        if (!IsAnyPlayerWon())
            roundText.text = $"Round { currentRound }";
        else
            roundText.text = $"Player { Player.Winner(playerWhite, playerBlack).id } won";

        playerWhiteScoreText.text = playerWhite.score.ToString();
        playerBlackScoreText.text = playerBlack.score.ToString();
    }

    private bool IsAnyPlayerWon()
    {
        var potentialWeight = (ROUND_LIMIT - currentRound) * 2;

        // if round is equal to limit
        if (currentRound == ROUND_LIMIT)
            return true;

        // if white player is winning, 
        // and if black player + potential still less than white player,
        // white player won
        if (playerWhite.score > playerBlack.score &&
            playerBlack.score + potentialWeight < playerWhite.score)
            return true;

        // if black player is winning, 
        // and if white player + potential still less than black player,
        // black player won
        if (playerBlack.score > playerWhite.score &&
            playerWhite.score + potentialWeight < playerBlack.score)
            return true;

        return false;
    }

    private void ShowGameEndScreen()
    {
        // update game end screen
        UpdateGameEndScreen();

        // enable game end screen
        gameEndScreen.SetActive(true);
    }

    private void HideGameEndScreen()
    {
        // disable game end screen
        gameEndScreen.SetActive(false);
    }

    private void RollDices()
    {
         if(currentPlayer.UserId == PassData.Match.Self.UserId)
            {
        if (!currentPlayer.rolledDice)
        {
            DiceController.instance.ThrowDices();
            currentPlayer.rolledDice = true;
            StartCoroutine(AfterRolledDice());

                buttonController.EnableDoneButton();
                buttonController.EnableUndoButton();
        }
        else
        {
            Debug.Log("Current player rolled the dice");
        }
        }

              }

    private IEnumerator AfterRolledDice()
    {
        nextTurnButton.interactable = false;
        
        yield return new WaitForSeconds(2f);
        if (!currentPlayer.IsMoveLeft())
        {
            NextTurn();
        }
        else
        {
            nextTurnButton.interactable = true;
        }
    }

    private void HideDiceValues()
    {
        firstDiceValueImage.gameObject.SetActive(false);
        secondDiceValueImage.gameObject.SetActive(false);
    }

    private void ShowDiceValues()
    {
        firstDiceValueImage.gameObject.SetActive(true);
        secondDiceValueImage.gameObject.SetActive(true);

        firstDiceValueImage.sprite = DiceController.instance.firstValueSprite;
        secondDiceValueImage.sprite = DiceController.instance.secondValueSprite;

 


    }

    #endregion

    #region Public

    public void CheckRoundFinish()
    {
        if (IsFinished())
        {
            var score = CalculateScore();
            // increment won round of player
            playerWonRound.score += score;

            ShowGameEndScreen();
        }
    }

    #endregion

    private int CalculateScore()
    {
        var enemyOutside = (Piece.GetEnemyType(playerWonRound.pieceType) == PieceType.White) ?
            BoardManager.instance.whiteOutside.GetComponent<Slot>() :
            BoardManager.instance.blackOutside.GetComponent<Slot>();

        return (enemyOutside.pieces.Count == 0) ? 2 : 1;
    }

    public void NextTurn()
    {

        Debug.Log(currentPlayer.id + " " + currentPlayer.UserId + " " + currentPlayer.pieceType);


        //--------------------------------
        // reset current player's fields
        //--------------------------------
        // flush moves log
        turnPlayer.movesPlayed.Clear();

        // reset dice
        ResetDice();
        var timer = MatchDataJson.SetTimer(60);
        SendMatchState(OpCodes.Player_Timer, timer);
        //--------------------------------
        // turn the set to the next player
        //--------------------------------
        if (turnPlayer.pieceType == PieceType.White)
        {
            turnPlayer = playerBlack;
            currentPlayer = turnPlayer;

            Debug.Log(currentPlayer.id + " "+currentPlayer.UserId+" "+currentPlayer.pieceType);

            var state = MatchDataJson.SetCurrentPlayer("Black");
              SendMatchState(OpCodes.current_player, state);

           

            return;
        }
        if (turnPlayer.pieceType == PieceType.Black)
        {
            playerWhite.UserId = PassData.hostPresence.UserId;
            turnPlayer = playerWhite;
            currentPlayer = turnPlayer;

 
            Debug.Log(currentPlayer.id + " " + currentPlayer.UserId + " " + currentPlayer.pieceType);

            var state = MatchDataJson.SetCurrentPlayer("White");
            SendMatchState(OpCodes.current_player, state);

            return;
        }
 

    }

    private bool IsFinished()
    {
        var whiteFinished = Slot.GetOutside(PieceType.White).pieces.Count == 15;
        var blackFinished = Slot.GetOutside(PieceType.Black).pieces.Count == 15;

        if (whiteFinished)
            playerWonRound = playerWhite;

        if (blackFinished)
            playerWonRound = playerBlack;

        if (whiteFinished || blackFinished)
            return true;

        return false;
    }

    private void RestartBoard()
    {
        ResetDice();

        BoardManager.instance.ResetBoard();

        // reset pieces
        BoardManager.instance.PlacePiecesOnBoard();
    }

    private void ResetDice()
    {
        turnPlayer.rolledDice = false;
        HideDiceValues();
    }

    private void UndoPiece()
    {
        if (currentPlayer.movesPlayed.Count == 0)
        {
            Debug.Log("You must have played a move for undo");
            return;
        }

        var lastMove = currentPlayer.movesPlayed.Last();

        // undo move action
        lastMove.piece.PlaceOn(lastMove.from);

        Debug.Log(lastMove.from.ToString());
        var state = MatchDataJson.SetPieceStack(lastMove.piece.name, lastMove.from.name, lastMove.to.name, lastMove.step.ToString() , lastMove.action.ToString());
        SendMatchState(OpCodes.undo ,state);

        // undo hit action
        if ((lastMove.action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
        {
            var enemyBar = Slot.GetBar(Piece.GetEnemyType(lastMove.piece.pieceType));
            var enemyPiece = enemyBar.pieces.Last();
            enemyPiece.PlaceOn(lastMove.to);
        }

        //undo bear action
       
           ConvertPieceOutside.instance.FromOutToSlot(lastMove.piece);
            lastMove.piece.IncreaseColliderRadius();

 


        currentPlayer.movesPlayed.Remove(lastMove);
    }
}
