using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GerenciadorCenas : MonoBehaviour
{
    #region Variaveis

    [Header("Configurações de Transição")]
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private float minimumFadeTime = 1f;

    public static GerenciadorCenas Instancia { get; private set; }

    #endregion

    #region Ciclo

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    #region Funcoes

    private void CarregarCena(string nomeCena)
    {
        Time.timeScale = 1f; //Volta o tempo para o normal caso esteja pausado (evita animações pausadas)
        SceneManager.LoadScene(nomeCena);
    }

    private void CarregarCenaTransicao(string nomeCena)
    {
        Time.timeScale = 1f; //Volta o tempo para o normal caso esteja pausado (evita animações pausadas)
        StartCoroutine(CarregarCenaComTransicao(nomeCena));
    }

    private IEnumerator CarregarCenaComTransicao(string nomeDaCena)
    {
        // 1. Inicia o carregamento assíncrono em segundo plano
        AsyncOperation operation = SceneManager.LoadSceneAsync(nomeDaCena);

        // Impede que a nova cena abra de surpresa antes da tela ficar preta
        operation.allowSceneActivation = false;

        // 2. Dispara a animação de Fade Out no Canvas desta cena
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("StartFadeOut");
        }

        // 3. Aguarda o tempo da animação rodar
        float timer = 0f;
        while (timer < minimumFadeTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // 4. Aguarda a engine terminar de ler a nova cena do disco
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        // 5. Ativa a nova cena com a tela 100% preta
        operation.allowSceneActivation = true;
        
        // 2. Dispara a animação de Fade Out no Canvas desta cena
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("StartFadeIn");
        };
    }

    public void CarregarMenu() => CarregarCena("MenuPrincipal");
    public void CarregarCadastro() => CarregarCena("Identificacao");
    public void CarregarJogo() => CarregarCenaTransicao("GamePlay");

    public void RecarregarCenaAtual() => CarregarCena(SceneManager.GetActiveScene().name);

    public void Sair()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion
}