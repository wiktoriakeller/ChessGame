using System.Collections.Generic;

public static class PiecePositionEvaluation
{
    public static IDictionary<(PieceType, TeamColor), int[,]> PositionEvaluation;
    public static IDictionary<PieceType, int> PieceValue = new Dictionary<PieceType, int>() {
        {PieceType.Pawn, 300}, {PieceType.Knight, 520}, {PieceType.Bishop, 530 }, {PieceType.Rook, 1200},
        {PieceType.Queen, 3500}, {PieceType.King, 20000}
    };

    private static int[,] pawnEvalBlack = 
    {
         { 70,  70,  70,  70,  70,  70,  70,  70 },
         { 40, 40, 40, 40, 40, 40, 40, 40 },
         { 10, 10, 20, 30, 30, 20, 10, 10 },
         { 5,  5, 10, 25, 25, 10,  5,  5 },
         { 0,  0,  0, 20, 20,  0,  0,  0 },
         { 5, -5,-10,  0,  0,-10, -5,  5 },
         { 5, 10, 10,-5,-5, 10, 10,  5 },
         { 0,  0,  0,  0,  0,  0,  0,  0 },
    };

    private static int[,] pawnEvalWhite = new int[ChessGridInfo.GRID_SIZE, ChessGridInfo.GRID_SIZE];

    private static int[,] knightEvalBlack =
    {
        { -50,-40,-30,-30,-30,-30,-40,-50 },
        { -40,-20,  0,  0,  0,  0,-20,-40 },
        { -30,  0, 10, 15, 15, 10,  0,-30 },
        { -30,  5, 15, 20, 20, 15,  5,-30 },
        { -30,  0, 15, 20, 20, 15,  0,-30 },
        { -30,  5, 10, 15, 15, 10,  5,-30 },
        { -40,-20,  0,  5,  5,  0,-20,-40 },
        { -50,-40,-30,-30,-30,-30,-40,-50 }
    };

    private static int[,] knightEvalWhite = new int[ChessGridInfo.GRID_SIZE, ChessGridInfo.GRID_SIZE];

    private static int[,] bishopEvalBlack =
    {
        { -20,-10,-10,-10,-10,-10,-10,-20 },
        { -10,  0,  0,  0,  0,  0,  0,-10 },
        { -10,  0,  5, 10, 10,  5,  0,-10 },
        { -10,  5,  5, 10, 10,  5,  5,-10 },
        { -10,  0, 10, 10, 10, 10,  0,-10 },
        { -10, 10, 10, 10, 10, 10, 10,-10 },
        { -10,  5,  0,  0,  0,  0,  5,-10 },
        { -20,-10,-10,-10,-10,-10,-10,-20 },
    };

    private static int[,] bishopEvalWhite = new int[ChessGridInfo.GRID_SIZE, ChessGridInfo.GRID_SIZE];

    private static int[,] rookEvalBlack =
    {
          { 0,  0,  0,  0,  0,  0,  0,  0 },
          { 5, 10, 10, 10, 10, 10, 10,  5 },
          { -5,  0,  0,  0,  0,  0,  0, -5 },
          { -5,  0,  0,  0,  0,  0,  0, -5 },
          { -5,  0,  0,  0,  0,  0,  0, -5 },
          { -5,  0,  0,  0,  0,  0,  0, -5 },
          { -5,  0,  0,  0,  0,  0,  0, -5 },
          { 0,  0,  0,  5,  5,  0,  0,  0 }
    };

    private static int[,] rookEvalWhite = new int[ChessGridInfo.GRID_SIZE, ChessGridInfo.GRID_SIZE];

    private static int[,] queenEvalBlack =
    {
        { -20,-10,-10, -5, -5,-10,-10,-20 },
        { -10,  0,  0,  0,  0,  0,  0,-10 },
        { -10,  0,  5,  5,  5,  5,  0,-10 },
        { -5,  0,  5,  5,  5,  5,  0, -5 },
        {  0,  0,  5,  5,  5,  5,  0, -5 },
        { -10,  5,  5,  5,  5,  5,  0,-10 },
        { -10,  0,  5,  0,  0,  0,  0,-10 },
        { -20,-10,-10, -5, -5,-10,-10,-20 }
    };

    private static int[,] queenEvalWhite = new int[ChessGridInfo.GRID_SIZE, ChessGridInfo.GRID_SIZE];

    private static int[,] kingEvalBlack =
    {
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -30,-40,-40,-50,-50,-40,-40,-30 },
        { -20,-30,-30,-40,-40,-30,-30,-20 },
        { -10,-20,-20,-20,-20,-20,-20,-10 },
        { 20, 20,  0,  0,  0,  0, 20, 20  },
        { 20, 30, 10,  0,  0, 10, 30, 20 }
    };

    private static int[,] kingEvalWhite = new int[ChessGridInfo.GRID_SIZE, ChessGridInfo.GRID_SIZE];

    static PiecePositionEvaluation()
    {
        InverseEvaluationArray(pawnEvalBlack);
        InverseEvaluationArray(bishopEvalBlack);
        InverseEvaluationArray(knightEvalBlack);
        InverseEvaluationArray(rookEvalBlack);
        InverseEvaluationArray(queenEvalBlack);

        ReverseRowsEvaluationArray(pawnEvalBlack, pawnEvalWhite);
        ReverseRowsEvaluationArray(knightEvalBlack, knightEvalWhite);
        ReverseRowsEvaluationArray(bishopEvalBlack, bishopEvalWhite);
        ReverseRowsEvaluationArray(rookEvalBlack, rookEvalWhite);
        ReverseRowsEvaluationArray(queenEvalBlack, queenEvalWhite);
        ReverseRowsEvaluationArray(kingEvalBlack, kingEvalWhite);

        PositionEvaluation = new Dictionary<(PieceType, TeamColor), int[,]>()
        {
            {(PieceType.Pawn, TeamColor.White), pawnEvalWhite},
            {(PieceType.Bishop, TeamColor.White), bishopEvalWhite},
            {(PieceType.Knight, TeamColor.White), knightEvalWhite},
            {(PieceType.Rook, TeamColor.White), rookEvalWhite},
            {(PieceType.Queen, TeamColor.White), queenEvalWhite},
            {(PieceType.King, TeamColor.White), kingEvalWhite},

            {(PieceType.Pawn, TeamColor.Black), pawnEvalBlack},
            {(PieceType.Bishop, TeamColor.Black), bishopEvalBlack},
            {(PieceType.Knight, TeamColor.Black), knightEvalBlack},
            {(PieceType.Rook, TeamColor.Black), rookEvalBlack},
            {(PieceType.Queen, TeamColor.Black), queenEvalBlack},
            {(PieceType.King, TeamColor.Black), kingEvalBlack},
        };
    }   

    private static void ReverseRowsEvaluationArray(int[,] evalArray, int[,] reversedEvalArray)
    {
        for(int i = evalArray.GetLength(0) - 1, g = 0; i >= 0; i--, g++)
        {
            for(int j = 0; j < evalArray.GetLength(1); j++)
            {
                reversedEvalArray[g, j] = evalArray[i, j] * -1;
            }
        }
    }

    private static void InverseEvaluationArray(int[,] evalArray)
    {
        for (int i = 0; i < evalArray.GetLength(0); i++)
        {
            for (int j = 0; j < evalArray.GetLength(1); j++)
            {
                evalArray[i, j] *= -1;
            }
        }
    }
}