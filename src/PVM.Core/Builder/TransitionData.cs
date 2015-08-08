namespace PVM.Core.Builder
{
    internal class TransitionData
    {
        public TransitionData(string name, string target, string source)
        {
            Name = name;
            Target = target;
            Source = source;
        }

        public string Name { get; set; }
        public string Target { get; set; }
        public string Source { get; set; }
    }
}