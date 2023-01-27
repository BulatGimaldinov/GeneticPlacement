using System.IO;

namespace GeneticPlacement;

public class Algorithm
{
    public static void Play(PrintedCircuitBoard pcb, Elements elements)
    {
        TextWriter tw1 = new StreamWriter(@"C:\Users\bgima\OneDrive\Рабочий стол\Новая папка\GeneticPlacement (2)\GeneticPlacement\Output.txt");
        Population pop1 = new Population();
        pop1.MakeFirstPop(tw1);
        pop1.Skrech(MainWindow.MatrixConnection[0].Length / 2, tw1);
        pop1.Mutation(tw1);
        pop1.Selection(pcb, elements,tw1);
        for (int i = 0; i < MainWindow.NumberOfIterations; i++)
        {
            pop1.Skrech(MainWindow.MatrixConnection[0].Length / 2,tw1);
            pop1.Mutation(tw1);
            pop1.Selection(pcb, elements,tw1);
        }
        tw1.Close();
        int j = 1; // вспомогательная переменная
        foreach (int i in Population.BestPlacementOfElements)
        {
            elements.Coordinates[i] = pcb.Places[j];
            j++;
        }
        
    }
}