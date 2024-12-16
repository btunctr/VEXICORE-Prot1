using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Prot1
{
    
    public class PlayerCardButton : Button
    {
        public static readonly DependencyProperty ImageSourceProperty =
           DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(PlayerCardButton), new PropertyMetadata(null));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty CardTextProperty =
            DependencyProperty.Register("CardText", typeof(string), typeof(PlayerCardButton), new PropertyMetadata(""));

        public string CardText
        {
            get { return (string)GetValue(CardTextProperty); }
            set { SetValue(CardTextProperty, value); }
        }

        public static readonly DependencyProperty CardLevelTextProperty =
            DependencyProperty.Register("CardLevelText", typeof(string), typeof(PlayerCardButton), new PropertyMetadata(""));

        public string CardLevelText
        {
            get { return (string)GetValue(CardLevelTextProperty); }
            set { SetValue(CardLevelTextProperty, value); }
        }

        public static readonly DependencyProperty RankImageSourceProperty =
            DependencyProperty.Register("RankImageSource", typeof(ImageSource), typeof(PlayerCardButton), new PropertyMetadata(null));

        public ImageSource RankImageSource
        {
            get { return (ImageSource)GetValue(RankImageSourceProperty); }
            set { SetValue(RankImageSourceProperty, value); }
        }

        public static readonly DependencyProperty PeakRankImageSourceProperty =
            DependencyProperty.Register("PeakRankImageSource", typeof(ImageSource), typeof(PlayerCardButton), new PropertyMetadata(null));

        public ImageSource PeakRankImageSource
        {
            get { return (ImageSource)GetValue(PeakRankImageSourceProperty); }
            set { SetValue(PeakRankImageSourceProperty, value); }
        }

        public static readonly DependencyProperty AvgRankImageSourceProperty =
            DependencyProperty.Register("AvgRankImageSource", typeof(ImageSource), typeof(PlayerCardButton), new PropertyMetadata(null));

        public ImageSource AvgRankImageSource
        {
            get { return (ImageSource)GetValue(AvgRankImageSourceProperty); }
            set { SetValue(AvgRankImageSourceProperty, value); }
        }

        public static readonly DependencyProperty LevelBorderIconProperty =
           DependencyProperty.Register("LevelBorderIcon", typeof(ImageSource), typeof(PlayerCardButton), new PropertyMetadata(null));

        public ImageSource LevelBorderIcon
        {
            get { return (ImageSource)GetValue(LevelBorderIconProperty); }
            set { SetValue(LevelBorderIconProperty, value); }
        }

        public static readonly DependencyProperty CardBackColorProperty =
            DependencyProperty.Register("CardBackColor", typeof(Brush), typeof(PlayerCardButton), new PropertyMetadata(null));

        public Brush CardBackColor
        {
            get { return (Brush)GetValue(CardBackColorProperty); }
            set { SetValue(CardBackColorProperty, value); }
        }

        private void SetColorRed()
        {
            CardBackColor = new SolidColorBrush(Color.FromRgb(250, 69, 85));
        }

        private void SetColorGreen()
        {
            CardBackColor = new SolidColorBrush(Color.FromRgb(22, 229, 180));
        }

        static PlayerCardButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlayerCardButton), new FrameworkPropertyMetadata(typeof(PlayerCardButton)));
        }

        public enum CardColor
        {
            Green,
            Red
        }

        public void SetCardColor(CardColor color)
        {
            if (color == CardColor.Green)
                SetColorGreen();
            else
                SetColorRed();
        }

        public PlayerCardButton()
        {
            SetColorGreen();
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    }
}
