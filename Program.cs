using System;
using System.Collections.Generic;
using System.Linq;

namespace ReceptenApp
{

    public class Ingredient
    {
        public string Naam { get; }
        public double Hoeveelheid { get; }
        public string Eenheid { get; }

        public Ingredient(string naam, double hoeveelheid, string eenheid)
        {
            Naam = naam;
            Hoeveelheid = hoeveelheid;
            Eenheid = eenheid;
        }

        public override string ToString()
        {
            if (Hoeveelheid <= 0 && string.IsNullOrWhiteSpace(Eenheid))
            {
                return Naam;
            }
            return $"{Hoeveelheid} {Eenheid} {Naam}".Trim();
        }
    }

    public class Stap
    {
        public int Nummer { get; }
        public string Beschrijving { get; }

        public Stap(int nummer, string beschrijving)
        {
            Nummer = nummer;
            Beschrijving = beschrijving;
        }
    }

    public class Recept
    {
        public string Naam { get; }
        public string Categorie { get; }
        public int BereidingstijdMinuten { get; }
        public string Omschrijving { get; }
        public List<Ingredient> Ingredienten { get; }
        public List<Stap> Stappen { get; }

        public Recept(string naam, string categorie, int bereidingstijdMinuten, string omschrijving)
        {
            Naam = naam;
            Categorie = categorie;
            BereidingstijdMinuten = bereidingstijdMinuten;
            Omschrijving = omschrijving;
            Ingredienten = new List<Ingredient>();
            Stappen = new List<Stap>();
        }

        public void VoegIngredientToe(Ingredient ingredient) => Ingredienten.Add(ingredient);

        public void VoegStapToe(string beschrijving)
        {
            Stappen.Add(new Stap(Stappen.Count + 1, beschrijving));
        }

        public void ToonDetails()
        {
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine($"{Naam.ToUpper()} ({Categorie} - {BereidingstijdMinuten} min)");
            Console.WriteLine("==================================================");
            Console.WriteLine($"Omschrijving: {Omschrijving}");
            Console.WriteLine();
            Console.WriteLine("Ingrediënten (basisportie: 1 persoon):");
            foreach (var ing in Ingredienten)
            {
                Console.WriteLine($" • {ing}");
            }
            Console.WriteLine();
            Console.WriteLine("Bereidingswijze:");
            foreach (var stap in Stappen)
            {
                Console.WriteLine($" {stap.Nummer}. {stap.Beschrijving}");
            }
            Console.WriteLine("==================================================");
        }
    }

    public class Receptenboek
    {
        public List<Recept> Recepten { get; } = new List<Recept>();

        public void VoegReceptToe(Recept recept) => Recepten.Add(recept);

        public void ToonOverzicht()
        {
            Console.WriteLine("\n--- Beschikbare Recepten ---");
            for (int i = 0; i < Recepten.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Recepten[i].Naam} ({Recepten[i].Categorie}, {Recepten[i].BereidingstijdMinuten} min)");
            }
        }

