using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terminals.ExportImport
{
    public abstract class Integration<TIntegrationType> where TIntegrationType : IIntegration
    {
        protected Dictionary<string, TIntegrationType> providers;

        protected TIntegrationType FindProvider(string fileName, int index)
        {
            this.LoadProviders();

            if (index < 1)
                index = 1;

            return providers[providers.Keys.ElementAt(index - 1)];
        }

        protected void AddProviderFilter(StringBuilder stringBuilder, TIntegrationType provider)
        {
            if (stringBuilder.Length != 0)
            {
                stringBuilder.Append("|");
            }

            stringBuilder.Append(provider.Name);
            stringBuilder.Append(" (*");
            stringBuilder.Append(provider.KnownExtension);
            stringBuilder.Append(")|*");
            stringBuilder.Append(provider.KnownExtension); // already in lowercase
        }

        protected abstract void LoadProviders();
    }
}