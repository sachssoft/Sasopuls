using Sachssoft.Sasopuls.Test;
using System;
using System.ComponentModel;

class Program
{
    static void Main(string[] args)
    {
        var cat = new AnimalModel
        {
            Name = "Simba",
            Type = "Cat"
        };

        var catVM = new AnimalViewModel(cat);

        catVM.PropertyChanged += VM_PropertyChanged;

        Console.WriteLine($"Current name: {catVM.Name}");
        Console.Write("Enter new name: ");

        string? input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
        {
            catVM.Name = input;
        }

        Console.ReadKey();
    }

    private static void VM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not AnimalViewModel vm)
            return;

        Console.WriteLine("ViewModel PropertyChanged:");
        Console.WriteLine($"Property: {e.PropertyName}");
        Console.WriteLine($"Animal: {vm.Type} - Name: {vm.Name}");
    }
}