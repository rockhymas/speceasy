using System.Threading.Tasks;
using System;
using System.Globalization;

namespace SpecEasy
{
    public interface IContext
    {
        void Verify(Action addSpecs);
    }

    internal class Context : IContext
    {
        public const string GivenConjunction = "given ";
        public const string AndConjunction = "  and ";

        private const string UnnamedContextPrefix = "SPECEASY";
        private const string DefaultFirstConjunction = GivenConjunction;
        private const string DefaultJoiningConjunction = AndConjunction;

        private static int nuSpecContextId;
        private static string CreateUnnamedContextName()
        {
            return UnnamedContextPrefix + (nuSpecContextId++).ToString(CultureInfo.InvariantCulture);
        }


        private readonly string conjunction;
        private readonly Func<Task> setupAction = async delegate { };
        private Action enterAction = delegate { };

        public Context()
        {
        }

        public Context(Func<Task> setup, string description = null, string conjunction = null)
        {
            setupAction = setup;
            Description = description ?? CreateUnnamedContextName();
            this.conjunction = conjunction;
        }

        public Context(Func<Task> setup, Action addSpecs, string description = null, string conjunction = null) : this(setup, description, conjunction)
        {
            enterAction = addSpecs;
        }

        public void Verify(Action addSpecs)
        {
            var cachedEnterAction = enterAction;
            enterAction = () => { cachedEnterAction(); addSpecs(); };
        }

        public void EnterContext()
        {
            enterAction();
        }

        public async Task SetupContext()
        {
            await setupAction().ConfigureAwait(false);
        }

        internal string Description
        {
            get;
            private set;
        }

        internal bool IsNamedContext
        {
            get
            {
                return !Description.StartsWith(UnnamedContextPrefix);
            }
        }

        internal string Conjunction(bool first)
        {
            return conjunction ?? (first ? DefaultFirstConjunction : DefaultJoiningConjunction);
        }
    }
}