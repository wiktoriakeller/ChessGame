using System;
using UnityEngine;

public class StartGameSettings : MonoBehaviour
{
    private TeamColor? playAs;
    private PlayerType? playWith;

    private void Awake()
    {
        playAs = null;
        playWith = null;
    }

    public void SetSetting(string setting)
    {
        if (Enum.TryParse(setting, true, out TeamColor teamColor))
        {
            playAs = teamColor;
        }
        else if (Enum.TryParse(setting, true, out PlayerType playerType))
        {
            playWith = playerType;
        }
    }

    public bool AreSettingsSet()
    {
        if (playAs != null && playWith != null)
            return true;
        return false;
    }

    public void ResetSettings()
    {
        playAs = null;
        playWith = null;
    }

    public void InitializeSettings()
    {
        if (AreSettingsSet())
        {
            ChessGameSettings.WhitePlayerUsesAi = false;
            ChessGameSettings.BlackPlayerUsesAi = false;
            ChessGameSettings.HumanPlayerIsWhite = false;

            if (playAs == TeamColor.White)
                ChessGameSettings.HumanPlayerIsWhite = true;

            if (playAs == TeamColor.Black && playWith == PlayerType.Computer)
                ChessGameSettings.WhitePlayerUsesAi = true;

            if (playAs == TeamColor.White && playWith == PlayerType.Computer)
                ChessGameSettings.BlackPlayerUsesAi = true;
        }
    }
}