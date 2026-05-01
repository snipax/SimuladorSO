# SimuladorSO

Simulador de Sistema Operacional em C# (.NET Framework 4.7.2) com foco didático em escalonamento, gerenciamento de memória e métricas. O projeto implementa uma simulação em console com filas de processos, quantum e prioridades, além de paginação simples na memória.

## Objetivo
Consolidar conteúdos clássicos de Sistemas Operacionais por meio de uma simulação incremental:
- Escalonamento de CPU (Round-Robin com prioridades)
- Bloqueio de processos (IO/espera)
- Gerenciamento de memória com paginação
- Métricas de desempenho (turnaround, espera, throughput)

## Tecnologias
- C# 7.3
- .NET Framework 4.7.2
- Console App

## Estrutura do Projeto
```
SimuladorSO/
  Core/
    Escalonador.cs
    GerenciadorDeMemoria.cs
  Models/
    Processo.cs
    ProcessState.cs
    NivelPrioridade.cs
  Program.cs
```

## Como Executar
1. Abra o projeto no Visual Studio.
2. Compile e execute o projeto.
3. O console exibirá os ciclos de CPU e permitirá comandos periódicos.

## Comandos da Simulação
A cada 5 ciclos, o simulador solicita um comando:
- `add` — adiciona novo processo
- `stop` — remove (finaliza) um processo
- `block` — bloqueia um processo por N ciclos (IO)
- `list` — lista processos por fila
- `metrics` — exibe métricas e processos finalizados
- `pages` — mostra tabela de páginas de um processo
- `continue` — continua a simulação
- `exit` — encerra

## Funcionamento Atual (Resumo)
### Escalonamento
- Filas por prioridade: Alta, Média, Baixa
- Seleção sempre pela maior prioridade disponível
- Quantum fixo (Round-Robin dentro da mesma fila)

### Estados de Processo
- `Pronto`
- `Executando`
- `Bloqueado`
- `Finalizado`

### Gerenciamento de Memória
- Paginação simples
- Cada processo mantém uma tabela de páginas (`TabelaPaginas`)
- Alocação por frames livres

### Métricas
- `TempoChegada`, `TempoInicioExecucao`, `TempoConclusao`
- `TempoTurnaround`, `TempoEspera`
- `Throughput` (processos finalizados por ciclo)

## Exemplo de Uso
```
add
ID: 4
Nome: Editor
Memoria: 12
Tempo total: 8
Prioridade: Alta

block
ID: 2
Tempo de bloqueio: 3

metrics
pages
ID: 1
```