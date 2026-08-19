using UnityEngine;
using UnityEngine.InputSystem;

public class PecaGrid : MonoBehaviour
{
    #region Variaveis

    private float tempoUltimoPasso;
    private float tempoPorPasso = 0.8f;

    [Header("Configuração de Queda Rápida")]
    [Tooltip("Velocidade da queda ao segurar para baixo (ex: 0.05s por passo, muito mais rápido)")]
    public float velocidadeQuedaRapida = 0.05f;

    private float velocidadeGerenciador = 0.8f; //usado para armazenar a velocidade no start

    #endregion

    #region Ciclo

    void Start()
    {
        tempoUltimoPasso = Time.time;

        velocidadeGerenciador = GerenciadorJogo.Instancia.ObterVelocidadeAtual();
        tempoPorPasso = velocidadeGerenciador;

        // Se nascer em posição inválida de cara, é GameOver imediato
        if (!PosicaoValida())
        {
            GerenciadorJogo.Instancia.FinalizarJogo();
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!GerenciadorJogo.Instancia.jogoAtivo || GerenciadorJogo.Instancia.jogoPausado) return;

        var teclado = Keyboard.current;

        // SoftDrop
        if (teclado != null && (teclado.downArrowKey.isPressed || teclado.sKey.isPressed)) // Se estiver segurando a Seta para Baixo ou a tecla S, usa a velocidade rápida fixa
        {
            tempoPorPasso = velocidadeQuedaRapida;
        }
        else
        {
            tempoPorPasso = velocidadeGerenciador;
        }

        // Queda Automática
        if (Time.time - tempoUltimoPasso >= tempoPorPasso)
        {
            transform.position += new Vector3(0, -1, 0);

            if (!PosicaoValida())
            {
                // Desfaz o passo para baixo
                transform.position += new Vector3(0, 1, 0);

                // Fixa todos os quadradinhos filhos individualmente na matriz
                GerenciadorGrid.FixarPecaNoGrid(transform);

                // Destrói apenas o objeto Pai vazio, deixando os blocos fixos na cena
                Destroy(this);

                // Spawna a próxima peça
                FindAnyObjectByType<GeradorDeBlocos>().CriarBloco();
                return;
            }
            tempoUltimoPasso = Time.time;
        }

        // Movimentação do teclado
        if (teclado != null)
        {
            if (teclado.leftArrowKey.wasPressedThisFrame || teclado.aKey.wasPressedThisFrame) Mover(new Vector3(-1, 0, 0));
            if (teclado.rightArrowKey.wasPressedThisFrame || teclado.dKey.wasPressedThisFrame) Mover(new Vector3(1, 0, 0));
            if (teclado.downArrowKey.wasPressedThisFrame || teclado.sKey.wasPressedThisFrame) Mover(new Vector3(0, -1, 0));

            //Seta para cima ou W rotaciona a peça em 90 graus!
            if (teclado.upArrowKey.wasPressedThisFrame || teclado.wKey.wasPressedThisFrame) Rotacionar();
        }
    }

    #endregion

    #region Funcoes

    void Mover(Vector3 direcao)
    {
        transform.position += direcao;
        if (!PosicaoValida()) transform.position -= direcao;
    }

    void Rotacionar()
    {
        // Rotaciona o objeto pai em 90 graus no eixo Z
        transform.Rotate(0, 0, 90);

        // Se a rotação fizer a peça bater na parede ou em outra peça, desfaz a rotação
        if (!PosicaoValida()) transform.Rotate(0, 0, -90);
    }

    bool PosicaoValida()
    {
        // Passa por cada quadradinho filho que compõe a peça
        foreach (Transform filho in transform)
        {
            int x = Mathf.FloorToInt(filho.position.x);
            int y = Mathf.FloorToInt(filho.position.y);

            // Se apenas um bloco filho estiver fora, a posição inteira da peça é inválida
            if (!GerenciadorGrid.VerificarPosicao(x, y)) return false;
        }
        return true;
    }

    #endregion
}
