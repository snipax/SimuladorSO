using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorSO.Models;

namespace SimuladorSO.Core
{
    public class GerenciadorDeMemoria
    {
        private bool[] paginas;
        private int tamanhoPagina;

        public GerenciadorDeMemoria(int tamanhoTotal, int tamanhopagina)
        {
            paginas = new bool[tamanhoTotal / tamanhopagina];
            tamanhoPagina = tamanhopagina;
        }

        public bool Alocar(Processo processo)
        {
            int paginasNecessarias = (int)Math.Ceiling((double)processo.TamanhoMemoria / tamanhoPagina);
            processo.TabelaPaginas.Clear();

            for (int i = 0; i < paginas.Length; i++)
            {
                if (!paginas[i])
                {
                    paginas[i] = true;
                    processo.TabelaPaginas.Add(i); // Mapeia a página para o processo
                    paginasNecessarias--;
                    if (paginasNecessarias == 0)
                    {
                        Console.WriteLine($"Páginas alocadas para o Processo {processo.ID}: {string.Join(", ", processo.TabelaPaginas)}");
                        return true;
                    }
                }
            }
            for (int i = 0; i < processo.TabelaPaginas.Count; i++)
            {
                paginas[processo.TabelaPaginas[i]] = false; // Libera a página
            }
            processo.TabelaPaginas.Clear();
            Console.WriteLine($"Falha ao alocar memória para o Processo {processo.ID}. Espaço insuficiente.");
            return false;
        }

        public void Liberar(Processo processo)
        {
            if (processo.TabelaPaginas != null)
            {
                for (int i = 0; i < processo.TabelaPaginas.Count; i++)
                {
                    paginas[processo.TabelaPaginas[i]] = false; // Libera a página

                }
            }
            else
            {
                Console.WriteLine("Processo não possui páginas alocadas.");
            }
            processo.TabelaPaginas.Clear();
            Console.WriteLine($"Memória liberada do Processo {processo.ID}.");

        }
    }
}
