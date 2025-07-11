using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KorytoApp.Services
{
    public enum SpeechContext
    {
        Start,
        AddMeal,
        MaxCalories,
        AddWater,
        AddMealAndWater
        // Dodaj kolejne, jeśli chcesz!
    }

    public class SpeechBubbleService
    {
        private readonly Dictionary<SpeechContext, List<string>> _speechTextsByContext = new()
        {
            [SpeechContext.Start] = new()
        {
            "Cześć knurze! Dodaj coś dziś do koryta.",
            "Nowy dzień – nowa wyżerka!",
            "Pusty żołądek – czas to zmienić!"
        },
            [SpeechContext.AddMeal] = new()
        {
            "Mniam! Jeszcze trochę kalorii!",
            "Ale pyszności, knurku!",
            "Jeszcze jedno danie i idę spać...",
            "Jeszcze mi mało!"
        },
            [SpeechContext.MaxCalories] = new()
        {
            "Ostrożnie, zaraz pęknę!",
            "Już wystarczy... Chyba?",
            "Limit przekroczony! Czas na dietę!",
            "Uspokój się świnio!"
        },
            [SpeechContext.AddWater] = new()
        {
            "Pijesz jak świnka! Tak trzymaj!",
            "No, nareszcie trochę wody.",
            "Nawadnianie pełną parą!"
        },
            [SpeechContext.AddMealAndWater] = new()
        {
            "Pijesz i żresz jak świnka! Tak trzymaj!",
            "No, nareszcie trochę wody i żarcia.",
            "Om! Nom! Nom! Nom!"
        }

        };

        private readonly Random _rnd = new();

        public string GetRandomSpeech(SpeechContext context)
        {
            if (_speechTextsByContext.TryGetValue(context, out var texts) && texts.Any())
            {
                return texts[_rnd.Next(texts.Count)];
            }
            return "Knur nie wie co powiedzieć...";
        }
    }

}
