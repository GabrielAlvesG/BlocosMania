using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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
        if (mainMixer.GetFloat("MusicVol", out float dM))
            musicSlider.value = Mathf.Pow(10, dM / 20);

        if (mainMixer.GetFloat("SFXVol", out float dS))
            sfxSlider.value = Mathf.Pow(10, dS / 20);

        // Vincula os Sliders às funções via código
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        musicSlider.value = PlayerPrefs.GetFloat("MusicVol", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVol", 1);
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
