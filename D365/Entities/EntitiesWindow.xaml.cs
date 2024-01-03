using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.CrmConnectControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfTutorial
{
    public partial class EntitiesWindow : Window, IEntitiesSelectionHandler
    {
        private readonly CrmConnectionManager crmConnectionManager;
        public List<EntityItem> EntityItems { get; set; }

        public EntitiesWindow(List<string> entities, CrmConnectionManager connectionManager)
        {
            InitializeComponent();

            EntityItems = entities.Select(entity => new EntityItem { EntityName = entity }).ToList();
            EntitiesListBox.ItemsSource = EntityItems;
            crmConnectionManager = connectionManager;
        }

        public event EventHandler<List<EntityItem>> EntitiesSelected;


        // Update the OkButton_Click method
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window or perform any action needed

            // Filter out only the selected entities
            var selectedEntities = EntityItems.Where(item => item.IsSelected).ToList();

            // Show attributes window for each selected entity
            foreach (var selectedEntity in selectedEntities)
            {
                ShowAttributesWindow(selectedEntity.EntityName);
            }
            EntitiesSelected?.Invoke(this, selectedEntities);


            Close();
        }

        // Add a new method to show the attributes window for a single entity
        private void ShowAttributesWindow(string entityName)
        {
            var request = new RetrieveEntityRequest
            {
                LogicalName = entityName,
                EntityFilters = EntityFilters.Attributes,
                RetrieveAsIfPublished = true
            };

            try
            {
                // Execute the request
                var response = (RetrieveEntityResponse)crmConnectionManager.CrmSvc.Execute(request);

                var entityMetadata = response.EntityMetadata;

                var attributeNames = new List<string>();

                foreach (var attribute in entityMetadata.Attributes)
                {
                    attributeNames.Add(attribute.LogicalName);
                }

                // Show the attributes window for the current entity
                AttributesWindow attributeDisplay = new AttributesWindow(attributeNames);
                attributeDisplay.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving attributes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

    public interface IEntitiesSelectionHandler
    {
        event EventHandler<List<EntityItem>> EntitiesSelected;
    }

    public class EntityItem
    {
        public string EntityName { get; set; }
        public bool IsSelected { get; set; }
    }
}
