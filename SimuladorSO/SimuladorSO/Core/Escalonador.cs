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
        private  Queue<Processo> processosBloqueados;
        private GerenciadorDeMemoria gerenciadorMemoria;
        private int quantum = 2;

        public Escalonador(GerenciadorDeMemoria gm)
        {
            processosProntos = new Queue<Processo>();
            processosBloqueados = new Queue<Processo>();
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
            List<Processo> listaTemporariaBloqueados = processosBloqueados.ToList();
            Processo processoParaRemover = listaTemporaria.FirstOrDefault(p => p.ID == processoId);            

            if (processoParaRemover == null) 
            {
                processoParaRemover = listaTemporariaBloqueados.FirstOrDefault(p => p.ID == processoId);            
            }

            if (processoParaRemover != null)
            {
                // Libera a memória antes de remover
                gerenciadorMemoria.Liberar(processoParaRemover);

                if (processoParaRemover.Status == ProcessState.Pronto)
                {
                    processosProntos = new Queue<Processo>(listaTemporaria.Where(p => p.ID != processoId));
                    Console.WriteLine($"Processo {processoId} foi parado e removido da fila.");
                }
                else if (processoParaRemover.Status == ProcessState.Bloqueado)
                {
                    processosBloqueados = new Queue<Processo>(listaTemporariaBloqueados.Where(p => p.ID != processoId));
                    Console.WriteLine($"Processo {processoId} foi parado e removido da fila de bloqueados.");
                }
            }
            else
            {
                Console.WriteLine($"Processo com ID {processoId} não encontrado na fila.");
            }
        }
        

        public void BloquearProcesso(int processoId, int tempoDeBloqueio)
        {
            List<Processo> listaTemporaria = processosProntos.ToList();
            Processo processoParaBloquear = listaTemporaria.FirstOrDefault(p => p.ID == processoId);
            if (processoParaBloquear != null && processoParaBloquear.Status != ProcessState.Bloqueado)
            {
                processoParaBloquear.Status = ProcessState.Bloqueado;
                processoParaBloquear.TempoBloqueioRestante = tempoDeBloqueio;
                processosBloqueados.Enqueue(processoParaBloquear);
                processosProntos = new Queue<Processo>(listaTemporaria.Where(p => p.ID != processoId));
                Console.WriteLine($"Processo {processoId} foi bloqueado.");
            }
            else if (processoParaBloquear != null && processoParaBloquear.Status == ProcessState.Bloqueado)
            {
                Console.WriteLine($"Processo {processoId} já está bloqueado.");
            }
             else
            {
                Console.WriteLine($"Processo com ID {processoId} não encontrado na fila.");
            }
        }

        public void ListarProcessos()
        {
            if (processosProntos.Count == 0 && processosBloqueados.Count == 0 )
            {
                Console.WriteLine("--- Nenhum processo ativo ---");
                return;
            }


            Console.WriteLine("--- Processos Prontos ---");
            foreach (var processo in processosProntos)
            {
                Console.WriteLine($"ID: {processo.ID} - Nome: {processo.Nome} Status: {processo.Status}");
            }
            Console.WriteLine("--- Processos Bloqueados ---");
            foreach (var processo in processosBloqueados)
            {
                Console.WriteLine($"ID: {processo.ID} - Nome: {processo.Nome} Status: {processo.Status} Tempo de bloqueio: {processo.TempoBloqueioRestante}");
            }
        }


        public void ExecutarCicloDaCPU()
        {
            if (processosBloqueados.Count > 0)
            {
                int bloqueadosCount = processosBloqueados.Count;
                for (int i = 0; i < bloqueadosCount; i++)
                {
                    Processo processoBloqueado = processosBloqueados.Dequeue();
                    processoBloqueado.TempoBloqueioRestante -= 1;
                    if (processoBloqueado.TempoBloqueioRestante <= 0)
                    {
                        processoBloqueado.Status = ProcessState.Pronto;
                        processosProntos.Enqueue(processoBloqueado);
                        Console.WriteLine($"Processo ID: {processoBloqueado.ID} ({processoBloqueado.Nome}) desbloqueado e movido para a fila de prontos.");
                    }
                    else
                    {
                        processosBloqueados.Enqueue(processoBloqueado);
                    }
                }
            }

            if (processosProntos.Count > 0)
            {

                Processo processoAtual = processosProntos.Peek();

                processoAtual.Status = ProcessState.Executando;
                if (processoAtual.QuantumRestante == 0)
                {
                    processoAtual.QuantumRestante = quantum;
                }
                Console.WriteLine($"--> Executando processo ID: {processoAtual.ID} ({processoAtual.Nome})");

                Thread.Sleep(500);
                processoAtual.TempoDeExecucao += 1;
                processoAtual.QuantumRestante -= 1;

                if (processoAtual.TempoDeExecucao >= processoAtual.TempoTotal)
                {
                    processoAtual.Status = ProcessState.Finalizado;
                    Console.WriteLine($"Processo ID: {processoAtual.ID} ({processoAtual.Nome}) {processoAtual.Status}.");
                    gerenciadorMemoria.Liberar(processoAtual);
                    processosProntos.Dequeue();
                }
                else if (processoAtual.QuantumRestante == 0)
                {
                    processoAtual.Status = ProcessState.Pronto;
                    processosProntos.Dequeue();
                    processosProntos.Enqueue(processoAtual);
                }
            }
            else
            {
                Console.WriteLine("CPU ociosa...");
            }
        }

    }
}