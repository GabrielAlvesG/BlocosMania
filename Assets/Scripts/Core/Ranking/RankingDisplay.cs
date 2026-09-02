using Assets.Scripts.Core.Data.Modelos;
using Assets.Scripts.Core.Data.Repositorio;
using System.Collections;
using UnityEngine;

public class RankingDisplay : MonoBehaviour
{
    #region Variavei

    [Header("Componentes")]
    [SerializeField] private Transform container; // Onde os itens serão colocados (ex: Content de um ScrollView ou Vertical Layout Group)
    [SerializeField] private GameObject rankingItemPrefab; // Prefab com os textos de Nome, Score e Tempo

    [Header("Configurações de Exibição")]
    [SerializeField] private float delayBetweenItems = 0.5f; // Tempo em segundos entre cada item

    private const int TOTAL_SLOTS = 10; // Garante que sempre renderize 10 linhas

    #endregion

    #region Ciclo

    private void OnEnable()
    {
        // Atualiza o ranking sempre que o painel for ativado
        DisplayRanking();
    }

    private void OnDisable()
    {
        StopAllCoroutines(); // Para a rotina de exibição caso o painel seja desativado
    }

    #endregion

    #region Funcoes

    public void DisplayRanking()
    {
        // Limpa itens antigos que já estejam no container
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Inicia a rotina de exibição gradual
        StartCoroutine(AnimateRankingDisplay());
    }

    private IEnumerator AnimateRankingDisplay()
    {
        // Carrega a lista do arquivo
        HighScoreList highScoreList = GerenciadorScore.LoadListaPontuacoes();

        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
            GameObject itemObj = Instantiate(rankingItemPrefab, container);
            RankingItemUI itemUI = itemObj.GetComponent<RankingItemUI>();

            if (itemUI != null)
            {
                int rankPosition = i + 1;

                // Verifica se existe uma pontuação para essa posição no array
                if (i < highScoreList.scores.Count)
                {
                    ScoreData data = highScoreList.scores[i];
                    itemUI.Setup(rankPosition, data.Nome, data.Pontuacao, data.TempoJogado);
                }
                else
                {
                    // Slot vazio: passa valores nulos/padrão
                    itemUI.Setup(rankPosition, "", 0, 0f, true);
                }
            }

            // Delay entre a aparição de cada linha do ranking
            yield return new WaitForSeconds(delayBetweenItems);
        }
    }

    #endregion
}