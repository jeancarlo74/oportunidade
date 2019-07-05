using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FeedServices
{
    public class Topico
    {
        private string _titulo;
        private string _sumario;
        private Feed _feed;

        public string Titulo { get => _titulo; }
        public string Sumario { get => _sumario; }
        public Feed Feed { get => _feed; }


        public Topico(Feed feed, string titulo, string sumario)
        {
            _feed = feed;
            _titulo = titulo;
            _sumario = sumario;
        }

        /// <summary>
        /// Obtém a quantidade de palavras do tópico
        /// </summary>
        /// <returns>Quantidade de palavras</returns>
        public int ObterQuantidadeDePalavras()
        {
            List<Metrica> metricas = ObterMetricasDePalavras();
            return metricas.Sum(g => g.NumeroOcorrecias);
        }

        /// <summary>
        /// Obtém a métricas de palavras do tópico
        /// </summary>
        /// <param name="texto"></param>
        /// <returns>retorna um lista de Métricas: Palavra/Quantidade</returns>
        public List<Metrica> ObterMetricasDePalavras(string texto="")
        {
            if (string.IsNullOrEmpty(texto))
                texto = _sumario;

            if (Feed.FrasesDesconsiderar != null)
                Feed.FrasesDesconsiderar.ForEach(s => texto = texto.Replace(s, ""));

            var metricas = Regex.Split(texto.ToLower(), @"\W+")
                                .Where(s => (Feed.Artigos == null || !Feed.Artigos.Contains(s)) &&
                                            (Feed.Preposicoes == null || !Feed.Preposicoes.Contains(s)) &&
                                            !string.IsNullOrEmpty(s) &&
                                            s.Length >= Feed.TamanhoMinimoPalavras)
                                .GroupBy(s => s)
                                .Select(group => new Metrica
                                {
                                    Palavra = group.Key,
                                    NumeroOcorrecias = group.Count()
                                })
                                .ToList();

            return metricas;
        }
    }

}
