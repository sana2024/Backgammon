using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System.Threading.Tasks;
using System;

[RequireComponent(typeof(Collider2D))]
public class Piece : MonoBehaviour
{
    //----------------------------
    // identifier fields
    //----------------------------
    public int pieceId;
    public PieceType pieceType;


    //----------------------------
    //  Class Instance
    //----------------------------

    public static Piece instance;


    //----------------------------
    // identifier properties
    //----------------------------

    public int Position {
        get {
            if (currentSlot == null)
                return -1;
            return currentSlot.slotId;
        }
    }
    public int DenormalizedPosition {
        get {
            return pieceType == PieceType.Black ? 24 - Position : Position;
        }
    }

    //----------------------------
    // for touch and drag
    //----------------------------

    private static bool multipleSelection = false;
    private static IEnumerable<Piece> multipleSelectionList;

    // for returning from invalid move
    private Vector2 startPos;
    private float offsetY;

    // flag indicating moving this piece
    private bool isBeingHeld = false;

    [HideInInspector]
    public Slot currentSlot = null;
    private Slot collisionSlot = null;

    private Slot UndoSlot;

    private CircleCollider2D circleCollider2D;

    GameObject SlotPos;
    Slot undoSlot;




    //----------------------------
    // Nakama Elements
    //----------------------------
    ISocket isocket;


    #region Unity API

    private void Awake()
    {

        if(instance == null)
        {
            instance = this;        }

        circleCollider2D = GetComponent<CircleCollider2D>();

     
    

    }

