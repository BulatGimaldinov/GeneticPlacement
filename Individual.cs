using System;
using System.Collections.Generic;

namespace GeneticPlacement;

public class Individual
{
    int n; // количество элементов в особи
    public List<int> Indiv1; //список одной особи

    public Individual(int _n)
    {
        n = _n;
    }
    public void Make_List_Indexes() // создаёт список элементов для одной особи
    {
        Indiv1 = new List<int>();
        for (int i = 0; i < n; i++)
        {
            Indiv1.Add(i+1);
        }
        Random_List_Indexes(Indiv1);
    }
    public void Random_List_Indexes(List<int> indiv1) // меняет элементы местами случайным образом
    {
        Random random = new Random();
        for (int i = indiv1.Count - 1; i >= 0; i--) // формируем особь в случайном порядке
        {
            int j = random.Next(i + 1);  // создание случайного индекса < i+1
            (indiv1[j], indiv1[i]) = (indiv1[i], indiv1[j]);
        }
    }
}