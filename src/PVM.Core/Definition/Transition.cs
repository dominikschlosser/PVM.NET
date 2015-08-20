namespace PVM.Core.Definition
{
    public class Transition
    {
        public Transition(string identifier, INode source, INode destination)
        {
            Source = source;
            Destination = destination;
            Identifier = identifier;
        }

        public INode Source { get; private set; }
        public INode Destination { get; private set; }
        public string Identifier { get; private set; }
        public bool Executed { get; set; }
    }
}