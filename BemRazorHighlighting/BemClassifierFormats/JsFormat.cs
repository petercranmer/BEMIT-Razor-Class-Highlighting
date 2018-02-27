using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace BemRazorHighlighting.BemClassifierFormats
{
    [ClassificationType(ClassificationTypeNames = BemClassifier.BEM_JS_CLASSIFICATION)]
    [Name(BemClassifier.BEM_JS_CLASSIFICATION)]
    internal sealed class JsFormat : BemFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BemClassifierBlockFormat"/> class.
        /// </summary>
        public JsFormat() : base("JS", Colors.Green)
        {
        }
    }
}
