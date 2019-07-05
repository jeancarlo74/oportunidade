using FeedServices;
using System;
using System.Collections.Generic;

namespace FeedStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "https://www.minutoseguros.com.br/blog/feed/";

            Feed feed = new Feed(url);

            feed.Artigos = new List<string>
                { "o", "a", "os", "as", "um", "uma", "uns", "umas" };

            feed.Preposicoes = new List<string>
                {"ao", "à", "aos", "às","de", "do", "da", "dos", "das"
                ,"em", "no", "na", "nos", "nas", "por", "per", "pelo", "pela", "pelos", "pelas"
                , "num", "numa", "nuns", "numas","de", "dum", "duma", "duns", "dumas"};

            feed.FrasesParaExclusao = new List<string>
                { "O post", "apareceu primeiro em", "Blog Minuto Seguros" };

            if (!feed.CarregarUltimosTopicos(numeroDeTopicos: 10, tamanhoMinimoPalavra: 0))
            {
                Console.WriteLine($"Não foi possível carregar o Feed '{url}'");
                Console.Read();
                return;
            }

            Console.WriteLine("Gerador de estatísticas de Feeds\n");

            Console.WriteLine($"Título do Feed: {feed.Titulo}");
            Console.WriteLine($"Descrição: {feed.Descricao}\n");

            Console.WriteLine("Quantidade de palavras por tópico (útimos 10 tópicos):");
            Console.Write(feed.ObterEstatisticasPorTopico());

            Console.WriteLine("\nDez principais palavras abordadas:");
            Console.Write(feed.ObterEstatisticasDoFeed(numeroPalavrasMaisCitadas: 10));

            Console.Read();
        }
    }
}
