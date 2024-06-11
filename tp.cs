using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

class Candidato
{
  private string nomeCandidato { get; }
  private double notaRed { get; }
  private double notaMat { get; }
  private double notaLing { get; }
  private int codCursoOp1 { get; }
  private int codCursoOp2 { get; }
  private double notaMedia { get; }

  public string NomeCandidato
  {
    get { return nomeCandidato; }
  }
  public double NotaRed
  {
    get { return notaRed; }
  }
  public double NotaMat
  {
    get { return notaMat; }
  }
  public double NotaLing
  {
    get { return notaLing; }
  }
  public int CodCursoOp1
  {
    get { return codCursoOp1; }
  }
  public int CodCursoOp2
  {
    get { return codCursoOp2; }
  }
  public double NotaMedia
  {
    get { return notaMedia; }
  }
  
  public Candidato(string n, double r, double m, double l, int c1, int c2)
  {
    this.nomeCandidato = n;
    this.notaRed = r;
    this.notaMat = m;
    this.notaLing = l;
    this.codCursoOp1 = c1;
    this.codCursoOp2 = c2;
    this.notaMedia = Math.Round((r + m + l) / 3, 2);
  }
}

class Fila
{
  private Candidato[] array;
  int primeiro, ultimo;

  public Fila (int tamanho)
  {
    array = new Candidato[tamanho+1];
    primeiro = ultimo = 0;
  }

  public void Inserir(Candidato x)
  {
    if (((ultimo + 1) % array.Length) == primeiro) throw new Exception("Erro!");
    array[ultimo] = x;
    ultimo = (ultimo + 1) % array.Length;
  }

  public Candidato Remover()
  {
    if (primeiro == ultimo) throw new Exception("Erro!");
    Candidato resp = array[primeiro];
    primeiro = (primeiro + 1) % array.Length;
    return resp;
  }

  public void Mostrar()
  {
    int i = primeiro;
    while (i != ultimo)
    {
      Console.WriteLine($"{array[i].NomeCandidato} {array[i].NotaMedia} {array[i].NotaRed} {array[i].NotaMat} {array[i].NotaLing}");
      i = (i + 1) % array.Length;
    }
  }
}

class Curso
{
  private string nomeCurso;
  private int qtdVagas;
  private List<Candidato> selecionados { get; set; }
  private Fila filaDeEspera { get; set; }

  public string NomeCurso
  {
    get { return nomeCurso; }
  }

  public int QtdVagas
  {
    get { return qtdVagas; }
  }

  public List<Candidato> Selecionados
  {
    get { return selecionados; }
    set { selecionados = value; }
  }

  public Fila FilaDeEspera
  {
    get { return filaDeEspera; }
    set { filaDeEspera = value; }
  }

  public Curso(string nomeCurso, int qtdVagas)
  {
    this.nomeCurso = nomeCurso;
    this.qtdVagas = qtdVagas;
    selecionados = new List<Candidato>(qtdVagas);
    filaDeEspera = new Fila(10);
  }

  public void MostrarSelecionados(){
    for(int i = 0; i < selecionados.Count; i++)
    {
      Console.WriteLine($"{selecionados[i].NomeCandidato} {selecionados[i].NotaMedia} {selecionados[i].NotaRed} {selecionados[i].NotaMat} {selecionados[i].NotaLing}");
    }
  }
}

class Program
{
  public static void Quicksort(Candidato[] array, int esq, int dir)
  {
    int i = esq, j = dir;
    double pivo = array[(esq+dir)/2].NotaMedia;
    
    while (i <= j) {
      while (array[i].NotaMedia > pivo) i++;
      while (array[j].NotaMedia < pivo) j--;
      if (i <= j)
      {
        Candidato temp = array[i];
        array[i] = array[j];
        array[j] = temp;
        i++;
        j--;
      }
    }
    if (esq < j) Quicksort(array, esq, j);
    if (i < dir) Quicksort(array, i, dir);
  }

  public static Candidato[] Preparacao(out Dictionary<int, Curso> cursos)
  {
    string linha;
    Candidato [] candidatosTotal;
    cursos = new Dictionary<int, Curso>();
    try
    {
      StreamReader arq = new StreamReader("Arq1.txt", Encoding.UTF8);
      linha = arq.ReadLine();

      string [] infoGeral = linha.Split(';');
      linha = arq.ReadLine();

      candidatosTotal = new Candidato[int.Parse(infoGeral[1])];
      int count = 0;

      while (linha != null)
      {
        string [] infos = linha.Split(';');
        if(int.TryParse(infos[0], out _))
        {
          Curso curso = new Curso(infos[1], int.Parse(infos[2]));
          cursos.Add(int.Parse(infos[0]), curso);
        }
        else
        {
          Candidato candidato = new Candidato(infos[0], double.Parse(infos[1]), double.Parse(infos[2]), double.Parse(infos[3]), int.Parse(infos[4]), int.Parse(infos[5]));
          candidatosTotal[count] = candidato;
          count++;
        }

        linha = arq.ReadLine();
      }

      for (int i = 0; i < candidatosTotal.Length; i++) Console.WriteLine($"{candidatosTotal[i].NomeCandidato}: {candidatosTotal[i].NotaMedia} ");

      Quicksort(candidatosTotal, 0, candidatosTotal.Length-1);

      Console.WriteLine($" ");

      for (int i = 0; i < candidatosTotal.Length; i++) Console.WriteLine($"{candidatosTotal[i].NomeCandidato}: {candidatosTotal[i].NotaMedia} ");

      arq.Close();
      return candidatosTotal;
    }
    catch (Exception e)
    {
      Console.WriteLine("Exception: " + e.Message);
      return null;
    }
    
  }

  public static void ImprimirRelatorio(Dictionary<int, Curso> cursos)
  {
    foreach (KeyValuePair<int, Curso> curso in cursos)
    {
      Console.WriteLine($"{curso.Value.NomeCurso} {curso.Value.Selecionados.Last().NotaMedia}");
      Console.WriteLine($"Selecionados");
      curso.Value.MostrarSelecionados();
      Console.WriteLine($"Fila de espera");
      curso.Value.FilaDeEspera.Mostrar();
      Console.WriteLine($"");
    }
  }

  public static void DistruiCandidatos(Dictionary<int, Curso> cursos, Candidato[] candidatosTotal)
  {
    foreach(Candidato c in candidatosTotal){
      Curso curso1 = cursos[c.CodCursoOp1];
      Curso curso2 = cursos[c.CodCursoOp2];
      if (curso1.Selecionados.Count < curso1.QtdVagas)
      {
        curso1.Selecionados.Add(c);
      }
      else if (curso2.Selecionados.Count < curso2.QtdVagas)
      {
        curso2.Selecionados.Add(c);
        curso1.FilaDeEspera.Inserir(c);
      }
      else
      {
        curso1.FilaDeEspera.Inserir(c);
        curso2.FilaDeEspera.Inserir(c);
      }
    }
  }
  
  public static void Main (string[] args)
  {
    Dictionary<int, Curso> cursos;

    Candidato [] candidatosTotal = Preparacao(out cursos);
    
    DistruiCandidatos(cursos, candidatosTotal);

    ImprimirRelatorio(cursos);
  }
}