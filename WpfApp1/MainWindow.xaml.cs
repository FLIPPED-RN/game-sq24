using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SquashGame
{
    public partial class MainWindow : Window
    {
        private int record = 0;
        private double ballSpeedX = 4;
        private double ballSpeedY = 4;
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private TcpClient client;
        private TcpListener server;
        private NetworkStream stream;
        private bool isServer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
            this.KeyDown += MainWindow_KeyDown;

            // Выбор режима (сервер или клиент)
            MessageBoxResult result = MessageBox.Show("Start as server?", "Game Setup", MessageBoxButton.YesNo);
            isServer = (result == MessageBoxResult.Yes);

            if (isServer)
            {
                StartServer();
            }
            else
            {
                StartClient();
            }
        }

        private void InitializeGame()
        {
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
        }

        private async void StartServer()
        {
            server = new TcpListener(System.Net.IPAddress.Any, 12345);
            server.Start();
            client = await server.AcceptTcpClientAsync();
            stream = client.GetStream();
            ReceiveData();
        }

        private async void StartClient()
        {
            client = new TcpClient();
            await client.ConnectAsync("192.168.1.2", 12345);
            //Для сервера остается неизменным "192.168.1.2", для клиента, на том же пк что и сервер "127.0.0.1". То есть для сервера пишем 192... для клиента 127...
            //Если подключаем по локальной сети с 2-х пк то берем только ip пк с сервером для клиента и сервера.
            stream = client.GetStream();
            ReceiveData();
        }

        private async void ReceiveData()
        {
            byte[] buffer = new byte[256];
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string[] parts = data.Split(',');

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (parts[0] == "P1")
                        {
                            double newLeft = double.Parse(parts[1]);
                            Canvas.SetLeft(player1Paddle, newLeft);
                        }
                        else if (parts[0] == "P2")
                        {
                            double newLeft = double.Parse(parts[1]);
                            Canvas.SetLeft(player2Paddle, newLeft);
                        }
                        else if (parts[0] == "BALL")
                        {
                            double newBallX = double.Parse(parts[1]);
                            double newBallY = double.Parse(parts[2]);
                            Canvas.SetLeft(ball, newBallX);
                            Canvas.SetTop(ball, newBallY);
                        }
                        else if (parts[0] == "START")
                        {
                            StartGame();
                        }
                        else if (parts[0] == "STOP")
                        {
                            StopGame();
                        }
                    });
                }
            }
        }

        private async void SendData(string data)
        {
            if (stream != null)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MoveBall();
            SendData($"BALL,{Canvas.GetLeft(ball)},{Canvas.GetTop(ball)}");
        }

        private void MoveBall()
        {
            double currentX = Canvas.GetLeft(ball);
            double currentY = Canvas.GetTop(ball);

            double newX = currentX + ballSpeedX;
            double newY = currentY + ballSpeedY;

            if (newX < 0 || newX > gameCanvas.ActualWidth - ball.Width)
            {
                ballSpeedX *= -1;
            }
            if (newY < 0 || newY > gameCanvas.ActualHeight - ball.Height)
            {
                ballSpeedY *= -1;

                if (newY > gameCanvas.ActualHeight - ball.Height)
                {
                    gameTimer.Stop();
                    SendData("STOP");
                    MessageBox.Show("Game Over");
                }
            }
            if (IsCollision(ball, player1Paddle) || IsCollision(ball, player2Paddle))
            {
                newX = currentX - ballSpeedX;
                newY = currentY - ballSpeedY;
                ballSpeedY *= -1;
                ballSpeedX *= 1.2;
                ballSpeedY *= 1.2;
                record++;
                recordText.Text = $"{record}";
            }

            Canvas.SetLeft(ball, newX);
            Canvas.SetTop(ball, newY);
        }

        private bool IsCollision(FrameworkElement element1, FrameworkElement element2)
        {
            Rect rect1 = new Rect(Canvas.GetLeft(element1), Canvas.GetTop(element1), element1.Width, element1.Height);
            Rect rect2 = new Rect(Canvas.GetLeft(element2), Canvas.GetTop(element2), element2.Width, element2.Height);
            return rect1.IntersectsWith(rect2);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                double newLeft = Canvas.GetLeft(player1Paddle) - 10;
                if (newLeft >= 0)
                {
                    Canvas.SetLeft(player1Paddle, newLeft);
                    SendData($"P1,{newLeft}");
                }
            }
            else if (e.Key == Key.Right)
            {
                double newLeft = Canvas.GetLeft(player1Paddle) + 10;
                if (newLeft + player1Paddle.Width <= gameCanvas.ActualWidth)
                {
                    Canvas.SetLeft(player1Paddle, newLeft);
                    SendData($"P1,{newLeft}");
                }
            }
            else if (e.Key == Key.A)
            {
                double newLeft = Canvas.GetLeft(player2Paddle) - 10;
                if (newLeft >= 0)
                {
                    Canvas.SetLeft(player2Paddle, newLeft);
                    SendData($"P2,{newLeft}");
                }
            }
            else if (e.Key == Key.D)
            {
                double newLeft = Canvas.GetLeft(player2Paddle) + 10;
                if (newLeft + player2Paddle.Width <= gameCanvas.ActualWidth)
                {
                    Canvas.SetLeft(player2Paddle, newLeft);
                    SendData($"P2,{newLeft}");
                }
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
            SendData("START");
        }

        private void StartGame()
        {
            record = 0;
            recordText.Text = $"{record}";
            Canvas.SetLeft(ball, 50);
            Canvas.SetTop(ball, 50);
            ballSpeedX = 4;
            ballSpeedY = 4;
            gameTimer.Start();
            Canvas.SetLeft(player1Paddle, 100);
            Canvas.SetTop(player1Paddle, 400);
            Canvas.SetLeft(player2Paddle, 500);
            Canvas.SetTop(player2Paddle, 400);
        }

        private void StopGame()
        {
            gameTimer.Stop();
        }
    }
}
