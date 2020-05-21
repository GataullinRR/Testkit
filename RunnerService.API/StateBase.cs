namespace RunnerService.APIModels
{
    public abstract class StateBase
    {
        public State State { get; set; }

        protected StateBase(State state)
        {
            State = state;
        }
    } 
}
