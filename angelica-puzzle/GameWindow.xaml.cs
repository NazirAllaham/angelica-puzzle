using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace angelica_puzzle
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window, IGameWindow
    {
        int[] dr = { 1,-1, 0, 0 };
        int[] dc = { 0, 0, 1,-1 };

        Window previous;
        Game gameObject;
        Button[,] buttons;
        int rows;
        int cols;

        DispatcherTimer timer;
        int ticks=0;

        public GameWindow()
        {
            InitializeComponent();

            this.timer = new DispatcherTimer();
            this.timer.Interval = new TimeSpan(0, 0, 1);
            this.timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.time.Content = TimeSpan.FromSeconds(++ticks).ToString("mm':'ss");
            ticks++;
        }

        public GameWindow(Window previous) : this()
        {
            this.previous = previous;

            try
            {
                gameObject = new Game(Settings.GetPlayer(), Settings.GetGameRows(), Settings.GetGameColumns(), Settings.GetGamePattern(), this);

                rows = Settings.GetGameRows();
                cols = Settings.GetGameColumns();

                this.state.ColumnDefinitions.Clear();
                this.state.RowDefinitions.Clear();
                for (int i = 0; i < cols; i++)
                {
                    this.state.ColumnDefinitions.Add(new ColumnDefinition());
                }
                for (int i = 0; i < rows; i++)
                {
                    RowDefinition r = new RowDefinition();
                    r.Height = new GridLength(100);
                    this.state.RowDefinitions.Add(r);
                }
                this.state.Width = 100 * cols;

                Render();
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            } finally
            {
                if (this.gameObject == null)
                    this.Close();

                this.timer.Start();
            }
        }

        private void Render()
        {
            this.state.Visibility = Visibility.Hidden;
            string pattern = gameObject.GetPattern();

            buttons = new Button[rows, cols];
            this.state.Children.Clear();

            for (int i = 0, k = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++, k++)
                {
                    buttons[i, j] = new Button();
                    buttons[i, j].Content = pattern[k];
                    buttons[i, j].SetValue(Grid.RowProperty, i);
                    buttons[i, j].SetValue(Grid.ColumnProperty, j);
                    if (pattern[k] == '.')
                    {
                        buttons[i, j].Click += OnBlackCellClicked;
                    }else
                    {
                        buttons[i, j].Click += OnCellClicked;
                    }

                    this.state.Children.Add(buttons[i, j]);
                }
            }
            this.state.Visibility = Visibility.Visible;
        }

        int black_cell_row = -1;
        int black_cell_col = -1;
        bool black_cell_clicked = false;

        private void OnCellClicked(object sender, EventArgs e)
        {
            if(black_cell_col == -1 || black_cell_row == -1 || !black_cell_clicked)
            {
                return;
            }

            Button button = sender as Button;
            int btn_row = int.Parse(button.GetValue(Grid.RowProperty).ToString());
            int btn_col = int.Parse(button.GetValue(Grid.ColumnProperty).ToString());

            if (Math.Abs(btn_row - black_cell_row) + Math.Abs(btn_col - black_cell_col) != 1)
                return;

            UnMarkBounds();
            this.gameObject.DoMove(btn_row - black_cell_row, btn_col - black_cell_col);

            Button tmp = buttons[black_cell_row, black_cell_col];
            buttons[black_cell_row, black_cell_col] = buttons[btn_row, btn_col];
            buttons[btn_row, btn_col] = tmp;

            black_cell_clicked = false;
            Console.WriteLine("Swapping Done");
        }

        private void OnBlackCellClicked(object sender, EventArgs e)
        {
            Button button = sender as Button;
            black_cell_clicked = true;
            black_cell_row = int.Parse(button.GetValue(Grid.RowProperty).ToString());
            black_cell_col = int.Parse(button.GetValue(Grid.ColumnProperty).ToString());
            MarkBounds();
        }

        private void MarkBounds()
        {
            Console.WriteLine("Row " + black_cell_row + " Col " + black_cell_col);
            for (int i=0; i<4; i++)
            {
                int r = black_cell_row + dr[i],
                    c = black_cell_col + dc[i];
                
                if(c >= 0 && r >= 0 && c < this.cols && r < this.rows)
                {
                    this.state.Children[c + r * this.cols].SetValue(BackgroundProperty, new SolidColorBrush(Color.FromRgb(188,188,220)));
                }
            }
        }

        private void UnMarkBounds()
        {
            for (int i = 0; i < 4; i++)
            {
                int r = black_cell_row + dr[i],
                    c = black_cell_col + dc[i];

                if (c >= 0 && r >= 0 && c < this.cols && r < this.rows)
                {
                    this.state.Children[c + r * this.cols].SetValue(BackgroundProperty, new SolidColorBrush(Color.FromRgb(220, 220, 220)));
                }
            }
        }

        public void Finish()
        {
            
            MessageBox.Show("GAME IS FINISHED AND ANGELICA PUZZLE IS SOLVED AND NAZIR ALLAHAM IS THE KING !!, YOU PASSES THIS MESSION USING " + this.gameObject.GetStepsCount() + " MOVES DURING " + this.ticks + " SECOND(S)");
            this.Close();
        }

        public void Update(State currentState)
        {
            Render();
            this.stepsCount.Content = this.gameObject.GetStepsCount().ToString();
            Console.WriteLine("Updated!");
        }

        public void Update(Stack<State> track)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, e) =>
            {
                GameWindow gameWindow = (GameWindow)e.Argument;
                while (track.Count > 0)
                {
                    gameWindow.Dispatcher.Invoke(new Action(delegate () {
                        gameWindow.gameObject.SetCurrent(track.Pop());
                        gameWindow.gameObject.IncreaseStepsCount(1);
                        Render();
                        gameWindow.stepsCount.Content = gameWindow.gameObject.GetStepsCount().ToString();
                        Console.WriteLine("Updated!");
                    }), DispatcherPriority.Normal);

                    Thread.Sleep(2);
                }
            };
            worker.RunWorkerAsync(this);
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            App.Current.MainWindow = this.previous;
            this.previous.Show();
        }

        public void GetDfsHint(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("DFS");
            Dfs.Calculate(gameObject.GetCurrent(), gameObject.GetFinal(), this);
        }

        public void GetBfsHint(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("BFS");
            Bfs.Calculate(this.gameObject.GetCurrent(), this);
        }

        public void GetDijkstraHint(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Dijkstra");
            Dijkstra.Calculate(gameObject.GetCurrent(), this);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                if (this.Pause.Visibility == Visibility.Collapsed)
                {
                    this.Pause.Visibility = Visibility.Visible;
                    this.timer.Stop();
                }else
                {
                    this.Pause.Visibility = Visibility.Collapsed;
                    this.timer.Start();
                }
            }
        }
    }
}
