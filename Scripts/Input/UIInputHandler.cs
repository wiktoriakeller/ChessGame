using UnityEngine;

public class UIInputHandler : MonoBehaviour
{
    private ChessGameController gameController;

    private void Awake()
    {
        gameController = GetComponent<ChessGameController>();
    }

    private void Update()
    {
        StopGame();   
    }

    private void StopGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (gameController.State == GameState.Normal || gameController.State == GameState.Check))
        {
            gameController.SetGameStateToNone();
        }
    }
}