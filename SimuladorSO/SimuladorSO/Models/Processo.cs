using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorSO.Models
{
    public class Processo
    {
        public int ID { get; private set; }
        public string Nome { get; private set; }
        public ProcessState Status { get; set; }
        public int TempoDeExecucao { get; set; }
        public int TamanhoMemoria { get; private set; }
        public int EnderecoInicialMemoria { get; set; }

        public Processo(int id, string nome, int tamanhoMemoria)
        {
            ID = id;
            Nome = nome;
            TamanhoMemoria = tamanhoMemoria;
            Status = ProcessState.Pronto;
            TempoDeExecucao = 0;
            EnderecoInicialMemoria = -1;
        }
    }
}