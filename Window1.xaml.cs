using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace UP_11
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private Agents _currentAgent = new Agents();
        private string Pathfile;

        public Window1(object agentId, Agents agent)
        {
            InitializeComponent();
            cbType.ItemsSource = NiceRustleEntities.GetContext().AgentTypes.ToList();
            if (agent != null) { cbType.SelectedIndex = agent.AgentTypes.Id - 1; }
            else { cbType.SelectedIndex = 0; }
            agentId = null;
            if (agent == null)
            {
                _currentAgent.Id = Convert.ToInt32(agentId);
            }

            else _currentAgent = agent;
            {
                DataContext = _currentAgent;
            }

        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            MemoryStream memoryStream = new MemoryStream();
            if (Pathfile == null)
            {
                Pathfile = "default.png";
                System.Drawing.Image image = System.Drawing.Image.FromFile(Pathfile);
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(Pathfile);
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                
            }
            byte [] logoTip = memoryStream.ToArray();
            string nameAgent = tbNameAgent.Text;
            string priority = tbPriority.Text;
            string adress = tbAdress.Text;
            string INN = tbINN.Text;
            string KPP = tbKPP.Text;
            string director = tbDirector.Text;
            string phone = tbPhone.Text;
            string email = tbEmail.Text;

            if (_currentAgent.Id == 0)
            {
                _currentAgent.Id = NiceRustleEntities.GetContext().Agents.Count() + 1;
                _currentAgent.Logo = logoTip;
                NiceRustleEntities.GetContext().Agents.Add(_currentAgent);
            }

            if (String.IsNullOrEmpty(nameAgent) || String.IsNullOrEmpty(priority) ||
                String.IsNullOrEmpty(adress) || String.IsNullOrEmpty(INN) ||
                String.IsNullOrEmpty(KPP) || String.IsNullOrEmpty(director) || String.IsNullOrEmpty(phone) ||
                String.IsNullOrEmpty(email))
            {
                MessageBox.Show("Некорректный ввод данных!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    _currentAgent.Logo = logoTip;
                    NiceRustleEntities.GetContext().SaveChanges();
                    if (MessageBox.Show("Информация сохранена", "", MessageBoxButton.OK) == MessageBoxResult.OK)
                        Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                imLogo.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                Pathfile = openFileDialog.FileName;
            }
        }

        private void cbType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _currentAgent.AgentTypeId = cbType.SelectedIndex + 1;
        }
    }
}
