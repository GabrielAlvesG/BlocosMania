using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class PecaGrid : MonoBehaviour
{
    #region Variaveis

    private float tempoUltimoPasso;
    private float tempoPorPasso = 0.8f;

    [Header("Configuração de Queda Rápida")]
    [Tooltip("Velocidade da queda ao segurar para baixo (ex: 0.05s por passo, muito mais rápido)")]
    public float velocidadeQuedaRapida = 0.05f;

    [Header("Configuração de Vento")]
    public float intervaloVento = 1.2f; // A cada 1.2 segundos o vento empurra
    public AudioMixerGroup OutPutGroup; // Grupo de mixer para efeitos sonoros
    public AudioClip audioClipVento; // Som que será tocado quando o vento empurrar a peça

    private float tempoUltimoEmpurraoVento;
    private int direcaoDoVentoNestaPeca = 0; // 0 = Sem vento, -1 = Esquerda, 1 = Direita
    private AudioSource audioSource; // Fonte de áudio para tocar o som de vento

    #endregion

    #region Ciclo

    void Start()
    {
        tempoUltimoPasso = Time.time;
        tempoUltimoEmpurraoVento = Time.time; // Inicializa o vento

        tempoPorPasso = GerenciadorJogo.Instancia.ObterVelocidadeAtual();

        ChecarSeTemVentilador();

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
            tempoPorPasso = GerenciadorJogo.Instancia.ObterVelocidadeAtual();
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

                //zeramos o HUD do vento (pois a peça que tinha o bloco de vento foi fixada)
                GerenciadorJogo.Instancia.AtualizarHUDVento(0);

                // Destrói apenas o objeto Pai vazio, deixando os blocos fixos na cena
                Destroy(this);

                // Spawna a próxima peça
                FindAnyObjectByType<GeradorDeBlocos>().CriarBloco();
                return;
            }
            tempoUltimoPasso = Time.time;
        }

        // Ventilador
        if (direcaoDoVentoNestaPeca != 0 && Time.time - tempoUltimoEmpurraoVento >= intervaloVento)
        {
            Mover(new Vector3(direcaoDoVentoNestaPeca, 0, 0));
            tempoUltimoEmpurraoVento = Time.time;
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

    private void OnDestroy()
    {
        // Para o som de vento quando a peça é destruída
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(audioSource);
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

    void ChecarSeTemVentilador()
    {
        BlocoVento blocoDeVentoEncontrado = GetComponentInChildren<BlocoVento>();
        if (blocoDeVentoEncontrado != null)
        {
            // Adota a direção que o bloco sorteou
            direcaoDoVentoNestaPeca = blocoDeVentoEncontrado.minhaDirecaoVento;

            if (audioSource == null)
            {
                audioSource = transform.AddComponent<AudioSource>();
                audioSource.playOnAwake = false; // Desativa para não tocar ao carregar
                audioSource.outputAudioMixerGroup = OutPutGroup; // Configura para o grupo de SFX
            }

            audioSource.loop = true; // Faz o som de vento tocar em loop enquanto a peça estiver ativa
            audioSource.clip = audioClipVento; // Atribui o som do vento ao AudioSource
            audioSource.Play();
        }
        else
        {
            direcaoDoVentoNestaPeca = 0; // Peça comum, sem vent
        }

        GerenciadorJogo.Instancia.AtualizarHUDVento(direcaoDoVentoNestaPeca);
    }

    #endregion
}
