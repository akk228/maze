// See https://aka.ms/new-console-template for more information
using MazeGenerator;

var length = GetDimension("Length");
var width = GetDimension("Width");


var maze = new Maze();
var generatedMaze = maze.Generate(length, width);

Maze.PrintMaze(generatedMaze);

int GetDimension(string message)
{
    do {
        Console.Write(message + " : ");
        var input = Console.ReadLine();
        if (int.TryParse(input, out var dimension) && dimension > 0)
        {
            if (dimension < 3)
            {
                Console.WriteLine("Dimension should be at least 3. Please try again.");
                continue;
            }
            return dimension;
        }
        Console.WriteLine("Invalid input. Please enter a positive integer. Or press Ctrl+C to exit.");   
    } while (true);
}