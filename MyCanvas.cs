using System.Windows;
using System.Windows.Media;

namespace SortingAlgorithmAnimation;

public class MyCanvas : FrameworkElement
{
    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        nameof(Background), typeof(Brush), typeof(MyCanvas), new PropertyMetadata(default(Brush)));

    private readonly DrawingVisual _visual = new();
    private List<Brush> _brushList = new();
    private List<Rect> _rectList = new();

    public MyCanvas()
    {
        AddVisualChild(_visual);
    }

    public Brush Background
    {
        get => (Brush)GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public List<Brush> BrushList
    {
        get => _brushList;
        set
        {
            _brushList = value;
            InvalidateVisual();
        }
    }

    public List<Rect> RectList
    {
        get => _rectList;
        set
        {
            _rectList = value;
            InvalidateVisual();
        }
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int index)
    {
        return index switch
        {
            0 => _visual,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);

        var widthRate = sizeInfo.NewSize.Width / sizeInfo.PreviousSize.Width;
        var heightRate = sizeInfo.NewSize.Height / sizeInfo.PreviousSize.Height;

        for (var i = 0; i < _rectList.Count; i++)
        {
            var width = _rectList[i].Width * widthRate;
            var height = _rectList[i].Height * heightRate;

            var x = _rectList[i].X * widthRate;
            var y = _rectList[i].Y * heightRate;

            _rectList[i] = new Rect(x, y, width, height);
        }

        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        using var dc = _visual.RenderOpen();
        dc.DrawRectangle(Background, null, new Rect(RenderSize));

        for (var i = 0; i < RectList.Count; i++)
        {
            var rect = RectList[i];
            var brush = BrushList[i];
            dc.DrawRectangle(brush, null, rect);
        }
    }
}