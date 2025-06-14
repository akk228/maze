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
        (0, 2),
        (2, 0),
        (0, -2),
        (-2, 0)
    };

    public char[,] Generate(int length, int width)
    {
        if (length < 3 || width < 3)
        {
            throw new ArgumentException("Maze dimensions must be at least 3x3.");
        }

        var maze = new char[length, width];

        Fill(maze);

        var entrance = SetEntrance(maze);
        var visited = new bool[length, width];

        BuildMaze(maze, entrance, visited);
        SetExit(maze);

        return maze;
    }


    /// <summary>
    /// Builds maze such that it recursively carves passages from the entrance.
    /// making sure that pasage always has thickness of 1 menaing that if a cell is a passage
    /// it has to be surrounded by walls on all sides except for the entrance and exit.
    /// </summary>
    /// <param name="maze"></param>
    /// <param name="current"></param>
    /// <param name="visited"></param>
    private void BuildMaze(char[,] maze, (int X, int Y) current, bool[,] visited)
    {
        if (visited[current.X, current.Y])
        {
            return;
        }

        visited[current.X, current.Y] = true;

        var directions = GetShuffledDirections();

        foreach (var (dx, dy) in directions)
        {
            (int x, int y) passage = (current.X + dx / 2, current.Y + dy / 2);
            (int X, int Y) next = (current.X + dx, current.Y + dy);

            if (IsValidPosition(next, passage, maze))
            {
                maze[passage.x, passage.y] = Passage;
                BuildMaze(maze, passage, visited);
            }
        }
    }

    private bool IsValidPosition((int x, int y) next, (int x, int y) pos, char[,] maze)
    {
        // out of bounds
        if (!(next.x >= 0 && next.x < maze.GetLength(0) &&
                next.y >= 0 && next.y < maze.GetLength(1)))
        {
            return false;
        }

        // pos isnt on the boundary
        if (!(pos.x > 0 && pos.x < maze.GetLength(0) - 1 &&
                pos.y > 0 && pos.y < maze.GetLength(1) - 1))
        {
            return false;
        }
        // next is not a wall
        if (maze[next.x, next.y] != Wall && maze[next.x, next.y] != Exit)
        {
            return false;
        }

        if (maze[next.x, next.y] == Exit)
        {
            return true;
        }

        var dx = Math.Abs(next.x - pos.x);
        var dy = Math.Abs(next.y - pos.y);

        if (dx == 1 &&
            !((maze[pos.x, pos.y - 1] == Wall || maze[pos.x, pos.y - 1] == Exit) &&
            (maze[pos.x, pos.y + 1] == Wall || maze[pos.x, pos.y + 1] == Exit)))
        {
            return false; // Horizontal passage
        }

        if (dy == 1 &&
            !((maze[pos.x - 1, pos.y] == Wall || maze[pos.x - 1, pos.y] == Exit) &&
            (maze[pos.x + 1, pos.y] == Wall || maze[pos.x + 1, pos.y] == Exit)))
        {
            return false; // Vertical passage
        }

        return true;
    }

    /// <summary>
    /// Shuffles the directions array to ensure that the maze is generated in a random manner.
    /// </summary>
    /// <returns>arr of shuffled directions</returns>
    /// <remarks>
    /// Please, pay attention that this method changes the state of maze every time,
    /// so <param name="_directions"/> shouldnt be ever expected to have particualr order.
    /// </remarks>
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

    private void SetExit(char[,] maze)
    {
        var potentialexits = new List<(int X, int Y)>();
        var maxX = maze.GetLength(0) - 1;
        var maxY = maze.GetLength(1) - 1;

        for (var y = 0; y <= maxY; y++)
        {
            if (maze[0, y] == Entrance || maze[maxX, y] == Entrance)
            {
                continue;
            }

            if (maze[1, y] != Wall)
            {
                potentialexits.Add((0, y)); // Top wall
            }
            if (maze[maxX - 1, y] != Wall)
            {
                potentialexits.Add((maxX, y)); // Bottom wall
            }
        }

        for (var x = 0; x <= maxX; x++)
        {
            if (maze[x, 0] == Entrance || maze[x, maxY] == Entrance)
            {
                continue;
            }
            if (maze[x, 1] != Wall)
            {
                potentialexits.Add((x, 0)); // Left wall
            }
            if (maze[x, maxY - 1] != Wall)
            {
                potentialexits.Add((x, maxY)); // Right wall
            }
        }
        // never gonna happen
        if (potentialexits.Count == 0)
        {
            throw new InvalidOperationException("No valid exit found.");
        }

        var exit = potentialexits[_random.Next(potentialexits.Count)];

        maze[exit.X, exit.Y] = Exit;
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