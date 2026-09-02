using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    #region Variaveis 

    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    #endregion

    #region Ciclos 

    private void Start()
    {

        //Carrega os valores salvos no PlayerPrefs e aplica ao mixer
        float musicVolume = PlayerPrefs.GetFloat("MusicVol", 1);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVol", 1);

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        //Aplica os valores aos sliders, se eles existirem
        if (musicSlider)
        {
            musicSlider.value = musicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    #endregion

    #region Funcoes

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVol", value);
        float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
        mainMixer.SetFloat("MusicVol", dB);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVol", value);
        float dB = value > 0 ? Mathf.Log10(value) * 20 : -80f;
        mainMixer.SetFloat("SFXVol", dB);
    }

    #endregion
}
