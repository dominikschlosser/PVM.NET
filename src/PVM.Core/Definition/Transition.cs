namespace PVM.Core.Definition
{
    public class Transition
    {
        public Transition(string name, INode source, INode destination)
        {
            Source = source;
            Destination = destination;
            Name = name;
        }

        public INode Source { get; private set; }
        public INode Destination { get; private set; }
        public string Name { get; private set; }
    }
}