namespace PVM.Core.Definition
{
    public class Transition<T>
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
        public bool Execute { get; set; }

        protected bool Equals(Transition<T> other)
        {
            return string.Equals(Identifier, other.Identifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Transition<T>) obj);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }
    }
}