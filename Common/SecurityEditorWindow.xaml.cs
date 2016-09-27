using System.Windows;
using System.Windows.Input;
using StockSharp.BusinessEntities;

namespace CommonRobot
{
    using StockSharp.Algo;

    public partial class SecurityEditorWindow
	{
		public static readonly DependencyProperty SelectedSecurityIdProperty =
			DependencyProperty.Register("SelectedSecurityId", typeof(string), typeof(SecurityEditorWindow), new PropertyMetadata(string.Empty));

		public string SelectedSecurityId
		{
			get { return (string)GetValue(SelectedSecurityIdProperty); }
			set
			{
				SetValue(SelectedSecurityIdProperty, value);
			}
		}

		public static RoutedCommand PickSecurityCommand = new RoutedCommand();

		public SecurityEditorWindow()
		{
			InitializeComponent();

			Picker.SecurityProvider = new FilterableSecurityProvider(SafeConnectionSingle.Instance.Trader);
		}

		private void ExecutedPickSecutiry(object sender, ExecutedRoutedEventArgs e)
		{
			SelectedSecurityId = Picker.SelectedSecurity.Id;
			DialogResult = true;
			Close();
		}

		private void CanExecutePickSecutiry(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Picker.SelectedSecurity != null;
			e.Handled = true;
		}

		private void Picker_SecurityDoubleClick(Security security)
		{
			SelectedSecurityId = security.Id;
			DialogResult = true;
			Close();
		}
	}
}
