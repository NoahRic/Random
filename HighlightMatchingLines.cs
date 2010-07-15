using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace HighlightMatchingLines
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("plaintext")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class ViewCreationListener : IWpfTextViewCreationListener
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("HighlightLines")]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        [TextViewRole(PredefinedTextViewRoles.Document)]
        public AdornmentLayerDefinition editorAdornmentLayer = null;

        public void TextViewCreated(IWpfTextView textView)
        {
            textView.LayoutChanged += ViewLayoutChanged;
        }

        void ViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            IWpfTextView view = sender as IWpfTextView;
            var adornmentLayer = view.GetAdornmentLayer("HighlightLines");

            foreach (var line in e.NewOrReformattedLines)
            {
                if (line.Extent.GetText().StartsWith("logger.", StringComparison.OrdinalIgnoreCase))
                {
                    Rectangle rect = new Rectangle()
                        {
                            Width = view.ViewportWidth + view.MaxTextRightCoordinate,
                            Height = line.Height,
                            Fill = Brushes.AliceBlue
                        };

                    Canvas.SetTop(rect, line.Top);
                    Canvas.SetLeft(rect, 0);
                    adornmentLayer.AddAdornment(line.Extent, null, rect);
                }
            }
        }
    }
}
