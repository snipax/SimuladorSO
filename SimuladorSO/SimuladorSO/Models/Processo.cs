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
        public int TempoTotal { get; private set; }
        public int QuantumRestante { get; set; }
        public int TempoBloqueioRestante { get; set; }
        public int TamanhoMemoria { get; private set; }
        public List<int> TabelaPaginas { get; set; }
        public NivelPrioridade Prioridade { get; set; }
        public int TempoChegada { get; set; }
        public int TempoInicioExecucao{ get; set; }
        public int TempoConclusao { get; set; }
        public int TempoTurnaround { get; set; }
        public int TempoEspera { get; set; }



        public Processo(int id, string nome, int tamanhoMemoria, int tempototal, NivelPrioridade prioridade)
        {
            ID = id;
            Nome = nome;
            TamanhoMemoria = tamanhoMemoria;
            Status = ProcessState.Pronto;
            Prioridade = prioridade;
            TempoTotal = tempototal;
            TempoDeExecucao = 0; 
            TabelaPaginas = new List<int>();
        }
    }
}