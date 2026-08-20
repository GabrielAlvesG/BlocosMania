using UnityEngine;

public class BlocoGelo : MonoBehaviour
{
    public void AtivarGelo()
    {
        if (GerenciadorJogo.Instancia != null)
        {
            GerenciadorJogo.Instancia.AtivarEfeitoGelo(8f); // 8 segundos de lentidão
        }
    }
}
