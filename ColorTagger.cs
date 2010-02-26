using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text.Adornments;

namespace RegexTagger
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType("text")]
    class TaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new ColorTagger(buffer)) as ITagger<T>;
        }
    }

    class ColorTagger : RegexTagger<IErrorTag>
    {
        static Regex[] ColorRegexes;

        static ColorTagger()
        {
            ColorRegexes = new Regex[] { new Regex(@"#[0-9a-f]{6,8}"),
                                         new Regex(@"Colors\.\w+"),
                                         new Regex(@"Brushes\.\w+") };
        }

        public ColorTagger(ITextBuffer buffer)
            : base(buffer, ColorRegexes)
        {}

        protected override IErrorTag TryCreateTagForMatch(Match match)
        {
            if (match.Value.StartsWith("Colors"))
                return new ErrorTag(PredefinedErrorTypeNames.CompilerError);
            else if (match.Value.StartsWith("Brushes"))
                return new ErrorTag(PredefinedErrorTypeNames.SyntaxError);
            else
                return new ErrorTag(PredefinedErrorTypeNames.Warning);
        }
    }
}
