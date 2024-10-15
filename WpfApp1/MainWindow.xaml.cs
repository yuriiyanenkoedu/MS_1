using System.IO;
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
using OfficeOpenXml;

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
        try
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
        catch
        {
            MessageBox.Show("Something went wrong");
        }

    }

    private void WriteToListBox(int[] randomInts, int listboxIndex)
    {
        randoms[listboxIndex].Items.Clear();
        
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

    private void ClearPlotView(int plotIndex)
    {
        plotViews[plotIndex].Model = new PlotModel();
        plotViews[plotIndex].InvalidatePlot(true);
    }
    
    private void CreateDiagram(int[] intervals, int plotIndex)
    {   
        ClearPlotView(plotIndex);
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

    private void ExportToExcel(string fileName)
    {
        using (var package = new ExcelPackage())
        {
            var workSheet = package.Workbook.Worksheets.Add("Sheet1");
            workSheet.Cells[1, 1].Value = "Standart random";
            workSheet.Cells[1, 2].Value = "Linear random";
            workSheet.Cells[1, 3].Value = "Middle square random";
            for(int i = 0; i < amountOfTests; i++)
            {
                workSheet.Cells[i + 2, 1].Value = randoms[0].Items[i];
                workSheet.Cells[i + 2, 2].Value = randoms[1].Items[i];
                workSheet.Cells[i + 2, 3].Value = randoms[2].Items[i];
            }
            workSheet.Cells[1, 4].Value = maxValue;
            workSheet.Cells[1, 5].Value = intervalCount;
            workSheet.Cells[1, 6].Value = amountOfTests;
            FileStream file = new FileStream($"../../../../xlsFiles/{fileName}", FileMode.Append);
            file.Close();
            var fileInfo = new FileInfo($"../../../../xlsFiles/{fileName}");
            package.SaveAs(fileInfo);
        }
    }

    private void ImportFromExcel(string fileName)
    {
        List<int> msRand = new List<int>();
        List<int> stdRand = new List<int>();
        List<int> linRand = new List<int>();

        using (var package = new ExcelPackage(new FileInfo($"../../../../xlsFiles/{fileName}")))
        {
            var workSheet = package.Workbook.Worksheets[0];
            for (int row = 2; row <= workSheet.Dimension.End.Row; row++)
            {
                if(workSheet.Cells[row, 1].Value == null) break;
                
                stdRand.Add(Convert.ToInt32(workSheet.Cells[row, 1].Value));
                linRand.Add(Convert.ToInt32( workSheet.Cells[row, 2].Value));
                msRand.Add(Convert.ToInt32(workSheet.Cells[row, 3].Value));
            }
            
            maxValue = Convert.ToInt32(workSheet.Cells[1, 4].Value);
            intervalCount = Convert.ToInt32(workSheet.Cells[1, 5].Value);
            amountOfTests = Convert.ToInt32(workSheet.Cells[1, 6].Value);
            
            WriteToListBox(stdRand.ToArray(), 0);
            WriteToListBox(linRand.ToArray(), 1);
            WriteToListBox(msRand.ToArray(), 2);
            
            CreateDiagram(CountNumbersInIntervals(stdRand.ToArray()), 0);
            CreateDiagram(CountNumbersInIntervals(linRand.ToArray()), 1);
            CreateDiagram(CountNumbersInIntervals(msRand.ToArray()), 2);
        }
        
    }

    private void Export_OnClick(object sender, RoutedEventArgs e)
    {
        ExportToExcel("test.xlsx");
        MessageBox.Show("Exported to Excel");
    }

    private void Import_OnClick(object sender, RoutedEventArgs e)
    {
        ImportFromExcel("test.xlsx");
    }
}