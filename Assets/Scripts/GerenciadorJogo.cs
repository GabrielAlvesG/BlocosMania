using TMPro;
using UnityEngine;

public class GerenciadorJogo : MonoBehaviour
{
    #region Variaveis

    // Padrão Singleton para qualquer script conseguir acessar o GameManager facilmente
    public static GerenciadorJogo Instancia { get; private set; }

    [Header("Estados do Jogo")]
    public bool jogoAtivo = true;

    [Header("Interface do Usuário (UI)")]
    public TextMeshProUGUI textoPontuacao;

    [Header("Configurações de Nível e Velocidade")]
    [Tooltip("Velocidade do Nível 1 (segundos por passo)")]
    public float velocidadeInicial = 0.8f;
    [Tooltip("O quanto a queda acelera a cada novo nível (ex: reduz 0.1s por nível)")]
    public float reducaoPorNivel = 0.08f;
    [Tooltip("Velocidade máxima limite para o jogo não ficar impossível")]
    public float velocidadeMaximaLimite = 0.1f;
    [Tooltip("Tempo em segundos para subir de nível (ex: a cada 30 segundos)")]
    public float tempoPorNivel = 30f;


    private int pontuacaoAtual = 0;


    // Variáveis de controle interno
    private float velocidadeGlobalAtual;
    private int nivelAtual = 1;
    private float cronometroNivel;

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

        // Avança o cronômetro do nível atual
        cronometroNivel += Time.deltaTime;

        // Se o tempo do nível estourou, sobe de nível!
        if (cronometroNivel >= tempoPorNivel)
        {
            SubirDeNivel();
        }
    }

    #endregion

    #region Funcoes

    void IniciarNovoJogo()
    {
        jogoAtivo = true;
        pontuacaoAtual = 0;
        nivelAtual = 1;
        cronometroNivel = 0f;

        velocidadeGlobalAtual = velocidadeInicial;

        AtualizarTextoVisual();

        //TODO: trocar pela animacao de inicio do jogo, com contagem regressiva
        DispararPrimeiroBloco();
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
        return velocidadeGlobalAtual;
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

    public void FinalizarJogo()
    {
        if (!jogoAtivo) return;

        jogoAtivo = false;
        Debug.LogWarning("🚨 GAMEOVER! GerenciadorDeJogo interrompeu a partida.");

        if (textoPontuacao != null)
        {
            textoPontuacao.text = "GAME OVER\n" + pontuacaoAtual.ToString("D5");
        }
    }

    void DispararPrimeiroBloco()
    {
        // Procura o gerador na cena e manda ele criar o bloco
        GeradorDeBlocos gerador = FindAnyObjectByType<GeradorDeBlocos>();
        if (gerador != null)
        {
            gerador.CriarBloco();
        }
    }

    void AtualizarTextoVisual()
    {
        if (textoPontuacao != null)
        {
            textoPontuacao.text = "PONTOS\n" + pontuacaoAtual.ToString("D5");
        }
    }

    #endregion
}