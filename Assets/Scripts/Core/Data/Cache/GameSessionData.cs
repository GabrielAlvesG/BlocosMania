using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Core.Data.Cache
{
    //Usamos para armazenar dados da sessao do jogo que vem de outras cenas, como o nome do jogador, etc.
    //Guardar temporariamente na RAM
    internal class GameSessionData
    {
        public static string NomeJogador = "Player";
    }
}
