using UnityEngine;

public class GeradorDeBlocos : MonoBehaviour
{
    #region Variaveis

    [Header("Modelos")]
    public GameObject prefabBloco; // Arraste seu Prefab de Bloco aqui no Unity

    #endregion

    #region Ciclo

    private void Start()
    {
        //TODO: Remover apos criar um timer para iniciar o game!!!
        CriarBloco();
    }

    #endregion

    #region Funcoes

    public void CriarBloco()
    {
        // Pega os valores atualizados direto do GerenciadorGrid
        int meioX = GerenciadorGrid.largura / 2;
        int topoY = GerenciadorGrid.altura - 1; // -1 para ficar dentro do grid (Ex: se altura é 20, o topo é 19)

        Vector3 posicaoSpawn = new Vector3(meioX, topoY, 0f);
        Instantiate(prefabBloco, posicaoSpawn, Quaternion.identity);
    }

    #endregion

}
