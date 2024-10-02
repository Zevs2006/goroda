using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    // Список всех городов из файла
    static List<string> allCities = new List<string>();
    // Список названных городов
    static List<string> usedCities = new List<string>();
    // Список активных игроков
    static List<string> players = new List<string>();

    static void Main()
    {
        // Загружаем города из файла
        LoadCitiesFromFile("C:\\Users\\kuno_\\OneDrive\\Рабочий стол\\txt-cities-russia.txt");

        Console.WriteLine("Добро пожаловать в игру 'Города'!");
        Console.WriteLine("Правила: игроки по очереди называют города. Название следующего города должно начинаться с последней буквы предыдущего города.");
        Console.WriteLine("Чтобы завершить игру, введите 'сдаюсь'.");

        // Получаем количество игроков
        int playerCount = GetPlayerCount();

        // Инициализируем список игроков
        for (int i = 1; i <= playerCount; i++)
        {
            players.Add($"Игрок {i}");
        }

        string previousCity = "";
        int currentPlayerIndex = 0;

        while (players.Count > 1)
        {
            // Текущий игрок
            string currentPlayer = players[currentPlayerIndex];
            Console.WriteLine($"\nХод {currentPlayer}");

            Console.Write("Введите город: ");
            string currentCity = Console.ReadLine().Trim();

            if (currentCity.ToLower() == "сдаюсь")
            {
                Console.WriteLine($"{currentPlayer} сдался.");
                players.RemoveAt(currentPlayerIndex);

                // Если игрок сдался, уменьшаем индекс, чтобы не пропустить следующего игрока
                if (currentPlayerIndex >= players.Count)
                {
                    currentPlayerIndex = 0;
                }
            }
            else if (!IsCityInList(currentCity))
            {
                Console.WriteLine("Такого города не существует. Попробуйте снова.");
            }
            else if (usedCities.Contains(currentCity.ToLower()))
            {
                Console.WriteLine("Этот город уже был назван. Попробуйте другой.");
            }
            else if (!IsValidCity(previousCity, currentCity))
            {
                Console.WriteLine($"Город должен начинаться с буквы '{GetLastLetter(previousCity)}'. Попробуйте снова.");
            }
            else
            {
                usedCities.Add(currentCity.ToLower());
                previousCity = currentCity;

                // Переход хода к следующему игроку
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            }
        }

        // Победитель
        Console.WriteLine($"\nПобедил {players[0]}! Игра завершена.");
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
}
