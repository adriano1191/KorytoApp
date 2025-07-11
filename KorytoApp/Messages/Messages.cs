using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace KorytoApp.Messages
{
    class Messages
    {
    }

    public class MealAddedMessage
    {
        public int Calories { get; }
        public int Water { get; }

        public MealAddedMessage(int calories, int water)
        {
            Calories = calories;
            Water = water;
        }
    }
}
