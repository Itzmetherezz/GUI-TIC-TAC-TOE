using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace TIC_TAC_TOE
{

    public partial class MainWindow : Window
    {
        private readonly Dictionary<Player, ImageSource> imagesources = new()
        {

            {Player.X, new BitmapImage(new Uri("pack://application:,,,/ASSET/X15.png")) },
            {Player.O, new BitmapImage(new Uri("pack://application:,,,/ASSET/O15.png")) }

        };


        private readonly Dictionary<Player, ObjectAnimationUsingKeyFrames> animations = new()

        {
            {Player.X, new ObjectAnimationUsingKeyFrames() },
            {Player.O, new ObjectAnimationUsingKeyFrames() }


        };


        private readonly DoubleAnimation fadeOutAnimation = new DoubleAnimation
        {

            Duration = TimeSpan.FromSeconds(0.5),
            From = 1,
            To = 0

        };

        private readonly DoubleAnimation fadeInAnimation = new DoubleAnimation
        {

            Duration = TimeSpan.FromSeconds(0.5),
            From = 0,
            To = 1

        };


        private readonly Image[,] imageControls = new Image[3, 3];



        private readonly GameState gameState = new GameState();




        public MainWindow()
        {
            InitializeComponent();
            SetupGameGrid();
            SetupAnimations();
            gameState.MoveMade += OnMoveMade;
            gameState.GameOverEvent += OnGameOverEvent;
            gameState.GameReset += OnGameReset;
        }


        private void SetupGameGrid()
        {

            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    Image imageControl = new Image();
                    GameGrid.Children.Add(imageControl);
                    imageControls[row, column] = imageControl;
                }
            }




        }



        private void SetupAnimations()
        {
            animations[Player.X].Duration = TimeSpan.FromSeconds(0.25);
            animations[Player.O].Duration = TimeSpan.FromSeconds(0.25);

            for (int i = 0; i < 16; i++)
            {

                Uri xUri = new Uri($"pack://application:,,,/ASSET/X{i}.png");
                BitmapImage xImg = new BitmapImage(xUri);
                DiscreteObjectKeyFrame xFrame = new DiscreteObjectKeyFrame(xImg);
                animations[Player.X].KeyFrames.Add(xFrame);

                Uri oUri = new Uri($"pack://application:,,,/ASSET/O{i}.png");
                BitmapImage oImg = new BitmapImage(oUri);
                DiscreteObjectKeyFrame oFrame = new DiscreteObjectKeyFrame(oImg);
                animations[Player.O].KeyFrames.Add(oFrame);
            }
        }




        private async Task FadeOut(UIElement element)
        {

            element.BeginAnimation(OpacityProperty, fadeOutAnimation);
            await Task.Delay(fadeOutAnimation.Duration.TimeSpan);
            element.Visibility = Visibility.Hidden;

        }

        private async Task FadeIn(UIElement element)
        {

            element.BeginAnimation(OpacityProperty, fadeInAnimation);
             
            element.Visibility = Visibility.Visible;
            await Task.Delay(fadeInAnimation.Duration.TimeSpan);

        }



        private async Task TransitionToEndScreen(string text , ImageSource winnerimage)
        {

            await Task.WhenAll(FadeOut(TurnPanel), FadeOut(GameCanvas));
            ResultText.Text = text;
            WinnerImage.Source = winnerimage;
            await FadeIn(EndScreen);


        }

        private async Task TransitionToGameScreen()
        {
            await FadeOut(EndScreen);
            
            
            Line.Visibility = Visibility.Hidden;
            await Task.WhenAll(FadeIn(TurnPanel), FadeIn(GameCanvas));
        }

        private (Point, Point) FindLinePoints(WinInfo info)
        {
            double squareSize = GameGrid.Width / 3;
            double margin = squareSize / 2;


            if (info.WinType == WinType.Row)
            {
                double y = info.Index * squareSize + margin;
                return (new Point(0, y), new Point(GameGrid.Width, y));

            }
            if (info.WinType == WinType.Column)
            {
                double x = info.Index * squareSize + margin;
                return (new Point(x, 0), new Point(x, GameGrid.Height));

            }
            if (info.WinType == WinType.MainDiagonal)
            {
                return (new Point(0, 0), new Point(GameGrid.Width, GameGrid.Height));
            }
            return (new Point(0, GameGrid.Height), new Point(GameGrid.Width, 0)); 


        }

        private async Task ShowWinLine(WinInfo info)
        {
            (Point start, Point end) = FindLinePoints(info);

            Line.X1 = start.X;
            Line.Y1 = start.Y;

            DoubleAnimation x2animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.25),
                From = start.X,
                To = end.X


            };

            DoubleAnimation y2animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(0.25),
                From = start.Y,
                To = end.Y
            };



            Line.Visibility = Visibility.Visible;
            Line.BeginAnimation(Line.X2Property, x2animation);
            Line.BeginAnimation(Line.Y2Property, y2animation);
            await Task.Delay(x2animation.Duration.TimeSpan);


        }


            private void OnMoveMade(int row, int column)
        {
            Player player = gameState.Gamegrid[row, column];
            imageControls[row, column].BeginAnimation(Image.SourceProperty, animations[player]);    
            PlayerImage.Source = imagesources[gameState.CurrentPlayer];

        }

        private async void OnGameOverEvent(GameResult result)
        {
            await Task.Delay(1000);
            if (result.Winner == Player.None)
            {
                await TransitionToEndScreen("It's a draw", null);
            }
            else
            {
                await ShowWinLine(result.WinInfo);
                await Task.Delay(1000);

                await TransitionToEndScreen("Winner:", imagesources[result.Winner]);
            }


        }


        private async void OnGameReset()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    imageControls[row, column].BeginAnimation(Image.SourceProperty, null);
                    imageControls[row, column].Source = null;
                }
            }
            PlayerImage.Source = imagesources[gameState.CurrentPlayer];
            await TransitionToGameScreen();



        }




        private void GameGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double squareSize = GameGrid.Width / 3;
            Point clickPosition = e.GetPosition(GameGrid);
            int row = (int)(clickPosition.Y / squareSize);
            int column = (int)(clickPosition.X / squareSize);
            gameState.MakeMove(row, column);


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameState.GameOver )
            gameState.ResetGame();

        }


    }
}