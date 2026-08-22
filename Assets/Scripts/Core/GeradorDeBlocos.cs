using System.Collections.Generic;
using UnityEngine;

public class GeradorDeBlocos : MonoBehaviour
{
    #region Variaveis

    [Header("Preview Proxima")]
    public Transform posicaoPreview;
    private GameObject pecaEmPreview;

    [Header("Formatos de Peças"), Tooltip("Uma lista contendo todas as peças diferentes que você vai criar no editor")]
    public GameObject[] prefabsPecas;

    [Header("Configuração de Bombas")]
    [Range(0f, 100f), Tooltip("Porcentagem de chance de uma peça nascer contendo um bloco de bomba")]
    public float chanceDeBomba = 25f;
    public Sprite SpriteBomba;

    [Header("Configuração do Bloco de Gelo")]
    [Range(0f, 100f), Tooltip("Porcentagem de chance de uma peça nascer contendo um bloco de gelo")]
    public float chanceDeBlocoGelo = 25f;
    public Sprite spriteGelo;

    [Header("Configuração do Bloco de Vento")]
    [Range(0f, 100f), Tooltip("Porcentagem de chance de uma peça nascer contendo um bloco de vento")]
    public float chanceDeBlocoVento = 25f;
    public Sprite spriteVentilador;


    [Header("Configuração do Bloco de Moeda")]
    [Range(0f, 100f), Tooltip("Porcentagem de chance de uma peça nascer contendo um bloco de moeda")]
    public float chanceDeBlocoMoeda = 25f;
    public Sprite spriteMoeda;

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

        List<Transform> blocosFilhos = GetBlocosFilhos(novaPeca);

        Sortear<BlocoDinheiro>(blocosFilhos, chanceDeBlocoMoeda, spriteMoeda);
        Sortear<BlocoBomba>(blocosFilhos, chanceDeBomba, SpriteBomba);
        Sortear<BlocoGelo>(blocosFilhos, chanceDeBlocoGelo, spriteGelo);
        Sortear<BlocoVento>(blocosFilhos, chanceDeBlocoVento, spriteVentilador);

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

    public List<Transform> GetBlocosFilhos(GameObject peca)
    {
        Transform[] filhos = peca.GetComponentsInChildren<Transform>();

        // Filtra apenas os filhos reais (excluindo o próprio objeto pai)
        System.Collections.Generic.List<Transform> blocosFilhos = new System.Collections.Generic.List<Transform>();
        foreach (Transform t in filhos)
        {
            if (t != peca.transform) blocosFilhos.Add(t);
        }

        return blocosFilhos;
    }

    // Sorteia um bloco aleatório da peça e adiciona o componente T nele, caso a chance seja atendida.
    public void Sortear<T>(List<Transform> blocosFilhos, float change, Sprite sprite) where T : Component
    {
        Debug.Log("lista de blocos filhos: " + blocosFilhos.Count);

        //Troca um bloco aleatório da peça pelo componente
        if (Random.Range(0f, 100f) <= change)
        {
            if (blocosFilhos.Count > 0)
            {
                // Escolhe um bloco filho totalmente aleatório dentro da peça
                int filhoSorteado = Random.Range(0, blocosFilhos.Count);
                GameObject blocoAlvo = blocosFilhos[filhoSorteado].gameObject;

                // Transforma esse bloco adicionando o componente novo
                blocoAlvo.AddComponent<T>();
                if (sprite != null)
                {
                    blocoAlvo.GetComponent<SpriteRenderer>().sprite = sprite;
                    blocoAlvo.GetComponent<SpriteRenderer>().color = Color.white;
                }

                blocosFilhos.Remove(blocosFilhos[filhoSorteado]); // Remove o bloco sorteado da lista para não sortear ele novamente
            }
        }
    }

    #endregion

}
