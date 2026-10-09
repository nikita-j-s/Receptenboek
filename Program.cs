using System;
using System.Collections.Generic;
using System.Linq;

namespace ReceptenApp
{
    public class PlantaardigAlternatief
    {
        public string Naam { get; }
        public int KcalPerBasisportie { get; }

        public PlantaardigAlternatief(string naam, int kcalPerBasisportie)
        {
            Naam = naam;
            KcalPerBasisportie = kcalPerBasisportie;
        }
    }

    public class Ingredient
    {
        public string Naam { get; }
        public double Hoeveelheid { get; }
        public string Eenheid { get; }
        public int KcalPerBasisportie { get; }
        public PlantaardigAlternatief? Alternatief { get; }

        public Ingredient(string naam, double hoeveelheid, string eenheid, int kcalPerBasisportie = 0, PlantaardigAlternatief? alternatief = null)
        {
            Naam = naam;
            Hoeveelheid = hoeveelheid;
            Eenheid = eenheid;
            KcalPerBasisportie = kcalPerBasisportie;
            Alternatief = alternatief;
        }

        public string NaarTekst(double schaalfactor, bool gebruikPlantaardig)
        {
            string effectieveNaam = (gebruikPlantaardig && Alternatief != null) ? Alternatief.Naam : Naam;
            double geschaaldeHoeveelheid = Math.Round(Hoeveelheid * schaalfactor, 2);

            if (geschaaldeHoeveelheid <= 0 && string.IsNullOrWhiteSpace(Eenheid))
            {
                return effectieveNaam;
            }

            return $"{geschaaldeHoeveelheid} {Eenheid} {effectieveNaam}".Trim();
        }

        public int BerekenKcal(double schaalfactor, bool gebruikPlantaardig)
        {
            int basisKcal = (gebruikPlantaardig && Alternatief != null) ? Alternatief.KcalPerBasisportie : KcalPerBasisportie;
            return (int)Math.Round(basisKcal * schaalfactor);
        }
    }

    public class Stap
    {
        public int Nummer { get; }
        public string Beschrijving { get; }
        public string? Tip { get; }

        public Stap(int nummer, string beschrijving, string? tip = null)
        {
            Nummer = nummer;
            Beschrijving = beschrijving;
            Tip = tip;
        }
    }

    public class Recept
    {
        public string Naam { get; }
        public string Categorie { get; }
        public int BereidingstijdMinuten { get; }
        public int BasisPersonen { get; }
        public string Omschrijving { get; }
        public List<Ingredient> Ingredienten { get; }
        public List<Stap> Stappen { get; }

        public Recept(string naam, string categorie, int bereidingstijdMinuten, string omschrijving, int basisPersonen = 1)
        {
            Naam = naam;
            Categorie = categorie;
            BereidingstijdMinuten = bereidingstijdMinuten;
            Omschrijving = omschrijving;
            BasisPersonen = basisPersonen <= 0 ? 1 : basisPersonen;
            Ingredienten = new List<Ingredient>();
            Stappen = new List<Stap>();
        }

        public void VoegIngredientToe(Ingredient ingredient) => Ingredienten.Add(ingredient);

        public void VoegStapToe(string beschrijving, string? tip = null)
        {
            Stappen.Add(new Stap(Stappen.Count + 1, beschrijving, tip));
        }

        public virtual void ToonDetailsInteractief(int gewenstePersonen, bool plantaardigeVoorkeur)
        {
            double schaalfactor = (double)gewenstePersonen / BasisPersonen;

            int totaalKcal = Ingredienten.Sum(i => i.BerekenKcal(schaalfactor, plantaardigeVoorkeur));
            int kcalPerPersoon = gewenstePersonen > 0 ? (int)Math.Round((double)totaalKcal / gewenstePersonen) : 0;

            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine($"{Naam.ToUpper()} ({Categorie}, {BereidingstijdMinuten} min)");
            Console.WriteLine($"Aantal personen: {gewenstePersonen} (originele basis: {BasisPersonen} pers.)");
            Console.WriteLine($"Geschatte voedingswaarde: ca. {totaalKcal} kcal totaal ({kcalPerPersoon} kcal p.p.)");
            if (plantaardigeVoorkeur)
            {
                bool heeftAlternatieven = Ingredienten.Any(i => i.Alternatief != null);
                Console.WriteLine(heeftAlternatieven ? "Voorkeur: Plantaardige opties toegepast waar beschikbaar." : "Voorkeur: Plantaardig (geen vervangers noodzakelijk voor dit recept).");
            }
            Console.WriteLine("==================================================");
            Console.WriteLine($"Omschrijving: {Omschrijving}");
            ToonExtraEigenschappen();
            Console.WriteLine();

            Console.WriteLine($"Ingrediënten voor {gewenstePersonen} persoon/personen:");
            foreach (var ing in Ingredienten)
            {
                Console.WriteLine($" • {ing.NaarTekst(schaalfactor, plantaardigeVoorkeur)}");
            }

            Console.WriteLine();
            Console.WriteLine("Bereidingswijze:");
            foreach (var stap in Stappen)
            {
                Console.WriteLine($" {stap.Nummer}. {stap.Beschrijving}");
                if (!string.IsNullOrWhiteSpace(stap.Tip))
                {
                    Console.WriteLine($"    💡 Tip: {stap.Tip}");
                }
            }
            Console.WriteLine("==================================================");
        }

