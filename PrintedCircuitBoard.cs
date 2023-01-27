using System.Collections.Generic;

namespace GeneticPlacement;

public class PrintedCircuitBoard
{
    public int Width;
    public int Length;
    public Dictionary<int, (int x, int y)> Places;

    public PrintedCircuitBoard(int width, int length, Dictionary<int, (int x, int y)> places) 
    {
        Width = width;
        Length = length;
        Places = places;
    }
}