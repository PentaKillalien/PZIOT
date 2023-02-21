
using PZIOT.Common.Helper;
using PZIOT.Model.Models;
using PZIOT.Tasks.Rule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PZIOT.Tasks.Trigger
{
    public class PzTcpTriggerEventArgs : EventArgs
    {
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public string EquipmentIp { get; set; }
    }
    public class PzTcpTriggerData
    {
        private int value;
        public event EventHandler<PzTcpTriggerEventArgs> ValueChanged;
        public string EquipmentIp;
        public int Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    var oldValue = this.value;
                    this.value = value;

                    ValueChanged?.Invoke(this, new PzTcpTriggerEventArgs { OldValue = oldValue, NewValue = value, EquipmentIp = EquipmentIp });

                }
            }
        }


    }
}
