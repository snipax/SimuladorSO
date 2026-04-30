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
        
        public Queue<Processo> processosBloqueados { get; private set; }
        public Queue<Processo> processosPrioridadeAlta { get; private set; }
        public Queue<Processo> processosPrioridadeMedia { get; private set; }
        public Queue<Processo> processosPrioridadeBaixa { get; private set; }
        public Queue<Processo> processosFinalizados { get; private set; }
        private GerenciadorDeMemoria gerenciadorMemoria;
        private int quantum = 2;
        private int ciclos = 0;

        public Escalonador(GerenciadorDeMemoria gm)
        {
            processosPrioridadeAlta = new Queue<Processo>();
            processosPrioridadeMedia = new Queue<Processo>();
            processosPrioridadeBaixa = new Queue<Processo>();
            processosBloqueados = new Queue<Processo>();
            processosFinalizados = new Queue<Processo>();
            gerenciadorMemoria = gm;
        }

        public void AdicionarProcesso(Processo p)
        {
            if (gerenciadorMemoria.Alocar(p))
            {
                p.Status = ProcessState.Pronto;
                p.TempoChegada = ciclos;
                switch (p.Prioridade)
                {
                    case NivelPrioridade.Alta:
                        processosPrioridadeAlta.Enqueue(p);
                        break;
                    case NivelPrioridade.Media:
                        processosPrioridadeMedia.Enqueue(p);
                        break;
                    case NivelPrioridade.Baixa:
                        processosPrioridadeBaixa.Enqueue(p);
                        break;
                }
                Console.WriteLine($"Processo {p.ID} ({p.Nome}) adicionado à fila de prontos.");
            }
        }

        public void PararProcesso(int processoId)
        {
            // Converte a fila para uma lista para facilitar a busca e remoção
            List<Processo> listaTemporaria = processosPrioridadeAlta.ToList().Concat(processosPrioridadeMedia).Concat(processosPrioridadeBaixa).ToList();
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
                    switch (processoParaRemover.Prioridade)
                    {
                        case NivelPrioridade.Alta:
                            processosPrioridadeAlta = new Queue<Processo>(processosPrioridadeAlta.Where(p => p.ID != processoId));
                            break;
                        case NivelPrioridade.Media:
                            processosPrioridadeMedia = new Queue<Processo>(processosPrioridadeMedia.Where(p => p.ID != processoId));
                            break;
                        case NivelPrioridade.Baixa:
                            processosPrioridadeBaixa = new Queue<Processo>(processosPrioridadeBaixa.Where(p => p.ID != processoId));
                            break;
                    }

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
            List<Processo> listaTemporaria = processosPrioridadeAlta.ToList().Concat(processosPrioridadeMedia).Concat(processosPrioridadeBaixa).ToList();
            Processo processoParaBloquear = listaTemporaria.FirstOrDefault(p => p.ID == processoId);
            if (processoParaBloquear != null && processoParaBloquear.Status != ProcessState.Bloqueado)
            {
                processoParaBloquear.Status = ProcessState.Bloqueado;
                processoParaBloquear.TempoBloqueioRestante = tempoDeBloqueio;
                processosBloqueados.Enqueue(processoParaBloquear);

                switch (processoParaBloquear.Prioridade)
                {
                    case NivelPrioridade.Alta:
                        processosPrioridadeAlta = new Queue<Processo>(processosPrioridadeAlta.Where(p => p.ID != processoId));
                        break;
                    case NivelPrioridade.Media:
                        processosPrioridadeMedia = new Queue<Processo>(processosPrioridadeMedia.Where(p => p.ID != processoId));
                        break;
                    case NivelPrioridade.Baixa:
                        processosPrioridadeBaixa = new Queue<Processo>(processosPrioridadeBaixa.Where(p => p.ID != processoId));
                        break;
                }
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
            if (processosPrioridadeAlta.Count == 0 && processosPrioridadeMedia.Count == 0 && processosPrioridadeBaixa.Count == 0 && processosBloqueados.Count == 0)
            {
                Console.WriteLine("--- Nenhum processo ativo ---");
                return;
            }


            Console.WriteLine("--- Processos Prontos em prioridade alta ---");
            foreach (var processo in processosPrioridadeAlta)
            {
                Console.WriteLine($"ID: {processo.ID} - Nome: {processo.Nome} Status: {processo.Status}");
            }
            Console.WriteLine("--- Processos Prontos em prioridade média ---");
            foreach (var processo in processosPrioridadeMedia)
            {
                Console.WriteLine($"ID: {processo.ID} - Nome: {processo.Nome} Status: {processo.Status}");
            }
            Console.WriteLine("--- Processos Prontos em prioridade baixa ---");
            foreach (var processo in processosPrioridadeBaixa)
            {
                Console.WriteLine($"ID: {processo.ID} - Nome: {processo.Nome} Status: {processo.Status}");
            }
            Console.WriteLine("--- Processos Bloqueados ---");
            foreach (var processo in processosBloqueados)
            {
                Console.WriteLine($"ID: {processo.ID} - Nome: {processo.Nome} Status: {processo.Status} Tempo de bloqueio: {processo.TempoBloqueioRestante}");
            }
        }

        public void ExibirMetricas()
        {
            if (processosFinalizados.Count == 0)
            {
                Console.WriteLine("--- Nenhum processo finalizado ---");
                return;
            }
            
            Console.WriteLine("--- Processos Finalizados ---");
            foreach (var processo in processosFinalizados)
            {
                Console.WriteLine($"ID: {processo.ID} - Nome: {processo.Nome} Status: {processo.Status} Tempo de execução: {processo.TempoDeExecucao}, Tempo de espera: {processo.TempoEspera}, Turnaround: {processo.TempoTurnaround}");
            }

            int tempoEsperaTotal = processosFinalizados.Sum(p => p.TempoEspera);
            int tempoExecucaoTotal = processosFinalizados.Sum(p => p.TempoDeExecucao);
            int tempoTurnaroundTotal = processosFinalizados.Sum(p => p.TempoTurnaround);
            int quantidadeProcessos = processosFinalizados.Count;

            Console.WriteLine("--- Métricas de Desempenho ---");
            Console.WriteLine($"Tempo médio de espera: {(double)tempoEsperaTotal / quantidadeProcessos}");
            Console.WriteLine($"Tempo médio de execução: {(double)tempoExecucaoTotal / quantidadeProcessos}");
            Console.WriteLine($"Tempo médio de turnaround: {(double)tempoTurnaroundTotal / quantidadeProcessos}");
            Console.WriteLine($"Throughput: {(double)quantidadeProcessos / ciclos}");
        }

        public void ExibirTabelaDePaginas(int processoId)
        {
            List<Processo> listaTemporaria = processosPrioridadeAlta.ToList().Concat(processosPrioridadeMedia).Concat(processosPrioridadeBaixa).ToList();
            List<Processo> listaTemporariaBloqueados = processosBloqueados.ToList();
            Processo processo = listaTemporaria.FirstOrDefault(p => p.ID == processoId);
            if (processo == null)
            {
                processo = listaTemporariaBloqueados.FirstOrDefault(p => p.ID == processoId);
            }
            if (processo != null)
            {
                Console.WriteLine($"Tabela de Páginas para o Processo {processo.ID} ({processo.Nome}):");
                if (processo.TabelaPaginas.Count > 0)
                {
                    for (int i = 0; i < processo.TabelaPaginas.Count; i++)
                    { 
                        Console.WriteLine($"Página {i} -> Frame {processo.TabelaPaginas[i]}"); 
                    }
                }
                else
                {
                    Console.WriteLine("Nenhuma página alocada.");
                }
            }
            else
            {
                Console.WriteLine($"Processo com ID {processoId} não encontrado na fila.");
            }
        }


        public void ExecutarCicloDaCPU()
        {
            ciclos ++;
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
                        switch (processoBloqueado.Prioridade)
                        {
                            case NivelPrioridade.Alta:
                                processosPrioridadeAlta.Enqueue(processoBloqueado);
                                break;
                            case NivelPrioridade.Media:
                                processosPrioridadeMedia.Enqueue(processoBloqueado);
                                break;
                            case NivelPrioridade.Baixa:
                                processosPrioridadeBaixa.Enqueue(processoBloqueado);
                                break;
                        }

                        Console.WriteLine($"Processo ID: {processoBloqueado.ID} ({processoBloqueado.Nome}) desbloqueado e movido para a fila de prontos.");
                    }
                    else
                    {
                        processosBloqueados.Enqueue(processoBloqueado);
                    }
                }
            }

            if (processosPrioridadeAlta.Count > 0 || processosPrioridadeMedia.Count > 0 || processosPrioridadeBaixa.Count > 0)
            {

                Processo processoAtual = null;
                if (processosPrioridadeAlta.Count > 0)
                {
                    processoAtual = processosPrioridadeAlta.Peek();
                }
                else if (processosPrioridadeMedia.Count > 0)
                {
                    processoAtual = processosPrioridadeMedia.Peek();
                }
                else if (processosPrioridadeBaixa.Count > 0)
                {
                    processoAtual = processosPrioridadeBaixa.Peek();
                }

                processoAtual.Status = ProcessState.Executando;
                if (processoAtual.QuantumRestante == 0)
                {
                    processoAtual.QuantumRestante = quantum;
                }
                if (processoAtual.TempoDeExecucao == 0)
                {
                    processoAtual.TempoInicioExecucao = ciclos;
                }
                Console.WriteLine($"--> Executando processo ID: {processoAtual.ID} ({processoAtual.Nome})");

                Thread.Sleep(500);
                processoAtual.TempoDeExecucao += 1;
                processoAtual.QuantumRestante -= 1;

                if (processoAtual.TempoDeExecucao >= processoAtual.TempoTotal)
                {
                    processoAtual.TempoConclusao = ciclos;
                    processoAtual.TempoTurnaround = processoAtual.TempoConclusao - processoAtual.TempoChegada;
                    processoAtual.TempoEspera = processoAtual.TempoTurnaround - processoAtual.TempoTotal;
                    switch (processoAtual.Prioridade)
                    {
                        case NivelPrioridade.Alta:
                            processoAtual.Status = ProcessState.Finalizado;
                            Console.WriteLine($"Processo ID: {processoAtual.ID} ({processoAtual.Nome}) {processoAtual.Status}. Tempo de execução: {processoAtual.TempoDeExecucao}, Tempo de espera: {processoAtual.TempoEspera}, Turnaround: {processoAtual.TempoTurnaround}");
                            gerenciadorMemoria.Liberar(processoAtual);
                            processosPrioridadeAlta.Dequeue();
                            processosFinalizados.Enqueue(processoAtual);
                            break;
                        case NivelPrioridade.Media:
                            processoAtual.Status = ProcessState.Finalizado;
                            Console.WriteLine($"Processo ID: {processoAtual.ID} ({processoAtual.Nome}) {processoAtual.Status}. Tempo de execução: {processoAtual.TempoDeExecucao}, Tempo de espera: {processoAtual.TempoEspera}, Turnaround: {processoAtual.TempoTurnaround}");
                            gerenciadorMemoria.Liberar(processoAtual);
                            processosPrioridadeMedia.Dequeue();
                            processosFinalizados.Enqueue(processoAtual);
                            break;
                        case NivelPrioridade.Baixa:
                            processoAtual.Status = ProcessState.Finalizado;
                            Console.WriteLine($"Processo ID: {processoAtual.ID} ({processoAtual.Nome}) {processoAtual.Status}. Tempo de execução: {processoAtual.TempoDeExecucao}, Tempo de espera: {processoAtual.TempoEspera}, Turnaround: {processoAtual.TempoTurnaround}");
                            gerenciadorMemoria.Liberar(processoAtual);
                            processosPrioridadeBaixa.Dequeue();
                            processosFinalizados.Enqueue(processoAtual);
                            break;
                    }
                }
                else if (processoAtual.QuantumRestante == 0)
                {
                    switch (processoAtual.Prioridade)
                    {
                        case NivelPrioridade.Alta:
                            processosPrioridadeAlta.Dequeue();
                            processosPrioridadeAlta.Enqueue(processoAtual);
                            break;
                        case NivelPrioridade.Media:
                            processosPrioridadeMedia.Dequeue();
                            processosPrioridadeMedia.Enqueue(processoAtual);
                            break;
                        case NivelPrioridade.Baixa:
                            processosPrioridadeBaixa.Dequeue();
                            processosPrioridadeBaixa.Enqueue(processoAtual);
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("CPU ociosa...");
            }
        }

    }
}