        public void ToonDetailsStandaard()
        {
            ToonDetailsInteractief(BasisPersonen, false);
        }

        protected virtual void ToonExtraEigenschappen() { }

        public bool BevatIngredient(string ingredientNaam)
        {
            if (string.IsNullOrWhiteSpace(ingredientNaam)) return false;
            return Ingredienten.Any(ing => ing.Naam.Contains(ingredientNaam, StringComparison.OrdinalIgnoreCase) ||
                                           (ing.Alternatief != null && ing.Alternatief.Naam.Contains(ingredientNaam, StringComparison.OrdinalIgnoreCase)));
        }
    }

    public class OvenschotelRecept : Recept
    {
        public int OventemperatuurInGraden { get; }

        public OvenschotelRecept(string naam, string categorie, int bereidingstijdMinuten, string omschrijving, int oventemperatuurInGraden, int basisPersonen = 4)
            : base(naam, categorie, bereidingstijdMinuten, omschrijving, basisPersonen)
        {
            OventemperatuurInGraden = oventemperatuurInGraden;
        }

        protected override void ToonExtraEigenschappen()
        {
            Console.WriteLine($"Type: Ovenschotel (Oventemperatuur: {OventemperatuurInGraden}°C)");
        }
    }

    public class SnelRecept : Recept
    {
        public bool IsVegetarisch { get; }

        public SnelRecept(string naam, string categorie, int bereidingstijdMinuten, string omschrijving, bool isVegetarisch, int basisPersonen = 1)
            : base(naam, categorie, bereidingstijdMinuten, omschrijving, basisPersonen)
        {
            IsVegetarisch = isVegetarisch;
        }

        protected override void ToonExtraEigenschappen()
        {
            Console.WriteLine($"Type: Snelle maaltijd | Dieet: {(IsVegetarisch ? "Vegetarisch" : "Niet vegetarisch")}");
        }
    }

    public class Receptenboek
    {
        public List<Recept> Recepten { get; } = new List<Recept>();

        public void VoegReceptToe(Recept recept) => Recepten.Add(recept);

        public bool VerwijderRecept(int index)
        {
            if (index >= 0 && index < Recepten.Count)
            {
                Recepten.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void ToonOverzicht()
        {
            Console.WriteLine("\n--- Beschikbare Recepten ---");
            if (!Recepten.Any())
            {
                Console.WriteLine("Het receptenboek is momenteel leeg.");
                return;
            }

            for (int i = 0; i < Recepten.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Recepten[i].Naam} ({Recepten[i].Categorie}, {Recepten[i].BereidingstijdMinuten} min, {Recepten[i].BasisPersonen} pers.)");
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
                Console.WriteLine("2. Kies recept en bekijk details (met porties & plantaardig)");
                Console.WriteLine("3. Zoek recept op naam");
                Console.WriteLine("4. Zoek recept op ingrediënt");
                Console.WriteLine("5. Nieuw recept toevoegen");
                Console.WriteLine("6. Recept verwijderen");
                Console.WriteLine("7. Afsluiten");
                Console.Write("Maak een keuze (1-7): ");

                string keuze = Console.ReadLine() ?? string.Empty;

                switch (keuze)
                {
                    case "1":
                        kookboek.ToonOverzicht();
                        break;

                    case "2":
                        kookboek.ToonOverzicht();
                        if (!kookboek.Recepten.Any()) break;

                        Console.Write("\nVoer het gewenste nummer in: ");
                        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= kookboek.Recepten.Count)
                        {
                            StartInteractieveReceptWeergave(kookboek.Recepten[index - 1]);
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
                            StartInteractieveReceptWeergave(gevondenRecept);
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

                            Console.Write("\nWil je een recept bekijken? Voer nummer in (of druk op Enter): ");
                            if (int.TryParse(Console.ReadLine(), out int gekozenIndex) && gekozenIndex >= 1 && gekozenIndex <= gevondenLijst.Count)
                            {
                                StartInteractieveReceptWeergave(gevondenLijst[gekozenIndex - 1]);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"\n[Fout] Geen recepten gevonden die '{zoektermIng}' bevatten.");
                        }
                        break;

                    case "5":
                        VoegNieuwReceptToeFlow(kookboek);
                        break;

                    case "6":
                        kookboek.ToonOverzicht();
                        if (!kookboek.Recepten.Any()) break;

                        Console.Write("\nVoer het nummer in van het recept dat je wilt verwijderen: ");
                        if (int.TryParse(Console.ReadLine(), out int verwijderIndex) && verwijderIndex >= 1 && verwijderIndex <= kookboek.Recepten.Count)
                        {
                            string verwijderdeNaam = kookboek.Recepten[verwijderIndex - 1].Naam;
                            kookboek.VerwijderRecept(verwijderIndex - 1);
                            Console.WriteLine($"Recept '{verwijderdeNaam}' is succesvol verwijderd.");
                        }
                        else
                        {
                            Console.WriteLine("Ongeldig nummer. Verwijderen afgebroken.");
                        }
                        break;

                    case "7":
                        actief = false;
                        Console.WriteLine("Tot ziens!");
                        break;

                    default:
                        Console.WriteLine("Ongeldige invoer, probeer het opnieuw.");
                        break;
                }
            }
        }

