namespace MazeGenerator;

public class Maze
{
    private const char Entrance = 'I';
    private const char Exit = 'O';
    private const char Wall = 'X';
    private const char Passage = '.';

    private Random _random = new Random();
    private (int dx, int dy)[] _directions =
    {
        (0, 2),   // Right
        (2, 0),   // Down
        (0, -2),  // Left
        (-2, 0)   // Up
    };

    public char[,] Generate(int length, int width)
    {
        var maze = new char[length, width];

        Fill(maze);

        var entrance = SetEntrance(maze);
        var exit = SetExit(maze, entrance);
        var visited = new bool[length, width];

        BuildMaze(maze, entrance, visited);

        return maze;
    }
    private void BuildMaze(char[,] maze, (int X, int Y) current, bool[,] visited)
    {
        if (visited[current.X, current.Y])
        {
            return;
        }

        visited[current.X, current.Y] = true;

        var length = maze.GetLength(0);
        var width = maze.GetLength(1);

        var directions = GetShuffledDirections();

        foreach (var (dx, dy) in directions)
        {
            var x = current.X + dx / 2;
            var y = current.Y + dy / 2;
            var nextX = current.X + dx;
            var nextY = current.Y + dy;

            if (nextX >= 0 && nextX < length &&
                nextY >= 0 && nextY < width &&
                (maze[nextX, nextY] == Wall || maze[nextX, nextY] == Exit) &&
                (!(x == 0 || x == length - 1 || y == 0 || y == width - 1) || maze[x, y] == Exit))
            {
                if (maze[x, y] != Exit)
                {
                    maze[x, y] = Passage;
                }

                // PrintMaze(maze);
                // Console.WriteLine();
                BuildMaze(maze, (x, y), visited);
            }
        }
    }

    private (int dx, int dy)[] GetShuffledDirections()
    {
        for (int i = _directions.Length - 1; i > 0; i--)
        {
            var j = _random.Next(0, i + 1);
            // Swap directions
            (_directions[i], _directions[j]) = (_directions[j], _directions[i]);
        }

        return _directions;
    }

    private void Fill(char[,] maze)
    {
        var length = maze.GetLength(0);
        var width = maze.GetLength(1);

        for (var x = 0; x < length; x++)
        {
            for (var y = 0; y < width; y++)
            {
                maze[x, y] = Wall;
            }
        }
    }

    private (int X, int Y) SetEntrance(char[,] maze)
    {
        var length = maze.GetLength(0);
        var width = maze.GetLength(1);

        // Set entrance
        var entrance = SetEndPoint(length, width);
        maze[entrance.X, entrance.Y] = Entrance;

        return entrance;
    }

    private (int X, int Y) SetExit(char[,] maze, (int X, int Y) entrance)
    {
        // Ensure exit is not at the same position as entrance
        var exit = SetEndPoint(maze.GetLength(0), maze.GetLength(1));
        while (exit.X == entrance.X && exit.Y == entrance.Y)
        {
            exit = SetEndPoint(maze.GetLength(0), maze.GetLength(1));
        }
        maze[exit.X, exit.Y] = Exit;

        return exit;
    }

    private (int X, int Y) SetEndPoint(int length, int width)
    {

        var wallNumber = _random.Next(0, 3);

        if (wallNumber % 2 == 0)
        {
            // Top or bottom wall
            var x = wallNumber == 0 ? 0 : length - 1;
            var y = _random.Next(1, width - 1);
            return (x, y);
        }
        else
        {
            // Left or right wall
            var y = wallNumber == 1 ? 0 : width - 1;
            var x = _random.Next(1, length - 1);
            return (x, y);
        }
    }
    
    public static void PrintMaze(char[,] maze)
    {
        for (var x = 0; x < maze.GetLength(0); x++)
        {
            for (var y = 0; y < maze.GetLength(1); y++)
            {
                Console.Write(maze[x, y]);
            }
            Console.WriteLine();
        }
    }
}