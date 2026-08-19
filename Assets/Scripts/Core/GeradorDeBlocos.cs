using UnityEngine;

public class GeradorDeBlocos : MonoBehaviour
{
    #region Variaveis

    [Header("Preview Proxima")]
    public Transform posicaoPreview;
    private GameObject pecaEmPreview;

    [Header("Formatos de Peças")]
    // Uma lista contendo todas as peças diferentes que você vai criar no editor
    public GameObject[] prefabsPecas;

    [Header("Configuração de Bombas")]
    [Range(0f, 100f), Tooltip("Porcentagem de chance de uma peça nascer contendo uma bomba")]
    public float chanceDeBomba = 25f;
    public Sprite SpriteBomba;

    private int indicePecaAtual = 0;
    private int indiceProximaPeca = 0;
    private bool primeiroSpawn = false;

    #endregion

    #region Funcoes

    public void CriarBloco()
    {
        if (!GerenciadorJogo.Instancia.jogoAtivo) return;

        // Se nao tiver prefabs de peças, exibe erro no console e retorna
        if (prefabsPecas == null || prefabsPecas.Length == 0)
        {
            Debug.LogError("Por favor, adicione os prefabs de peças no Gerador!");
            return;
        }

        //Para exibir Preview a proxima peça
        if (primeiroSpawn)
        {
            indicePecaAtual = Random.Range(0, prefabsPecas.Length);
            indiceProximaPeca = Random.Range(0, prefabsPecas.Length);
            primeiroSpawn = false;
        }
        else
        {
            indicePecaAtual = indiceProximaPeca;
            indiceProximaPeca = Random.Range(0, prefabsPecas.Length);
        }

        // Adicionamos + 0.5f para alinhar o pivô do bloco perfeitamente no meio do quadrado azul (o bloco tem 1x1 unidade)
        float meioX = (GerenciadorGrid.largura / 2) + 0.5f;
        float topoY = (GerenciadorGrid.altura - 1) + 0.5f;

        Vector3 posicaoSpawn = new Vector3(meioX, topoY, 0f);
        GameObject novaPeca = Instantiate(prefabsPecas[indicePecaAtual], posicaoSpawn, Quaternion.identity);

        //Troca um bloco aleatório da peça por uma bomba
        if (Random.Range(0f, 100f) <= chanceDeBomba)
        {
            // Pega todos os quadradinhos filhos que compõem essa peça
            Transform[] filhos = novaPeca.GetComponentsInChildren<Transform>();

            // Filtra apenas os filhos reais (excluindo o próprio objeto pai)
            System.Collections.Generic.List<Transform> blocosFilhos = new System.Collections.Generic.List<Transform>();
            foreach (Transform t in filhos)
            {
                if (t != novaPeca.transform) blocosFilhos.Add(t);
            }

            if (blocosFilhos.Count > 0)
            {
                // Escolhe um bloco filho totalmente aleatório dentro da peça
                int filhoSorteado = Random.Range(0, blocosFilhos.Count);
                GameObject blocoAlvo = blocosFilhos[filhoSorteado].gameObject;

                // Transforma esse bloco em uma Bomba adicionando o componente novo
                blocoAlvo.AddComponent<BlocoBomba>();
                if (SpriteBomba != null)
                {
                    blocoAlvo.GetComponent<SpriteRenderer>().sprite = SpriteBomba;

                }
            }
        }

        AtualizarPreviewVisual();
    }

    private void AtualizarPreviewVisual()
    {
        // Se já existia uma peça flutuando no preview, deleta ela antes de criar a nova
        if (pecaEmPreview != null)
        {
            Destroy(pecaEmPreview);
        }

        if (posicaoPreview == null)
        {
            Debug.LogWarning("Por favor, configure o objeto 'posicaoPreview' no Inspector do Gerador.");
            return;
        }

        // Cria a próxima peça exatamente na posição de preview configurada
        pecaEmPreview = Instantiate(prefabsPecas[indiceProximaPeca], posicaoPreview.position, Quaternion.identity, posicaoPreview);


        // MUITO IMPORTANTE: Desativa o script de queda dela para ela não cair e nem ler o teclado enquanto está no painel!
        if (pecaEmPreview.TryGetComponent<PecaGrid>(out PecaGrid scriptPeca))
        {
            scriptPeca.enabled = false;
        }

        //Joga o preview dos blocos para frente, para não ficar atrás do painel de UI
        foreach (var item in pecaEmPreview.GetComponentsInChildren<SpriteRenderer>())
        {
            item.sortingOrder = 10; // Coloca a peça de preview na frente de tudo
        }
    }

    #endregion

}
