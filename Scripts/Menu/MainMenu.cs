using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject startGameMenu;
    [SerializeField] private GameObject endGameMenu;
    [SerializeField] private GameObject inGameMenu;
    [SerializeField] private GameObject result;
    [SerializeField] private GameObject winner;
    [SerializeField] private ChessGameController gameController;
    
    [SerializeField] private GameObject[] playWith;
    [SerializeField] private GameObject[] playAs;

    private StartGameSettings settingsHandler;
    private RandomBoard randomBoard;

    private void Awake()
    {
        gameController.EnterEndGame += ActivateEndGameMenu;
        gameController.EnterNoneState += ActivateInGameMenu;
        settingsHandler = GetComponent<StartGameSettings>();
        randomBoard = GetComponent<RandomBoard>();
    }

    private void Start()
    {
        GoToMenu();
    }

    public void Play()
    {
        if (settingsHandler.AreSettingsSet())
        {
            DeactivateOptions(playWith);
            DeactivateOptions(playAs);
            randomBoard.Clear();
            settingsHandler.InitializeSettings();
            settingsHandler.ResetSettings();
            ActivateGame();
            gameController.StartNewGame();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GoToMenu()
    {
        gameController.OnEnteredNormalMode();
        gameController.Board.ChessGrid.ResetGrid();
        randomBoard.CreateRandomBoardLayout();
        gameController.CameraController.RandomCameraMovement();
        ActivateMainMenu();
    }

    public void ActivateMainMenu()
    {
        inGameMenu.SetActive(false);
        endGameMenu.SetActive(false);
        menu.SetActive(true);
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        startGameMenu.SetActive(false);
    }

    public void ActivateSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        startGameMenu.SetActive(false);
    }

    public void ActivateStartGameMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        startGameMenu.SetActive(true);
    }

    public void ActivateGame()
    {
        menu.SetActive(false);
    }

    public void ActivateEndGameMenu()
    {
        endGameMenu.SetActive(true);
        result.GetComponent<TMP_Text>().text = $"Result: {gameController.State}";
        
        if(gameController.State == GameState.Mate)
        {
            winner.GetComponent<TMP_Text>().text = $"Winner: {gameController.GetOppositeToActivePlayer().TeamColor}";
        }
        else
        {
            winner.GetComponent<TMP_Text>().text = "No winner";
        }
    }

    public void ActivateInGameMenu()
    {
        inGameMenu.SetActive(true);
    }

    public void DeactivateInGameMenu()
    {
        inGameMenu.SetActive(false);
        gameController.DeactivateNoneState();
    }

    public void OnOptionSet(GameObject choice)
    {
        bool find = SetOption(choice, playAs);

        if (!find)
            SetOption(choice, playWith);
    }

    private bool SetOption(GameObject choice, GameObject[] options)
    {
        bool find = false;

        foreach (var option in options)
        {
            if (option == choice)
            {
                option.GetComponent<Button>().interactable = false;
                settingsHandler.SetSetting(option.GetComponent<TMP_Text>().text);
                find = true;
            }
        }

        if (find)
        {
            foreach (var option in options)
            {
                if (option != choice)
                    option.GetComponent<Button>().interactable = true;
            }
        }

        return find;
    }

    private void DeactivateOptions(GameObject[] options)
    {
        foreach (var option in options)
        {
            option.GetComponent<Button>().interactable = true;
        }
    }
}