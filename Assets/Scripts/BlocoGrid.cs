using UnityEngine;
using UnityEngine.InputSystem;

public class BlocoGrid : MonoBehaviour
{
    #region Variaveis

    private float tempoUltimoPasso;
    public float tempoPorPasso = 0.5f; // Velocidade da queda (segundos por quadrado)

    #endregion

    #region Ciclo

    void Update()
    {
        // Se o jogo já acabou, impede qualquer movimento ou ação deste bloco
        if (!GerenciadorGrid.jogoAtivo) return;

        // Faz o bloco descer automaticamente no tempo determinado
        if (Time.time - tempoUltimoPasso >= tempoPorPasso)
        {
            transform.position += new Vector3(0, -1, 0); // Move 1 unidade para baixo

            // Verifica se a nova posição é válida
            if (PosicaoValida())
            {
                GerenciadorGrid.AtualizarGrid(this);
            }
            else
            {
                // Se bateu em algo, desfaz o passo para baixo voltar para a posição certa
                transform.position += new Vector3(0, 1, 0);


                int yAtual = Mathf.RoundToInt(transform.position.y);

                // SE TRAVOU NO TOPO OU ACIMA DELE: GAMEOVER!
                if (yAtual >= GerenciadorGrid.altura - 1)
                {
                    GerenciadorGrid.jogoAtivo = false;
                    Debug.LogWarning("🚨 GAMEOVER! Os blocos atingiram o topo da tela.");
                    enabled = false;// Desativa este script para o bloco parar de se mover
                    return;
                }

                // Se a posição for segura, fixa o bloco e chama o próximo
                GerenciadorGrid.FixarBlocoNoGrid(this);
                enabled = false; // Desativa este script para o bloco parar de se mover
                FindAnyObjectByType<GeradorDeBlocos>().CriarBloco(); // Chama o próximo
            }
            tempoUltimoPasso = Time.time;
        }

        // ========================================================
        // NOVA LEITURA DE TECLADO (COMPATÍVEL COM O INPUT SYSTEM)
        // ========================================================
        var teclado = Keyboard.current;

        if (teclado != null) // Verifica se existe um teclado conectado
        {
            // Seta Esquerda ou tecla A
            if (teclado.leftArrowKey.wasPressedThisFrame || teclado.aKey.wasPressedThisFrame)
            {
                Mover(new Vector3(-1, 0, 0));
            }
            // Seta Direita ou tecla D
            if (teclado.rightArrowKey.wasPressedThisFrame || teclado.dKey.wasPressedThisFrame)
            {
                Mover(new Vector3(1, 0, 0));
            }
            // Seta para Baixo ou tecla S (Acelerar a queda - opcional)
            if (teclado.downArrowKey.wasPressedThisFrame || teclado.sKey.wasPressedThisFrame)
            {
                Mover(new Vector3(0, -1, 0));
            }
        }
    }

    #endregion

    #region Funcoes


    void Mover(Vector3 direcao)
    {
        transform.position += direcao;
        if (!PosicaoValida()) transform.position -= direcao; // Desfaz se for inválido
    }

    bool PosicaoValida()
    {
        // Arredonda a posição para garantir números inteiros no Grid
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);

        return GerenciadorGrid.VerificarPosicao(x, y);
    }

    #endregion
}
