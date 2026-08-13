using UnityEngine;
using UnityEngine.InputSystem;

public class PecaGrid : MonoBehaviour
{
    #region Variaveis

    private float tempoUltimoPasso;
    public float tempoPorPasso = 0.8f;

    #endregion

    #region Ciclo

    void Start()
    {
        tempoUltimoPasso = Time.time;

        // Se nascer em posição inválida de cara, é GameOver imediato
        if (!PosicaoValida())
        {
            GerenciadorGrid.jogoAtivo = false;
            Debug.LogWarning("🚨 GAMEOVER! O topo encheu.");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!GerenciadorGrid.jogoAtivo) return;

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
        var teclado = Keyboard.current;
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
