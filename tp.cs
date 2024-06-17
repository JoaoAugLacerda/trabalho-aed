using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

public class Candidato
{
    public string nomeCandidato { get; set; }
    public double notaRed { get; set; }
    public double notaMat { get; set; }
    public double notaLing { get; set; }
    public int codCursoOp1 { get; set; }
    public int codCursoOp2 { get; set; }
    public double notaMedia { get; set; }
}

public class Fila
{
    private Candidato[] array;
    int primeiro, ultimo;

    public Fila(int tamanho)
    {
        array = new Candidato[tamanho + 1];
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

    public void Mostrar(StreamWriter write)
    {
        int i = primeiro;
        while (i != ultimo)
        {
            write.WriteLine($"{array[i].nomeCandidato} {array[i].notaMedia} {array[i].notaRed} {array[i].notaMat} {array[i].notaLing}");
            i = (i + 1) % array.Length;
        }
    }
}

public class Curso
{
    public string nomeCurso { get; set; }
    public int qtdVagas { get; set; }
    public List<Candidato> selecionados { get; set; }
    public Fila filaDeEspera { get; set; }

    public void Mostrar(StreamWriter write)
    {
        for (int i = 0; i < selecionados.Count; i++)
            write.WriteLine($"{selecionados[i].nomeCandidato} {selecionados[i].notaMedia} {selecionados[i].notaRed} {selecionados[i].notaMat} {selecionados[i].notaLing}");
    }
}

class Program
{
    public static void Quicksort(Candidato[] array, int esq, int dir)
    {
        int i = esq, j = dir;
        Candidato pivo = array[(esq + dir) / 2];

        while (i <= j)
        {
            while (CompararCandidatos(array[i], pivo) < 0) i++;
            while (CompararCandidatos(array[j], pivo) > 0) j--;
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

    public static int CompararCandidatos(Candidato a, Candidato b)
    {
        int compara = b.notaMedia.CompareTo(a.notaMedia);
        if (compara == 0)
        {
            compara = b.notaRed.CompareTo(a.notaRed);
            if (compara == 0)
            {
                compara = b.notaMat.CompareTo(a.notaMat);
                if (compara == 0)
                {
                    compara = b.notaLing.CompareTo(a.notaLing);
                }
            }
        }
        return compara;
    }


    public static Candidato[] Preparacao(out Dictionary<int, Curso> cursos)
    {
        string linha;
        Candidato[] candidatosTotal;
        cursos = new Dictionary<int, Curso>();
        try
        {
            StreamReader arq = new StreamReader("Arq1.txt", Encoding.UTF8);
            linha = arq.ReadLine();

            string[] infoGeral = linha.Split(';');
            linha = arq.ReadLine();

            candidatosTotal = new Candidato[int.Parse(infoGeral[1])];
            int count = 0;

            while (linha != null)
            {
                string[] infos = linha.Split(';');
                if (int.TryParse(infos[0], out _))
                {
                    Curso curso = new Curso { nomeCurso = infos[1], qtdVagas = int.Parse(infos[2]), selecionados = new List<Candidato>(int.Parse(infos[2])), filaDeEspera = new Fila(10)};
                    cursos.Add(int.Parse(infos[0]), curso);
                }
                else
                {
                    Candidato candidato = new Candidato { nomeCandidato =  infos[0], notaRed = double.Parse(infos[1]), notaMat = double.Parse(infos[2]), notaLing = double.Parse(infos[3]), codCursoOp1 = int.Parse(infos[4]), codCursoOp2 = int.Parse(infos[5]), notaMedia = Math.Round((double.Parse(infos[1]) + double.Parse(infos[2]) + double.Parse(infos[3])) / 3, 2) };
                    candidatosTotal[count] = candidato;
                    count++;
                }

                linha = arq.ReadLine();
            }

            Quicksort(candidatosTotal, 0, candidatosTotal.Length - 1);

            Console.WriteLine($" ");

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
        using (StreamWriter write = new StreamWriter("Saida.txt"))
        {
            foreach (var curso in cursos)
            {
                write.WriteLine($"{curso.Value.nomeCurso} {curso.Value.selecionados.Last().notaMedia}");
                write.WriteLine($"Selecionados");
                curso.Value.Mostrar(write); 
                write.WriteLine($"Fila de espera");
                curso.Value.filaDeEspera.Mostrar(write);
                write.WriteLine($"");
            }
        }
    }

    public static void DistruiCandidatos(Dictionary<int, Curso> cursos, Candidato[] candidatosTotal)
    {
        foreach (Candidato c in candidatosTotal)
        {
            Curso curso1 = cursos[c.codCursoOp1];
            Curso curso2 = cursos[c.codCursoOp2];
            if (curso1.selecionados.Count < curso1.qtdVagas)
            {
                curso1.selecionados.Add(c);
            }
            else if (curso2.selecionados.Count < curso2.qtdVagas)
            {
                curso2.selecionados.Add(c);
                curso1.filaDeEspera.Inserir(c);
            }
            else
            {
                curso1.filaDeEspera.Inserir(c);
                curso2.filaDeEspera.Inserir(c);
            }
        }
    }

    public static void Main(string[] args)
    {
        Dictionary<int, Curso> cursos;

        Candidato[] candidatosTotal = Preparacao(out cursos);

        DistruiCandidatos(cursos, candidatosTotal);

        ImprimirRelatorio(cursos);

        Console.WriteLine("Finalizado!");
    }
}
