using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
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
        public List<string> FrasesParaExclusao { get; set; }

        public Feed(string url)
        {
            _url = url;
        }

        public bool CarregarUltimosTopicos(int numeroDeTopicos, int tamanhoMinimoPalavra)
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
                    string sumario = TextHelpers.RemoveHtmlTags(item.Summary.Text);
                    string titulo = TextHelpers.RemoveHtmlTags(item.Title.Text);

                    var metricas = ObterMetricasDePalavras(sumario, tamanhoMinimoPalavra);

                    Topicos.Add(item: new Topico(titulo, metricas));
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public string ObterEstatisticasPorTopico()
        {
            string resultado = "";

            foreach (var topico in Topicos)
            {
                resultado += $"{topico.ObterQuantidadeDePalavras()} - {topico.Titulo}\n";
            }

            return resultado;
        }

        public string ObterEstatisticasDoFeed(int numeroPalavrasMaisCitadas)
        {
            string resultado = "";

            var metricasDoFeed = new Dictionary<string, int>();

            foreach (var topico in Topicos)
            {
                int ocorrencias = 0;
                foreach (var item in topico.Metricas)
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

        private List<Metrica> ObterMetricasDePalavras(string texto, int tamanhoMinimoPalavra)
        {
            FrasesParaExclusao.ForEach(s => texto = texto.Replace(s, ""));

            var metricas = Regex.Split(texto.ToLower(), @"\W+")
                                .Where(s => !Artigos.Contains(s) &&
                                            !Preposicoes.Contains(s) &&
                                            !string.IsNullOrEmpty(s)  &&
                                            s.Length >= tamanhoMinimoPalavra)
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
