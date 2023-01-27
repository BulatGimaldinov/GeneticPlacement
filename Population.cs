using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GeneticPlacement;

public class Population
{
    private List<List<int>> ListOfIndividuals = new (); // список нескольких особей
    private List<List<int>> Deti = new (); // список детей
    private List<List<int>> PopDoSel = new (); // список популяции до селекции
    private List<List<int>> PopPosSel = new (); // список популяции после селекции
    public static double MinimumTotalLengthOfConnections; // минимальная суммарная взвешенная длина
    public static List<int> BestPlacementOfElements = new (); // лучшее размещение элементов

    public void MakeFirstPop(TextWriter tw1) // создаёт первую популяцию
    {
        tw1.WriteLine("Популяция");
        for (int i = 0; i < MainWindow.NumberOfPopulations; i++)
        {
            Individual indiv = new Individual(MainWindow.MatrixConnection[0].Length);
            indiv.Make_List_Indexes();
            ListOfIndividuals.Add(indiv.Indiv1);
        }
        foreach (var j in ListOfIndividuals)
        {
            foreach (var k in j)
            {
                tw1.Write(k + " ");
            }
            tw1.WriteLine();
        }
    }

    public void Skrech(int splitPoint, TextWriter tw1) // создаёт новые особи путём скрещивания старых
    {   
        List<List<int>> ma = new List<List<int>>();// список матерей
        List<List<int>> pa = new List<List<int>>();// список отцов

        if (MainWindow.NumberOfPopulations % 2 == 0) // если число особей чётное
        {
            for (int j = 0; j < MainWindow.NumberOfPopulations; j++)
            {
                if (j % 2 == 0)
                {
                    ma.Add(ListOfIndividuals[j]);
                }
                if (j % 2 != 0)
                {
                    pa.Add(ListOfIndividuals[j]);
                }
            }
        }
        if (MainWindow.NumberOfPopulations % 2 != 0) // если число особей нечётное
        {
            for (int j = 0; j < MainWindow.NumberOfPopulations - 1; j++)
            {
                if (j % 2 == 0)
                {
                    ma.Add(ListOfIndividuals[j]);
                }
                if (j % 2 != 0)
                {
                    pa.Add(ListOfIndividuals[j]);
                }
            }
        }
        
        tw1.WriteLine("мама");
        foreach (var i in ma)
        {
            foreach (var j in i)
            {
                tw1.Write(j+" ");
            }
            tw1.WriteLine();
        }
        tw1.WriteLine("папа");
        foreach (var i in pa)
        {
            foreach (var j in i)
            {
                tw1.Write(j+" ");
            }
            tw1.WriteLine();
        }
        for (int i = 0; i < ma.Count; i++) // сначала материнские гены
        {
            List<int> reb = new List<int>(); // список ребёнка
            for (int j = 0; j < splitPoint; j++)
            {
                reb.Add(ma[i][j]);
            }
            for (int j = 0; j < ma[i].Count; j++) // потом отцовские
            {
                if (!reb.Contains(pa[i][j]))
                {
                    reb.Add(pa[i][j]);
                }
            }
            Deti.Add(reb);
        }
        for (int i = 0; i < ma.Count; i++) // сначала отцовские гены
        {
            List<int> reb = new List<int>(); // список ребёнка
            for (int j = 0; j < splitPoint; j++)
            {
                reb.Add(pa[i][j]);
            }
            for (int j = 0; j < ma[i].Count; j++) // потом МАТЕРИНСКИЕ
            {
                if (!reb.Contains(ma[i][j]))
                {
                    reb.Add(ma[i][j]);
                }
            }
            Deti.Add(reb);
        }
        tw1.WriteLine("Дети");
        foreach (var i in Deti)
        {
            foreach (var j in i)
            {
                tw1.Write(j+" ");
            }
            tw1.WriteLine();
        }
    }

    public void Mutation(TextWriter tw1) // мутация списка детей
    {
        Random random1 = new Random();
        foreach (var i in Deti)
        {
            int perem = random1.Next(0, i.Count - 2); // perem - случайное целое число индекса списка Детей
            int perem2 = random1.Next(2, i.Count - perem + 1); // perem2 - случайное целое число длины интервала разворота списка Детей
            i.Reverse(perem, perem2);
        }
        tw1.WriteLine("Дети после мутации");
        foreach (var i in Deti)
        {
            foreach (var j in i)
            {
                tw1.Write(j+" ");
            }
            tw1.WriteLine();
        }
    }

