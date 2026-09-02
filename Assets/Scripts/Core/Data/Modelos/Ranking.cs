using System;
using System.Collections.Generic;

namespace Assets.Scripts.Core.Data.Modelos
{
    [Serializable]
    public class ScoreData
    {
        public string Nome;
        public int Pontuacao;
        public float TempoJogado;

        public ScoreData(string nome, float tempoJogado, int pontuacao)
        {
            this.Nome = nome;
            this.TempoJogado = tempoJogado;
            this.Pontuacao = pontuacao;
        }
    }

    [Serializable]
    public class HighScoreList
    {
        public List<ScoreData> scores = new List<ScoreData>();
    }
}
