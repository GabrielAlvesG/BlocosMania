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
    [Tooltip("Margem extra nas laterais para caber a UI (Valores maiores afastam a câmera para os lados)")]
    public float margemLateralExtra = 4.0f; 
    [Tooltip("Espaço extra que você quer abrir no topo da tela (valores maiores sobem a câmera)")]
    public float espacoExtraNoTopo = 3.0f;


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

        //// Guarda as bombas encontradas para ativá-las depois que todos os blocos estiverem salvos na matriz
        //List<BlocoBomba> bombasParaAtivar = new List<BlocoBomba>();

        foreach (Transform filho in filhos)
        {
            int x = Mathf.FloorToInt(filho.position.x);
            int y = Mathf.FloorToInt(filho.position.y);

            // Força a posição visual de cada bloquinho a travar no grid de forma absoluta
            filho.position = new Vector3(x + 0.5f, y + 0.5f, 0f);

            if (y >= 0 && y < altura && x >= 0 && x < largura)
            {
                grid[x, y] = filho;

                //if (filho.TryGetComponent<BlocoBomba>(out BlocoBomba bomba)) //se bomba guardamos para ativar
                //{
                //    bombasParaAtivar.Add(bomba);
                //}
            }

            // Desacopla o bloco do objeto Pai para que ele vire um bloco independente no cenário
            filho.parent = null;
        }

        //// Ativa todas as bombas que acabaram de pousar
        //foreach (BlocoBomba bomba in bombasParaAtivar)
        //{
        //    bomba.AtivarBomba();
        //}

        // Faz a varredura se alguma linha completou
        FindAnyObjectByType<GerenciadorGrid>().IniciarChecagemDeLinhas();
    }

    private static void ApagarLinha(int y)
    {
        for (int x = 0; x < largura; x++)
        {
            if (grid[x, y] != null)
            {
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
                }
            }
        }
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
        float centroX = LarguraGrid / 2f;
        float centroY = (AlturaGrid / 2f) + espacoExtraNoTopo;

        // Move a câmera para esse centro (mantendo o Z em -10)
        cam.transform.position = new Vector3(centroX, centroY, -10f);

        // Ajusta o tamanho do zoom com uma pequena margem (+1)
        cam.orthographic = true;

        // Calculamos o tamanho da tela com base na largura desejada + margem das laterais.
        // Dividimos pela proporção (aspect ratio) da tela atual do jogador.
        float tamanhoBaseadoNaLargura = (LarguraGrid / 2f + margemLateralExtra) / cam.aspect;

        // Calculamos também baseado na altura padrão com a margem do topo
        float tamanhoBaseadoNaAltura = (AlturaGrid / 2f) + 1f;

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

