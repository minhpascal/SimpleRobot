using System.Windows;
using System.Windows.Data;
using StockSharp.Xaml;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace CommonRobot
{
    public partial class PortfolioNameEditor : ITypeEditor
	{
		public PortfolioNameEditor()
		{
			InitializeComponent();
		}

		PropertyItem _item;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var editor = new PortfolioPickerWindow()
			{
                Trader = SafeConnectionSingle.Instance.Trader
			};

			var binding = new Binding("Value")
			{
				Source = _item,
				Mode = _item.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
			};

			BindingOperations.SetBinding(editor, PortfolioPickerWindow.SelectedPortfolioNameProperty, binding);
			editor.ShowDialog();
		}

		public FrameworkElement ResolveEditor(PropertyItem propertyItem)
		{
			_item = propertyItem;
			return this;
		}
	}
}
