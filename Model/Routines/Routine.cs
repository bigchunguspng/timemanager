using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TimeManager.Utilities;

namespace TimeManager.Model.Routines
{
    public abstract class Routine
    {
        protected int First;
        protected int Last;
        
        protected Routine() { }
        protected Routine(string description)
        {
            Description = description;
            Times = new List<Period>();
        }
        
        [JsonProperty] protected string Description { get; set; }
        [JsonProperty] protected List<Period> Times { get; set; }

        public int HowManyTimes(Period period)
        {
            CalculateFirstAndLastTimes(period);
            return Last - First;
        }

        public float AverageFrequency(Period period, int per = 7 /*days*/)
        {
            CalculateFirstAndLastTimes(period);
            return (Last - First) / (period.Duration().Days / (float) per);
        }

        public TimeSpan TimeSinceLastTime() => Times[Times.Count - 1].TimePassed();


        /// <summary>Присвоює "First" індекс першого елементу після початку періоду, а "Last" - першого елементу після закінчення періоду</summary>
        protected void CalculateFirstAndLastTimes(Period period)
        {
            First = -1;
            Last  = -1;

            for (var i = 0; i < Times.Count; i++)
                if (First < 0)
                {
                    if (Times[i].Start >= period.Start)
                        First = i;
                }
                else if (Times[i].Start > period.End)
                    Last = i;

            if (First < 0) First = 0;
            if (Last < 0) Last = Times.Count - 1;
        }
    }
}