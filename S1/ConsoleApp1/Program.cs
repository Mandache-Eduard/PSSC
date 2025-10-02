using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCartApp
{
    public interface IQuantity
    {
        decimal GetValue();
    }

    public record CantitateUnitati(int Unitati) : IQuantity
    {
        public decimal GetValue() => Unitati;
    }

    public record CantitateKilograme(decimal Kilograme) : IQuantity
    {
        public decimal GetValue() => Kilograme;
    }
    
    public record Produs(string Name, decimal PretUnitar);
    
    public record CartItem(Produs Produs, IQuantity Cantitate)
    {
        public decimal GetTotalPrice() =>
            Cantitate switch
            {
                CantitateUnitati uq => uq.Unitati * Produs.PretUnitar,
                CantitateKilograme kq => kq.Kilograme * Produs.PretUnitar,
                _ => throw new NotImplementedException()
            };
    }

    // ==== COS DE CUMPARATURI ====
    public class ShoppingCart
    {
        private readonly List<CartItem> _items = new();

        public void AddProduct(Produs produs, IQuantity quantity)
        {
            _items.Add(new CartItem(produs, quantity));
        }

        public void RemoveProduct(string productName)
        {
            var item = _items.FirstOrDefault(i => 
                i.Produs.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                _items.Remove(item);
                Console.WriteLine($"{productName} eliminat din coș.");
            }
            else
            {
                Console.WriteLine("Produsul nu a fost găsit în coș.");
            }
        }

        public void ShowCart()
        {
            if (!_items.Any())
            {
                Console.WriteLine("Coșul este gol.");
                return;
            }

            Console.WriteLine("=== Coșul curent ===");
            foreach (var item in _items)
            {
                Console.WriteLine($"{item.Produs.Name} - {item.Cantitate.GetValue()} - Total: {item.GetTotalPrice()}");
            }
            Console.WriteLine($"TOTAL GENERAL: {GetTotal()}");
        }

        public decimal GetTotal() => _items.Sum(i => i.GetTotalPrice());
    }
    
    class Program
    {
        static void Main()
        {
            var cart = new ShoppingCart();

            static int PromptInt(string message)
            {
                Console.Write(message);
                return int.Parse(Console.ReadLine() ?? "0");
            }

            static decimal PromptDecimal(string message)
            {
                Console.Write(message);
                return decimal.Parse(Console.ReadLine() ?? "0");
            }
            
            while (true)
            {
                Console.WriteLine("\n=================");
                Console.WriteLine("1. Adauga produs");
                Console.WriteLine("2. Elimina produs");
                Console.WriteLine("3. Afisează cos");
                Console.WriteLine("4. Ieșire");
                Console.WriteLine("\n=================");
                Console.Write("Alege opțiunea: ");

                var option = Console.ReadLine();

                switch (option)
                {

                    case "1":
                        Console.Write("Nume produs: ");
                        var name = Console.ReadLine();
                        Console.Write("Pret per unitate/kg: ");
                        var price = decimal.Parse(Console.ReadLine() ?? "0");

                        Console.Write("Tip cantitate ('u' = unitati, 'kg' = kilograme): ");
                        var type = Console.ReadLine();

                        IQuantity quantity = type switch
                        {
                            "u" => new CantitateUnitati(
                                PromptInt("Cantitate produs (numar de unitati): ")
                            ),
                            "kg" => new CantitateKilograme(
                                PromptDecimal("Cantitate produs (numar de kilograme): ")
                            ),
                            _ => throw new ArgumentException("Tip invalid.")
                        };

                        var product = new Produs(name ?? "Necunoscut", price);
                        cart.AddProduct(product, quantity);
                        break;

                    case "2":
                        Console.Write("Nume produs de eliminat: ");
                        var productName = Console.ReadLine();
                        cart.RemoveProduct(productName ?? "");
                        break;

                    case "3":
                        cart.ShowCart();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Opțiune invalidă.");
                        break;
                }
            }
        }
    }
}