        private static void StartInteractieveReceptWeergave(Recept recept)
        {
            // Aantal personen uitvragen met validatie
            int gewenstePersonen = recept.BasisPersonen;
            while (true)
            {
                Console.Write($"\nVoor hoeveel personen wil je koken? (Standaard basis is {recept.BasisPersonen}): ");
                string invoer = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(invoer)) break;

                if (int.TryParse(invoer, out int p) && p > 0)
                {
                    gewenstePersonen = p;
                    break;
                }
                Console.WriteLine("Vul een geldig positief geheel getal in (bijv. 1, 2, 4).");
            }

            // Plantaardige voorkeur uitvragen
            Console.Write("Wil je plantaardige alternatieven gebruiken waar mogelijk? (j/n): ");
            string plantKeuze = Console.ReadLine()?.Trim().ToLower() ?? "n";
            bool plantaardig = plantKeuze == "j" || plantKeuze == "ja";

            recept.ToonDetailsInteractief(gewenstePersonen, plantaardig);
        }

        private static void VoegNieuwReceptToeFlow(Receptenboek kookboek)
        {
            Console.WriteLine("\n--- Nieuw Recept Toevoegen ---");

            Console.Write("Naam recept: ");
            string naam = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(naam))
            {
                Console.WriteLine("Naam mag niet leeg zijn. Toevoegen afgebroken.");
                return;
            }

