using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    #region Variaveis 

    [SerializeField] public AudioMixer mainMixer;
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

public static class AudioUtils
{
    public static void TocarSomComMixer(AudioClip clip, Vector3 posicao, AudioMixerGroup canalMixer, float volume = 1f)
    {
        if (clip == null) return;

        // Cria um objeto temporário só para o som na hierarquia
        GameObject emissorTemp = new GameObject("TempSFX");
        emissorTemp.transform.position = posicao;

        AudioSource source = emissorTemp.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.outputAudioMixerGroup = canalMixer; // <--- ROTEADO PARA O CANAL SFX DO MIXER
        source.Play();

        // O próprio Unity destrói apenas o emissor temporário quando o som acaba
        Object.Destroy(emissorTemp, clip.length);
    }
}