using Prot1.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Valorant;

namespace Prot1.Forms
{
    /// <summary>
    /// Interaction logic for Ayarlar.xaml
    /// </summary>
    public partial class Ayarlar : Page
    {
        public Ayarlar()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe)
                if (fe.Tag != null && fe.Tag is string str)
                {
                    if (Globals.UserSettings.PlayerMessages.ContainsKey(str))
                    {
                        var tagEditor = new TagEditor(str, "agent", "skin", "puuid", "player", "playertag");
                        tagEditor.OnClosed += () =>
                        {
                            MainWindow.Instance.popupGrid.Children.Remove(tagEditor);
                            MainWindow.Instance.popupGrid.Visibility = Visibility.Collapsed;
                        };
                        MainWindow.Instance.popupGrid.Visibility = Visibility.Visible;
                        MainWindow.Instance.popupGrid.Children.Add(tagEditor);
                    }
                }
        }
    }
}
