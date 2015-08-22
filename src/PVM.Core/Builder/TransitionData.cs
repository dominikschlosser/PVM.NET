namespace PVM.Core.Builder
{
    internal class TransitionData
    {
        public TransitionData(string name, bool isDefault, string target, string source)
        {
            IsDefault = isDefault;
            Name = name;
            Target = target;
            Source = source;
        }

        public string Name { get; set; }
        public string Target { get; set; }
        public string Source { get; set; }
        public bool IsDefault { get; set; }
    }
}