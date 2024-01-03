using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace D365.UserControls
{
    public partial class EntitiesList : UserControl
    {
        public static readonly DependencyProperty EntityNamesProperty =
            DependencyProperty.Register("EntityNames", typeof(List<string>), typeof(EntitiesList));

        public List<string> EntityNames
        {
            get { return (List<string>)GetValue(EntityNamesProperty); }
            set { SetValue(EntityNamesProperty, value); }
        }

        public EntitiesList()
        {
            InitializeComponent();
            DataContext = this;  // Set DataContext to this instance of the user control
        }
    }
}
