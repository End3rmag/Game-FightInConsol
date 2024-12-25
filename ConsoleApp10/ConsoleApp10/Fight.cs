using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

public class Program
{
    static int currentPlayerIndex = 0;
    static List<Character> players = new List<Character>();
    static int poleSize = 10;
    static char[,] gameGrid = new char[poleSize, poleSize];

    static void Main(string[] args)
    {
        int numPlayers;
        do
        {
            Console.Write("Введите количество игроков (от 2 до 4): ");
        } while (!int.TryParse(Console.ReadLine(), out numPlayers) || numPlayers < 2 || numPlayers > 4);

        for (int i = 0; i < numPlayers; i++)
        {
            players.Add(ChooseCharacter());
        }

        string gameMode = ChooseGameMode();

        InitializeGrid();
        PlacePlayersOnGrid();

        if (gameMode == "Групповая битва")
        {
            TeamBattleLoop();
        }
        else
        {
            DeathmatchLoop();
        }
    }

    static string ChooseGameMode()
    {
        Console.WriteLine("Выберите режим игры:");
        Console.WriteLine("1. Групповая битва");
        Console.WriteLine("2. Последний выживший");
        string mode = Console.ReadLine();
        return mode == "1" ? "Групповая битва" : "Последний выживший";
    }

    static void InitializeGrid()
    {
        Random random = new Random();
        for (int i = 0; i < poleSize; i++)
        {
            for (int j = 0; j < poleSize; j++)
            {
                gameGrid[i, j] = (random.Next(10) < 2) ? '#' : '.';
            }
        }
    }

