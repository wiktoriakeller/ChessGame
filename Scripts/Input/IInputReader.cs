using UnityEngine;

public interface IInputReader
{
    void ReadInput(Vector3 point, ChessGameController gameController);
    Vector3 SelectedPosition { get; }
}