    private void Start()
    {
        isocket = PassData.isocket;
        var mainThread = UnityMainThreadDispatcher.Instance();
        isocket.ReceivedMatchState += m => mainThread.Enqueue(async () => await OnReceivedMatchState(m));


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
            case 1:
                if(pieceId == int.Parse(state["PeiceID"]))
                {
                    if (this != null)
                    {
                        transform.position = new Vector2(float.Parse(state["Pos_x"]), -float.Parse(state["pos_y"]));

                      
                    }

                }

                break;


            case 8:



                GameObject pieceOb = GameObject.Find(state["PeiceID"]);
                Piece piece = pieceOb.GetComponent<Piece>();

                GameObject from = GameObject.Find(state["From"]);
                Slot FromSlot = from.GetComponent<Slot>();

                FromSlot.pieces.Remove(piece);

                GameObject to = GameObject.Find(state["To"]);
                Slot ToSlot = to.GetComponent<Slot>();

                int steps = int.Parse(state["Steps"]);

                MoveActionTypes ActionType = (MoveActionTypes)Enum.Parse(typeof(MoveActionTypes), state["ActionType"]);

                ToSlot.pieces.Add(piece);
                piece.PlaceOn(ToSlot);

                var movesPlayedList = GameManager.instance.currentPlayer.movesPlayed;
                movesPlayedList.Add(new Move { piece = this, from = currentSlot, to = ToSlot, step = steps, action = ActionType });

                if (pieceId == piece.pieceId)
                {
                    if (this != null)
                    {
                        Debug.Log(ActionType);
                        OnReciveMove(piece ,movesPlayedList.Last().to, movesPlayedList.Last().action, movesPlayedList.Last().step);
                    }

                }
 
                break;
        }


        }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && IsMouseOverThis() && IsCurrentPlayerTurn() && IsCurrentPlayerRolled() && IsCurrentPlayerPiece() && IsCurrentPlayerMoveLeft())
        {
 
            OnPieceClick();
        }

        if (multipleSelection && Input.GetButtonUp("Fire1"))
        {
            foreach (var piece in multipleSelectionList)
            piece.OnPieceRelease();

            var state = MatchDataJson.SetPeicePos(pieceId, transform);
            SendMatchState(OpCodes.Peice_Pos, state);

            multipleSelection = false;
            multipleSelectionList = null;

 
        }
        else if (isBeingHeld && Input.GetButtonUp("Fire1"))
        {
            OnPieceRelease();

            var state = MatchDataJson.SetPeicePos(pieceId, transform);
            SendMatchState(OpCodes.Peice_Pos, state);


        }

        if (isBeingHeld)
        {
            var mousePos = GetMousePos();

            gameObject.transform.position = new Vector3(mousePos.x, mousePos.y + (currentSlot.IsTopSlot() ? -offsetY : offsetY), 0);

           // var state = new Dictionary<string, string> { { "PeiceID", pieceId.ToString() }, { "PeiceX", gameObject.transform.position.x.ToString() }, { "PeiceY", gameObject.transform.position.y.ToString() } }.ToJson();
           // SendMatchState(1, state);
           // Debug.Log(gameObject.transform.position);

            var state = MatchDataJson.SetPeicePos(pieceId , transform);
            SendMatchState(OpCodes.Peice_Pos, state);


        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // check collision with slot
        var slot = collision.gameObject.GetComponent<Slot>();
        if (slot != null)
        {
            //Debug.Log($"Piece #{ pieceId } in Slot #{ slot.slotId }");
            collisionSlot = slot;
        }
    }

    #endregion

    #region Draw Methods

    private void DecreaseColliderRadius()
    {
        circleCollider2D.radius = .1f;
    }

    public void IncreaseColliderRadius()
    {
        circleCollider2D.radius = 2.24898f;
    }

    private float GetOffsetMultiplier(SlotType type)
    {
        switch(type)
        {
            case SlotType.Board:
                return .7f;
            case SlotType.Bar:
                return .1f;
        };
        return 0f;
    }

    public void PlaceOn(Slot slot)

    {
        var slotPos = slot.transform;
        //-------------------------------------------------
        // calculate offset of y value
        //-------------------------------------------------
        float posY = slot.pieces.Count * GetOffsetMultiplier(slot.slotType);
        // if slot is on top region
        if (slot.slotId >= 13 && slot.slotId <= 24 || 
            (slot.slotType == SlotType.Bar && pieceType == PieceType.Black))
            posY *= -1;

        //-------------------------------------------------
        // change slot
        //-------------------------------------------------
        // if slot has been in another slot
        if (currentSlot != null)
            currentSlot.pieces.Remove(this);

        // change current slot
        currentSlot = slot;

        // add piece to slot
        if (!slot.pieces.Contains(this))
            slot.pieces.Add(this);

        //-------------------------------------------------
        // update ui of piece
        //-------------------------------------------------
        transform.parent = slotPos;
        transform.localPosition = new Vector3(0, posY, 0);
        this.GetComponent<SpriteRenderer>().sortingOrder = slot.pieces.Count;
 
    }

    private void ResetToOldPosition()
    {
        transform.position = new Vector3(startPos.x, startPos.y, 0);

        var state = MatchDataJson.SetPeicePos(pieceId, transform);
        SendMatchState(OpCodes.Peice_Pos, state);
 
    }

    #endregion

    private bool IsCurrentPlayerMoveLeft()
    {
        return GameManager.instance.currentPlayer.IsMoveLeft();
    }

    public IEnumerable<Slot> GetForwardSlots()
    {
        if (IsOutside())
            return null;

        var list = new List<Slot>();
        var allSlots = BoardManager.instance.slotArray;
        var outside = Slot.GetOutside(pieceType);

        foreach (var slot in allSlots.Select(x => x.GetComponent<Slot>()))
        {
            if (Rule.IsMovingToHome(this, slot))
                list.Add(slot);
        }

        if (pieceType == PieceType.White)
            list.Reverse();

        list.Add(outside);

        return list;
    }

    private bool IsOutside()
    {
        var list = pieceType == PieceType.White ?
            BoardManager.instance.whiteOutside.GetComponent<Slot>() :
            BoardManager.instance.blackOutside.GetComponent<Slot>();

        return list.pieces.Contains(this);
    }

    private bool IsCurrentPlayerRolled()
    {
        return GameManager.instance.currentPlayer.rolledDice;
    }

    private bool IsCurrentPlayerPiece()
    {
        return GameManager.instance.currentPlayer.pieceType == pieceType;
    }

    private bool IsCurrentPlayerTurn()
    {
        return GameManager.instance.currentPlayer == GameManager.instance.turnPlayer;
    }

    private void OnPieceClick()
    {
        // if current player does not rolled the dice yet
        if (!IsCurrentPlayerRolled())
        {
            Debug.Log("Player is not rolled the dice");
            BeforeRelease();
        }

        // if there is piece on bar, it must be placed on first
        else if (!IsBarEmpty() && currentSlot.slotType != SlotType.Bar)
        {
            Debug.Log("first, peices on bar must be placed");
            BeforeRelease();
        }

        else if (currentSlot.slotType == SlotType.Outside)
        {
            Debug.Log("no other possible moves");
            BeforeRelease();
        }

        // if it is not top piece
        else if (!Slot.IsTopPiece(currentSlot, this))
        {
            if (!DiceController.instance.IsDoubleMove())
            {
                Debug.Log("piece is not top of stack");
                BeforeRelease();
            }

            // TODO: if current dice has value to move above pieces
            //look if above pieces can be moved ?
            // yes => move above pieces at same time
            else
            {
                multipleSelection = true;
                multipleSelectionList = Slot.GetAbovePieces(this.currentSlot, this).Reverse();

                foreach (var piece in multipleSelectionList)
                    piece.Hold();
 
            }
        }


        else
        {
            Hold();
        }
    }

    private void Hold()
    {
        // hold the piece
        isBeingHeld = true;

        // store current position for invalid move
        startPos.x = transform.position.x;
        startPos.y = transform.position.y;

        // store offset
        offsetY = Mathf.Abs(GetMousePos().y - this.transform.position.y);

        // for easing placing
        DecreaseColliderRadius();

 
    }

    private void BeforeRelease()
    {
        // reset holding flag
        isBeingHeld = false;

        // for easing placing
        IncreaseColliderRadius();
    }

    private void AfterRelease()
    {
        // reset current position
        startPos.x = 0;
        startPos.y = 0;

 

        // reset offset of hold
        offsetY = 0;
    }

    private bool IsBarEmpty()
    {
        var barList = (GameManager.instance.currentPlayer.pieceType == PieceType.White) ?
            BoardManager.instance.whiteBar.GetComponent<Slot>().pieces :
            BoardManager.instance.blackBar.GetComponent<Slot>().pieces;

        if (barList.Count != 0)
            return false;

        return true;
    }

    private void OnPieceRelease()
    {
        BeforeRelease();

        if (multipleSelection)
            collisionSlot = multipleSelectionList.Last().collisionSlot;

        // if collision not happen
        if (collisionSlot == null)
        {
            // reset the position
            ResetToOldPosition();
        }
        else
        {
            // current player
            var currentPlayer = GameManager.instance.currentPlayer;
            // get moves left
            var movesLeft = DiceController.instance.GetMovesLeftList(currentPlayer.movesPlayed.Select(x => x.step));

            MoveActionTypes action = MoveActionTypes.Move;
            MoveError error = MoveError.Unknown;
            int stepPlayed = -1;

            // loop through dice values
            foreach (var step in movesLeft)
            {
                stepPlayed = step;
                error = Rule.ValidateMove(this, collisionSlot, step, out action);

                // if the move valid, do not continue
                if (error == MoveError.NoError)
                    break;
            }

            // move to place if move was valid,
            if (error == MoveError.NoError)
            {
                OnSuccessfulMove(action, stepPlayed);
            }
            // else try combining dice values to get there
            else
            {
                ICollection<Move> movesPlayed;

                error = Rule.ValidateCombinedMove(this, collisionSlot, movesLeft, out movesPlayed);

                // if there are any combined move, move
                if (error == MoveError.NoError)
                {
                    foreach (var move in movesPlayed)
                    {
                        OnSuccessfulMove(move.to, move.action, move.step);
                    }
                }
                // roll back to the position you were before
                else
                {
                    OnFailedMove(error);
                }

            }
        }

        AfterRelease();
    }

    private void OnFailedMove(MoveError error)
    {
        ResetToOldPosition();
        Debug.Log(error);
    }

    private void OnSuccessfulMove(MoveActionTypes action, int stepPlayed)
    {
        OnSuccessfulMove(collisionSlot, action, stepPlayed);
    }

    private void OnSuccessfulMove(Slot to, MoveActionTypes action, int stepPlayed)
    {
        var movesPlayedList = GameManager.instance.currentPlayer.movesPlayed;

        // log played moves for undo
        movesPlayedList.Add(new Move { piece = this, from = currentSlot, to = to, step = stepPlayed, action = action });
        var state = MatchDataJson.SetPieceStack(this.name, currentSlot.name , to.name , stepPlayed.ToString(), action.ToString());
        SendMatchState(OpCodes.stack, state);

        //---------------------------------------
        // action events
        //---------------------------------------
        // move enemy to bar
        if ((action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
        {
            var enemyPiece = to.GetComponent<Slot>().pieces.Last();
            var enemyBar = Slot.GetBar(Piece.GetEnemyType(pieceType));

            enemyPiece.PlaceOn(enemyBar.GetComponent<Slot>());
        }

        // move yourself to outside
        if ((action & MoveActionTypes.Bear) == MoveActionTypes.Bear)
        {

 

            ConvertPieceOutside.instance.FromSlotToOut(this);

            var slotOutside = Slot.GetOutside(pieceType).GetComponent<Slot>();

            for(int i=0; i < slotOutside.pieces.Count; i++)
            {
              //  slotOutside.pieces[i].transform.position =new Vector3(slotOutside.pieces[i].transform.position.x, slotOutside.pieces[i].transform.position.y+i, slotOutside.pieces[i].transform.position.z);
            }
     

            PlaceOn(slotOutside);

            // check round finish
            GameManager.instance.CheckRoundFinish();
        }
        // place on new slot
        else
            PlaceOn(to);
    }

    private void OnReciveMove(Piece piece, Slot to, MoveActionTypes action, int stepPlayed)
    {
        var movesPlayedList = GameManager.instance.currentPlayer.movesPlayed;

        var lastMove = movesPlayedList.Last();
 

        //moves enemy to bar
        if ((action & MoveActionTypes.Hit) == MoveActionTypes.Hit)
        {
            var enemyPiece = to.GetComponent<Slot>().pieces.First();
            var enemyBar = Slot.GetBar(Piece.GetEnemyType(pieceType));

            enemyPiece.PlaceOn(enemyBar.GetComponent<Slot>());

            Debug.Log(enemyPiece);
            Debug.Log(enemyBar);

            lastMove.piece.PlaceOn(enemyBar.GetComponent<Slot>());
        }

        // move yourself to outside
        if ((action & MoveActionTypes.Bear) == MoveActionTypes.Bear)
        {
          //  DecreaseColliderRadius();
            ConvertPieceOutside.instance.FromSlotToOut(this);

            var slotOutside = Slot.GetOutside(pieceType);

            PlaceOn(slotOutside);

            // check round finish
            GameManager.instance.CheckRoundFinish();
        }
        // place on new slot
        else

         lastMove.piece.PlaceOn(lastMove.to);

    }

    private bool IsMouseOverThis()
    {
        var hit = Physics2D.Raycast(GetMousePos(), Vector2.zero, 0, Constants.LAYER_PIECE);
        if (hit.collider != null)
        {
            print(hit.collider.name);

            if (currentSlot.slotType == SlotType.Bar && Slot.IsTopPiece(currentSlot, this))
                return true;

            if (hit.collider.name == name)
                return true;
        }
        return false;
    }

    private Vector3 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

       return worldPos;
    }

    #region Static Methods

    public static Piece CreateFromPrefab(GameObject prefab, int id, PieceObject pieceObject)
    {
        var go = Instantiate(prefab);

        // set sprite
        var spriteRenderer = go.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = pieceObject.sprite;

        // set piece parameters
        var piece = go.GetComponent<Piece>();
        piece.name = $"Piece #{ id }";
        piece.pieceId = id;
        piece.pieceType = pieceObject.pieceType;

        return piece;
    }


    public static PieceType GetEnemyType(PieceType you)
    {
        if (you == PieceType.White)
            return PieceType.Black;

        return PieceType.White;
    }

    public static Piece CreateEmpty()
    {
        var go = new GameObject("Piece Empty");
        go.AddComponent<BoxCollider2D>();
        return go.AddComponent<Piece>();
    }

    #endregion
}
