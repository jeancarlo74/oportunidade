using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;

namespace FeedServices
{
    public class Feed
    {
        private string _url;

        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public List<Topico> Topicos { get; set; }
        public List<string> Artigos { get; set; }
        public List<string> Preposicoes { get; set; }
        public List<string> FrasesDesconsiderar { get; set; }
        public int TamanhoMinimoPalavras { get;  set; }

        public Feed(string url)
        {
            _url = url;
        }

        /// <summary>
        /// Carrega os últimos tópicos publicados do Feed
        /// </summary>
        /// <param name="numeroDeTopicos"></param>
        /// <returns>true: Carregado com Sucesso | false: Falha ao carregar tópicos do Feed</returns>
        public bool CarregarUltimosTopicos(int numeroDeTopicos)
        {
            try
            {
                XmlReader reader = XmlReader.Create(_url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                reader.Close();

                Titulo = feed.Title.Text;
                Descricao = feed.Description.Text;

                var feedLastItens = feed.Items
                    .OrderByDescending(o => o.PublishDate)
                    .Take(numeroDeTopicos);

                Topicos = new List<Topico>();

                foreach (SyndicationItem item in feedLastItens)
                {
                    string titulo = TextHelpers.RemoveHtmlTags(item.Title.Text);
                    string sumario = TextHelpers.RemoveHtmlTags(item.Summary.Text);

                    Topicos.Add(item: new Topico(this, titulo, sumario));
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtém a contagem de palavra do Feed por Tópico
        /// </summary>
        /// <returns>Texto com a contagem</returns>
        public string ObterEstatisticasPorTopico()
        {
            string resultado = "";

            foreach (var topico in Topicos)
            {
                resultado += $"{topico.ObterQuantidadeDePalavras()} - {topico.Titulo}\n";
            }

            return resultado;
        }
        /// <summary>
        /// Obter a contagem de palavras do Feed
        /// </summary>
        /// <param name="numeroPalavrasMaisCitadas">Quantidade de Palavras retornadas</param>
        /// <returns></returns>
        public string ObterEstatisticasDoFeed(int numeroPalavrasMaisCitadas)
        {
            string resultado = "";

            var metricasDoFeed = new Dictionary<string, int>();

            foreach (var topico in Topicos)
            {
                int ocorrencias = 0;

                var metricas = topico.ObterMetricasDePalavras();
                foreach (var item in metricas)
                {
                    if (metricasDoFeed.TryGetValue(item.Palavra, out ocorrencias))
                    {
                        metricasDoFeed[item.Palavra] += item.NumeroOcorrecias;
                    }
                    else
                    {
                        metricasDoFeed.Add(item.Palavra, item.NumeroOcorrecias);
                    }
                }
            }

            metricasDoFeed.OrderByDescending(o => o.Value)
                          .ThenBy(t=> t.Key)
                          .Take(numeroPalavrasMaisCitadas)
                          .ToList()
                          .ForEach(f => resultado += $"{f.Value} - {f.Key}\n");

            return resultado;
        }
    }
}
