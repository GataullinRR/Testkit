namespace Runner
{
    public abstract class TargetBase
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public abstract string TargetType { get; }
    }
}
