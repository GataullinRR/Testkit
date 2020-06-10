namespace Shared.Types
{
    public abstract class ParameterBase
    {
        public string Name { get; set; }
        public ParameterBase? Parrent { get; set; }
    }
}
