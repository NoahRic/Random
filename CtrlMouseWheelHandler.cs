// CtrlMouseWheelHandler - Handler Ctrl+mouse wheel events to scroll by pages instead of lines

using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace CtrlMouseWheel
{
    [Export(typeof(IMouseProcessorProvider))]
    [Name("Ctrl Mouse Processor")]
    [Order(Before = "Zoom")] // Make sure to pre-empt the Zoom mouse processor in handling ctrl+wheel
    [ContentType("any")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    class MouseProcessorProvider : IMouseProcessorProvider
    {
        public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return new CtrlMouseProcessor(wpfTextView);
        }
    }

    class CtrlMouseProcessor : MouseProcessorBase
    {
        ITextView _view;

        int currentDelta = 0;

        public CtrlMouseProcessor(ITextView view)
        {
            _view = view;
        }

        public override void PreprocessMouseWheel(MouseWheelEventArgs e)
        {
            // Only handle this when the ctrl key is held down
            if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                return;

            currentDelta += e.Delta;

            // Check to see if we've accumulated enough clicks
            int pagesToScroll = currentDelta / Mouse.MouseWheelDeltaForOneLine;
            if (pagesToScroll != 0)
            {
                currentDelta -= (pagesToScroll * Mouse.MouseWheelDeltaForOneLine);

                // A negative value means to scroll down, positive up
                ScrollDirection direction = pagesToScroll < 0 ? ScrollDirection.Down : ScrollDirection.Up;

                for (int i = Math.Abs(pagesToScroll); i > 0; i--)
                    _view.ViewScroller.ScrollViewportVerticallyByPage(direction);
            }

            e.Handled = true;
        }
    }
}
