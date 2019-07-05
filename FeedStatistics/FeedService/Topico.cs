using System.Collections.Generic;
using System.Linq;

namespace FeedServices
{
    public class Topico
    {
        private string _titulo;
        private List<Metrica> _metricas;

        public string Titulo { get => _titulo; }
        public List<Metrica> Metricas { get => _metricas; }

        public Topico(string titulo, List<Metrica> metricas)
        {
            _titulo = titulo;
            _metricas = metricas;
        }

        public int ObterQuantidadeDePalavras()
        {
            return Metricas.Sum(g => g.NumeroOcorrecias);
        }
    }

}
