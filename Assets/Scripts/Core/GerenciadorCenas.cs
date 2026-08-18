using UnityEngine;
using UnityEngine.SceneManagement;

public class GerenciadorCenas : MonoBehaviour
{
    #region Variaveis

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

    public void CarregarMenu() => SceneManager.LoadScene("MenuPrincipal");
    public void CarregarJogo() => SceneManager.LoadScene("GamePlay");

    public void RecarregarCenaAtual() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

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