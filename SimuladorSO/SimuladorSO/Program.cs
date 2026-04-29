using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimuladorSO.Core;
using SimuladorSO.Models;

namespace SimuladorSO
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Inicializa os componentes do SO
            GerenciadorDeMemoria gm = new GerenciadorDeMemoria(128); // 128MB de RAM simulada
            Escalonador escalonador = new Escalonador(gm);

            // Adiciona processos ao sistema
            escalonador.AdicionarProcesso(new Processo(1, "Notepad", 10, 5));
            escalonador.AdicionarProcesso(new Processo(2, "Paint", 20, 15));
            escalonador.AdicionarProcesso(new Processo(3, "Chrome", 50, 30));

            Console.WriteLine("\n--- Iniciando Simulação do Sistema Operacional ---\n");

            int ciclos = 0;
            while (true)
            {
                escalonador.ExecutarCicloDaCPU();
                ciclos++;

                if (ciclos % 5 == 0)
                {
                    Console.WriteLine("--- comando (add, stop, block, list, continue, exit) ---");
                    string option = Console.ReadLine();
                    option = option.ToLowerInvariant();
                    switch (option) 
                    {
                        case "add":
                            Console.WriteLine("--- Adicionar Processo ---");
                            Console.WriteLine("Digite o ID do processo:");
                            int.TryParse(Console.ReadLine(), out int id);
                            Console.WriteLine("Digite o nome do processo:");
                            string nome = Console.ReadLine();
                            Console.WriteLine("Digite o tamanho de memória necessário (em MB):");
                            int.TryParse(Console.ReadLine(), out int memoria);
                            Console.WriteLine("Digite o tempo total de execução:");
                            int.TryParse(Console.ReadLine(), out int tempoTotal);
                            escalonador.AdicionarProcesso(new Processo(id, nome, memoria, tempoTotal));
                            break;

                        case "stop":
                            Console.WriteLine("--- Parar Processo ---");
                            escalonador.ListarProcessos();
                            Console.WriteLine("Digite o ID do processo a ser parado:");
                            int.TryParse(Console.ReadLine(), out int idParar);
                            escalonador.PararProcesso(idParar);
                            break;

                        case "block":
                            Console.WriteLine("--- Bloquear Processo ---");
                            escalonador.ListarProcessos();
                            Console.WriteLine("Digite o ID do processo a ser bloqueado:");
                            int.TryParse(Console.ReadLine(), out int idBloquear);
                            Console.WriteLine("Digite o tempo de bloqueio em ciclos (espera de IO)");
                            int.TryParse(Console.ReadLine(), out int tempoBloqueio);
                            escalonador.BloquearProcesso(idBloquear, tempoBloqueio);
                            break;

                        case "list":
                            Console.WriteLine("--- Listar Processos ---");
                            escalonador.ListarProcessos();
                            break;

                        case "continue":
                            Console.WriteLine("--- Continuando simulaçao ---");
                            break;

                        case "exit":
                            Console.WriteLine("--- Encerrando Simulação ---");
                            return;

                        default:
                            Console.WriteLine("--- comando (add, stop, block, list, continue, exit) ---");
                            break;
                    }
                }
            }
        }
    }

}