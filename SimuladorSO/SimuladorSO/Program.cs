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
            escalonador.AdicionarProcesso(new Processo(1, "Notepad", 10));
            escalonador.AdicionarProcesso(new Processo(2, "Paint", 20));
            escalonador.AdicionarProcesso(new Processo(3, "Chrome", 50));

            Console.WriteLine("\n--- Iniciando Simulação do Sistema Operacional ---\n");

            // Simula alguns ciclos e depois para um processo
            for (int i = 0; i < 5; i++)
            {
                escalonador.ExecutarCicloDaCPU();
                Thread.Sleep(1000);
            }

            Console.WriteLine("\n--- Tentando parar o processo 2 (Paint) ---\n");
            escalonador.PararProcesso(2);

            Console.WriteLine("\n--- Continunando a simulação ---\n");

            // Loop principal infinito
            while (true)
            {
                escalonador.ExecutarCicloDaCPU();
                Thread.Sleep(1000);
            }
        }
    }

}