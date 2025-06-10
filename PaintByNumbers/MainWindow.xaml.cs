using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PaintByNumbers
{
    public class PaletteColor
    {
        public int Number { get; set; }
        public Brush Brush { get; set; }
    }

    public partial class MainWindow : Window
    {
        private const int GridSize = 10;
        private int[,] numberGrid;
        private Rectangle[,] cellRectangles;
        private Dictionary<int, Brush> colorMap;
        private int selectedColorNumber = 1;
        private int coloredCells = 0;
        private int totalCellsToColor = 0;

        public List<PaletteColor> PaletteColors { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeColorMap();
            InitializeNumberGrid();
            DataContext = this;
        }

        private void InitializeColorMap()
        {
            colorMap = new Dictionary<int, Brush>
            {
                { 1, Brushes.Red },
                { 2, Brushes.Blue },
                { 3, Brushes.Green },
                { 4, Brushes.Yellow },
                { 5, Brushes.Purple }
            };

            PaletteColors = colorMap.Select(kv => new PaletteColor
            {
                Number = kv.Key,
                Brush = kv.Value
            }).ToList();

            ColorComboBox.ItemsSource = PaletteColors;
            ColorComboBox.DisplayMemberPath = "Number";
        }

        private void InitializeNumberGrid()
        {

            DrawingGrid.Children.Clear();
            DrawingGrid.RowDefinitions.Clear();
            DrawingGrid.ColumnDefinitions.Clear();

            Random rand = new Random();
            numberGrid = new int[GridSize, GridSize];
            cellRectangles = new Rectangle[GridSize, GridSize];
            coloredCells = 0;
            totalCellsToColor = GridSize * GridSize;

            for (int i = 0; i < GridSize; i++)
            {
                DrawingGrid.RowDefinitions.Add(new RowDefinition());
                DrawingGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    int number = rand.Next(1, 6);
                    numberGrid[row, col] = number;

                    var border = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };

                    var textBlock = new TextBlock
                    {
                        Text = number.ToString(),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    border.Child = textBlock;
                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, col);

                    var rectangle = new Rectangle
                    {
                        Fill = Brushes.White,
                        Opacity = 0
                    };

                    rectangle.MouseDown += Cell_MouseDown;

                    Grid.SetRow(rectangle, row);
                    Grid.SetColumn(rectangle, col);

                    DrawingGrid.Children.Add(border);
                    DrawingGrid.Children.Add(rectangle);

                    cellRectangles[row, col] = rectangle;
                }
            }

            UpdateProgress();
        }

        private void Cell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var rectangle = sender as Rectangle;
            if (rectangle == null) return;

            int row = Grid.GetRow(rectangle);
            int col = Grid.GetColumn(rectangle);

            int cellNumber = numberGrid[row, col];
            if (cellNumber == selectedColorNumber)
            {
                if (rectangle.Fill != colorMap[selectedColorNumber])
                {
                    rectangle.Fill = colorMap[selectedColorNumber];
                    rectangle.Opacity = 1;
                    coloredCells++;
                    UpdateProgress();
                }
            }
        }

        private void DrawingGrid_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(DrawingGrid);
            int row = (int)(position.Y / (DrawingGrid.ActualHeight / GridSize));
            int col = (int)(position.X / (DrawingGrid.ActualWidth / GridSize));

            if (row >= 0 && row < GridSize && col >= 0 && col < GridSize)
            {
                HoverText.Text = $"Сегмент: {numberGrid[row, col]}";
                HoverText.Visibility = Visibility.Visible;
            }
            else
            {
                HoverText.Visibility = Visibility.Collapsed;
            }
        }

        private void DrawingGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            HoverText.Visibility = Visibility.Collapsed;
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorComboBox.SelectedItem is PaletteColor selected)
            {
                selectedColorNumber = selected.Number;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeNumberGrid();
        }

        private void UpdateProgress()
        {
            double progress = (double)coloredCells / totalCellsToColor * 100;
            ProgressText.Text = $"Прогресс: {progress:F0}%";
        }
    }
}