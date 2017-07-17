using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace CSharpScriptCommandsPlugin
{
    public sealed class TextMarkerService : DocumentColorizingTransformer, IBackgroundRenderer, ITextMarkerService, ITextViewConnect
    {
        TextSegmentCollection<TextMarker> markers;
        TextDocument document;

        public TextMarkerService(TextDocument document)
        {
            this.document = document ?? throw new ArgumentNullException("document");
            this.markers = new TextSegmentCollection<TextMarker>(document);
        }

        #region ITextMarkerService
        public ITextMarker Create(int startOffset, int length)
        {
            if (this.markers == null)
            {
                throw new InvalidOperationException("Cannot create a marker when not attached to a document");
            }

            var textLength = this.document.TextLength;
            if (startOffset < 0 || startOffset > textLength)
            {
                throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between 0 and " + textLength);
            }

            if (length < 0 || startOffset + length > textLength)
            {
                throw new ArgumentOutOfRangeException("length", length, "length must not be negative and startOffset+length must not be after the end of the document");
            }

            var m = new TextMarker(this, startOffset, length);
            this.markers.Add(m);

            return m;
        }

        public IEnumerable<ITextMarker> GetMarkersAtOffset(int offset)
        {
            if (this.markers == null)
            {
                return Enumerable.Empty<ITextMarker>();
            }
            else
            {
                return this.markers.FindSegmentsContaining(offset);
            }
        }

        public IEnumerable<ITextMarker> TextMarkers
        {
            get { return this.markers ?? Enumerable.Empty<ITextMarker>(); }
        }

        public void RemoveAll(Predicate<ITextMarker> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            if (this.markers != null)
            {
                foreach (var m in this.markers.ToArray())
                {
                    if (predicate(m))
                    {
                        Remove(m);
                    }
                }
            }
        }

        public void Remove(ITextMarker marker)
        {
            if (marker == null)
            {
                throw new ArgumentNullException("marker");
            }

            var m = marker as TextMarker;
            if (this.markers != null && this.markers.Remove(m))
            {
                Redraw(m);
                m.OnDeleted();
            }
        }
        
        internal void Redraw(ISegment segment)
        {
            foreach (var view in this.textViews)
            {
                view.Redraw(segment, DispatcherPriority.Normal);
            }
            RedrawRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler RedrawRequested;
        #endregion

        #region DocumentColorizingTransformer
        protected override void ColorizeLine(DocumentLine line)
        {
            if (this.markers == null)
            {
                return;
            }

            var lineStart = line.Offset;
            var lineEnd = lineStart + line.Length;
            foreach (var marker in this.markers.FindOverlappingSegments(lineStart, line.Length))
            {
                Brush foregroundBrush = null;
                if (marker.ForegroundColor != null)
                {
                    foregroundBrush = new SolidColorBrush(marker.ForegroundColor.Value);
                    foregroundBrush.Freeze();
                }
                ChangeLinePart(
                    Math.Max(marker.StartOffset, lineStart),
                    Math.Min(marker.EndOffset, lineEnd),
                    element => {
                        if (foregroundBrush != null)
                        {
                            element.TextRunProperties.SetForegroundBrush(foregroundBrush);
                        }
                        var tf = element.TextRunProperties.Typeface;
                        element.TextRunProperties.SetTypeface(new Typeface(
                            tf.FontFamily,
                            marker.FontStyle ?? tf.Style,
                            marker.FontWeight ?? tf.Weight,
                            tf.Stretch
                        ));
                    }
                );
            }
        }
        #endregion

        #region IBackgroundRenderer
        public KnownLayer Layer
        {
            get
            {
                return KnownLayer.Selection;
            }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (textView == null)
            {
                throw new ArgumentNullException("textView");
            }

            if (drawingContext == null)
            {
                throw new ArgumentNullException("drawingContext");
            }

            if (this.markers == null || !textView.VisualLinesValid)
            {
                return;
            }

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
            {
                return;
            }

            var viewStart = visualLines.First().FirstDocumentLine.Offset;
            var viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
            foreach (var marker in this.markers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
            {
                if (marker.BackgroundColor != null)
                {
                    var geoBuilder = new BackgroundGeometryBuilder()
                    {
                        AlignToWholePixels = true,
                        CornerRadius = 3
                    };
                    geoBuilder.AddSegment(textView, marker);
                    var geometry = geoBuilder.CreateGeometry();
                    if (geometry != null)
                    {
                        var color = marker.BackgroundColor.Value;
                        var brush = new SolidColorBrush(color);
                        brush.Freeze();
                        drawingContext.DrawGeometry(brush, null, geometry);
                    }
                }
                var underlineMarkerTypes = TextMarkerTypes.SquigglyUnderline | TextMarkerTypes.NormalUnderline | TextMarkerTypes.DottedUnderline;
                if ((marker.MarkerTypes & underlineMarkerTypes) != 0)
                {
                    foreach (var r in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker))
                    {
                        var startPoint = r.BottomLeft;
                        var endPoint = r.BottomRight;

                        Brush usedBrush = new SolidColorBrush(marker.MarkerColor);
                        usedBrush.Freeze();
                        if ((marker.MarkerTypes & TextMarkerTypes.SquigglyUnderline) != 0)
                        {
                            var offset = 2.5;

                            var count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);

                            var geometry = new StreamGeometry();

                            using (var ctx = geometry.Open())
                            {
                                ctx.BeginFigure(startPoint, false, false);
                                ctx.PolyLineTo(CreatePoints(startPoint, endPoint, offset, count).ToArray(), true, false);
                            }

                            geometry.Freeze();

                            var usedPen = new Pen(usedBrush, 1);
                            usedPen.Freeze();
                            drawingContext.DrawGeometry(Brushes.Transparent, usedPen, geometry);
                        }
                        if ((marker.MarkerTypes & TextMarkerTypes.NormalUnderline) != 0)
                        {
                            var usedPen = new Pen(usedBrush, 1);
                            usedPen.Freeze();
                            drawingContext.DrawLine(usedPen, startPoint, endPoint);
                        }
                        if ((marker.MarkerTypes & TextMarkerTypes.DottedUnderline) != 0)
                        {
                            var usedPen = new Pen(usedBrush, 1)
                            {
                                DashStyle = DashStyles.Dot
                            };
                            usedPen.Freeze();
                            drawingContext.DrawLine(usedPen, startPoint, endPoint);
                        }
                    }
                }
            }
        }

        IEnumerable<Point> CreatePoints(Point start, Point end, double offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Point(start.X + i * offset, start.Y - ((i + 1) % 2 == 0 ? offset : 0));
            }
        }
        #endregion

        #region ITextViewConnect
        readonly List<TextView> textViews = new List<TextView>();

        void ITextViewConnect.AddToTextView(TextView textView)
        {
            if (textView != null && !this.textViews.Contains(textView))
            {
                Debug.Assert(textView.Document == this.document);
                this.textViews.Add(textView);
            }
        }

        void ITextViewConnect.RemoveFromTextView(TextView textView)
        {
            if (textView != null)
            {
                Debug.Assert(textView.Document == this.document);
                this.textViews.Remove(textView);
            }
        }
        #endregion
    }

    public sealed class TextMarker : TextSegment, ITextMarker
    {
        readonly TextMarkerService service;

        public TextMarker(TextMarkerService service, int startOffset, int length)
        {
            this.service = service ?? throw new ArgumentNullException("service");
            this.StartOffset = startOffset;
            this.Length = length;
            this.markerTypes = TextMarkerTypes.None;
        }

        public event EventHandler Deleted;

        public bool IsDeleted
        {
            get { return !this.IsConnectedToCollection; }
        }

        public void Delete()
        {
            this.service.Remove(this);
        }

        internal void OnDeleted()
        {
            Deleted?.Invoke(this, EventArgs.Empty);
        }

        void Redraw()
        {
            this.service.Redraw(this);
        }

        Color? backgroundColor;

        public Color? BackgroundColor
        {
            get { return this.backgroundColor; }
            set
            {
                if (this.backgroundColor != value)
                {
                    this.backgroundColor = value;
                    Redraw();
                }
            }
        }

        Color? foregroundColor;

        public Color? ForegroundColor
        {
            get { return this.foregroundColor; }
            set
            {
                if (this.foregroundColor != value)
                {
                    this.foregroundColor = value;
                    Redraw();
                }
            }
        }

        FontWeight? fontWeight;

        public FontWeight? FontWeight
        {
            get { return this.fontWeight; }
            set
            {
                if (this.fontWeight != value)
                {
                    this.fontWeight = value;
                    Redraw();
                }
            }
        }

        FontStyle? fontStyle;

        public FontStyle? FontStyle
        {
            get { return this.fontStyle; }
            set
            {
                if (this.fontStyle != value)
                {
                    this.fontStyle = value;
                    Redraw();
                }
            }
        }

        public object Tag { get; set; }

        TextMarkerTypes markerTypes;

        public TextMarkerTypes MarkerTypes
        {
            get { return this.markerTypes; }
            set
            {
                if (this.markerTypes != value)
                {
                    this.markerTypes = value;
                    Redraw();
                }
            }
        }

        Color markerColor;

        public Color MarkerColor
        {
            get { return this.markerColor; }
            set
            {
                if (this.markerColor != value)
                {
                    this.markerColor = value;
                    Redraw();
                }
            }
        }

        public object ToolTip { get; set; }
    }
}
