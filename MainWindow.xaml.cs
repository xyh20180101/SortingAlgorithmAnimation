using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SortingAlgorithmAnimation.SortingAlgorithms;

namespace SortingAlgorithmAnimation;

public partial class MainWindow : Window
{
    /// <summary>
    /// 事件列表
    /// </summary>
    private readonly List<RenderEventArgs> _events = new();

    private readonly ViewModel _viewModel;

    /// <summary>
    /// 当前的排序方法
    /// </summary>
    private Action<ListeningList<int>>? _currentSort;

    /// <summary>
    /// 用于排序的数据
    /// </summary>
    private ListeningList<int> _list = new();

    public MainWindow()
    {
        InitializeComponent();

        DataContext = _viewModel = new ViewModel();

        InitComboBox();
    }

    private void InitComboBox()
    {
        //使用反射自动列出排序算法
        var methods = typeof(SortingAlgorithms.SortingAlgorithms).GetMethods();
        foreach (var method in methods)
            if (method.IsPublic && method.Name.Contains("Sort"))
                MyComboBox.Items.Add(method.Name);
        MyComboBox.SelectedIndex = 0;
    }

    private void MyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var methodInfo = typeof(SortingAlgorithms.SortingAlgorithms).GetMethods()
            .FirstOrDefault(p => p.Name == (string)MyComboBox.SelectedValue)!;

