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

    // Basisklasse voor alle recepten
    public class Recept
    {
        public string Naam { get; }
        public string Categorie { get; }
        public int BereidingstijdMinuten { get; }
        public int Personen { get; }
        public string Omschrijving { get; }
        public List<Ingredient> Ingredienten { get; }
        public List<Stap> Stappen { get; }

        public Recept(string naam, string categorie, int bereidingstijdMinuten, string omschrijving, int personen = 1)
        {
            Naam = naam;
            Categorie = categorie;
            BereidingstijdMinuten = bereidingstijdMinuten;
            Omschrijving = omschrijving;
            Personen = personen;
            Ingredienten = new List<Ingredient>();
            Stappen = new List<Stap>();
        }

        public void VoegIngredientToe(Ingredient ingredient) => Ingredienten.Add(ingredient);

        public void VoegStapToe(string beschrijving)
        {
            Stappen.Add(new Stap(Stappen.Count + 1, beschrijving));
        }

        
        public virtual void ToonDetails()
        {
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine($"{Naam.ToUpper()} ({Categorie}, {BereidingstijdMinuten} min, {Personen} pers.)");
            Console.WriteLine("==================================================");
            Console.WriteLine($"Omschrijving: {Omschrijving}");
            ToonExtraEigenschappen();
            Console.WriteLine();
            Console.WriteLine($"Ingrediënten (basisportie: {Personen} persoon/personen):");
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

        protected virtual void ToonExtraEigenschappen()
        {
            
        }

        // Hulpfunctie (zoeken op ingrediënt)
        public bool BevatIngredient(string ingredientNaam)
        {
            if (string.IsNullOrWhiteSpace(ingredientNaam)) return false;
            return Ingredienten.Any(ing => ing.Naam.Contains(ingredientNaam, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Variant 1: OvenschotelRecept met eigen specifieke kenmerken en gedrag
    public class OvenschotelRecept : Recept
    {
        public int OventemperatuurInGraden { get; }

        public OvenschotelRecept(string naam, string categorie, int bereidingstijdMinuten, string omschrijving, int oventemperatuurInGraden, int personen = 4)
            : base(naam, categorie, bereidingstijdMinuten, omschrijving, personen)
        {
            OventemperatuurInGraden = oventemperatuurInGraden;
        }

        protected override void ToonExtraEigenschappen()
        {
            Console.WriteLine($"Type: Ovenschotel (Vereiste oventemperatuur: {OventemperatuurInGraden}°C)");
        }
    }

    // Variant 2: SnelRecept / Lunch met vegetarisch kenmerk
    public class SnelRecept : Recept
    {
        public bool IsVegetarisch { get; }

        public SnelRecept(string naam, string categorie, int bereidingstijdMinuten, string omschrijving, bool isVegetarisch, int personen = 1)
            : base(naam, categorie, bereidingstijdMinuten, omschrijving, personen)
        {
            IsVegetarisch = isVegetarisch;
        }

        protected override void ToonExtraEigenschappen()
        {
            string vegaTekst = IsVegetarisch ? "Vegetarisch" : "Niet vegetarisch (bevat vlees/vis)";
            Console.WriteLine($"Type: Snelle maaltijd | Dieet: {vegaTekst}");
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
                Console.WriteLine($"{i + 1}. {Recepten[i].Naam} ({Recepten[i].Categorie}, {Recepten[i].BereidingstijdMinuten} min, {Recepten[i].Personen} pers.)");
            }
        }

        // Zoeken op receptnaam
        public Recept? ZoekOpNaam(string naam)
        {
            if (string.IsNullOrWhiteSpace(naam)) return null;
            string zoeknaam = naam.Trim();

            return Recepten.FirstOrDefault(r =>
                r.Naam.Equals(zoeknaam, StringComparison.OrdinalIgnoreCase) ||
                r.Naam.Contains(zoeknaam, StringComparison.OrdinalIgnoreCase));
        }

        // Zoeken op ingrediëntnaam
        public List<Recept> ZoekOpIngredient(string ingredientNaam)
        {
            if (string.IsNullOrWhiteSpace(ingredientNaam)) return new List<Recept>();

            string zoekterm = ingredientNaam.Trim();
            return Recepten.Where(r => r.BevatIngredient(zoekterm)).ToList();
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
                Console.WriteLine("4. Zoek recept op ingrediënt");
                Console.WriteLine("5. Afsluiten");
                Console.Write("Maak een keuze (1-5): ");

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
                        string zoektermNaam = Console.ReadLine() ?? string.Empty;

                        if (string.IsNullOrWhiteSpace(zoektermNaam))
                        {
                            Console.WriteLine("Geen zoekterm opgegeven.");
                            break;
                        }

                        var gevondenRecept = kookboek.ZoekOpNaam(zoektermNaam);

                        if (gevondenRecept != null)
                        {
                            gevondenRecept.ToonDetails();
                        }
                        else
                        {
                            Console.WriteLine($"\n[Fout] Er is geen recept gevonden met de naam '{zoektermNaam}'.");
                        }
                        break;

                    case "4":
                        Console.Write("\nVoer de (deel van de) ingrediëntnaam in: ");
                        string zoektermIng = Console.ReadLine() ?? string.Empty;

                        if (string.IsNullOrWhiteSpace(zoektermIng))
                        {
                            Console.WriteLine("Geen ingrediëntnaam opgegeven.");
                            break;
                        }

                        var gevondenLijst = kookboek.ZoekOpIngredient(zoektermIng);

                        if (gevondenLijst.Any())
                        {
                            Console.WriteLine($"\nGevonden recepten met '{zoektermIng}':");
                            for (int i = 0; i < gevondenLijst.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {gevondenLijst[i].Naam} ({gevondenLijst[i].Categorie})");
                            }

                            Console.Write("\nWil je direct de details van een recept bekijken? Voer nummer in (of druk Enter om over te slaan): ");
                            if (int.TryParse(Console.ReadLine(), out int gekozenIndex) && gekozenIndex >= 1 && gekozenIndex <= gevondenLijst.Count)
                            {
                                gevondenLijst[gekozenIndex - 1].ToonDetails();
                            }
                        }
                        else
                        {
                            Console.WriteLine($"\n[Fout] Geen recepten gevonden die het ingrediënt '{zoektermIng}' bevatten.");
                        }
                        break;

                    case "5":
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

            // Recept 1: Moussaka (Ovenschotel - Hoofdgerecht, 60 min, 4 pers.)
            var moussaka = new OvenschotelRecept(
                "Moussaka",
                "Hoofdgerecht",
                60,
                "Griekse ovenschotel met aubergine en gehakt — eigen type met langere bereidingstijd.",
                180,
                4
            );
            moussaka.VoegIngredientToe(new Ingredient("aubergine", 2, "stuks"));
            moussaka.VoegIngredientToe(new Ingredient("gehakt", 500, "g"));
            moussaka.VoegIngredientToe(new Ingredient("aardappelen", 400, "g"));
            moussaka.VoegIngredientToe(new Ingredient("bechamelsaus", 250, "ml"));
            moussaka.VoegStapToe("Snijd de aubergines in plakken en bak ze aan.");
            moussaka.VoegStapToe("Rul het gehakt met tomatensaus en kruiden.");
            moussaka.VoegStapToe("Bouw de laagjes op in de ovenschaal en giet de bechamel erover.");
            moussaka.VoegStapToe("Bak 45 minuten goudbruin in de oven op 180°C.");
            boek.VoegReceptToe(moussaka);

            // Recept 2: Basisrecept pannenkoeken (SnelRecept - Ontbijt, 20 min, 4 pers.)
            var pannenkoeken = new SnelRecept(
                "Basisrecept pannenkoeken",
                "Ontbijt",
                20,
                "Simpel ontbijtrecept — ander soort dan hoofdgerecht; zoek op 'melk' of 'ei'.",
                true,
                4
            );
            pannenkoeken.VoegIngredientToe(new Ingredient("bloem", 250, "g"));
            pannenkoeken.VoegIngredientToe(new Ingredient("melk", 500, "ml"));
            pannenkoeken.VoegIngredientToe(new Ingredient("ei", 2, "stuks"));
            pannenkoeken.VoegIngredientToe(new Ingredient("snufje zout", 0, ""));
            pannenkoeken.VoegStapToe("Meng de bloem en het zout in een kom.");
            pannenkoeken.VoegStapToe("Klop de melk en de eieren erdoor tot een glad beslag.");
            pannenkoeken.VoegStapToe("Bak de pannenkoeken aan weerszijden goudbruin in boter.");
            boek.VoegReceptToe(pannenkoeken);

            // Recept 3: Wraps met kip (SnelRecept - Lunch, 25 min, 2 pers.)
            var wrapsKip = new SnelRecept(
                "Wraps met kip",
                "Lunch",
                25,
                "Snelle lunch — derde soort; ingrediënt 'kip' onderscheidt van vegetarische gerechten.",
                false,
                2
            );
            wrapsKip.VoegIngredientToe(new Ingredient("kipfilet", 300, "g"));
            wrapsKip.VoegIngredientToe(new Ingredient("tortilla wraps", 4, "stuks"));
            wrapsKip.VoegIngredientToe(new Ingredient("sla", 50, "g"));
            wrapsKip.VoegIngredientToe(new Ingredient("tomaat", 2, "stuks"));
            wrapsKip.VoegStapToe("Bak de kipfiletreepjes goudbruin en gaar.");
            wrapsKip.VoegStapToe("Verwarm de wraps kort in de pan.");
            wrapsKip.VoegStapToe("Beleg de wraps met kip, sla en stukjes tomaat en rol ze op.");
            boek.VoegReceptToe(wrapsKip);

            // Recept 4: Pasta Carbonara (Thema 1 klassieker als regulier Recept)
            var carbonara = new Recept(
                "Pasta carbonara",
                "Hoofdgerecht",
                25,
                "Klassieke pasta met spek, ei en pecorino — romig zonder room.",
                1
            );
            carbonara.VoegIngredientToe(new Ingredient("spaghetti", 100, "g"));
            carbonara.VoegIngredientToe(new Ingredient("guanciale of spek", 75, "g"));
            carbonara.VoegIngredientToe(new Ingredient("ei", 1, "stuks"));
            carbonara.VoegIngredientToe(new Ingredient("pecorino (geraspt)", 25, "g"));
            carbonara.VoegIngredientToe(new Ingredient("zwarte peper", 0, ""));
            carbonara.VoegStapToe("Kook de pasta al dente in gezouten water.");
            carbonara.VoegStapToe("Bak het spek krokant in een droge pan.");
            carbonara.VoegStapToe("Klop ei met pecorino; meng van het vuur af door de pasta.");
            carbonara.VoegStapToe("Serveer direct met versgemalen zwarte peper.");
            boek.VoegReceptToe(carbonara);

            return boek;
        }
    }
}