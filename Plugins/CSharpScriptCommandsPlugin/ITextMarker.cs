using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CSharpScriptCommandsPlugin
{
    public interface ITextMarker
    {
        int StartOffset { get; }

        int EndOffset { get; }

        int Length { get; }

        void Delete();

        bool IsDeleted { get; }

        event EventHandler Deleted;

        Color? BackgroundColor { get; set; }

        Color? ForegroundColor { get; set; }

        FontWeight? FontWeight { get; set; }

        FontStyle? FontStyle { get; set; }

        TextMarkerTypes MarkerTypes { get; set; }

        Color MarkerColor { get; set; }

        object Tag { get; set; }

        object ToolTip { get; set; }
    }

    [Flags]
    public enum TextMarkerTypes
    {
        None = 0x0000,
        SquigglyUnderline = 0x001,
        NormalUnderline = 0x002,
        DottedUnderline = 0x004,
        LineInScrollBar = 0x0100,
        ScrollBarRightTriangle = 0x0400,
        ScrollBarLeftTriangle = 0x0800,
        CircleInScrollBar = 0x1000
    }

    public interface ITextMarkerService
    {
        ITextMarker Create(int startOffset, int length);

        IEnumerable<ITextMarker> TextMarkers { get; }
        
        void Remove(ITextMarker marker);

        void RemoveAll(Predicate<ITextMarker> predicate);

        IEnumerable<ITextMarker> GetMarkersAtOffset(int offset);
    }
}
