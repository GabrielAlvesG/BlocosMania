using UnityEngine;
using UnityEngine.Audio;

public class BlocoGelo : MonoBehaviour
{
    public AudioMixerGroup grupoMixer;
    public AudioClip somColeta; // Som que será tocado ao coletar moeda

    public void ConfigurarAudio(AudioClip clip, AudioMixerGroup grupoMixer)
    {
        this.somColeta = clip;
        this.grupoMixer = grupoMixer;
    }

    public void AtivarGelo()
    {
        if (somColeta != null)
        {
            AudioUtils.TocarSomComMixer(somColeta, transform.position, grupoMixer);
        }

        if (GerenciadorJogo.Instancia != null)
        {
            GerenciadorJogo.Instancia.AtivarEfeitoGelo(8f); // 8 segundos de lentidão
        }
    }
}