        _currentSort =
            (Action<ListeningList<int>>?)Delegate.CreateDelegate(typeof(Action<ListeningList<int>>), methodInfo);
    }

    private void RunButton_Click(object sender, RoutedEventArgs @event)
    {
        if (IsSorted(_list)) return;

        _list.ReadEvent += ReadHandler;
        _list.WriteEvent += WriteHandler;
        _list.SwapEvent += SwapHandler;
        _list.ExternalSpaceEvent += ExternalSpaceHandler;

        _events.Clear();

        var sw = new Stopwatch();
        sw.Start();
        _currentSort?.Invoke(_list);
        sw.Stop();
        var usedTime = sw.ElapsedTicks * 1000d / Stopwatch.Frequency;

        _list.ReadEvent -= ReadHandler;
        _list.WriteEvent -= WriteHandler;
        _list.SwapEvent -= SwapHandler;
        _list.ExternalSpaceEvent -= ExternalSpaceHandler;

        Task.Run(async () =>
        {
            var totalCount = _events.Count;
            for (var i = 0; i < _events.Count; i++)
            {
                var e = _events[i];
                await Dispatcher.Invoke(async () =>
                {
                    switch (e.EventType)
                    {
                        case EventType.Read:
                            await ReadEventHandler(e.Params[0], e.Params[1]);
                            break;
                        case EventType.Write:
                            await WriteEventHandler(e.Params[0], e.Params[1], e.Params[2]);
                            break;
                        case EventType.Swap:
                            await SwapEventHandler(e.Params[0], e.Params[1], e.Params[2], e.Params[3],
                                e.Params[4], e.Params[5]);
                            break;
                        case EventType.ExternalSpace:
                            ExternalSpaceEventHandler(e.Params[0]);
                            break;
                    }

                    _viewModel.ElapsedTime = usedTime * ((double)i / totalCount);
                });
            }

            Dispatcher.Invoke(MyCanvas.InvalidateVisual);
        });
    }

    private void StopButton_OnClick(object sender, RoutedEventArgs e)
    {
        _events.Clear();
    }

    private static bool IsSorted<T>(IList<T> list) where T : IComparable<T>
    {
        for (var i = 0; i < list.Count - 1; i++)
            if (list[i].CompareTo(list[i + 1]) > 0)
                return false;
        return true;
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        WinApi.TimeBeginPeriod(1);
    }

    private void MainWindow_OnClosed(object? sender, EventArgs e)
    {
        WinApi.TimeEndPeriod(1);
    }

    #region 初始化排序集合

    private void ShuffleButton_Click(object sender, RoutedEventArgs e)
    {
        var array = Enumerable.Range(1, _viewModel.SampleCount).ToArray();
        new Random().Shuffle(array);
        _list = new ListeningList<int>(array.ToList());

        FirstDraw();
        _viewModel.ReadCount = _viewModel.WriteCount = 0;
        _viewModel.ElapsedTime = 0;
        _viewModel.ExternalSpace = 0;
    }

    private void ReverseButton_OnClick(object sender, RoutedEventArgs e)
    {
        var list = Enumerable.Range(1, _viewModel.SampleCount).Reverse().ToList();
        _list = new ListeningList<int>(list);

        FirstDraw();
        _viewModel.ReadCount = _viewModel.WriteCount = 0;
        _viewModel.ElapsedTime = 0;
        _viewModel.ExternalSpace = 0;
    }

    private void FirstDraw()
    {
        MyCanvas.RectList.Clear();
        MyCanvas.BrushList.Clear();

        var rectWidth = MyCanvas.ActualWidth / _list.Count;

        for (var i = 0; i < _list.Count; i++)
        {
            var width = Math.Ceiling(rectWidth);
            var height = MyCanvas.ActualHeight / _list.Count * _list[i];

            var x = rectWidth * i;
            var y = MyCanvas.ActualHeight - height;

            MyCanvas.RectList.Add(new Rect(x, y, width, height));
            MyCanvas.BrushList.Add(Brushes.White);
        }

        MyCanvas.InvalidateVisual();
    }

    #endregion

    #region 事件处理

    /*
     * xxxHandler 直接嵌入排序算法中执行,只负责向事件列表里插入事件
     * xxxEventHandler 在排序完毕后才从事件列表里取出事件执行
     * 详见RunButton_Click方法
     */

    private void ReadHandler(int index, int value)
    {
        _events.Add(new RenderEventArgs(EventType.Read, index, value));
    }

    private void WriteHandler(int index, int oldValue, int newValue)
    {
        _events.Add(new RenderEventArgs(EventType.Write, index, oldValue, newValue));
    }

    private void SwapHandler(int index1, int index2, int oldValue1, int oldValue2, int newValue1, int newValue2)
    {
        _events.Add(new RenderEventArgs(EventType.Swap, index1, index2, oldValue1, oldValue2, newValue1, newValue2));
    }

    private void ExternalSpaceHandler(int count)
    {
        _events.Add(new RenderEventArgs(EventType.ExternalSpace, count));
    }

    private async Task ReadEventHandler(int index, int value)
    {
        _viewModel.ReadCount++;

        MyCanvas.BrushList[index] = Brushes.Green;
        MyCanvas.InvalidateVisual();

        await Task.Delay(_viewModel.Delay);

        MyCanvas.BrushList[index] = Brushes.White;
    }

    private async Task WriteEventHandler(int index, int oldValue, int newValue)
    {
        _viewModel.WriteCount++;

        if (oldValue != newValue)
        {
            var rect = MyCanvas.RectList[index];

            rect.Height = MyCanvas.ActualHeight / _list.Count * newValue;
            rect.Y = MyCanvas.ActualHeight - rect.Height;

            MyCanvas.RectList[index] = rect;
        }

        MyCanvas.BrushList[index] = Brushes.Red;
        MyCanvas.InvalidateVisual();

        await Task.Delay(_viewModel.Delay);

        MyCanvas.BrushList[index] = Brushes.White;
    }

    private async Task SwapEventHandler(int index1, int index2, int oldValue1, int oldValue2, int newValue1,
        int newValue2)
    {
        _viewModel.ReadCount += 2;
        _viewModel.WriteCount += 2;

        var rect1 = MyCanvas.RectList[index1];
        var rect2 = MyCanvas.RectList[index2];

        rect1.Height = MyCanvas.ActualHeight / _list.Count * newValue1;
        rect2.Height = MyCanvas.ActualHeight / _list.Count * newValue2;

        rect1.Y = MyCanvas.ActualHeight - rect1.Height;
        rect2.Y = MyCanvas.ActualHeight - rect2.Height;

        MyCanvas.RectList[index1] = rect1;
        MyCanvas.RectList[index2] = rect2;

        MyCanvas.BrushList[index1] = Brushes.Blue;
        MyCanvas.BrushList[index2] = Brushes.Blue;
        MyCanvas.InvalidateVisual();

        await Task.Delay(_viewModel.Delay);

        MyCanvas.BrushList[index1] = Brushes.White;
        MyCanvas.BrushList[index2] = Brushes.White;
    }

    private void ExternalSpaceEventHandler(int count)
    {
        _viewModel.ExternalSpace += count;
    }

    #endregion
}

public enum EventType
{
    Read,
    Write,
    Swap,
    ExternalSpace
}

public class RenderEventArgs
{
    public RenderEventArgs(EventType eventType, params int[] param)
    {
        EventType = eventType;
        Params = param;
    }

    public EventType EventType { get; set; }

    public int[] Params { get; set; }
}

public static partial class WinApi
{
    [SuppressUnmanagedCodeSecurity]
    [LibraryImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
    public static partial uint TimeBeginPeriod(uint uMilliseconds);

    [SuppressUnmanagedCodeSecurity]
    [LibraryImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
    public static partial uint TimeEndPeriod(uint uMilliseconds);
}