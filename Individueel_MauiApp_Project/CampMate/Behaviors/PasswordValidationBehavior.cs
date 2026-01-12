using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;

namespace CampMate.Behaviors
{
    public class PasswordValidationBehavior : Behavior<Entry>
    {
        // Patronen voor wachtwoordsterkte
        // Minimaal 1 cijfer
        const string numberRegex = @"[0-9]+";
        // Minimaal 1 hoofdletter of kleine letter
        const string upperLowerRegex = @"[a-zA-Z]+";
        // Minimaal 1 speciaal teken (!@#$%^&* etc.)
        const string symbolRegex = @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]";

        // Minimale lengte
        const int minLength = 6;

        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }

        void HandleTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry) return;

            string password = e.NewTextValue ?? string.Empty;

            // 1. Als het wachtwoord leeg is -> Standaard kleur (Zwart of Wit afhankelijk van thema)
            if (string.IsNullOrEmpty(password))
            {
                entry.TextColor = Colors.Black; // Of AppThemeBinding gebruiken
                return;
            }

            // 2. Score berekenen
            int score = 0;

            if (password.Length >= minLength) score++;
            if (password.Length >= 8) score++; // Bonus voor lengte
            if (Regex.IsMatch(password, numberRegex)) score++;
            if (Regex.IsMatch(password, upperLowerRegex)) score++;
            if (Regex.IsMatch(password, symbolRegex)) score++;

            // 3. Kleur bepalen op basis van score
            if (score <= 2)
            {
                // ZWAK (Weak): Rood
                entry.TextColor = Colors.Red;
            }
            else if (score <= 4)
            {
                // GEMIDDELD (Medium): Oranje
                entry.TextColor = Colors.Orange;
            }
            else
            {
                // STERK (Strong): Groen
                entry.TextColor = Colors.Green;
            }
        }
    }
}