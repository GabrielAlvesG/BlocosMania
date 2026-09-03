using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Core.Data.Cache;

public class IdentificacaoUI : MonoBehaviour
{
    #region Variaveis

    [Header("Identificacao")]
    public GameObject painel_Identificacao;
    public TextMeshProUGUI text_Erro;
    public TMP_InputField inputField_Nome;

    [Header("Tutorial")]
    public GameObject painel_Tutorial;

    #endregion

    #region Ciclos

    void Start()
    {
        text_Erro.gameObject.SetActive(false);

        //Sempre q mudar o nome esconde o erro

        if (inputField_Nome)
        {
            inputField_Nome.onValueChanged.AddListener(OnChangeInputNome);
        }
    }

    #endregion

    #region Funcoes

    public void OnClick_Jogar()
    {
        if (ValidarSeDadosValidos())
        {
            GameSessionData.NomeJogador = inputField_Nome.text;
            GerenciadorCenas.Instancia.CarregarJogo();
        }
    }

    public void OnClick_Avancar()
    {
        if (ValidarSeDadosValidos())
        {
            OcultarPaineis();
            painel_Tutorial.SetActive(true);
        }
    }

    public void OnClick_VoltarIdentificacao()
    {
        OcultarPaineis();
        GerenciadorCenas.Instancia.CarregarMenu();
    }

    public void OnClick_VoltarTutorial()
    {
        OcultarPaineis();
        painel_Identificacao.SetActive(true);
    }

    public void OcultarPaineis()
    {
        painel_Identificacao.SetActive(false);
        painel_Tutorial.SetActive(false);
    }

    public bool ValidarSeDadosValidos()
    {
        if (string.IsNullOrEmpty(inputField_Nome.text) || string.IsNullOrWhiteSpace(inputField_Nome.text)) //Valida se vazio
        {
            text_Erro.text = "Nome inválido!";
            text_Erro.gameObject.SetActive(true);
            return false;
        }

        text_Erro.gameObject.SetActive(false);
        return true;//Tudo certo
    }

    public void OnChangeInputNome(string valor)
    {
        //Ocultamos o erro
        text_Erro.gameObject.SetActive(false);
    }

    #endregion
}
