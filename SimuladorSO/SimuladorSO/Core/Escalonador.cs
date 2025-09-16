using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimuladorSO.Models;

namespace SimuladorSO.Core
{
    public class Escalonador
    {
        private Queue<Processo> processosProntos;
        private GerenciadorDeMemoria gerenciadorMemoria;

        public Escalonador(GerenciadorDeMemoria gm)
        {
            processosProntos = new Queue<Processo>();
            gerenciadorMemoria = gm;
        }

        public void AdicionarProcesso(Processo p)
        {
            if (gerenciadorMemoria.Alocar(p))
            {
                p.Status = ProcessState.Pronto;
                processosProntos.Enqueue(p);
                Console.WriteLine($"Processo {p.ID} ({p.Nome}) adicionado à fila de prontos.");
            }
        }

        public void PararProcesso(int processoId)
        {
            // Converte a fila para uma lista para facilitar a busca e remoção
            List<Processo> listaTemporaria = processosProntos.ToList();
            Processo processoParaRemover = listaTemporaria.FirstOrDefault(p => p.ID == processoId);

            if (processoParaRemover != null)
            {
                // Libera a memória antes de remover
                gerenciadorMemoria.Liberar(processoParaRemover);

                // Recria a fila sem o processo removido
                processosProntos = new Queue<Processo>(listaTemporaria.Where(p => p.ID != processoId));
                Console.WriteLine($"Processo {processoId} foi parado e removido da fila.");
            }
            else
            {
                Console.WriteLine($"Processo com ID {processoId} não encontrado na fila.");
            }
        }


        public void ExecutarCicloDaCPU()
        {
            if (processosProntos.Count > 0)
            {
                Processo processoAtual = processosProntos.Dequeue();

                processoAtual.Status = ProcessState.Executando;
                Console.WriteLine($"--> Executando processo ID: {processoAtual.ID} ({processoAtual.Nome})");

                Thread.Sleep(500);
                processoAtual.TempoDeExecucao += 1;

                processoAtual.Status = ProcessState.Pronto;
                processosProntos.Enqueue(processoAtual);
            }
            else
            {
                Console.WriteLine("CPU ociosa...");
            }
        }

    }
}