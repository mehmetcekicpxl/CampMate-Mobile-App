using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;

namespace CampMate.Behaviors
{
    public class EmailValidationBehavior : Behavior<Entry>
    {
        // Regex voor het controleren van een geldig e-mailadres
        // Dit zorgt ervoor dat de gebruiker een correct formaat invoert (bijv. naam@domein.com)
        const string emailRegex = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        protected override void OnAttachedTo(Entry bindable)
        {
            // Abonneer op het TextChanged event wanneer het gedrag wordt gekoppeld
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            // Abonnement opzeggen om geheugenlekken te voorkomen wanneer het gedrag wordt losgekoppeld
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }

        void HandleTextChanged(object? sender, TextChangedEventArgs e)
        {
            bool isValid = false;

            // Controleer of de invoer niet leeg is en overeenkomt met de regex
            if (!string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                isValid = Regex.IsMatch(e.NewTextValue, emailRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }

            if (sender is Entry entry)
            {
                // Visuele feedback geven aan de gebruiker:
                // Als het e-mailadres geldig is: zwarte tekst (of standaardkleur).
                // Als het ongeldig is: rode tekst.
                entry.TextColor = isValid ? Colors.Black : Colors.Red;
            }
        }
    }
}