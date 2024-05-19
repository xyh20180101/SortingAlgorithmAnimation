using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace SortingAlgorithmAnimation;

public class ViewModel : INotifyPropertyChanged
{
    private int _delay = 10;

    private double _elapsedTime;

    private int _externalSpace;

    private int _readCount;

    private int _sampleCount = 16;

    private int _writeCount;

    public int SampleCount
    {
        get => _sampleCount;
        set
        {
            _sampleCount = value < 0 ? 0 : value;
            OnPropertyChanged();
        }
    }

    public int Delay
    {
        get => _delay;
        set
        {
            _delay = value < 0 ? 0 : value;
            OnPropertyChanged();
        }
    }

    public int ReadCount
    {
        get => _readCount;
        set
        {
            _readCount = value;
            OnPropertyChanged();
        }
    }

    public int WriteCount
    {
        get => _writeCount;
        set
        {
            _writeCount = value;
            OnPropertyChanged();
        }
    }

    public int ExternalSpace
    {
        get => _externalSpace;
        set
        {
            _externalSpace = value;
            OnPropertyChanged();
        }
    }

    public double ElapsedTime
    {
        get => _elapsedTime;
        set
        {
            _elapsedTime = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class DoubleToTimeStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return $"{(double)value:0.000}ms";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}