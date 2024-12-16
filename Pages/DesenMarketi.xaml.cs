using Prot1.Valorant;
using Prot1.Valorant.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Valorant;

namespace Prot1.Forms
{
    public class DesenMarketi_KeyCombinationCard : UserControl
    {
        public static readonly DependencyProperty SupplierNameProperty =
            DependencyProperty.Register("SupplierName", typeof(string), typeof(DesenMarketi_KeyCombinationCard));

        public static readonly DependencyProperty VpTextProperty =
            DependencyProperty.Register("VpText", typeof(string), typeof(DesenMarketi_KeyCombinationCard));

        public static readonly DependencyProperty PriceTextProperty =
            DependencyProperty.Register("PriceText", typeof(string), typeof(DesenMarketi_KeyCombinationCard));

        public string SupplierName
        {
            get { return (string)GetValue(SupplierNameProperty); }
            set { SetValue(SupplierNameProperty, value); }
        }

        public string VpText
        {
            get { return (string)GetValue(VpTextProperty); }
            set { SetValue(VpTextProperty, value); }
        }

        public string PriceText
        {
            get { return (string)GetValue(PriceTextProperty); }
            set { SetValue(PriceTextProperty, value); }
        }
    }

    public class DesenMarketi_WeaponCardButton : CheckBox
    {
        public static readonly DependencyProperty BundleIconProperty =
            DependencyProperty.Register("BundleIcon", typeof(ImageSource), typeof(DesenMarketi_WeaponCardButton));

        public static readonly DependencyProperty TierIconProperty =
            DependencyProperty.Register("TierIcon", typeof(ImageSource), typeof(DesenMarketi_WeaponCardButton));

        public static readonly DependencyProperty WeaponImageProperty =
            DependencyProperty.Register("WeaponImage", typeof(ImageSource), typeof(DesenMarketi_WeaponCardButton));

        public static readonly DependencyProperty WeaponNameProperty =
            DependencyProperty.Register("WeaponName", typeof(string), typeof(DesenMarketi_WeaponCardButton));

        public static readonly DependencyProperty BackBrushProperty =
            DependencyProperty.Register("BackBrush", typeof(Brush), typeof(DesenMarketi_WeaponCardButton));

        public ImageSource BundleIcon
        {
            get { return (ImageSource)GetValue(BundleIconProperty); }
            set { SetValue(BundleIconProperty, value); }
        }

        public ImageSource TierIcon
        {
            get { return (ImageSource)GetValue(TierIconProperty); }
            set { SetValue(TierIconProperty, value); }
        }

        public ImageSource WeaponImage
        {
            get { return (ImageSource)GetValue(WeaponImageProperty); }
            set { SetValue(WeaponImageProperty, value); }
        }

        public string WeaponName
        {
            get { return (string)GetValue(WeaponNameProperty); }
            set { SetValue(WeaponNameProperty, value); }
        }

        public Brush BackBrush
        {
            get { return (Brush)GetValue(BackBrushProperty); }
            set { SetValue(BackBrushProperty, value); }
        }
    }

    public partial class DesenMarketi : Page
    {
        private const string VPCurrencyID = "85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741";
        public DesenMarketi()
        {
            InitializeComponent();

            this.LoadingGrid.Visibility = Visibility.Visible;
            this.NormalGrid.Visibility = Visibility.Collapsed;

            this.secondPanel.Visibility = Visibility.Collapsed;
            this.firstPanel.Visibility = Visibility.Visible;

            this.Loaded += async (sender, e) =>
            {
                await LoadPage();
            };
        }