    static void PlacePlayersOnGrid()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].PositionX = FindValidPosition();
            players[i].PositionY = FindValidPosition();
            gameGrid[players[i].PositionX, players[i].PositionY] = (char)('1' + i);
        }
    }

    static int FindValidPosition()
    {
        Random random = new Random();
        int positionX, positionY;
        do
        {
            positionX = random.Next(poleSize);
            positionY = random.Next(poleSize);
        } while (gameGrid[positionX, positionY] == '#');

        return positionX;
    }

    static Character ChooseCharacter()
    {
        Console.WriteLine("Выберите персонажа:");
        Console.WriteLine("1. Воин");
        Console.WriteLine("2. Лучник");
        Console.WriteLine("3. Маг");
        Console.WriteLine("4. Ассасин");

        int choice = 0;
        while (choice < 1 || choice > 4)
        {
            try
            {
                choice = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                Console.WriteLine("Некорректный ввод. Попробуйте снова: (1-4)");
                continue;
            }
            if (choice < 1 || choice > 4)
            {
                Console.WriteLine("Некорректный ввод. Попробуйте снова: (1-4)");
            }
        }
        switch (choice)
        {
            case 1: return new Warrior();
            case 2: return new Archer();
            case 3: return new Mage();
            case 4: return new Assassin();
            default: return null;
        }
    }

    static void DisplayGrid()

    {
        for (int i = 0; i < poleSize; i++)
        {
            for (int j = 0; j < poleSize; j++)
            {
                Console.Write(gameGrid[i, j] + " ");
            }
            Console.WriteLine();
        }
    }

    static void DisplayHP()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Console.WriteLine($"Игрок {i + 1}: {players[i].HP} HP, CD: {players[i].SpecialAbilityCooldown}");
        }
    }

    static void DisplayTeamStatus(List<Character> team1, List<Character> team2)
    {
        Console.WriteLine("Состав команд:");
        Console.WriteLine("Команда 1:");
        foreach (var player in team1)
        {
            if (player.HP > 0)
            {
                Console.WriteLine($"- {player.ClassName} (HP: {player.HP}, CD: {player.SpecialAbilityCooldown})");
            }
            else
            {
                Console.WriteLine($"- {player.ClassName} (Мертв)");
            }
        }

        Console.WriteLine("\nКоманда 2:");
        foreach (var player in team2)
        {
            if (player.HP > 0)
            {
                Console.WriteLine($"- {player.ClassName} (HP: {player.HP}, CD: {player.SpecialAbilityCooldown})");
            }
            else
            {
                Console.WriteLine($"- {player.ClassName} (Мертв)");
            }
        }
    }

    static void MovePlayer(Character player, ConsoleKey key)
    {
        int newPlayerX = player.PositionX;
        int newPlayerY = player.PositionY;

        switch (key)
        {
            case ConsoleKey.W: newPlayerX--; break;
            case ConsoleKey.S: newPlayerX++; break;
            case ConsoleKey.A: newPlayerY--; break;
            case ConsoleKey.D: newPlayerY++; break;
        }

        if (newPlayerX >= 0 && newPlayerX < poleSize &&
            newPlayerY >= 0 && newPlayerY < poleSize &&
            gameGrid[newPlayerX, newPlayerY] != '#')
        {
            gameGrid[player.PositionX, player.PositionY] = '.';
            player.PositionX = newPlayerX;
            player.PositionY = newPlayerY;
            gameGrid[player.PositionX, player.PositionY] = (char)('1' + players.IndexOf(player));
        }
    }

    static int CalculateDistance(Character attacker, Character target)
    {
        return Math.Abs(attacker.PositionX - target.PositionX) + Math.Abs(attacker.PositionY - target.PositionY);
    }

    static void AttackPlayer(Character attacker)
    {
        foreach (var target in players)
        {
            if (target != attacker)
            {
                int distance = CalculateDistance(attacker, target);
                int maxDistance = (attacker is Warrior || attacker is Assassin) ? 1 : 2;

                if (distance <= maxDistance)
                {
                    int damage = attacker.GetDamage();
                    Console.WriteLine($"{attacker.ClassName} наносит {damage} урона {target.ClassName}!");
                    target.HP -= damage;

                    if (attacker is not Mage)
                    {
                        attacker.ResetDamage();
                    }

                    break;
                }
            }
        }
    }



    static void TeamBattleLoop()
    {
        int teamSize = players.Count / 2;
        List<Character> team1 = players.Take(teamSize).ToList();
        List<Character> team2 = players.Skip(teamSize).ToList();
        List<Character> activePlayers = new List<Character>(players);

        int currentPlayerIndex = 0;

        while (team1.Any(p => p.HP > 0) && team2.Any(p => p.HP > 0))
        {
            Console.Clear();
            DisplayGrid();
            DisplayTeamStatus(team1, team2);

            Character currentPlayer = activePlayers[currentPlayerIndex];

            if (currentPlayer.HP <= 0)
            {
                currentPlayerIndex++;
                                      
                if (currentPlayerIndex >= activePlayers.Count)
                {
                    currentPlayerIndex = 0;
                }
                continue;
            }

            Console.WriteLine($"Ход игрока {currentPlayerIndex + 1} ({currentPlayer.ClassName})");

            var input = Console.ReadKey(true).Key;

            if (input == ConsoleKey.E)
            {
                break;
            }

            if (input == ConsoleKey.U)
            {
                currentPlayer.UseSpecialAbility();
                Console.WriteLine("Теперь вы можете выполнить еще одно действие (W/A/S/D/F): ");
                input = Console.ReadKey(true).Key;
                if (input == ConsoleKey.F)
                {
                    AttackPlayer(currentPlayer);
                }
                else
                {
                    MovePlayer(currentPlayer, input);
                }
            }
            else if (input == ConsoleKey.F)
            {
                AttackPlayer(currentPlayer);
            }
            else
            {
                MovePlayer(currentPlayer, input);
            }


            currentPlayer.UpdateCooldown();
            if (!team1.Any(p => p.HP > 0) || !team2.Any(p => p.HP > 0))
            {
                break;
            }
            currentPlayerIndex++;

            if (currentPlayerIndex >= activePlayers.Count)
            {
                currentPlayerIndex = 0;
            }
        }

        Console.Clear();
        if (team1.Any(p => p.HP > 0))
        {
            Console.WriteLine("Команда 1 победила!");
        }
        else
        {
            Console.WriteLine("Команда 2 победила!");
        }
    }



    static void DeathmatchLoop()
    {
        while (players.Count > 1) 
        {
            Console.Clear();
            DisplayGrid();
            DisplayHP();
            Character currentPlayer = players[currentPlayerIndex];

            Console.WriteLine($"Ход игрока {currentPlayerIndex + 1} ({currentPlayer.ClassName}): (W/A/S/D) для перемещения или (F) для атаки, (E) для выхода, (U) для использования особого умения");

            var input = Console.ReadKey(true).Key;

            if (input == ConsoleKey.E)
            {
                break;
            }

            if (input == ConsoleKey.U)
            {
                currentPlayer.UseSpecialAbility();
                Console.WriteLine("Теперь вы можете выполнить еще одно действие (W/A/S/D/F): ");
                input = Console.ReadKey(true).Key;
                if (input == ConsoleKey.F)
                {
                    AttackPlayer(currentPlayer);
                }
                else
                {
                    MovePlayer(currentPlayer, input);
                }
            }
            else if (input == ConsoleKey.F)
            {
                AttackPlayer(currentPlayer);
            }
            else
            {
                MovePlayer(currentPlayer, input);
            }

            currentPlayer.UpdateCooldown(); 

            if (players.Count == 1) 
            {
                Console.WriteLine($"Игрок {Array.IndexOf(players.ToArray(), players[0]) + 1} ({players[0].ClassName}) победил!");
                return;
            }

            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        }
    }
}