using PVM.Core.Definition.Nodes;

namespace PVM.Core.Definition
{
    public class Transition
    {
        public Transition(string identifier, bool isDefault, INode source, INode destination)
        {
            IsDefault = isDefault;
            Source = source;
            Destination = destination;
            Identifier = identifier;
        }

        public INode Source { get; private set; }
        public INode Destination { get; private set; }
        public string Identifier { get; private set; }
        public bool Executed { get; set; }
        public bool IsDefault { get; private set; }

        protected bool Equals(Transition other)
        {
            return string.Equals(Identifier, other.Identifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Transition) obj);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }
    }
}