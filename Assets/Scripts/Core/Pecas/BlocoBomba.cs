using System.Collections;
using UnityEngine;

public class BlocoBomba : MonoBehaviour
{
    #region Variaveis

    private bool ativada = false;
    private bool explodiu = false; // Nova trava para evitar loops infinitos de colisão
    private Coroutine rotinaPiscar;

    private Color corDeAlerta = new Color(2.0f, 0.0f, 0.0f, 1.0f);
    private Color corOriginal;
    private SpriteRenderer sr;

    public float tempoMinimoExplosao = 5.0f;
    public float tempoMaximoExplosao = 8.0f;

    #endregion

    #region Ciclo

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            corOriginal = sr.color;
        }
    }

    #endregion

    #region Funcoes

    public void AtivarBomba()
    {
        if (ativada) return;
        ativada = true;

        float tempoParaExplodir = Random.Range(tempoMinimoExplosao, tempoMaximoExplosao); //Tempo de explosao 
        rotinaPiscar = StartCoroutine(ContagemRegressiva(tempoParaExplodir));
    }

    public void ExplodirImediatamente()
    {
        if (explodiu) return;

        // Se a coroutine de piscar ainda estiver rodando, para ela na hora
        if (rotinaPiscar != null)
        {
            StopCoroutine(rotinaPiscar);
        }

        Explodir();
    }

    private IEnumerator ContagemRegressiva(float tempoTotal)
    {
        float tempoRestante = tempoTotal;
        bool alternarCor = false;

        while (tempoRestante > 0)
        {
            if (sr != null)
            {
                // Alterna entre a cor original do sprite e a cor de alerta
                sr.color = alternarCor ? corDeAlerta : corOriginal;
                alternarCor = !alternarCor;
            }

            // A MÁGICA DA ACELERAÇÃO:
            // Quanto menor o tempoRestante, menor será o intervalo de espera, fazendo piscar mais rápido!
            // No início (ex: 8s restantes), o intervalo será maior (~0.4s). No final (0s), cai para 0.05s.
            float progresso = tempoRestante / tempoTotal; // Vai de 1.0 (início) até 0.0 (fim)
            float intervaloPiscar = Mathf.Lerp(0.05f, 0.4f, progresso);

            yield return new WaitForSeconds(intervaloPiscar);
            tempoRestante -= intervaloPiscar;
        }

        Explodir();
    }

    void Explodir()
    {
        if (explodiu) return;
        explodiu = true;

        int minhaX = Mathf.FloorToInt(transform.position.x);
        int minhaY = Mathf.FloorToInt(transform.position.y);

        Debug.Log($"💥 BOMBA EXPLODIU EM CRUZ EM: X:{minhaX}, Y:{minhaY}");

        // Definimos as 5 posições da Cruz: o centro (0,0), cima (0,1), baixo (0,-1), esquerda (-1,0) e direita (1,0)
        Vector2Int[] posicoesCruz = new Vector2Int[]
        {
            new Vector2Int(0, 0),   // O próprio centro (a bomba)
            new Vector2Int(0, 1),   // Cima
            new Vector2Int(0, -1),  // Baixo
            new Vector2Int(-1, 0),  // Esquerda
            new Vector2Int(1, 0)    // Direita
        };

        // Passa por cada uma das 5 posições da cruz
        foreach (Vector2Int ponto in posicoesCruz)
        {
            int alvoX = minhaX + ponto.x;
            int alvoY = minhaY + ponto.y;

            // Verifica os limites do tabuleiro
            if (alvoX >= 0 && alvoX < GerenciadorGrid.largura &&
                alvoY >= 0 && alvoY < GerenciadorGrid.altura)
            {
                Transform blocoAlvo = GerenciadorGrid.grid[alvoX, alvoY];

                if (blocoAlvo != null)
                {
                    //reação em cadeia se atingir outra bomba!
                    if (blocoAlvo.TryGetComponent<BlocoBomba>(out BlocoBomba outraBomba))
                    {
                        GerenciadorGrid.grid[alvoX, alvoY] = null;
                        outraBomba.ExplodirImediatamente();
                    }
                    else
                    {
                        Destroy(blocoAlvo.gameObject);
                        GerenciadorGrid.grid[alvoX, alvoY] = null;
                    }
                }
            }
        }

        // Destrói o próprio objeto físico desta bomba após limpar a cruz
        Destroy(gameObject);

        GerenciadorGrid.AplicarGravidadeGlobal();
    }

    #endregion
}
