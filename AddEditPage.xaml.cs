using Microsoft.Win32;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Agent _currentAgent = new Agent();
        public AddEditPage(Agent selectedAgent)
        {
            InitializeComponent();

            if (selectedAgent != null)
                _currentAgent = selectedAgent;

            DataContext = _currentAgent;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.Address))
                errors.AppendLine("Укажите адрес агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.DirectorName))
                errors.AppendLine("Укажите ФИО директора");

            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите тип агента");

            if (_currentAgent.Priority < 0)
                errors.AppendLine("Укажите положительный приоритет агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.INN))
                errors.AppendLine("Укажите ИНН агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.KPP))
                errors.AppendLine("Укажите КПП агента");

            if (string.IsNullOrWhiteSpace(_currentAgent.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = _currentAgent.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "").Replace(" ", "");

                if (!((ph.Length == 11 && (ph[0] == '7' || ph[0] == '8'))
                      || (ph.Length == 12 && ph[0] == '3')))
                    errors.AppendLine("Укажите правильный телефон агента");
            }

            if (string.IsNullOrWhiteSpace(_currentAgent.Email))
                errors.AppendLine("Укажите почту агента");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_currentAgent.ID == 0)
                BashkircevGlazkiSaveEntities
                    .GetContext()
                    .Agent
                    .Add(_currentAgent);

            if (ComboType.SelectedIndex >= 0)
            {
                _currentAgent.AgentTypeID = ComboType.SelectedIndex + 1;
            }
            try
            {
                BashkircevGlazkiSaveEntities.GetContext().SaveChanges();

                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentAgent.ProductSale.Any())
            {
                MessageBox.Show("Нельзя удалить агента — есть информация о реализации продукции");
                return;
            }

            if (MessageBox.Show("Вы точно хотите удалить агента?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                // Удаление самого агента
                BashkircevGlazkiSaveEntities.GetContext().Agent.Remove(_currentAgent);

                BashkircevGlazkiSaveEntities.GetContext().SaveChanges();

                MessageBox.Show("Агент удалён");

                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _currentAgent.Logo = myOpenFileDialog.FileName;

                LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }
    }
}
