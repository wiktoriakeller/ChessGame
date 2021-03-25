using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resuolutionDropdown;
    [SerializeField] private TMP_Dropdown fullscreenDropdown;
    private Resolution[] resolutions;
    private bool isFullscreen;

    private void Awake()
    {
        SetupScreenMode();
        SetupResolution();
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, isFullscreen);
    }

    public void SetFullscreen(int index)
    {
        if(fullscreenDropdown.options[index].text.ToLower() == "fullscreen")
        {
            isFullscreen = true;
        }
        else
        {
            isFullscreen = false;
        }

        Screen.fullScreen = isFullscreen;
    }

    private void SetupScreenMode()
    {
        isFullscreen = true;

        for (int i = 0; i < fullscreenDropdown.options.Count; i++)
        {
            if(fullscreenDropdown.options[i].text.ToLower() == "fullscreen")
            {
                fullscreenDropdown.value = i;
                break;
            }
        }

        fullscreenDropdown.RefreshShownValue();
        Screen.fullScreen = isFullscreen;
    }

    private void SetupResolution()
    {
        resolutions = Screen.resolutions;
        resuolutionDropdown.ClearOptions();
        List<string> options = new List<string>(resolutions.Length);
        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            Resolution res = resolutions[i];
            string option = $"{res.width} x {res.height}";
            options.Add(option);

            if(res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resuolutionDropdown.AddOptions(options);
        resuolutionDropdown.value = currentResolutionIndex;
        resuolutionDropdown.RefreshShownValue();
    }
}