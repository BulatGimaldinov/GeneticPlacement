using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using static GeneticPlacement.FileUtils;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBoxButtons = System.Windows.MessageBoxButton;
using MessageBoxIcon = System.Windows.MessageBoxImage;

namespace GeneticPlacement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static Dictionary<int, (int x, int y)> DictPosition = new(); // словарь позиций элементов
        public static Dictionary<int, (int length, int width)> DictSizes = new(); // словарь размеров элементов
        public static int[][] MatrixConnection = Array.Empty<int[]>(); // Матрица смежности
        private string _pathFolderFiles; // путь к папке с файлами
        public static int NumberOfIterations;
        public static int NumberOfPopulations;
        public static Elements elements = new () // создание элемента
        {
            Dimensions = DictSizes,
            Coordinates = new Dictionary<int, (int x, int y)>()
        };

        private static PrintedCircuitBoard pcb = new (0, 0, DictPosition); // создание платы
        private TaskWindow TW; // окно с размещением
        
        

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button1_OnClick(object sender, RoutedEventArgs e) // кнопка считывания с папки и вывода данных
        {
            try
            {
                DictPosition.Clear();
                DictSizes.Clear();

                var openDialogChoiceFolder = new System.Windows.Forms.FolderBrowserDialog();
                openDialogChoiceFolder.ShowDialog();
                _pathFolderFiles = openDialogChoiceFolder.SelectedPath;
                // Размеры n: a, b
                var pathSizes = _pathFolderFiles+@"\sizes.txt";
                // Размеры плат
                var pathSizesBoard = _pathFolderFiles+@"\board.txt";
                // Матрица соединений NxN
                var pathMatrixConnection = _pathFolderFiles+@"\matrix.txt";
                // Координаты n: x, y
                var pathPositions = _pathFolderFiles+@"\position.txt";

                var sizes = ReadFiles(pathSizes);
                var sizesBoard = ReadFiles(pathSizesBoard);
                var matrix = ReadFiles(pathMatrixConnection);
                MatrixConnection = matrix;
                var position = ReadFiles(pathPositions);

                for (var i = 0; i < matrix.GetLength(0); i++)
                {
                    DictSizes.Add(sizes[i][0], (sizes[i][1], sizes[i][2]));
                    DictPosition.Add(position[i][0], (position[i][1], position[i][2]));
                }
                pcb.Length = sizesBoard[0][0];
                pcb.Width = sizesBoard[1][0];
                
                TextBlock1.Text = $"Размеры платы:\nДлина = {pcb.Length};\nШирина = {pcb.Width}";
                var tb2Text = "Размеры элементов\n(n: длина, ширина)\n";
                var tb3Text = "Координаты посадочных мест:\n(m: x, y)\n";
                var tb4Text = "Матрица смежности:\n";
                tb2Text += DictSizes.Aggregate("", (current, value) => current + $"{value.Key}: ({value.Value.Item1}, {value.Value.Item2})\n");
                tb3Text += DictPosition.Aggregate("", (current, value) => current + $"{value.Key}: [{value.Value.Item1}, {value.Value.Item2}]\n");
                foreach (var i in MatrixConnection)
                {
                    foreach (var j in i)
                    {
                        tb4Text += $"{j} ";
                    }
                    tb4Text += "\n";
                }
                TextBlock2.Text = tb2Text;
                TextBlock3.Text = tb3Text;
                TextBlock4.Text = tb4Text;
                TextBlock5.Text = "";
                TextBlock6.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при считывании файла\n{ex}");
            }
        }
        private void Button2_OnClick(object sender, RoutedEventArgs e) // кнопка запуска алгоритма и вывода решения на экран
        {
            try
            {
                Algorithm.Play(pcb,elements);
                var length = Convert.ToInt32(Population.MinimumTotalLengthOfConnections);
                TextBlock5.Text =$"Суммарная длина соединений = {length.ToString()}";
                var tb5Text = "Итоговое размещение:\nn:(x,y),(длина, ширина)\n";
                tb5Text += elements.Dimensions.Aggregate("", (current, value) => current + $"{value.Key}: ({elements.Coordinates[value.Key].x}, { elements.Coordinates[value.Key].y})," +
                    $" ({value.Value.length},{value.Value.width})\n");
                TextBlock6.Text = tb5Text;
                var taskWindow = new TaskWindow(pcb.Length, pcb.Width)
                {
                    Owner = this
                };
                TW = taskWindow;
                taskWindow.Show();
            }
            catch (Exception exception)
            {
               MessageBox.Show($"Ошибка при загрузке решения\n{exception}");
            }
        }
        private void Button3_OnClick(object sender, RoutedEventArgs e) // кнопка сохранения изображения размещения
        {
            try
            {
                Bitmap savedBit = new Bitmap(TW.pictureBox1.Width,TW.pictureBox1.Height);
                TW.pictureBox1.DrawToBitmap(savedBit, TW.pictureBox1.ClientRectangle);
            
                if (savedBit != null) //если в pictureBox есть изображение
                {
                    //создание диалогового окна "Сохранить как..", для сохранения изображения
                    SaveFileDialog savedialog = new SaveFileDialog();
                    savedialog.Title = "Сохранить картинку как...";
                    //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                    savedialog.OverwritePrompt = true;
                    //отображать ли предупреждение, если пользователь указывает несуществующий путь
                    savedialog.CheckPathExists = true;
                    //список форматов файла, отображаемый в поле "Тип файла"
                    savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                    //отображается ли кнопка "Справка" в диалоговом окне
                    savedialog.ShowHelp = true;
                    if (savedialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                    {
                        try 
                        {
                            savedBit.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        catch
                        {
                            MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Button4_OnClick(object sender, RoutedEventArgs e)// кнопка сохранения решения в файл
        {
            try
            {
                string text = $"{TextBlock5.Text}\n{TextBlock6.Text}";
                if (TextBlock5 != null && TextBlock6 != null) //если в pictureBox есть изображение
                { // камшот
                    //создание диалогового окна "Сохранить как..", для сохранения изображения
                    SaveFileDialog savedialog = new SaveFileDialog();
                    savedialog.Title = "Сохранить файл как...";
                    //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                    savedialog.OverwritePrompt = true;
                    //отображать ли предупреждение, если пользователь указывает несуществующий путь
                    savedialog.CheckPathExists = true;
                    //список форматов файла, отображаемый в поле "Тип файла"
                    savedialog.Filter = "Files(*.txt)|*.txt|Files(*.doc)|*.doc|All files (*.*)|*.*";
                    //отображается ли кнопка "Справка" в диалоговом окне
                    savedialog.ShowHelp = true;
                    if (savedialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                    {
                        try
                        {
                            using (StreamWriter incdate = File.AppendText(savedialog.FileName))
                            {
                                incdate.WriteLine(text, '\n');
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Невозможно сохранить данные", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Невозможно сохранить данные", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
        private void Button5_OnClick(object sender, RoutedEventArgs e) // кнопка выхода
        {
            Application.Current.Shutdown(); 
        }

        private void TextBox1_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void TextBox1_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NumberOfIterations = Convert.ToInt32(TextBox1.Text);
                NumberOfPopulations = Convert.ToInt32(TextBox2.Text);
            }
        }
        
       
    }
}