using System;
using System.Collections.Generic;
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

namespace BashkircevGlazkiSave
{
    /// <summary>
    /// Логика взаимодействия для AgentPage.xaml
    /// </summary>
    public partial class AgentPage : Page
    {
        int currentPage = 1;
        int itemsPerPage = 10;
        int totalPages = 1;
        public AgentPage()
        {
            InitializeComponent();

            FilterCombo.Items.Add("Все типы");

            foreach (var type in BashkircevGlazkiSaveEntities
                     .GetContext()
                     .AgentType)
            {
                FilterCombo.Items.Add(type.Title);
            }

            FilterCombo.SelectedIndex = 0;

            UpdateAgents();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void SortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();

        }

        private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();

        }

        private void UpdateAgents()
        {
            var agents = BashkircevGlazkiSaveEntities
                         .GetContext()
                         .Agent
                         .ToList();


            agents = agents.Where(a =>
                a.Title.ToLower().Contains(SearchBox.Text.ToLower())
                || a.Phone.Contains(SearchBox.Text)
                || a.Email.Contains(SearchBox.Text)
            ).ToList();


            if (FilterCombo.SelectedIndex > 0)
            {
                string type = FilterCombo.SelectedItem.ToString();

                agents = agents
                    .Where(a => a.AgentType.Title == type)
                    .ToList();
            }


            switch (SortCombo.SelectedIndex)
            {
                case 0:
                    agents = agents.OrderBy(a => a.Title).ToList();
                    break;

                case 1:
                    agents = agents.OrderByDescending(a => a.Title).ToList();
                    break;

                case 2:
                    agents = agents.OrderBy(a => a.DiscountPercent).ToList();
                    break;

                case 3:
                    agents = agents.OrderByDescending(a => a.DiscountPercent).ToList();
                    break;

                case 4:
                    agents = agents.OrderBy(a => a.Priority).ToList();
                    break;

                case 5:
                    agents = agents.OrderByDescending(a => a.Priority).ToList();
                    break;
            }
            totalPages = (int)Math.Ceiling((double)agents.Count / itemsPerPage);

            var pageList = agents
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            AgentListView.ItemsSource = pageList;

            DrawPageButtons();
        }

        private void DrawPageButtons()
        {
            PageNumbers.Children.Clear();

            for (int i = 1; i <= totalPages; i++)
            {
                Button btn = new Button();
                btn.Content = i;
                btn.Tag = i;
                btn.Width = 30;        
                btn.Height = 25;        
                btn.Margin = new Thickness(2);
                btn.Padding = new Thickness(0);
                btn.Click += PageButton_Click;

                PageNumbers.Children.Add(btn);
            }
        }

        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            currentPage = (int)btn.Tag;
            UpdateAgents();
        }


        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdateAgents();
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                UpdateAgents();
            }
        }

        private void AddAgentBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Agent));
        }
    }
}
