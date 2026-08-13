using UnityEngine;

public class GeradorDeBlocos : MonoBehaviour
{
    #region Variaveis

    [Header("Formatos de Peças")]
    // Uma lista contendo todas as peças diferentes que você vai criar no editor
    public GameObject[] prefabsPecas;

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
        if (!GerenciadorGrid.jogoAtivo) return;

        // Se nao tiver prefabs de peças, exibe erro no console e retorna
        if (prefabsPecas == null || prefabsPecas.Length == 0)
        {
            Debug.LogError("Por favor, adicione os prefabs de peças no Gerador!");
            return;
        }


        // Escolhe um índice de peça aleatório da lista
        int indiceAleatorio = Random.Range(0, prefabsPecas.Length);

        // Adicionamos + 0.5f para alinhar o pivô do bloco perfeitamente no meio do quadrado azul (o bloco tem 1x1 unidade)
        float meioX = (GerenciadorGrid.largura / 2) + 0.5f;
        float topoY = (GerenciadorGrid.altura - 1) + 0.5f;

        Vector3 posicaoSpawn = new Vector3(meioX, topoY, 0f);
        Instantiate(prefabsPecas[indiceAleatorio], posicaoSpawn, Quaternion.identity);
    }

    #endregion

}
