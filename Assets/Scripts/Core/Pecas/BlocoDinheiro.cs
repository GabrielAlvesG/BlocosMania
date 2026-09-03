using UnityEngine;
using UnityEngine.Audio;

public class BlocoDinheiro : MonoBehaviour
{
    #region Variaveis

    [Header("Configurações")]
    public int moedasAoQuebrar = 1; // Quantidade de moedas que o bloco dá

    public AudioMixerGroup grupoMixer;
    public AudioClip somColeta; // Som que será tocado ao coletar moeda

    #endregion

    #region Funcoes

    public void ConfigurarAudio(AudioClip clip, AudioMixerGroup grupoMixer)
    {
        this.somColeta = clip;
        this.grupoMixer = grupoMixer;
    }

    // Chamado pelo GerenciadorGrid quando a linha deste bloco estoura
    public void ColetarMoedas()
    {
        if (somColeta != null)
        {
            AudioUtils.TocarSomComMixer(somColeta, transform.position, grupoMixer);
        }

        if (GerenciadorJogo.Instancia != null)
        {
            GerenciadorJogo.Instancia.AdicionarMoedas(moedasAoQuebrar);
        }
    }

    #endregion
}
