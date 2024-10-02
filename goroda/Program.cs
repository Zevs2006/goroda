using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static List<string> allCities = new List<string>();  // Все города из файла
    static List<string> usedCities = new List<string>(); // Уже использованные города
    static List<string> players = new List<string>();    // Игроки
    static bool playAgainstComputer = false;             // Флаг игры против компьютера

    static void Main()
    {
        LoadCitiesFromFile("C:\\Users\\Student406-11\\Downloads\\txt-cities-russia.txt");  // Загрузка городов из файла

        Console.WriteLine("Добро пожаловать в игру 'Города'!");

        // Выбор игры: против компьютера или других людей
        playAgainstComputer = ChooseGameMode();

        int playerCount = playAgainstComputer ? 1 : GetPlayerCount();  // Количество игроков

        for (int i = 1; i <= playerCount; i++)
        {
            players.Add($"Игрок {i}");
        }

        if (playAgainstComputer)
        {
            players.Add("Компьютер");
        }

        string previousCity = "";  // Последний названный город
        int currentPlayerIndex = 0;

        // Игра продолжается, пока не останется 1 игрок
        while (players.Count > 1)
        {
            string currentPlayer = players[currentPlayerIndex];
            Console.WriteLine($"\nХод {currentPlayer}");

            string currentCity;

            // Логика хода компьютера
            if (currentPlayer == "Компьютер")
            {
                currentCity = ComputerTurn(previousCity);
                if (string.IsNullOrEmpty(currentCity))
                {
                    Console.WriteLine("Компьютер не может назвать город и проигрывает!");
                    players.Remove(currentPlayer);
                    break;
                }
                else
                {
                    Console.WriteLine($"Компьютер назвал город: {currentCity}");
                }
            }
            else
            {
                // Ход игрока
                Console.Write("Введите город: ");
                currentCity = Console.ReadLine().Trim();

                // Если игрок сдаётся
                if (currentCity.ToLower() == "сдаюсь")
                {
                    Console.WriteLine($"{currentPlayer} сдался.");
                    players.RemoveAt(currentPlayerIndex);
                    if (currentPlayerIndex >= players.Count)
                    {
                        currentPlayerIndex = 0;
                    }
                    continue;
                }

                // Приводим город к нижнему регистру для проверки
                string currentCityLower = currentCity.ToLower();

                // Проверка города игрока
                if (!IsCityInList(currentCityLower))
                {
                    Console.WriteLine("Такого города не существует. Попробуйте снова.");
                    continue;
                }
                else if (usedCities.Contains(currentCityLower))
                {
                    Console.WriteLine("Этот город уже был назван. Попробуйте другой.");
                    continue;
                }
                else if (!IsValidCity(previousCity, currentCityLower))
                {
                    Console.WriteLine($"Город должен начинаться с буквы '{GetLastLetter(previousCity)}'. Попробуйте снова.");
                    continue;
                }
            }

            // Добавляем город в список использованных
            usedCities.Add(currentCity.ToLower());
            previousCity = currentCity;

            // Проверка, остались ли города для следующего игрока
            if (!CanPlayerContinue(previousCity))
            {
                Console.WriteLine($"{currentPlayer} не может назвать город и проигрывает!");
                players.Remove(currentPlayer);
                break;
            }

            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        }

        if (players.Count == 1)
        {
            Console.WriteLine($"\nПобедил {players[0]}! Игра завершена.");
        }
    }

    // Метод для выбора режима игры
    static bool ChooseGameMode()
    {
        while (true)
        {
            Console.Write("Хотите играть против компьютера? (да/нет): ");
            string choice = Console.ReadLine().ToLower();

            if (choice == "да") return true;
            if (choice == "нет") return false;

            Console.WriteLine("Некорректный выбор. Пожалуйста, введите 'да' или 'нет'.");
        }
    }

    // Метод для получения количества игроков
    static int GetPlayerCount()
    {
        int count;
        while (true)
        {
            Console.Write("Введите количество игроков (2 или больше): ");
            if (int.TryParse(Console.ReadLine(), out count) && count >= 2)
            {
                return count;
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Попробуйте снова.");
            }
        }
    }

    // Логика хода компьютера
    static string ComputerTurn(string previousCity)
    {
        Random rand = new Random();
        char lastLetter = GetLastLetter(previousCity);

        // Найти все города, начинающиеся с последней буквы предыдущего города
        List<string> availableCities = allCities
            .Where(city => !usedCities.Contains(city.ToLower()) && city.ToLower()[0] == lastLetter)
            .ToList();

        if (availableCities.Count == 0) return null;

        // Компьютер выбирает случайный город
        string chosenCity = availableCities[rand.Next(availableCities.Count)];
        usedCities.Add(chosenCity.ToLower());  // Добавляем город в список использованных
        return chosenCity;
    }

    // Метод загрузки городов из файла
    static void LoadCitiesFromFile(string fileName)
    {
        try
        {
            allCities = new List<string>(File.ReadAllLines(fileName));
            Console.WriteLine("Города успешно загружены из файла.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
        }
    }

    // Проверка, существует ли город в списке
    static bool IsCityInList(string city)
    {
        return allCities.Contains(city, StringComparer.OrdinalIgnoreCase);
    }

    // Проверка, начинается ли город с нужной буквы
    static bool IsValidCity(string previousCity, string currentCity)
    {
        if (string.IsNullOrEmpty(previousCity)) return true;
        return currentCity[0] == GetLastLetter(previousCity);
    }

    // Получение последней буквы названия города
    static char GetLastLetter(string city)
    {
        city = city.ToLower();
        char lastChar = city[city.Length - 1];

        // Если последняя буква - "ь", "ы" или "ъ", берем предпоследнюю
        if (lastChar == 'ь' || lastChar == 'ы' || lastChar == 'ъ')
        {
            lastChar = city[city.Length - 2];
        }

        return lastChar;
    }

    // Метод для проверки, может ли игрок продолжить игру
    static bool CanPlayerContinue(string previousCity)
    {
        char lastLetter = GetLastLetter(previousCity);
        return allCities.Any(city => !usedCities.Contains(city.ToLower()) && city.ToLower()[0] == lastLetter);
    }
}
