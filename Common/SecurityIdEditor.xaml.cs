using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace CommonRobot
{
    public partial class SecurityIdEditor : ITypeEditor
	{
		public SecurityIdEditor()
		{
			InitializeComponent();
		}

		PropertyItem _item;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
            var editor = new SecurityEditorWindow();

            var binding = new Binding("Value")
            {
                Source = _item,
                Mode = _item.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };

            BindingOperations.SetBinding(editor, SecurityEditorWindow.SelectedSecurityIdProperty, binding);
            editor.ShowDialog();   
		}

		public FrameworkElement ResolveEditor(PropertyItem propertyItem)
		{
			_item = propertyItem;
			return this;
		}
	}
}
