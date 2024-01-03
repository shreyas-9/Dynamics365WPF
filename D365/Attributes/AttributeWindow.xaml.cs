// Update AttributesWindow.xaml.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WpfTutorial
{
    public partial class AttributesWindow : Window
    {
        public List<AttributeItem> AttributeItems { get; set; }

        public AttributesWindow(List<string> attributeNames)
        {
            InitializeComponent();

            AttributeItems = attributeNames.Select(attribute => new AttributeItem { AttributeName = attribute }).ToList();
            AttributeListBox.ItemsSource = AttributeItems;
        }
    }

    public class AttributeItem
    {
        public string AttributeName { get; set; }
        public bool IsSelected { get; set; }
    }
}
