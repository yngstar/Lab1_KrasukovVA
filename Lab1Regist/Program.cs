using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Serilog;

 public class Program
{
     public static bool RegisterUser(string login, string password, string confirmPassword, out string message)
    {

        Log.Information("Регистрация пользователя с логином: {Login}", login);

        if (!IsValidLogin(login, out message))
        {
            return false;
        }

        if (!IsValidPassword(password, out message))
        {
            return false;
        }

        if (password != confirmPassword)
        {
            message = "Пароль и подтверждение пароля не совпадают.";
            return false;
        }

        return true;
    }

    public static bool IsValidLogin(string input, out string message)
    {
        message = "";

        if (input.Length < 5)
        {
            message = "Логин должен содержать минимум 5 символов.";
            return false;
        }

        List<string> preinstalledLogins = new List<string>
        {
            "89231461381",
            "NekrasovNV",
            "admin"
        };

        if (preinstalledLogins.Contains(input))
        {
            message = "Такой логин уже зарегистрирован или находится в базе запрещенных.";
            return false;
        }

        if (!IsValidPhone(input))
        {
            message = "Телефон указан в недопустимом формате.";
            return false;
        }

        if (!IsValidEmail(input))
        {
            message = "Email указан в недопустимом формате.";
            return false;
        }

        if (!IsValidStringLogin(input))
        {
            message = "Логин должен быть в формате строки символов.";
            return false;
        }

        return true;
    }

    public static bool IsValidPhone(string input)
    {
        string pattern = @"^\+\d{1,3}-\d{3}-\d{3}-\d{4}$";
        return Regex.IsMatch(input, pattern);
    }

    static bool IsValidEmail(string input)
    {
        string pattern = @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$";
        return Regex.IsMatch(input, pattern);
    }

    static bool IsValidStringLogin(string input)
    {
        string pattern = @"^[a-zA-Z0-9_]+$";
        return Regex.IsMatch(input, pattern);
    }

    public static bool IsValidPassword(string input, out string message)
    {
        message = "";

        if (input.Length < 7)
        {
            message = "Пароль должен содержать минимум 7 символов.";
            return false;
        }

        string pattern = @"^(?=.*[а-я])(?=.*[А-Я])(?=.*\d)(?=.*[@#$%^&+=]).{7,}$";
        if (!Regex.IsMatch(input, pattern))
        {
            message = "Пароль не соответствует требованиям (минимум одна буква в верхнем и нижнем регистре, одна цифра и один спецсимвол).";
            return false;
        }

        return true;
    }

    static void Main()
    {
        // Конфигурация логгера
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console() // Логирование в консоль
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day) // Логирование в файл
            .CreateLogger();

        Log.Information("Программа успешно запущена");

        Console.WriteLine("Введите логин:");
        string login = Console.ReadLine();

        Console.WriteLine("Введите пароль:");
        string password = Console.ReadLine();

        Console.WriteLine("Подтвердите пароль:");
        string confirmPassword = Console.ReadLine();

        string message;
        bool result = RegisterUser(login, password, confirmPassword, out message);
        if (result)
        {
            Log.Information("Регистрация успешна для логина: {Login}", login);
        }
        else
        {
            Log.Error("Регистрация не удалась для логина: {Login}. Причина: {ErrorMessage}", login, message);
        }

        // Завершение работы логгера
        Log.CloseAndFlush();
    }
}