    public void Selection(PrintedCircuitBoard pcb, Elements elements,TextWriter tw1) // реализует селекцию
    {
            foreach (var i in ListOfIndividuals)
        {
            PopDoSel.Add(i);
        }

        foreach (var i in Deti)
        {
            PopDoSel.Add(i);
        }

        List<double> listLengthPath = new List<double>(); // список длин путей
        
        for (int i = 0; i < PopDoSel.Count; i++) // нахождение сумарной длины соединений
        {
            double lengthPath = 0;
            for (int k = 0; k < PopDoSel[i].Count; k++)
            {
                for (int j = 0; j < (PopDoSel[i].Count - 1 ); j++)
                {
                    if (MainWindow.MatrixConnection[PopDoSel[i][k] - 1][j] != 0)
                    {
                        lengthPath += MainWindow.MatrixConnection[PopDoSel[i][k] - 1][j] * Math.Sqrt(
                            Math.Pow(pcb.Places[k+1].x - pcb.Places[PopDoSel[i].IndexOf(j+1)+1].x, 2) +
                            Math.Pow(pcb.Places[k+1].y - pcb.Places[PopDoSel[i].IndexOf(j+1)+1].y, 2));
                        MainWindow.MatrixConnection[j][PopDoSel[i][k] - 1] = 0; // обнуление нижней диагонали, чтобы числа не суммировались повторно 
                    }
                }
            }
            listLengthPath.Add(lengthPath);
        }
        tw1.WriteLine("Популяция до селекции");
        for (var i=0;i< PopDoSel.Count; i++)
        {
            for (var j=0;j<PopDoSel[i].Count; j++)
            {
                tw1.Write(PopDoSel[i][j]+" ");
            }
            tw1.WriteLine(listLengthPath[i]);
        }
        List<double> listLengthPath2 = new List<double>(); // сортированный список длин путей
        int a = pcb.Width; // ширина платы
        int b = pcb.Length; // длина платы
        
        for (int i = 0; i < PopDoSel.Count; i++) // проверка на ограничения
        {
            if (listLengthPath[i] != 0)
            {
                for (int j = 0; j < PopDoSel[i].Count; j++)
                {
                    int ai = elements.Dimensions[PopDoSel[i][j]].width; // ширина элемента
                    int bi = elements.Dimensions[PopDoSel[i][j]].length; // длина элемента
                    int xi = pcb.Places[j + 1].x; // xi - координаты центров элементов (позиций)
                    int yi = pcb.Places[j + 1].y; // yi - координаты центров элементов (позиций)
                    if ((ai <= xi && xi <= a - ai) && (bi <= yi && yi <= b - bi)) // выход за границы
                    {
                        for (int k = 1; k < PopDoSel[i].Count - 1; k++)
                        {
                            int aj = elements.Dimensions[PopDoSel[i][k]].width; // длина следующего элемента
                            int bj = elements.Dimensions[PopDoSel[i][k]].length; // ширина следующего элемента
                            int xj = pcb.Places[k + 1].x; // xj - координаты центров следующи элементов (позиций)
                            int yj = pcb.Places[k + 1].y; // yj - координаты центров следующи элементов (позиций)
                            if ((xi + ai / 2 >= xj - aj / 2) && (xj + aj / 2 >= xi - ai / 2) &&
                                (yi + bi / 2 >= yj - bj / 2) &&
                                (yj + bj / 2 >= yi - yi / 2)) // взаимное непересечение элементов
                            {
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                listLengthPath2.Add(listLengthPath[i]);
            }
        }

        try
        {
            if (listLengthPath2.Count < MainWindow.NumberOfPopulations && listLengthPath2.Count != 0) // если подходящих длин меньше чем нужно, то дублируем уже имеющийся первый пока не достигнем нужного количества
            {
                for (int i = 0; i <= MainWindow.NumberOfPopulations - listLengthPath2.Count; i++)
                {
                    listLengthPath2.Add(listLengthPath2[0]);
                }
            }

            listLengthPath2.Sort();
            for (int i = 0; i < MainWindow.NumberOfPopulations; i++) // добавляем элементы прошедшие селекцию в список
            {
                for (int j = 0; j < PopDoSel.Count; j++)
                {
                    if (listLengthPath2[i] == listLengthPath[j])
                    {
                        PopPosSel.Add(PopDoSel[j]);
                        break;
                    }
                }
            }
            ListOfIndividuals.Clear();
            foreach (var i in PopPosSel) // переносим полученную популяцию в следующее поколение
            {
                ListOfIndividuals.Add(i);
            }
            BestPlacementOfElements = PopPosSel[0];
            tw1.WriteLine("Популяция после селекции");
            foreach (var i in PopPosSel)
            {
                foreach (var j in i)
                {
                    tw1.Write(j+" ");
                }
                tw1.WriteLine();
                
            }
            Deti.Clear();
            PopDoSel.Clear();
            PopPosSel.Clear();
            MinimumTotalLengthOfConnections = listLengthPath2[0];
            listLengthPath2.Clear();

        }
        catch (Exception e)
        {
                 MessageBox.Show("Ошибка"+e);
        }
    }
}