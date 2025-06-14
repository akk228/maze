// See https://aka.ms/new-console-template for more information
using MazeGenerator;

var maze = new Maze();
var generatedMaze = maze.Generate(10, 10);

Maze.PrintMaze(generatedMaze);