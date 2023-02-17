using Microsoft.Data.SqlClient;
using PZIOT.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZIOT.Tasks.Trigger
{
    public class TriggerEventArgs : EventArgs
    {
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public int MateId { get; set; }
    }
    public class TriggerData
    {
        private int value;
        public List<EquipmentMatesTrigger> rules;
        public event EventHandler<TriggerEventArgs> ValueChanged;
        public int mateId;
        public int Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    var oldValue = this.value;
                    this.value = value;

                    ValueChanged?.Invoke(this, new TriggerEventArgs { OldValue = oldValue, NewValue = value,MateId=mateId });

                    CheckRules();
                }
            }
        }

        private void CheckRules()
        {
            foreach (var rule in rules)
            {
                if (value >= rule.MinValue && value <= rule.MaxValue)
                {
                    Console.WriteLine($"Trigger fired: {rule.Description}");
                }
            }
        }

    }
}
