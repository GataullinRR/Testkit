using System;

namespace RunnerService.API.Models
{
    public class StateInfo
    {
        public string? State { get; set; }
        public DateTime? NextStateUpdate { get; set; }
        public bool IsFinal { get; set; }

        public StateInfo(string? state, DateTime? nextStateUpdate, bool isFinal)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            NextStateUpdate = nextStateUpdate;
            IsFinal = isFinal;
        }
    }
}
