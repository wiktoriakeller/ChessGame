using System;
using UnityEngine;

[RequireComponent(typeof(PieceFactory))]
[RequireComponent(typeof(Board))]
public class ChessGameController : MonoBehaviour
{
    public static PieceFactory PieceFactory;
    public ChessPlayer ActivePlayer { get; private set; }
    public Board Board { get; private set; }
    public GameState State { get; private set; }
    public Action EnterNormalMode;
    public Action EnterEndGame;
    public Action EnterNoneState;
    public CameraController CameraController { get; private set; }

    [SerializeField] private BoardLayout initialBoardLayout;

    private InputReciever[] inputRecievers;
    private ChessPlayer whitePlayer;
    private ChessPlayer blackPlayer;
    private GameState previousState;

    private void Awake()
    {
        PieceFactory = GetComponent<PieceFactory>();
        Board = GetComponent<Board>();
        inputRecievers = GetComponents<InputReciever>();
        CameraController = GetComponent<CameraController>();
    }

    private void Start()
    {
        State = GameState.None;
        Board.CompleteTurn += EndTurn;
        EnterEndGame += CameraController.DisableCam;
        EnterNormalMode += Board.SquareSelector.DeactiavateCheckMateSelector;
        EnterNoneState += CameraController.DisableCam;
        Board.SetGameController(this);
    }

    private void Update()
    {
        if(State != GameState.None)
        {
            RecieveInputs();
            ActivePlayer.HandleInput(this);
        }
    }

    public void StartNewGame()
    {
        CameraController.ResetCamera();
        CameraController.EnableCam();
        State = GameState.Normal;
        OnEnteredNormalMode();
        InitializePlayers();
        InitializeInputRecievers(whitePlayer.InputHandler as IInputReader, blackPlayer.InputHandler as IInputReader);
        Board.ChessGrid.ResetGrid();
        ImplementBoardLayout();
        ActivePlayer.GenerateAllPossibleMoves(Board.ChessGrid, true, true);
    }

    private void InitializePlayers()
    {
        IInputHandler whitePlayerInput = ChessGameSettings.WhitePlayerUsesAi ? new AiInput() as IInputHandler : new PlayerInputHandler();
        IInputHandler blackPlayerInput = ChessGameSettings.BlackPlayerUsesAi ? new AiInput() as IInputHandler : new PlayerInputHandler();
        whitePlayer = new ChessPlayer(whitePlayerInput, TeamColor.White);
        blackPlayer = new ChessPlayer(blackPlayerInput, TeamColor.Black);

        ActivePlayer = whitePlayer;
    }

    private void InitializeInputRecievers(params IInputReader[] readers)
    {
        foreach (var reader in readers)
        {
            if (reader != null)
            {
                foreach (var reciever in inputRecievers)
                    reciever.AddInputReader(reader);
            }
        }
    }

    private void RecieveInputs()
    {
        foreach (var reciever in inputRecievers)
            reciever.RecieveInput(this);
    }

    private void ImplementBoardLayout()
    {
        for(int i = 0; i < initialBoardLayout.GetNumberOfPieces(); i++)
        {
            Vector2Int squareIndex = initialBoardLayout.GetChessPieceSquareIndex(i);
            PieceType pieceType = initialBoardLayout.GetChessPieceType(i);
            TeamColor teamColor = initialBoardLayout.GetChessPieceTeamColor(i);

            CreatePiecePrefabAndInitialize(squareIndex, pieceType, teamColor, Board.ChessGrid, GetChessPlayerFromTeam(teamColor));
        }
    }

    public static Piece CreateChessPiece(Vector2Int squareIndex, PieceType pieceType, ChessGridInfo grid, ChessPlayer player)
    {
        Piece piece = PieceFactory.CreatePiece(pieceType.ToString());
        grid.SetChessPieceOnGrid(piece, squareIndex);
        return piece;
    }

    public static void CreatePiecePrefabAndInitialize(Vector2Int squareIndex, PieceType pieceType, TeamColor teamColor,
        ChessGridInfo grid, ChessPlayer player)
    {
        Piece newPiece = CreateChessPiece(squareIndex, pieceType, grid, player);
        PieceEntity entity = PieceFactory.CreatePiecePrefab(pieceType.ToString()).GetComponent<PieceEntity>();

        IMovable pieceMovementType = PieceFactory.GetMovementType(pieceType.ToString()) == MovementType.MovesInLine ? 
            new MoveInLine() as IMovable : new Jump();

        newPiece.SetData(squareIndex, squareIndex, pieceType, teamColor, false, pieceMovementType, entity);
        player.AddPiece(newPiece);
    }

    public ChessPlayer GetChessPlayerFromTeam(TeamColor teamColor)
    {
        return teamColor == TeamColor.White ? whitePlayer : blackPlayer;
    }

    public ChessPlayer GetOppositeToActivePlayer()
    {
        return ActivePlayer.TeamColor == TeamColor.White ? blackPlayer : whitePlayer;
    }

    public void EndTurn()
    {
        Board.OnDeselectActivePiece();  
        ActivePlayer = GetOppositeToActivePlayer();
        ActivePlayer.GenerateAllPossibleMoves(Board.ChessGrid, true, true);
        GetOppositeToActivePlayer().GenerateAllPossibleMoves(Board.ChessGrid, false, false);
        SetGameState();
    }

    public void OnEnteredNormalMode()
    {
        EnterNormalMode();
    }

    private void SetGameState()
    {
        State = GameState.Normal;
        bool kingInCheck = ActivePlayer.IsKingInCheck(GetOppositeToActivePlayer());

        if (kingInCheck)
            State = GameState.Check;

        if (kingInCheck && !ActivePlayer.HasAnyMoves())
            State = GameState.Mate;

        if (!kingInCheck && !ActivePlayer.HasAnyMoves())
            State = GameState.Stalemate;

        if (State == GameState.Check || State == GameState.Mate)
            Board.SquareSelector.ActivateCheckMateSelector(ActivePlayer.FindFirstPieceOfType(PieceType.King));

        if (State == GameState.Stalemate || State == GameState.Mate)
            EnterEndGame();
    }

    public void SetGameStateToNone()
    {
        previousState = State;
        State = GameState.None;
        EnterNoneState();
    }

    public void DeactivateNoneState()
    {
        State = previousState;
        CameraController.EnableCam();
    }
}