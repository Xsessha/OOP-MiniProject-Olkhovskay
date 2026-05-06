namespace MyProject.Console.Menu;

using System;
using System.Collections.Generic;

public class MenuHandler
{
    private readonly Dictionary<int, Action> _actions = new();

    public void Register(int key, Action action)
    {
        _actions[key] = action;
    }

    private void ShowMenu()
    {
        Console.WriteLine("\nMENU");
        Console.WriteLine("1. Rent car");
        Console.WriteLine("0. Exit");
    }

    public void Run()
    {
        while (true)
        {
            ShowMenu();

            Console.Write("Choose option: ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int choice))
            {
                Console.WriteLine("Invalid input");
                continue;
            }

            if (choice == 0)
            {
                Console.WriteLine("Bye!");
                break;
            }

            if (_actions.TryGetValue(choice, out var action))
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Option not found");
            }
        }
    }
}