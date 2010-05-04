// A command filter for the editor.  Command filters get an opportunity to observe and handle commands before and after the editor acts on them.

using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace FixCtrlBackspace
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("any")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    class VsTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        IVsEditorAdaptersFactoryService AdaptersFactory = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            CommandFilter filter = new CommandFilter(textViewAdapter, AdaptersFactory);

            IOleCommandTarget next;
            if (ErrorHandler.Succeeded(textViewAdapter.AddCommandFilter(filter, out next)))
                filter.Next = next;
        }
    }

    class CommandFilter : IOleCommandTarget
    {
        IVsEditorAdaptersFactoryService _adaptersFactory;
        IVsTextView _viewAdapter;
        public CommandFilter(IVsTextView viewAdapter, IVsEditorAdaptersFactoryService adaptersFactory)
        {
            _viewAdapter = viewAdapter;
            _adaptersFactory = adaptersFactory;
        }

        /// <summary>
        /// The next command target in the filter chain (provided by <see cref="IVsTextView.AddCommandFilter"/>).
        /// </summary>
        internal IOleCommandTarget Next { get; set; }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K &&
                nCmdID == (uint)VSConstants.VSStd2KCmdID.DELETEWORDLEFT)
            {
                var textView = _adaptersFactory.GetWpfTextView(_viewAdapter);
                if (textView != null && textView.Selection.IsEmpty && textView.Caret.InVirtualSpace)
                {
                    textView.Caret.MoveTo(textView.Caret.Position.BufferPosition);
                    return VSConstants.S_OK;
                }
            }

            return Next.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return Next.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}