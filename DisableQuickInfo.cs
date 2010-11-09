// A quick info source that immediately dismisses the quick info session,
// thus disabling quick info.  To change the language this applies to,
// modify the ContentType attribute (you can supply multiple).

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

namespace DisableQuickInfo
{
    [Export(typeof(IQuickInfoSourceProvider))]
    [Name("disable quick info")]
    [ContentType("csharp")]
    class QuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new CancelingQuickInfoSource();
        }
    }

    class CancelingQuickInfoSource : IQuickInfoSource
    {
        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            // Dismiss the session immediately.
            session.Dismiss();
            applicableToSpan = null;
        }

        public void Dispose()
        {
        }
    }
}
