using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace sla
{
    public class TipoFluxoDto {
        public int IdTipoFluxo { get; set; }
        public string Descricao { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //var idsFluxoRemovidos = new int[] { 3, 5 };  // Funciona adequadamente
            var idsFluxoRemovidos = new List<int> { 3, 5 }; // Retorna Exceção
            Expression<Func<TipoFluxoDto, bool>> filtro = x => !idsFluxoRemovidos.Contains(x.IdTipoFluxo);

            Console.WriteLine(filtro.ToCustomString());
        }
    }
}