        public Recept? ZoekOpNaam(string naam)
        {
            if (string.IsNullOrWhiteSpace(naam)) return null;

            string zoeknaam = naam.Trim();

            return Recepten.FirstOrDefault(r =>
                r.Naam.Equals(zoeknaam, StringComparison.OrdinalIgnoreCase) ||
                r.Naam.Contains(zoeknaam, StringComparison.OrdinalIgnoreCase));
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            Receptenboek kookboek = InitialiseerRecepten();

            bool actief = true;
            while (actief)
            {
                Console.WriteLine("\n=== RECEPTENBOEK HOOFDMENU ===");
                Console.WriteLine("1. Toon alle recepten");
                Console.WriteLine("2. Toon receptdetails op nummer");
                Console.WriteLine("3. Zoek recept op naam");
                Console.WriteLine("4. Afsluiten");
                Console.Write("Maak een keuze (1-4): ");

                string keuze = Console.ReadLine() ?? string.Empty;

                switch (keuze)
                {
                    case "1":
                        kookboek.ToonOverzicht();
                        break;

                    case "2":
                        kookboek.ToonOverzicht();
                        Console.Write("\nVoer het gewenste nummer in: ");
                        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= kookboek.Recepten.Count)
                        {
                            kookboek.Recepten[index - 1].ToonDetails();
                        }
                        else
                        {
                            Console.WriteLine("Ongeldig nummer ingevoerd.");
                        }
                        break;

                    case "3":
                        Console.Write("\nVoer de (deel van de) receptnaam in: ");
                        string zoekterm = Console.ReadLine() ?? string.Empty;
                        var gevondenRecept = kookboek.ZoekOpNaam(zoekterm);

                        if (gevondenRecept != null)
                        {
                            gevondenRecept.ToonDetails();
                        }
                        else
                        {
                            // REQ-07 / AC-04.1: duidelijke foutmelding bij niet-bestaand recept
                            Console.WriteLine($"\n[Fout] Er is geen recept gevonden met de naam '{zoekterm}'.");
                        }
                        break;

                    case "4":
                        actief = false;
                        Console.WriteLine("Tot ziens!");
                        break;

                    default:
                        Console.WriteLine("Ongeldige invoer, probeer het opnieuw.");
                        break;
                }
            }
        }

        private static Receptenboek InitialiseerRecepten()
        {
            var boek = new Receptenboek();

            // Recept 1: Pasta Carbonara
            var carbonara = new Recept(
                "Pasta carbonara",
                "Hoofdgerecht",
                25,
                "Klassieke pasta met spek, ei en pecorino — romig zonder room."
            );
            carbonara.VoegIngredientToe(new Ingredient("spaghetti", 100, "g"));
            carbonara.VoegIngredientToe(new Ingredient("guanciale of spek", 75, "g"));
            carbonara.VoegIngredientToe(new Ingredient("ei", 1, "stuks"));
            carbonara.VoegIngredientToe(new Ingredient("pecorino (geraspt)", 25, "g"));
            carbonara.VoegIngredientToe(new Ingredient("Zwarte peper", 0, ""));
            carbonara.VoegStapToe("Kook de pasta al dente in gezouten water.");
            carbonara.VoegStapToe("Bak het spek krokant in een droge pan.");
            carbonara.VoegStapToe("Klop ei met pecorino; meng van het vuur af door de pasta.");
            carbonara.VoegStapToe("Serveer direct met versgemalen zwarte peper.");
            boek.VoegReceptToe(carbonara);

            // Recept 2: Courgettesoep
            var courgettesoep = new Recept(
                "Courgettesoep",
                "Soep",
                30,
                "Frisse groentesoep met een romige textuur."
            );
            courgettesoep.VoegIngredientToe(new Ingredient("courgette", 0.5, "stuks"));
            courgettesoep.VoegIngredientToe(new Ingredient("groentebouillon", 250, "ml"));
            courgettesoep.VoegIngredientToe(new Ingredient("ui (fijngesneden)", 0.25, "stuks"));
            courgettesoep.VoegIngredientToe(new Ingredient("knoflookteen", 0.5, "stuks"));
            courgettesoep.VoegIngredientToe(new Ingredient("olijfolie", 0.5, "el"));
            courgettesoep.VoegStapToe("Fruit de ui en knoflook in olijfolie tot glazig.");
            courgettesoep.VoegStapToe("Voeg de gesneden courgette toe en bak 3 minuten mee.");
            courgettesoep.VoegStapToe("Schenk de bouillon erbij en laat 15 minuten koken.");
            courgettesoep.VoegStapToe("Pureer de soep met een staafmixer tot een glad geheel.");
            boek.VoegReceptToe(courgettesoep);

            // Recept 3: Couscous salade met feta
            var couscous = new Recept(
                "Couscous salade met feta",
                "Salade",
                15,
                "Lichte en frisse maaltijdsalade."
            );
            couscous.VoegIngredientToe(new Ingredient("couscous", 75, "g"));
            couscous.VoegIngredientToe(new Ingredient("kokend water", 100, "ml"));
            couscous.VoegIngredientToe(new Ingredient("feta (verkruimeld)", 50, "g"));
            couscous.VoegIngredientToe(new Ingredient("komkommer (in blokjes)", 0.25, "stuks"));
            couscous.VoegIngredientToe(new Ingredient("cherrytomaten", 4, "stuks"));
            couscous.VoegIngredientToe(new Ingredient("citroensap en olijfolie", 1, "el"));
            couscous.VoegStapToe("Wel de couscous 5 minuten in heet water met een snufje zout.");
            couscous.VoegStapToe("Roer de couscous los met een vork en laat afkoelen.");
            couscous.VoegStapToe("Meng de komkommer, tomaten en feta door de couscous.");
            couscous.VoegStapToe("Breng op smaak met citroensap en olijfolie.");
            boek.VoegReceptToe(couscous);

            return boek;
        }
    }
}
