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
        private bool[] blocosDeMemoria;

        public GerenciadorDeMemoria(int tamanhoTotal)
        {
            blocosDeMemoria = new bool[tamanhoTotal];
        }

        public bool Alocar(Processo processo)
        {
            int blocosNecessarios = processo.TamanhoMemoria;
            int blocosLivresConsecutivos = 0;
            int indiceInicial = -1;

            for (int i = 0; i < blocosDeMemoria.Length; i++)
            {
                if (!blocosDeMemoria[i])
                {
                    if (blocosLivresConsecutivos == 0)
                        indiceInicial = i;
                    blocosLivresConsecutivos++;
                }
                else
                {
                    blocosLivresConsecutivos = 0;
                    indiceInicial = -1;
                }

                if (blocosLivresConsecutivos >= blocosNecessarios)
                {
                    processo.EnderecoInicialMemoria = indiceInicial;
                    for (int j = 0; j < blocosNecessarios; j++)
                    {
                        blocosDeMemoria[indiceInicial + j] = true;
                    }
                    Console.WriteLine($"Memória alocada para o Processo {processo.ID} no endereço {indiceInicial}.");
                    return true;
                }
            }
            Console.WriteLine($"Falha ao alocar memória para o Processo {processo.ID}. Espaço insuficiente.");
            return false;
        }

        public void Liberar(Processo processo)
        {
            if (processo.EnderecoInicialMemoria != -1)
            {
                for (int i = 0; i < processo.TamanhoMemoria; i++)
                {
                    blocosDeMemoria[processo.EnderecoInicialMemoria + i] = false;
                }
                Console.WriteLine($"Memória liberada do Processo {processo.ID}.");
            }
        }
    }
}