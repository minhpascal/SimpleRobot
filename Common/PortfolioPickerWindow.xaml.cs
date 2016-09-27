using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Ecng.Collections;
using Ecng.Common;
using Ecng.Xaml;
using StockSharp.BusinessEntities;

namespace CommonRobot
{
    public partial class PortfolioPickerWindow
	{
		public static RoutedCommand PickPortfolioCommand = new RoutedCommand();

		public static readonly DependencyProperty SelectedPortfolioNameProperty =
			DependencyProperty.Register("SelectedPortfolioName", typeof(string), typeof(PortfolioPickerWindow),
			new FrameworkPropertyMetadata(null));

		public string SelectedPortfolioName
		{
			get { return (string)GetValue(SelectedPortfolioNameProperty); }
			set { SetValue(SelectedPortfolioNameProperty, value); }
		}

		private readonly ObservableCollection<Portfolio> _portfolios = new ObservableCollection<Portfolio>();

		private IConnector _trader;

		public PortfolioPickerWindow()
		{
			InitializeComponent();

			listViewPortfolios.ItemsSource = _portfolios;
		}

		public Portfolio SelectedPortfolio
		{
			get { return listViewPortfolios.SelectedItem.To<Portfolio>(); }
		}

		public IConnector Trader
		{
			get { return _trader; }
			set
			{
				_trader = value;

				this.GuiAsync(() =>
				{
					if(_trader != null)
					{
						_portfolios.Clear();
						_portfolios.AddRange(_trader.Portfolios);						
					}
				});
			}
		}

		private void ExecutedPickPortfolio(object sender, ExecutedRoutedEventArgs e)
		{
			SelectedPortfolioName = SelectedPortfolio.Name;
			DialogResult = true;
			Close();
		}

		private void CanExecutePickPortfolio(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = SelectedPortfolio != null;
		}

		private void HandleDoubleClick(object sender, MouseButtonEventArgs e)
		{
		}
	}
}
