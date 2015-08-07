namespace PVM.Core.Definition
{
    public class Transition
    {
        public Transition(string name, Node source, Node destination)
        {
            Source = source;
            Destination = destination;
            Name = name;
        }

        public Node Source { get; private set; }
        public Node Destination { get; private set; }
        public string Name { get; private set; }
    }
}