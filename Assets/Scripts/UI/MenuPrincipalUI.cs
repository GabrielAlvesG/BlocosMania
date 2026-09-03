using UnityEngine;
using UnityEngine.UI;

public class MenuPrincipalUI : MonoBehaviour
{
    #region Variaveis

    public Button BtnJogar;
    public Button BtnRecord;
    public Button BtnConfiguracoes;
    public Button BtnSair;

    #endregion

    #region Ciclo

    void Start()
    { 
        //TODO: Adicionar cena de configuracoes e recordes

        BtnJogar.onClick.AddListener(() => GerenciadorCenas.Instancia.CarregarCadastro());
        BtnSair.onClick.AddListener(() => GerenciadorCenas.Instancia.Sair());
    }

    #endregion
}
