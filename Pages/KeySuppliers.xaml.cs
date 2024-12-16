using Prot1.Valorant;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using static Globals;

namespace Prot1.Forms
{
    public class KeySupplier_TableItem : UserControl
    {
        public string Seller
        {
            get { return (string)GetValue(SellerProperty); }
            set { SetValue(SellerProperty, value); }
        }

        public string Price
        {
            get { return (string)GetValue(PriceProperty); }
            set { SetValue(PriceProperty, value); }
        }

        public string Vp
        {
            get { return (string)GetValue(VpProperty); }
            set { SetValue(VpProperty, value); }
        }

        public static readonly DependencyProperty SellerProperty =
            DependencyProperty.Register("Seller", typeof(string), typeof(KeySupplier_TableItem), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PriceProperty =
            DependencyProperty.Register("Price", typeof(string), typeof(KeySupplier_TableItem), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty VpProperty =
            DependencyProperty.Register("Vp", typeof(string), typeof(KeySupplier_TableItem), new PropertyMetadata(string.Empty));
    }

    public partial class KeySuppliers : Page
    {
        public KeySuppliers()
        {
            InitializeComponent();

            this.Loaded += (sender, e) =>
            {
                var collection = KeySupplier.KeySupplierCollection.GetCollection();
                ClearTable();
                foreach (var c in collection)
                {
                    Table.Children.Add(new KeySupplier_TableItem()
                    {
                        Seller = c.SupplierName,
                        Vp = string.Format("{0:0}", c.VpPrice),
                        Price = string.Format("{0:0.00}", c.KeyPrice),
                        Tag = gen3key()
                    });
                }
            };
        }

        private int gen3key()
        {
            var table = Table.Children.Cast<FrameworkElement>();
            int key = -1;
            var rnd = new Random(DateTime.Now.Millisecond);
            do
            {
                key = rnd.Next(1000, 9999);
            } while (key == -1 || table.Any(x => x.Tag is int i && i == key));

            return key;
        }

        private bool ToDouble(string number, out double res)
           =>  double.TryParse(number, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out res);

        private void ClearTable()
        {
            foreach (FrameworkElement elem in Table.Children)
            {
                if (elem.Tag == null || elem.Tag.ToString() != "header")
                    Table.Children.Remove(elem);
            }
        }

        // DELETE
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement elem && elem.Tag is int i)
            {
                for (int j = Table.Children.Count - 1; j >= 0; j--)
                {
                    FrameworkElement child = (FrameworkElement)Table.Children[j];
                    if (child.Tag == elem.Tag)
                    {
                        Table.Children.RemoveAt(j);
                    }
                }
            }

        }

        // ADD
        private void MenuButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Table.Children.Add(new KeySupplier_TableItem()
            {
                Seller = "Satıcı",
                Vp = "000",
                Price = "000.00",
                Tag = gen3key()
            });
        }

        // SAVE
        private void MenuButton_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            var ksc = new KeySupplier.KeySupplierCollection();

            foreach (var element in Table.Children)
            {
                if (element is KeySupplier_TableItem ti)
                {
                    if (ToDouble(ti.Vp, out double vp) && ToDouble(ti.Price, out double price) && vp > 0.0 && price > 0.0)
                    {
                        var ks = new KeySupplier(ti.Seller, price, vp);

                        if (!ksc.Any(x => x.Equals(ks)))
                        {
                            ksc.Add(ks);
                        }
                        else
                        {
                            // AYNI ÜRÜNDEN BİRDEN FAZLA VAR
                            //Globals.ShowInformationDialog("Bir veya birden fazla ürün zaten mevcut", "Ürün Hatası!");
                            return;
                        }
                    }
                    else
                    {
                        // ÜRÜN FİYAT HATASI
                        //Globals.ShowInformationDialog("Bir veya birden fazla ürünün fiyatları geçersiz.", "Ürün fiyatlandırma hatası!");
                        return;
                    }
                }
            }

            ksc.SaveCollection();
            // İŞLEM BAŞARILI
        }
    }
}
