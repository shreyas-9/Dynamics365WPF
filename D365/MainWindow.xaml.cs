using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.CrmConnectControl;
using PowerApps.Samples.LoginUX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static WpfTutorial.EntitiesWindow;

namespace WpfTutorial
{
    public partial class MainWindow : Window
    {
        private ExampleLoginForm ctrl;
        private List<string> selectedEntities = new List<string>(); // Class-level variable to store selected entities


        public MainWindow()
        {
            InitializeComponent();

            // Show the EntitiesWindow on startup
        }

        private void connectcrm(object sender, RoutedEventArgs e)
        {
            ctrl = new ExampleLoginForm();
            ctrl.ConnectionToCrmCompleted += ctrl_ConnectionToCrmCompleted;
            ctrl.ShowDialog();

            if (ctrl.CrmConnectionMgr != null && ctrl.CrmConnectionMgr.CrmSvc != null && ctrl.CrmConnectionMgr.CrmSvc.IsReady)
            {
                btnSignIn.Content = "Connected";
                btnSignIn.Background = Brushes.LightGreen;
                MessageBox.Show("Connected to Dataverse version: " + ctrl.CrmConnectionMgr.CrmSvc.ConnectedOrgVersion.ToString() +
                    " Organization: " + ctrl.CrmConnectionMgr.CrmSvc.ConnectedOrgUniqueName, "Connection Status");
            }
            else
            {
                MessageBox.Show("Cannot connect; try again!", "Connection Status");
            }
        }

        private void ctrl_ConnectionToCrmCompleted(object sender, EventArgs e)
        {
            if (sender is ExampleLoginForm)
            {
                this.Dispatcher.Invoke(() =>
                {
                    ((ExampleLoginForm)sender).Close();
                });
            }
        }

        private void retrieveEntities(object sender, RoutedEventArgs e)
        {
            RetrieveEntities(ctrl.CrmConnectionMgr);


        }


        private void RetrieveEntities(CrmConnectionManager crmConnectionMgr)
        {
            try
            {
                // Create a request to retrieve all entities
                var request = new RetrieveAllEntitiesRequest
                {
                    EntityFilters = EntityFilters.Entity,
                    RetrieveAsIfPublished = true
                };



                // Execute the request
                var response = (RetrieveAllEntitiesResponse)crmConnectionMgr.CrmSvc.Execute(request);

                // Process the response and get the list of entities
                var entities = response.EntityMetadata;

                var entityNames = new List<string>();


                foreach (var entity in entities)
                {

                    entityNames.Add(entity.LogicalName);
                }



                //EntitiesWindow window1 = new EntitiesWindow(entityNames, crmConnectionMgr);
               
                EntitiesListBox.ItemsSource = entityNames;

                //window1.Show();




            }
            catch (Exception ex)
            {
                // Handle exception as needed
                MessageBox.Show($"Error retrieving entities: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void retrieveAttributes(object sender, RoutedEventArgs e)
        {
            foreach (var selectedEntity in selectedEntities)
            {
                RetrieveAttributes(ctrl.CrmConnectionMgr, selectedEntity);
            }
        }

        private void RetrieveAttributes(CrmConnectionManager crmConnectionMgr , string entityName)
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
                var response = (RetrieveEntityResponse)crmConnectionMgr.CrmSvc.Execute(request);

                var entityMetadata = response.EntityMetadata;

                var attributeNames = new List<string>();

                foreach (var attribute in entityMetadata.Attributes)
                {
                    attributeNames.Add(attribute.LogicalName);
                }

                // Show the attributes window for the current entity
                //AttributesWindow attributeDisplay = new AttributesWindow(attributeNames);
                //attributeDisplay.Show();

                AttributesListBox.ItemsSource = attributeNames;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving attributes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void orderEntities(object sender, RoutedEventArgs e)
        {

        }

        private void migrate(object sender, RoutedEventArgs e)
        {

        }


        private int currentEntityIndex = 0; // Class-level variable to track the current entity index

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            // Save the selected entities
            selectedEntities = new List<string>();

            foreach (var item in EntitiesListBox.Items)
            {
                ListBoxItem listBoxItem = EntitiesListBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                if (listBoxItem != null)
                {
                    CheckBox checkBox = FindVisualChild<CheckBox>(listBoxItem);
                    if (checkBox != null && checkBox.IsChecked == true)
                    {
                        selectedEntities.Add(item.ToString());
                    }
                }
            }
            EntitiesListBox.Visibility = Visibility.Collapsed;
            if (currentEntityIndex < selectedEntities.Count)
            {
                RetrieveAttributes(ctrl.CrmConnectionMgr, selectedEntities[currentEntityIndex]);
                currentEntityIndex++;
            }
            else
            {
                // Reset the index when all entities are processed
                currentEntityIndex = 0;

                // Optionally, hide or close the EntitiesListBox
                EntitiesListBox.Visibility = Visibility.Collapsed;

                // Perform further processing or navigation to the next step
                // ...
            }


            // Perform further processing or navigation to the next step
            // ...
        }

        // Helper method to find a child of a specified type in the visual tree
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

    }
}