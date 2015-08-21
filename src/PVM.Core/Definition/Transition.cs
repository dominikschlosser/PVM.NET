using PVM.Core.Data;

namespace PVM.Core.Definition
{
    public class Transition<T> where T : IProcessData<T>
    {
        public Transition(string identifier, INode<T> source, INode<T> destination)
        {
            Source = source;
            Destination = destination;
            Identifier = identifier;
        }

        public INode<T> Source { get; private set; }
        public INode<T> Destination { get; private set; }
        public string Identifier { get; private set; }
        public bool Executed { get; set; }
    }
}