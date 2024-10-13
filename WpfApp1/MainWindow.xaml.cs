using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace WpfApp1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int maxValue;
    private int amountOfTests;
    private int intervalCount;

    private List<ListBox> randoms = new List<ListBox>();
    private List<PlotView> plotViews = new List<PlotView>();
    
    public MainWindow()
    {
        InitializeComponent();
        randoms.Add(list1);
        randoms.Add(list2);
        randoms.Add(list3);
        
        plotViews.Add(plotView1);
        plotViews.Add(plotView2);
        plotViews.Add(plotView3);
    }

    private string ConvertArrayToString(int[] array)
    {
        string result = string.Empty;
        foreach (var val in array)
        {
            result += val.ToString() + " ";
        }
        return result;
    }

    private void GetMaxValueFromTextBox()
    {
        string textBoxValue = MaxValue.Text;
        maxValue = Convert.ToInt32(textBoxValue);
    }

    private void GetTestsAmountFromTextBox()
    {
        amountOfTests = Convert.ToInt32(Tests.Text);
    }
    
    private void GetIntervalsCountFromTextBox()
    {
        intervalCount = Convert.ToInt32(intervalsCount.Text);
    }

    private int[] GenerateRandomArray()
    {
        Random random = new Random();
        int[] valueArray = new int[amountOfTests];
        for(int i = 0; i < amountOfTests; i++)
            valueArray[i] = random.Next(maxValue);
        return valueArray;
    }
    
    private int[] GenerateLinearArray()
    {
        int[] valueArray = new int[amountOfTests];
        LinearRandom rand = new LinearRandom(1345, maxValue);
        for (int i = 0; i < amountOfTests; i++)
        {
            valueArray[i] = rand.Generate();
        }

        return valueArray;
    }

    private int[] GenerateMiddleSquareArray()
    {
        MiddleSquareRandom rand = new MiddleSquareRandom(8234, maxValue);
        int[] valueArray = new int[amountOfTests];
        for (int i = 0; i < valueArray.Length; i++)
        {
            valueArray[i] = rand.Generate();
        }   
        return valueArray;
    }
    
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        GetMaxValueFromTextBox();
        GetTestsAmountFromTextBox();
        GetIntervalsCountFromTextBox();
        
        int[] stdRandomArray = GenerateRandomArray();
        int[] linearRandomArray = GenerateLinearArray();
        int[] middleSquareRandomArray = GenerateMiddleSquareArray();
        
        WriteToListBox(stdRandomArray, 0);
        WriteToListBox(linearRandomArray, 1);
        WriteToListBox(middleSquareRandomArray, 2);
        
        CreateDiagram(CountNumbersInIntervals(stdRandomArray), 0);
        CreateDiagram(CountNumbersInIntervals(linearRandomArray), 1);
        CreateDiagram(CountNumbersInIntervals(middleSquareRandomArray), 2);
    }

    private void WriteToListBox(int[] randomInts, int listboxIndex)
    {
        foreach (var item in randomInts)
            randoms[listboxIndex].Items.Add(item);   
    }

    private int[] CountNumbersInIntervals(int[] numbers)
    {
        double intervalSize = (double)(maxValue - 0)/intervalCount;
        int[] intervals = new int[intervalCount];

        foreach (var number in numbers)
        {
            int intervalIdx = (int)((number - 0) / intervalSize);
            
            intervalIdx = intervalIdx==intervalCount ? --intervalIdx : intervalIdx;
            intervals[intervalIdx]++;
        }
        return intervals;
    }

    private void CreateDiagram(int[] intervals, int plotIndex)
    {
        var plotModel = new PlotModel();
        
        var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom, Key = "y1" };
        var valueAxis1 = new LinearAxis
        {
            Title = "Value Axis 1",
            Position = AxisPosition.Left,
            MinimumPadding = 0.06,
            MaximumPadding = 0.06,
            ExtraGridlines = new[] { 0d },
            Key = "x1"
        };
        
        plotModel.Axes.Add(categoryAxis);
        plotModel.Axes.Add(valueAxis1);

        var barSeries = new BarSeries();
        barSeries.XAxisKey = "x1";
        barSeries.YAxisKey = "y1";
        foreach (var num in intervals)
        {
            barSeries.Items.Add(new BarItem() { Value = num });
        }
        
        plotModel.Series.Add(barSeries);
        plotViews[plotIndex].Model = plotModel;
    }
    
    private static bool IsTextNumeric(string text)
    {
        // Регулярний вираз для перевірки числових значень
        Regex regex = new Regex("[^0-9]+");
        return !regex.IsMatch(text);
    }

    private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsTextNumeric(e.Text);
    }
}