            Console.Write("Categorie (bijv. Hoofdgerecht, Lunch, Ovenschotel): ");
            string categorie = Console.ReadLine()?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(categorie)) categorie = "Algemeen";

            Console.Write("Bereidingstijd in minuten: ");
            if (!int.TryParse(Console.ReadLine(), out int tijd) || tijd <= 0)
            {
                Console.WriteLine("Ongeldige bereidingstijd. Toevoegen afgebroken.");
                return;
            }

            Console.Write("Aantal personen basisportie: ");
            if (!int.TryParse(Console.ReadLine(), out int personen) || personen <= 0)
            {
                Console.WriteLine("Ongeldig aantal personen. Toevoegen afgebroken.");
                return;
            }

            Console.Write("Korte omschrijving: ");
            string omschrijving = Console.ReadLine()?.Trim() ?? string.Empty;

            var nieuwRecept = new Recept(naam, categorie, tijd, omschrijving, personen);

            // Ingrediënten toevoegen
            Console.WriteLine("\nVoeg ingrediënten toe (druk op Enter bij 'Ingrediëntnaam' om te stoppen):");
            while (true)
            {
                Console.Write("- Ingrediëntnaam: ");
                string ingNaam = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(ingNaam)) break;

                Console.Write("  Hoeveelheid (bijv. 200 of 1): ");
                double.TryParse(Console.ReadLine()?.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double hoeveelheid);

                Console.Write("  Eenheid (bijv. g, ml, stuks): ");
                string eenheid = Console.ReadLine()?.Trim() ?? string.Empty;

                Console.Write("  Kcal voor deze hoeveelheid (optioneel, standaard 0): ");
                int.TryParse(Console.ReadLine(), out int kcal);

                nieuwRecept.VoegIngredientToe(new Ingredient(ingNaam, hoeveelheid, eenheid, kcal));
            }

            // Stappen toevoegen
            Console.WriteLine("\nVoeg bereidingsstappen toe (druk op Enter bij 'Beschrijving' om te stoppen):");
            while (true)
            {
                Console.Write($"- Stap {nieuwRecept.Stappen.Count + 1} beschrijving: ");
                string stapBeschrijving = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(stapBeschrijving)) break;

                Console.Write("  Optionele tip bij deze stap (druk Enter om over te slaan): ");
                string tip = Console.ReadLine()?.Trim() ?? string.Empty;

                nieuwRecept.VoegStapToe(stapBeschrijving, string.IsNullOrWhiteSpace(tip) ? null : tip);
            }

            kookboek.VoegReceptToe(nieuwRecept);
            Console.WriteLine($"\nRecept '{nieuwRecept.Naam}' is succesvol toegevoegd aan het boek!");
        }

        private static Receptenboek InitialiseerRecepten()
        {
            var boek = new Receptenboek();

            // 1. Kip pesto plaattaart 
            var plaattaart = new Recept(
                "Kip pesto plaattaart",
                "Hoofdgerecht",
                45,
                "Plaattaart op bladerdeeg met kip en romige pesto.",
                4
            );
            plaattaart.VoegIngredientToe(new Ingredient("rol bladerdeeg", 1, "rol", 700));
            plaattaart.VoegIngredientToe(new Ingredient(
                "kipfilet", 400, "g", 440,
                new PlantaardigAlternatief("vegetarische kipstuckjes", 380)
            ));
            plaattaart.VoegIngredientToe(new Ingredient("groene pesto", 4, "el", 280));
            plaattaart.VoegIngredientToe(new Ingredient("creme fraiche", 125, "g", 240, new PlantaardigAlternatief("haver fraiche", 190)));
            plaattaart.VoegStapToe("Rol het bladerdeeg uit over de bakplaat.");
            plaattaart.VoegStapToe("Meng de creme fraiche met de helft van de pesto en verdeel over het deeg.");
            plaattaart.VoegStapToe("Beleg met kipreepjes en bak 25 minuten in de oven op 200°C.");
            plaattaart.VoegStapToe("Garneer direct na het bakken met de rest van de pesto.", "pesto niet verhitten voor behoud van smaak en kleur.");
            boek.VoegReceptToe(plaattaart);

            // 2. Romige bloemkool uit de oven (REQ-12 voorbeeld)
            var bloemkool = new Recept(
                "Romige bloemkool uit de oven",
                "Vegetarisch",
                35,
                "Vegetarisch hoofdgerecht met kaassaus — kcal per portie; geen vleesvervanger nodig.",
                2
            );
            bloemkool.VoegIngredientToe(new Ingredient("bloemkool", 1, "stronk", 120));
            bloemkool.VoegIngredientToe(new Ingredient("geraspte kaas", 100, "g", 380));
            bloemkool.VoegIngredientToe(new Ingredient("kookroom", 150, "ml", 300));
            bloemkool.VoegStapToe("Kook de bloemkoolroosjes 8 minuten voor.");
            bloemkool.VoegStapToe("Doe de bloemkool in een ovenschaal en giet de room erover.");
            bloemkool.VoegStapToe("Strooi de kaas erover en gratineer 15 minuten op 200°C.", "zet de laatste 3 minuten de ovengrill aan voor een knapperig korstje.");
            boek.VoegReceptToe(bloemkool);

            // 3. Moussaka (Ovenschotel-variant)
            var moussaka = new OvenschotelRecept(
                "Moussaka",
                "Hoofdgerecht",
                60,
                "Griekse ovenschotel met aubergine en gehakt.",
                180,
                4
            );
            moussaka.VoegIngredientToe(new Ingredient("aubergine", 2, "stuks", 80));
            moussaka.VoegIngredientToe(new Ingredient(
                "gehakt", 500, "g", 1100,
                new PlantaardigAlternatief("vegan rul gehakt", 700)
            ));
            moussaka.VoegIngredientToe(new Ingredient("aardappelen", 400, "g", 340));
            moussaka.VoegIngredientToe(new Ingredient("bechamelsaus", 250, "ml", 350));
            moussaka.VoegStapToe("Snijd de aubergines en bak ze licht aan.");
            moussaka.VoegStapToe("Rul het gehakt gaar met kruiden.");
            moussaka.VoegStapToe("Bouw de lagen op in de ovenschaal en giet de saus erover.");
            moussaka.VoegStapToe("Bak 45 minuten in de oven.");
            boek.VoegReceptToe(moussaka);

            // 4. Wraps met kip (Snel-variant)
            var wraps = new SnelRecept(
                "Wraps met kip",
                "Lunch",
                25,
                "Snelle lunch met kip en groenten.",
                false,
                2
            );
            wraps.VoegIngredientToe(new Ingredient(
                "kipfilet", 300, "g", 330,
                new PlantaardigAlternatief("tofu reepjes", 240)
            ));
            wraps.VoegIngredientToe(new Ingredient("tortilla wraps", 4, "stuks", 520));
            wraps.VoegIngredientToe(new Ingredient("sla", 50, "g", 10));
            wraps.VoegStapToe("Bak de kipreepjes gaar en goudbruin.");
            wraps.VoegStapToe("Verwarm de wraps en beleg ze naar wens.");
            boek.VoegReceptToe(wraps);

            return boek;
        }
    }
}
