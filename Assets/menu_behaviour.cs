using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class menu_behaviour : MonoBehaviour
{
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    private float stock_Volume = 100.0f;
    /// <summary>
    /// stops the game
    /// </summary>
    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Quit.");
    }

    /// <summary>
    /// sets the volume to a specific value
    /// </summary>
    /// <param name="volume">value of volume</param>
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeTextValue.text = volume.ToString("0.0");
    }


    /// <summary>
    /// saves the volume into the playerprefs which are saved over gamesessions
    /// </summary>
    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);

    }
    /// <summary>
    /// resets the volume to a specified default value
    /// </summary>
    public void VolumeReset()
    {
        SetVolume(stock_Volume);
        VolumeApply();
        volumeSlider.value = stock_Volume;

    }

}
