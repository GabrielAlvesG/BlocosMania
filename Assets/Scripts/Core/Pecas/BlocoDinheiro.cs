using UnityEngine;

public class BlocoDinheiro : MonoBehaviour
{
    [Header("Configurações")]
    public int moedasAoQuebrar = 1; // Quantidade de moedas que o bloco dá

    // Chamado pelo GerenciadorGrid quando a linha deste bloco estoura
    public void ColetarMoedas()
    {
        if (GerenciadorJogo.Instancia != null)
        {
            GerenciadorJogo.Instancia.AdicionarMoedas(moedasAoQuebrar);
        }
    }
}
