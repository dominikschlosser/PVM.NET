using System.ComponentModel.DataAnnotations;
using PVM.Core.Definition;

namespace PVM.Persistence.Sql.Model
{
    public class TransitionModel
    {
        [Key]
        public string Identifier { get; set; }

        public string Source { get; set; }
        public string Destination { get; set; }
        public bool Executed { get; set; }
        public bool IsDefault { get; set; }

        public static TransitionModel FromTransition(Transition transition)
        {
            return new TransitionModel
            {
                Identifier = transition.Identifier,
                Source = transition.Source.Name,
                Destination = transition.Destination == null ? null : transition.Destination.Name,
                Executed = transition.Executed,
                IsDefault = transition.IsDefault
            };
        }
    }
}