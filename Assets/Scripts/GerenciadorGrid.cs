using UnityEngine;


public class GerenciadorGrid : MonoBehaviour
{

    #region Variaveis

    [Header("Configurações do Grid")]
    public int LarguraGrid = 10;
    public int AlturaGrid = 20;

    public static int largura = 10;
    public static int altura = 20;
    public static Transform[,] grid;

    public static bool jogoAtivo = true;

    #endregion

    #region Ciclo
    void Awake()
    {
        jogoAtivo = true; // Reinicia o estado ao começar o jogo

        SincronizarConfiguracoes();
        grid = new Transform[largura, altura];
    }
    private void OnValidate()
    {
        SincronizarConfiguracoes();
        ConfigurarCamera();
    }

    private void OnDrawGizmos()
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

    //---- Privados ----//

    void SincronizarConfiguracoes()
    {
        largura = LarguraGrid;
        altura = AlturaGrid;
    }

    //---- Validadores ----//

    // Verifica se o bloco está dentro dos limites e se não bateu em outro bloco
    public static bool VerificarPosicao(int x, int y)
    {
        if (x < 0 || x >= largura || y < 0) return false;
        if (y < altura && grid[x, y] != null) return false;

        return true;
    }

    //Fixa o bloco no grid, tornando-o parte do grid e não mais móvel
    public static void FixarBlocoNoGrid(BlocoGrid bloco)
    {
        int x = Mathf.RoundToInt(bloco.transform.position.x);
        int y = Mathf.RoundToInt(bloco.transform.position.y);
        if (y < altura && x >= 0 && x < largura)
        {
            grid[x, y] = bloco.transform;
        }
    }

    public static void AtualizarGrid(BlocoGrid bloco) { }

    //---- Camera ----//

    void ConfigurarCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // Calcula o centro exato do grid azul
        float centroX = LarguraGrid / 2f;
        float centroY = AlturaGrid / 2f;

        // Move a câmera para esse centro (mantendo o Z em -10)
        cam.transform.position = new Vector3(centroX, centroY, -10f);

        // Ajusta o tamanho do zoom com uma pequena margem (+1)
        cam.orthographic = true;
        cam.orthographicSize = (AlturaGrid / 2f) + 1f;
    }

    #endregion
}