        public Wallet playerWallet = null;
        public async Task LoadPage()
        {
            try
            {
                var sf = Storefront.GetStorefront(Globals.GameAuth);
                if (sf == null)
                {
                    // Handle the case where sf is null
                    return;
                }

                var bundles = Globals.GAME_CONTENT.Bundles.ToDictionary(x => x.Uuid);
                var prices = Storefront.GetPrices(Globals.GameAuth);
                playerWallet = Wallet.GetWallet(Globals.GameAuth);

                Globals.IDM.LoadDictionary();

                Globals.IDM.ImageRetrived += (type, file) =>
                {
                    fileLoadingName.Dispatcher.Invoke(() => fileLoadingName.Text = file);
                };

                skins.Dispatcher.Invoke(() => skins.Children.Clear());

                var list = new List<(int, DesenMarketi_WeaponCardButton)>();

                if (sf.SkinsPanelLayout == null)
                    return;

                foreach (var l in sf.SkinsPanelLayout.SingleItemStoreOffers)
                {
                    double cost;
                    // VP
                    if (!l.Cost.TryGetValue(VPCurrencyID, out cost))
                        continue;

                    foreach (var r in l.Rewards)
                    {
                        var skin = Globals.GAME_CONTENT.WeaponSkins.FirstOrDefault(s => s.levels.Any(x => x.uuid.CompareEquality(r.ItemID)));
                        if (skin == null)
                        {
                            // Handle the case where skin is not found
                            continue;
                        }

                        var tier = Globals.ContentTiers[skin.contentTierUuid];

                        var c = CreateCustomBorder(null, await Globals.IDM.GetImageAsync(tier.ImageURL), await Globals.IDM.GetImageAsync(skin.displayIcon ?? skin.levels.FirstOrDefault()?.displayIcon), skin.displayName, tier.Brushes, (double)cost);
                        list.Add((tier.Order, c));
                    }
                }

                foreach (var b in sf.FeaturedBundle.Bundles)
                {
                    foreach (var bskin in b.Items)
                    {
                        // SKIN
                        if (!bskin.Item.ItemTypeID.CompareEquality("e7c63390-eda7-46e0-bb7a-a6abdacd2433"))
                            continue;

                        var price = prices.Offers.FirstOrDefault(o => o.OfferID.CompareEquality(bskin.Item.ItemID));

                        if (price == null)
                        {
                            // Handle the case where price is not found
                            continue;
                        }

                        decimal cost;
                        // VP
                        if (!price.Cost.TryGetValue(VPCurrencyID, out cost))
                            continue;

                        var skin = Globals.GAME_CONTENT.WeaponSkins.FirstOrDefault(s => s.uuid.CompareEquality(bskin.Item.ItemID) || s.levels.Any(x => x.uuid.CompareEquality(bskin.Item.ItemID)));

                        if (skin == null)
                        {
                            // Handle the case where skin is not found
                            continue;
                        }

                        var tier = Globals.ContentTiers[skin.contentTierUuid];
                        var bundle = bundles[b.DataAssetID];

                        var c = CreateCustomBorder(await Globals.IDM.GetImageAsync(bundle.DisplayIcon), await Globals.IDM.GetImageAsync(tier.ImageURL), await Globals.IDM.GetImageAsync(skin.displayIcon), skin.displayName, tier.Brushes, (double)cost);
                        list.Add((tier.Order, c));
                    }
                }

                skins.Dispatcher.Invoke(() =>
                {
                    foreach (var c in list.OrderByDescending(x => x.Item1))
                        skins.Children.Add(c.Item2);
                });

                Globals.IDM.SetDictionary();

                LoadingGrid.Dispatcher.Invoke(() => LoadingGrid.Visibility = Visibility.Hidden);
                NormalGrid.Dispatcher.Invoke(() => NormalGrid.Visibility = Visibility.Visible);
            }
            catch (Exception ex)
            {
                // Handle exceptions here, log or display an error message
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }


        private double TotalCostVP = 0;
        private void OnCheckStateChange(object sender, dynamic e)
        {
            if (sender is CheckBox cb)
            {
                if (cb.Tag is double cost)
                        TotalCostVP += cost * (cb.IsChecked.HasValue && cb.IsChecked.Value ? 1 : -1);
            }
        }

        public DesenMarketi_WeaponCardButton CreateCustomBorder(ImageSource bundleIcon, ImageSource tierIcon, ImageSource weaponImage, string weaponName, Brush[] brushes, object Tag)
        {
            var e = new DesenMarketi_WeaponCardButton()
            {
                BundleIcon = bundleIcon,
                TierIcon = tierIcon,
                WeaponImage = weaponImage,
                WeaponName = weaponName,
                BorderBrush = brushes[0],
                BackBrush = brushes[1],
                Tag = Tag
            };
            e.Click += OnCheckStateChange;

            return e;
        }

        private void MenuButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (TotalCostVP == 0)
                return;

            LoadSecondPage();

            firstPanel.Visibility = Visibility.Collapsed;
            secondPanel.Visibility = Visibility.Visible;
        }

        private void LoadSecondPage()
        {
            double playerVp = 0;

            if (playerWallet != null)
                if (playerWallet.Balances != null)
                    if (!playerWallet.Balances.TryGetValue(VPCurrencyID, out playerVp))
                        playerVp = 0;

            TotalVP.Text = (TotalCostVP - playerVp).ToString() + " VP";
            var collection = KeySupplier.KeySupplierCollection.GetCollection();
            if (collection != null)
            {
                if (collection.Count <= 0)
                    return;

                var combination = collection.GetCheapestCombinations(TotalCostVP - playerVp);

                double totalVP = 0, totalPrice = 0;

                combinationsStackPanel.Children.Clear();
                foreach (var c in combination)
                {
                    totalVP += c.VpPrice;
                    totalPrice += c.KeyPrice;

                    combinationsStackPanel.Children.Add(new DesenMarketi_KeyCombinationCard()
                    {
                        SupplierName = c.SupplierName,
                        PriceText = c.KeyPrice + " TL",
                        VpText = c.VpPrice + " VP"
                    });
                }

                RemainingVP.Text = (totalVP - (TotalCostVP - playerVp)).ToString() + " VP";
                MinPrice.Text = totalPrice.ToString("N1") + " TL";
            }
        }

        private void MenuButton_MouseLeftButtonDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            firstPanel.Visibility = Visibility.Visible;
            secondPanel.Visibility = Visibility.Collapsed;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta < 0)
            {
                scrollViewer.LineRight();
            }
            else
            {
                scrollViewer.LineLeft();
            }
            e.Handled = true;
        }
    }
}
