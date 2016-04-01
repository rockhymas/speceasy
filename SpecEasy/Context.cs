using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SpecEasy
{
    internal class Context : IContext
    {
        public const string GivenConjunction = "given ";
        public const string AndConjunction = "  and ";
        public const string ButConjunction = "  but ";

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
        private Context nextEquivalentContext;

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
            var currentContext = this;
            do
            {
                currentContext.VerifyInternal(addSpecs);
                currentContext = currentContext.nextEquivalentContext;
            } while (currentContext != null);
        }

        private void VerifyInternal(Action addSpecs)
        {
            var cachedEnterAction = enterAction;
            enterAction = () =>
            {
                cachedEnterAction();
                addSpecs();
            };
        }

        public IContext Or(string description)
        {
            return Or(description, () => { });
        }

        public IContext Or(string description, Action setup)
        {
            return Or(description, setup.Wrap());
        }

        public IContext Or(string description, Func<Task> setup)
        {
            AppendContext(new Context(setup, description, conjunction));
            return this;
        }

        private void AppendContext(Context equivalentContext)
        {
            var currentContext = this;
            while (currentContext.nextEquivalentContext != null)
                currentContext = currentContext.nextEquivalentContext;
            currentContext.nextEquivalentContext = equivalentContext;
        }

        public IEnumerable<Context> EquivalentContexts()
        {
            var currentContext = this;
            do
            {
                yield return currentContext;
                currentContext = currentContext.nextEquivalentContext;
            } while (currentContext != null);
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