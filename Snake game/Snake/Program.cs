using System;
using System.Collections.Generic;
using System.IO;
using Snake;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            ShowMenu();
            int choice = GetMenuChoice();

            switch (choice)
            {
                case 1:
                    StartGame(); // Start the game logic
                    break;
                case 2:
                    DisplayHighScores(); // Show high scores
                    break;
                case 3:
                    Environment.Exit(0); // Exit the game
                    break;
            }
        }
    }

    // Display the menu to the user
    static void ShowMenu()
    {
        Console.Clear();
        Console.WriteLine("=====================================");
        Console.WriteLine("           SNAKE GAME MENU");
        Console.WriteLine("=====================================");
        Console.WriteLine("1. Start Game");
        Console.WriteLine("2. View High Scores");
        Console.WriteLine("3. Exit");
        Console.Write("Select an option (1-3): ");
    }

    // Get user input for menu selection
    static int GetMenuChoice()
    {
        int choice = 0;
        while (choice < 1 || choice > 3)
        {
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= 3)
                return choice;

            Console.Write("Invalid choice. Select an option (1-3): ");
        }
        return choice;
    }

    // Get difficulty level from user
    static int GetDifficultyLevel()
    {
        Console.Clear();
        Console.WriteLine("Select Difficulty Level:");
        Console.WriteLine("1. Easy (Slower)");
        Console.WriteLine("2. Medium (Normal)");
        Console.WriteLine("3. Hard (Faster)");
        int difficulty = 0;
        while (difficulty < 1 || difficulty > 3)
        {
            if (int.TryParse(Console.ReadLine(), out difficulty) && difficulty >= 1 && difficulty <= 3)
                return difficulty;

            Console.Write("Invalid choice. Select an option (1-3): ");
        }
        return difficulty;
    }

    // Start the snake game logic
    static void StartGame()
    {
        // Ask for the player's name
        Console.Clear();
        Console.Write("Enter your name: ");
        string playerName = Console.ReadLine();

        // Ask for the difficulty level
        int difficulty = GetDifficultyLevel();
        int frameDelayMilli = difficulty == 1 ? 150 : difficulty == 2 ? 100 : 50; // Adjust speed based on difficulty

        Random random = new Random();
        Coord gridDimensions = new Coord(50, 20);

        // Adjust starting position to the middle of the grid
        Coord snakePos = new Coord(gridDimensions.X / 2, gridDimensions.Y / 2);
        Direction movementDirection = Direction.Down;
        List<Coord> snakePosHistory = new List<Coord>();
        int tailLength = 0; // Start with no tail length to avoid self-collision

        // Ensure the apple is not placed on top of the snake at the start
        Coord applePos;
        do
        {
            applePos = new Coord(random.Next(1, gridDimensions.X - 1), random.Next(1, gridDimensions.Y - 1));
        } while (applePos.Equals(snakePos)); // Avoid apple spawning on snake

        int score = 0;

        while (true)
        {
            Console.Clear();

            // Display a stylized header
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=====================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("           SNAKE GAME");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=====================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Score: {score}");
            Console.ResetColor();

            // Render the game grid
            for (int y = 0; y < gridDimensions.Y; y++)
            {
                for (int x = 0; x < gridDimensions.X; x++)
                {
                    Coord currentCoord = new Coord(x, y);

                    // Draw snake, apple, or walls
                    if (snakePos.Equals(currentCoord) || snakePosHistory.Contains(currentCoord))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("■"); // Snake is green
                        Console.ResetColor();
                    }
                    else if (applePos.Equals(currentCoord))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("a"); // Apple is red
                        Console.ResetColor();
                    }
                    else if (x == 0 || y == 0 || x == gridDimensions.X - 1 || y == gridDimensions.Y - 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("#"); // Walls are blue
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(" "); // Empty space
                    }
                }
                Console.WriteLine();
            }

            // Check if snake has picked up apple
            if (snakePos.Equals(applePos))
            {
                tailLength++;
                score++;
                do
                {
                    applePos = new Coord(random.Next(1, gridDimensions.X - 1), random.Next(1, gridDimensions.Y - 1));
                } while (applePos.Equals(snakePos) || snakePosHistory.Contains(applePos));
            }
            // Check for game over conditions - snake has hit wall or snake has hit tail
            else if (snakePos.X == 0 || snakePos.Y == 0 || snakePos.X == gridDimensions.X - 1 ||
                snakePos.Y == gridDimensions.Y - 1 || snakePosHistory.Contains(snakePos))
            {
                // Display Game Over Message
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("GAME OVER! Press any key to return to the menu...");
                Console.ResetColor();
                Console.ReadKey();

                // Save the score after the game ends
                SaveHighScore(playerName, score);

                break; // Exit the game loop and return to the menu
            }

            // Add the snake's current position to the snakePosHistory list
            snakePosHistory.Add(new Coord(snakePos.X, snakePos.Y));

            if (snakePosHistory.Count > tailLength)
                snakePosHistory.RemoveAt(0);

            // Delay the rendering of next frame for frameDelayMilli milliseconds whilst checking for player input
            DateTime time = DateTime.Now;

            while ((DateTime.Now - time).Milliseconds < frameDelayMilli)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;

                    // Assign snake new direction to move in based on what key was pressed
                    switch (key)
                    {
                        case ConsoleKey.LeftArrow:
                            if (movementDirection != Direction.Right)
                                movementDirection = Direction.Left;
                            break;
                        case ConsoleKey.RightArrow:
                            if (movementDirection != Direction.Left)
                                movementDirection = Direction.Right;
                            break;
                        case ConsoleKey.UpArrow:
                            if (movementDirection != Direction.Down)
                                movementDirection = Direction.Up;
                            break;
                        case ConsoleKey.DownArrow:
                            if (movementDirection != Direction.Up)
                                movementDirection = Direction.Down;
                            break;
                    }
                }
            }

            // Move the snake in the current direction
            snakePos = snakePos.ApplyMovementDirection(movementDirection);
        }
    }

    // Save high score to a file with player's name
    static void SaveHighScore(string playerName, int score)
    {
        string path = "highscores.txt";

        // Read existing scores
        List<(string Name, int Score)> highScores = new List<(string, int)>();

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 2 && int.TryParse(parts[1], out int existingScore))
                    highScores.Add((parts[0], existingScore));
            }
        }

        // Add new score and sort the list
        highScores.Add((playerName, score));
        highScores.Sort((a, b) => b.Score.CompareTo(a.Score)); // Sort in descending order

        // Write the top 5 scores back to the file
        using (StreamWriter writer = new StreamWriter(path))
        {
            for (int i = 0; i < Math.Min(5, highScores.Count); i++)
            {
                writer.WriteLine($"{highScores[i].Name}|{highScores[i].Score}");
            }
        }
    }

    // Display high scores from the file
    static void DisplayHighScores()
    {
        string path = "highscores.txt";

        Console.Clear();
        Console.WriteLine("=====================================");
        Console.WriteLine("           HIGH SCORES");
        Console.WriteLine("=====================================");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                var parts = lines[i].Split('|');
                if (parts.Length == 2)
                {
                    Console.WriteLine($"{i + 1}. {parts[0]} - {parts[1]}");
                }
            }
        }
        else
        {
            Console.WriteLine("No high scores yet!");
        }

        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey();
    }
}
