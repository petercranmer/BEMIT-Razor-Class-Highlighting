using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace BemRazorHighlighting
{
    /// <summary>
    /// <see cref="BemHighlightAdornment"/> places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class BemHighlightAdornment
    {
        private const string CLASS_INSTANCE_REGEX = @"[A-z_-]+";
        private const string CLASS_DEFINITION_REGEX = @"class\s?=\s?""(?>"+ CLASS_INSTANCE_REGEX + @"\s?)*""";        

        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView view;

        /// <summary>
        /// Adornment brush.
        /// </summary>
        private readonly Brush blockBrush, elementBrush, modifierBrush, qaBrush, jsBrush;

        /// Initializes a new instance of the <see cref="BemHighlightAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public BemHighlightAdornment(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            this.layer = view.GetAdornmentLayer(nameof(BemHighlightAdornment));

            this.view = view;
            this.view.LayoutChanged += this.OnLayoutChanged;

            byte transparencyVal = 0x20;

            this.blockBrush = new SolidColorBrush(Color.FromArgb(transparencyVal, 0x00, 0x00, 0xff));
            this.blockBrush.Freeze();

            this.elementBrush = new SolidColorBrush(Color.FromArgb(transparencyVal, 0xff, 0x00, 0x00));
            this.elementBrush.Freeze();

            this.modifierBrush = new SolidColorBrush(Color.FromArgb(transparencyVal, 0x00, 0xff, 0x00));
            this.modifierBrush.Freeze();

            this.qaBrush = new SolidColorBrush(Color.FromArgb(transparencyVal, 0x99, 0x99, 0x00));
            this.qaBrush.Freeze();

            this.jsBrush = new SolidColorBrush(Color.FromArgb(transparencyVal, 0x00, 0x99, 0x99));
            this.jsBrush.Freeze();
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            var identityMappingKey = "IdentityMapping";

            if (!this.view.TextBuffer.Properties.ContainsProperty(identityMappingKey))
            {
                return;
            }

            var identity = this.view.TextBuffer.Properties.GetProperty(identityMappingKey) as ITextBuffer;

            if (identity == null || identity.ContentType.TypeName != "RazorCSharp")
            {
                return;
            }

            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                this.CreateVisuals(line);
            }
        }

        /// <summary>
        /// Adds the scarlet box behind the 'a' characters within the given line
        /// </summary>
        /// <param name="line">Line to add the adornments</param>
        private void CreateVisuals(ITextViewLine line)
        {
            IWpfTextViewLineCollection textViewLines = this.view.TextViewLines;

            var lineContent = line.Snapshot.GetText(line.Start, line.Length);

            var classDefinitionMatches = Regex.Matches(
                lineContent,
                CLASS_DEFINITION_REGEX
            );

            foreach(Match classDefinitionMatch in classDefinitionMatches)
            {
                int startOfClassDeclaration = line.Start.Position + lineContent.IndexOf(classDefinitionMatch.Value);

                var classInstancesMatches = Regex.Matches(classDefinitionMatch.Value, CLASS_INSTANCE_REGEX);

                foreach(Match classInstanceMatch in classInstancesMatches)
                {
                    var classInstance = classInstanceMatch.Value;

                    if (classInstance == "class")
                    {
                        continue;
                    }

                    int startOfInstanceRelativeToDefinition = classInstanceMatch.Index;

                    int startOfClassInstance = startOfClassDeclaration + startOfInstanceRelativeToDefinition;

                    var textBounds = Span.FromBounds(startOfClassInstance, startOfClassInstance + classInstance.Length);

                    this.HighlightClassInstance(textViewLines, classInstance, textBounds);
                }
            }

            //// Loop through each character, and place a box around any 'a'
            //for (int charIndex = line.Start; charIndex < line.End; charIndex++)
            //{
            //    if (this.view.TextSnapshot[charIndex] == 'a')
            //    {
            //        SnapshotSpan span = new SnapshotSpan(this.view.TextSnapshot, Span.FromBounds(charIndex, charIndex + 1));
            //        Geometry geometry = textViewLines.GetMarkerGeometry(span);
            //        if (geometry != null)
            //        {
            //            var drawing = new GeometryDrawing(this.brush, this.pen, geometry);
            //            drawing.Freeze();

            //            var drawingImage = new DrawingImage(drawing);
            //            drawingImage.Freeze();

            //            var image = new Image
            //            {
            //                Source = drawingImage,
            //            };

            //            // Align the image with the top of the bounds of the text geometry
            //            Canvas.SetLeft(image, geometry.Bounds.Left);
            //            Canvas.SetTop(image, geometry.Bounds.Top);

            //            this.layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
            //        }
            //    }
            //}
        }

        private void HighlightClassInstance(IWpfTextViewLineCollection textViewLines, string className, Span lineBounds)
        {
            SnapshotSpan span = new SnapshotSpan(this.view.TextSnapshot, lineBounds);
            Geometry geometry = textViewLines.GetMarkerGeometry(span);

            if (geometry != null)
            {
                var brush = this.GetBrushForClassName(className);

                var pen = new Pen(brush, 0.5);

                var drawing = new GeometryDrawing(brush, pen, geometry);
                drawing.Freeze();

                var drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();

                var image = new Image
                {
                    Source = drawingImage,
                };

                // Align the image with the top of the bounds of the text geometry
                Canvas.SetLeft(image, geometry.Bounds.Left);
                Canvas.SetTop(image, geometry.Bounds.Top);

                this.layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
            }
        }

        private Brush GetBrushForClassName(string className)
        {
            if (className.StartsWith("js-"))
            {
                return this.jsBrush;
            }
            else if (className.StartsWith("qa-"))
            {
                return this.qaBrush;
            }
            else if (className.Contains("--"))
            {
                return this.modifierBrush;
            }
            else if (className.Contains("__"))
            {
                return this.elementBrush;
            }
            else
            {
                return this.blockBrush;
            }
        }
    }
}
