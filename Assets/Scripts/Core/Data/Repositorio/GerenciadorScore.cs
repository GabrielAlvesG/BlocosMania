using System.IO;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Core.Data.Modelos;

namespace Assets.Scripts.Core.Data.Repositorio
{
    public static class GerenciadorScore
    {
        private static string FilePath => Path.Combine(Application.persistentDataPath, "scores.json");
        private static int MAX_SCORES = 10;

        public static int Recorde => LoadListaPontuacoes().scores.Count > 0 ? LoadListaPontuacoes().scores.Max(x => x.Pontuacao) : 0;

        // Carrega a lista salva do arquivo .json
        public static HighScoreList LoadListaPontuacoes()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                return JsonUtility.FromJson<HighScoreList>(json);
            }

            return new HighScoreList();
        }

        // Adiciona uma nova pontuação com o tempo jogado
        public static void AddScore(string nome, int pontos, float tempo)
        {
            HighScoreList data = LoadListaPontuacoes();

            // Adiciona a nova pontuação
            data.scores.Add(new ScoreData(nome, tempo, pontos));

            // Salva apenas o maximo definido de pontuações, ordenando por pontuação e tempo jogado
            data.scores = data.scores
                .OrderByDescending(x => x.Pontuacao)
                .ThenByDescending(x => x.TempoJogado)
                .Take(MAX_SCORES)
                .ToList();

            // Salva o JSON formatado no disco
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);

            Debug.Log($"Ranking atualizado e salvo em: {FilePath}");
        }
    }
}