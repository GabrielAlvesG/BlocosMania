using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GerenciadorGrid : MonoBehaviour
{
    #region Variaveis

    [Header("Configurações do Grid")]
    public int LarguraGrid = 10;
    public int AlturaGrid = 20;

    [Header("Configuracoes UI")]
    public SpriteRenderer spriteFundo;

    [Header("Ajustes de Câmera")]
    [Tooltip("ajusta o zoom da camera UI (Valores maiores afastam a câmera)")]
    public float ZoomCamera = 1f;
    [Tooltip("Adiciona extra na posicao Y")]
    public float posExtraY = 2.0f;
    [Tooltip("Adiciona extra na posicao X")]
    public float posExtraX = 2.0f;


    public static int largura = 10;
    public static int altura = 20;
    public static Transform[,] grid;

    #endregion

    #region Ciclo

    void Awake()
    {
        SincronizarConfiguracoes();
        grid = new Transform[largura, altura];
    }

    void OnValidate()
    {
        SincronizarConfiguracoes();
        ConfigurarCamera();
        ConfigurarFundo();
    }

    void OnDrawGizmos()
    {
        // Define a cor das linhas (Cyan/Azul Piscina neste exemplo)
        Gizmos.color = Color.cyan;

        // Desenha as linhas verticais
        for (int x = 0; x <= LarguraGrid; x++)
        {
            Gizmos.DrawLine(new Vector3(x, 0, 0), new Vector3(x, AlturaGrid, 0));
        }

        // Desenha as linhas horizontais
        for (int y = 0; y <= AlturaGrid; y++)
        {
            Gizmos.DrawLine(new Vector3(0, y, 0), new Vector3(LarguraGrid, y, 0));
        }
    }

    #endregion

    #region Funcoes

    //---- Config ----//

    void SincronizarConfiguracoes()
    {
        largura = LarguraGrid;
        altura = AlturaGrid;
    }

    //---- Controladores ----//

    public static void FixarPecaNoGrid(Transform paiDaPeca)
    {
        // Cria uma lista temporária para não dar erro ao mudar o parentesco dos filhos em tempo real
        System.Collections.Generic.List<Transform> filhos = new System.Collections.Generic.List<Transform>();
        foreach (Transform filho in paiDaPeca) filhos.Add(filho);

        foreach (Transform filho in filhos)
        {
            int x = Mathf.FloorToInt(filho.position.x);
            int y = Mathf.FloorToInt(filho.position.y);

            // Força a posição visual de cada bloquinho a travar no grid de forma absoluta
            filho.position = new Vector3(x + 0.5f, y + 0.5f, 0f);

            if (y >= 0 && y < altura && x >= 0 && x < largura)
            {
                grid[x, y] = filho;
            }

            // Desacopla o bloco do objeto Pai para que ele vire um bloco independente no cenário
            filho.parent = null;
        }

        // Faz a varredura se alguma linha completou
        FindAnyObjectByType<GerenciadorGrid>().IniciarChecagemDeLinhas();
    }

    private static void ApagarLinha(int y)
    {
        for (int x = 0; x < largura; x++)
        {
            if (grid[x, y] != null)
            {
                AdicionarMoedas(grid[x, y]); // Adiciona moedas se o bloco for do tipo dinheiro

                // Destrói o quadradinho visual no Unity
                Destroy(grid[x, y].gameObject);
                // Limpa o registro na nossa matriz matemática
                grid[x, y] = null;
            }
        }
    }

    private static void EsconderLinhaVisualmente(int y)
    {
        //TODO: Adicionar animação para os blocos sumirem, por enquanto apenas desativa o GameObject
        // Deixa os blocos invisíveis temporariamente
        for (int x = 0; x < largura; x++)
        {
            if (grid[x, y] != null)
            {
                grid[x, y].gameObject.SetActive(false);
            }
        }
    }

    private static void AtivarSeForBomba(Transform bloco)
    {
        if (bloco.TryGetComponent<BlocoBomba>(out BlocoBomba bomba))
        {
            bomba.AtivarBomba();
        }
    }

    private static void AtivarSeForGelo(Transform bloco)
    {
        if (bloco.TryGetComponent<BlocoGelo>(out BlocoGelo gelo))
        {
            gelo.AtivarGelo();
        }
    }

    private static void AdicionarMoedas(Transform bloco)
    {
        if (bloco.TryGetComponent<BlocoDinheiro>(out BlocoDinheiro dinheiro))
        {
            dinheiro.ColetarMoedas();
        }
    }

    private static void DerrubarLinhasSuperiores(int linhaInicialY)
    {
        // Varre todas as linhas acima da que foi apagada
        for (int y = linhaInicialY; y < altura; y++)
        {
            for (int x = 0; x < largura; x++)
            {
                if (grid[x, y] != null)
                {
                    // Move o dado na matriz matemática para uma linha abaixo
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;

                    // Move visualmente o objeto físico do bloco 1 unidade para baixo
                    grid[x, y - 1].position += new Vector3(0, -1, 0);

                    AtivarSeForBomba(grid[x, y - 1]); // Se for bomba, ativa a explosão

                    AtivarSeForGelo(grid[x, y - 1]); // Se for gelo, ativa a gelo (lentidão temporária do jogo)
                }
            }
        }
    }

    public static void ForcarLimpezaDeLinhaEspecifica(int y)
    {
        // Garante que a linha está dentro dos limites reais do tabuleiro
        if (y >= 0 && y < altura)
        {
            // Executa o mesmo processo de ocultação e destruição que criamos antes
            for (int x = 0; x < largura; x++)
            {
                if (grid[x, y] != null)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }
            }

            // Derruba quem estava em cima para ocupar o buraco e organiza a gravidade
            DerrubarLinhasSuperiores(y + 1);
            AplicarGravidadeGlobal();
        }
    }

    public static void AplicarGravidadeGlobal()
    {
        bool algumBlocoCaiu;

        // Usamos um loop 'do-while' porque a queda de um bloco pode isolar outro bloco acima dele, 
        // então repetimos o processo até que nenhum bloco precise mais cair neste frame.
        do
        {
            algumBlocoCaiu = false;

            // Varre o grid de baixo para cima (começa no y = 1 porque o y = 0 já é o chão)
            for (int y = 1; y < altura; y++)
            {
                for (int x = 0; x < largura; x++)
                {
                    Transform blocoAtual = grid[x, y];

                    // Só checa se a posição atual contiver um bloco de verdade
                    if (blocoAtual != null)
                    {
                        // 1. REGRA DO CHÃO: O espaço logo abaixo precisa estar vazio
                        bool temChaoAbaixo = (grid[x, y - 1] != null);

                        // 2. REGRA DAS LATERAIS: Verifica se tem vizinho na esquerda
                        bool temVizinhoEsquerda = (x > 0 && grid[x - 1, y] != null);

                        // 3. REGRA DAS LATERAIS: Verifica se tem vizinho na direita
                        bool temVizinhoDireita = (x < largura - 1 && grid[x + 1, y] != null);

                        // O bloco só cai se NÃO tiver chão E NÃO tiver nenhum vizinho nas laterais (está sozinho!)
                        if (!temChaoAbaixo && !temVizinhoEsquerda && !temVizinhoDireita)
                        {
                            // Move o bloco na matriz matemática 1 unidade para baixo
                            grid[x, y - 1] = blocoAtual;
                            grid[x, y] = null;

                            // Atualiza a posição visual do Sprite no Unity
                            blocoAtual.position = new Vector3(x + 0.5f, (y - 1) + 0.5f, 0f);

                            // Marca que houve movimento para o loop checar novamente a grade toda
                            algumBlocoCaiu = true;
                        }
                    }
                }
            }
        } while (algumBlocoCaiu);

        // Após toda a poeira baixar e os blocos isolados caírem, 
        // fazemos uma varredura para ver se essa queda acabou completando alguma nova linha!
        FindAnyObjectByType<GerenciadorGrid>().IniciarChecagemDeLinhas();
    }

    //---- Validadores ----//

    public static bool VerificarPosicao(int x, int y)
    {
        // Verifica se o bloco está dentro dos limites e se não bateu em outro bloco

        if (x < 0 || x >= largura || y < 0) return false;
        if (y < altura && grid[x, y] != null) return false;

        return true;
    }

    void IniciarChecagemDeLinhas()
    {
        //Inicia a rotina de checagem de linhas completas com efeito visual
        StartCoroutine(ChecarLinhasCompletasRotina());
    }

    private IEnumerator ChecarLinhasCompletasRotina()
    {
        int linhasDestruidasNessaPeca = 0;

        for (int y = 0; y < altura; y++)
        {
            if (LinhaEstaCheia(y))
            {
                // 1. Deixa os blocos transparentes/ocultos primeiro para dar o efeito de "sumir"
                EsconderLinhaVisualmente(y);

                // 2. Espera 0.15 segundos (o jogador vai ver a linha desaparecer!)
                yield return new WaitForSeconds(0.15f);

                // 3. Agora sim, apaga os objetos da memória e derruba quem estava em cima
                ApagarLinha(y);
                DerrubarLinhasSuperiores(y + 1);
                linhasDestruidasNessaPeca++;//Adiciona 1 ponto na contagem de linhas destruídas nessa peça

                // Recheca a mesma linha pois tudo desceu
                y--;
            }
        }

        //Se destruiu linhas, adiciona pontos na pontuação do jogador
        if (linhasDestruidasNessaPeca > 0 && GerenciadorJogo.Instancia != null)
        {
            AplicarGravidadeGlobal();
            GerenciadorJogo.Instancia.AdicionarPontos(linhasDestruidasNessaPeca);
        }
    }

    private static bool LinhaEstaCheia(int y)
    {
        // Se encontrar qualquer espaço vazio na horizontal, a linha não está cheia
        for (int x = 0; x < largura; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }
        return true;
    }

    //---- Camera ----//

    void ConfigurarCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // Calcula o centro exato do grid azul
        float centroX = (LarguraGrid / 2f) + posExtraX;
        float centroY = (AlturaGrid / 2f) + posExtraY;

        // Move a câmera para esse centro (mantendo o Z em -10)
        cam.transform.position = new Vector3(centroX, centroY, -10f);

        // Ajusta o tamanho do zoom com uma pequena margem (+1)
        cam.orthographic = true;

        // Calculamos o tamanho da tela com base na largura desejada + margem das laterais.
        // Dividimos pela proporção (aspect ratio) da tela atual do jogador.
        float tamanhoBaseadoNaLargura = (LarguraGrid / 2f + ZoomCamera) / cam.aspect;

        float tamanhoBaseadoNaAltura = (AlturaGrid / 2f + ZoomCamera) + 1f;

        // A câmera escolhe o maior tamanho entre os dois para garantir que NADA fique cortado
        cam.orthographicSize = Mathf.Max(tamanhoBaseadoNaLargura, tamanhoBaseadoNaAltura);
    }

    //---- UI ----//

    void ConfigurarFundo()
    {

        if (spriteFundo == null) return;

        // 1. Força o modo de desenho para Tiled (Ladrilhado)
        spriteFundo.drawMode = SpriteDrawMode.Tiled;

        // 2. Define o tamanho do fundo exatamente igual à largura e altura do Grid
        spriteFundo.size = new Vector2(largura, altura);

        // 3. Posiciona o fundo no centro exato da grade azul
        float centroX = largura / 2f;
        float centroY = altura / 2f;

        // Mantemos o Z em 1f (positivo) para garantir que o fundo fique ATRÁS das peças que caem (Z = 0)
        spriteFundo.transform.position = new Vector3(centroX, centroY, 1f);
    }

    #endregion
}

