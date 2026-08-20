using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GerenciadorJogo : MonoBehaviour
{
    #region Variaveis

    // Padrão Singleton para qualquer script conseguir acessar o GameManager facilmente
    public static GerenciadorJogo Instancia { get; private set; }

    [Header("Estados do Jogo")]
    public bool jogoAtivo = true;
    public bool jogoPausado = false;

    [Header("Interface do Usuário (UI)")]

    //Paineis flutuantes

    [Header("HUD Game")]
    public GameObject HUD;
    public TextMeshProUGUI HUD_textoPontuacao;
    public TextMeshProUGUI HUD_textoNivel;
    public TextMeshProUGUI HUD_textoTempoJogo;

    [Header("Pause")]
    public GameObject painelPause;

    [Header("Game Over")]
    public GameObject painelGameover;
    public TextMeshProUGUI GameOver_textoPontuacao;
    public TextMeshProUGUI GameOver_textoNivel;
    public TextMeshProUGUI GameOver_textoTempoJogo;

    [Header("Configurações de Nível e Velocidade")]
    [Tooltip("Velocidade do Nível 1 (segundos por passo)")]
    public float velocidadeInicial = 0.8f;
    [Tooltip("O quanto a queda acelera a cada novo nível (ex: reduz 0.1s por nível)")]
    public float reducaoPorNivel = 0.08f;
    [Tooltip("Velocidade máxima limite para o jogo não ficar impossível")]
    public float velocidadeMaximaLimite = 0.1f;
    [Tooltip("Tempo em segundos para subir de nível (ex: a cada 30 segundos)")]
    public float tempoPorNivel = 30f;

    [Header("Efeito de Gelo (Freeze)")]
    public GameObject painelHudFrio;
    private bool geloAtivo = false;
    private float cronometroGelo = 0f;

    [Header("Efeito do Ventilador (Vento)")]
    public TextMeshProUGUI textoDirecaoVento; // TODO: Trocar por algum icone ou algo diferente

    // Variáveis de controle interno

    private int pontuacaoAtual = 0;

    private float velocidadeGlobalAtual;
    private int nivelAtual = 1;
    private float cronometroNivel;
    private float tempoJogoTotal;
    private float cronometroAtualizacaoUi;
    private const float IntervaloAtualizacaoUi = 0.25f;

    #endregion

    #region Ciclo

    void Awake()
    {
        // Configura o Singleton
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        IniciarNovoJogo();
    }

    void Update()
    {
        if (!jogoAtivo) return;

        var teclado = Keyboard.current;
        if (teclado != null && (teclado.escapeKey.wasPressedThisFrame || teclado.pKey.wasPressedThisFrame)) //ESC ou P para pausar/despausar
        {
            if (jogoPausado)
            {
                DespausarJogo();
            }
            else
            {
                PausarJogo();
            }
        }

        // Se estiver pausado, impede que o cronômetro do nível continue correndo
        if (jogoPausado) return;

        // Avança o cronômetro do nível atual e o tempo total de jogo
        cronometroNivel += Time.deltaTime;
        tempoJogoTotal += Time.deltaTime;

        // Se o tempo do nível estourou, sobe de nível!
        if (cronometroNivel >= tempoPorNivel)
        {
            SubirDeNivel();
        }

        cronometroAtualizacaoUi += Time.deltaTime;
        if (cronometroAtualizacaoUi >= IntervaloAtualizacaoUi)
        {
            cronometroAtualizacaoUi = 0f;
            AtualizarTextoVisual();
        }

        // Controle do tempo do Gelo
        if (geloAtivo)
        {
            cronometroGelo -= Time.deltaTime;
            if (cronometroGelo <= 0f)
            {
                DesativarEfeitoGelo();
            }
        }
    }

    #endregion

    #region Funcoes

    //---- Funcoes de controle ----//

    void IniciarNovoJogo()
    {
        Time.timeScale = 1f; // Garante que o tempo está normal ao começar/reiniciar

        //Esconde os paineis
        if (painelPause != null) painelPause.SetActive(false);
        if (painelGameover != null) painelGameover.SetActive(false);

        jogoAtivo = true;
        pontuacaoAtual = 0;
        nivelAtual = 1;
        cronometroNivel = 0f;
        tempoJogoTotal = 0f;
        cronometroAtualizacaoUi = 0f;

        velocidadeGlobalAtual = velocidadeInicial;

        AtualizarTextoVisual();

        Invoke(nameof(DispararPrimeiroBloco), 2f /*tempo anim contDown*/);
    }

    public void PausarJogo()
    {
        jogoPausado = true;
        Time.timeScale = 0f; // Congela o tempo do Unity (para tudo!)

        if (painelPause != null)
        {
            painelPause.SetActive(true); // Mostra o menu na tela
        }
    }

    public void DespausarJogo()
    {
        jogoPausado = false;
        Time.timeScale = 1f; // Faz o tempo voltar ao normal

        if (painelPause != null)
        {
            painelPause.SetActive(false); // Esconde o menu da tela
        }
    }

    public void FinalizarJogo()
    {
        if (!jogoAtivo) return;

        Debug.LogWarning("🚨 GAMEOVER! GerenciadorDeJogo interrompeu a partida.");

        jogoAtivo = false;
        Time.timeScale = 0f; // Congela tempo

        if (HUD != null) //Esconde o HUD de jogo
            HUD.SetActive(false);

        if (painelGameover != null) //Exibe o painel de GameOver
            painelGameover.SetActive(true);

        //Carrega os textos finais de GameOver



        if (GameOver_textoNivel != null)
        {
            GameOver_textoNivel.text = $"NÍVEL {nivelAtual}";
        }

        if (GameOver_textoTempoJogo != null)
        {
            GameOver_textoTempoJogo.text = $"TEMPO {FormatarTempo(tempoJogoTotal)}";
        }

        if (GameOver_textoPontuacao != null)
        {
            GameOver_textoPontuacao.text = $"PONTOS {pontuacaoAtual.ToString("D5")}";
        }

    }

    public void ReiniciarJogo()
    {
        GerenciadorCenas.Instancia.RecarregarCenaAtual();
    }

    public void MenuPrincipal()
    {
        GerenciadorCenas.Instancia.CarregarMenu();
    }

    //---- Funcoes de jogo ----//

    void DispararPrimeiroBloco()
    {
        // Procura o gerador na cena e manda ele criar o bloco
        GeradorDeBlocos gerador = FindAnyObjectByType<GeradorDeBlocos>();
        if (gerador != null)
        {
            gerador.CriarBloco();
        }
    }

    void SubirDeNivel()
    {
        nivelAtual++;
        cronometroNivel = 0f; // Zera o cronômetro para o próximo nível

        // Aplica o salto de velocidade se ainda não atingiu o limite máximo
        if (velocidadeGlobalAtual > velocidadeMaximaLimite)
        {
            velocidadeGlobalAtual -= reducaoPorNivel;

            // Garante que não ultrapasse o limite humano determinado
            if (velocidadeGlobalAtual < velocidadeMaximaLimite)
            {
                velocidadeGlobalAtual = velocidadeMaximaLimite;
            }
        }

        Debug.Log($"🚀 Subiu para o Nível {nivelAtual}! Nova velocidade: {velocidadeGlobalAtual}s por passo.");
        AtualizarTextoVisual();
    }

    public float ObterVelocidadeAtual()
    {
        // Se o gelo estiver ativo, a peça demora o DOBRO do tempo para cair (fica 2x mais lenta)
        if (geloAtivo)
        {
            return velocidadeGlobalAtual * 2f;
        }

        return velocidadeGlobalAtual;
    }

    public int ObterNivelAtual()
    {
        return nivelAtual;
    }

    public float ObterTempoJogo()
    {
        return tempoJogoTotal;
    }

    public void AdicionarPontos(int quantidadeLinhas)
    {
        if (!jogoAtivo) return;

        if (quantidadeLinhas == 1) pontuacaoAtual += 100;
        else if (quantidadeLinhas == 2) pontuacaoAtual += 300;
        else if (quantidadeLinhas == 3) pontuacaoAtual += 500;
        else if (quantidadeLinhas >= 4) pontuacaoAtual += 800;

        AtualizarTextoVisual();
    }

    //---- Funcoes de UI ----//

    void AtualizarTextoVisual()
    {
        if (HUD_textoNivel != null)
        {
            HUD_textoNivel.text = $"NÍVEL {nivelAtual}";
        }

        if (HUD_textoTempoJogo != null)
        {
            HUD_textoTempoJogo.text = $"TEMPO {FormatarTempo(tempoJogoTotal)}";
        }

        if (HUD_textoPontuacao != null)
        {
            HUD_textoPontuacao.text = "PONTOS\n" + pontuacaoAtual.ToString("D5");
        }
    }

    string FormatarTempo(float segundos)
    {
        int minutos = Mathf.FloorToInt(segundos / 60f);
        int segs = Mathf.FloorToInt(segundos % 60f);
        return $"{minutos:D2}:{segs:D2}";
    }

    //---- Funcoes de Efeito de Gelo ----//

    public void AtivarEfeitoGelo(float duracao)
    {
        geloAtivo = true;
        cronometroGelo = duracao;
        if (painelHudFrio != null) painelHudFrio.SetActive(true); // Mostra o efeito visual de frio
        Debug.Log("❄️ O jogo foi congelado! Peças mais lentas.");
    }

    void DesativarEfeitoGelo()
    {
        geloAtivo = false;
        if (painelHudFrio != null) painelHudFrio.SetActive(false); // Esconde o efeito visual
        Debug.Log("☀️ O gelo derreteu! Velocidade normal restabelecida.");
    }

    //---- Funcoes de Efeito de Vento ----//

    public void AtualizarHUDVento(int direcao) //TODO: Trocar por um icone ou outra coisa
    {
        if (textoDirecaoVento == null) return;

        if (direcao == -1) textoDirecaoVento.text = "<- Vento";
        else if (direcao == 1) textoDirecaoVento.text = "Vento ->";
        else textoDirecaoVento.text = "";
    }

    #endregion
}