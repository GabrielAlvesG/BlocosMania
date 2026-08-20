using UnityEngine;

public class BlocoVento : MonoBehaviour
{
    public int minhaDirecaoVento { get; private set; }

    void Awake()
    {
        // Sorteia se o vento vai empurrar para a esquerda (-1) ou para a direita (1)
        minhaDirecaoVento = Random.value > 0.5f ? 1 : -1;
    }
}
