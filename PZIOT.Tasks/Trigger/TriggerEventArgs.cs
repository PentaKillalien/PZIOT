
using PZIOT.Common.Helper;
using PZIOT.Model.Models;
using PZIOT.Tasks.Rule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PZIOT.Tasks.Trigger
{
    public class TriggerEventArgs : EventArgs
    {
        public double OldValue { get; set; }
        public double NewValue { get; set; }
        public double MateId { get; set; }
    }
    public class TriggerData
    {
        private double value;
        public List<EquipmentMatesTriggerInt> rules;
        public event EventHandler<TriggerEventArgs> ValueChanged;
        public string rulemethod;
        public double mateId;
        public EquipmentDataScada usedata;
        public double Value
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
                    Type[] implementingTypes = InterfaceImplementationHelper.GetImplementingTypes(typeof(IRules));
                    var myClass = implementingTypes.FirstOrDefault(type => type.Name == rulemethod);
                    if (myClass != null)
                    {
                        var myObject = (IRules)Activator.CreateInstance(myClass);
                        myObject.ExecuteRule(usedata);
                        Console.WriteLine($"Trigger fired: {rule.Description}");
                    }
                }
            }

        }

    }
}
