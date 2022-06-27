using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UP_11
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       //private static Agents _currentAgent;
        List<ListView> consul = new List<ListView>();
        List<Agents> agents = new List<Agents>();
        //int clickedBtnIndex = 1;
        public MainWindow()
        {
            InitializeComponent();
            var context = NiceRustleEntities.GetContext();
            var allTypes = context.AgentTypes.ToList();
            allTypes.Insert(0, new AgentTypes { TypeName = "Все типы" });
            cbFilter.ItemsSource = allTypes;
            cbFilter.SelectedIndex = 0;
            cbSort.Items.Add("Название по возрастанию");
            cbSort.Items.Add("Размер скидки по возрастанию");
            cbSort.Items.Add("Приоритет по возрастанию");
            cbSort.Items.Add("Название по убыванию");
            cbSort.Items.Add("Размер скидки по убыванию");
            cbSort.Items.Add("Приотритет по убыванию");
            cbSort.SelectedIndex = 0;
            tbPoisk.Text = "";
            UpdateAgents();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAgents();
        }

        private void cbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void cbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void tbPoisk_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAgents();
        }

        private void btAddAgent_Click(object sender, RoutedEventArgs e)
        {
            Window1 agentics = new Window1(lvAgents.SelectedItem, null);
            agentics.Owner = this;
            agentics.Title = "Добавление агента";
            agentics.ShowDialog();
            UpdateAgents();
        }
        private void UpdateAgents()
        {
            var currentAgents = NiceRustleEntities.GetContext().Agents.ToList();

            if (cbFilter.SelectedIndex > 0)
            {
                AgentTypes agentTypes = cbFilter.SelectedItem as AgentTypes;
                string filter = agentTypes.TypeName.ToString();
                currentAgents = currentAgents.Where(p => p.AType.Contains(filter)).ToList();
            }
            else currentAgents = NiceRustleEntities.GetContext().Agents.ToList();
            switch (cbSort.SelectedIndex)
            {
                case 0:
                    currentAgents = currentAgents.OrderBy(p => p.NameCompany).ToList();
                    break;
                case 1:
                    currentAgents = currentAgents.OrderBy(p => p.Skidka).ToList();
                    break;
                case 2:
                    currentAgents = currentAgents.OrderBy(p => p.Priority).ToList();
                    break;
                case 3:
                    currentAgents = currentAgents.OrderByDescending(p => p.NameCompany).ToList();
                    break;
                case 4:
                    currentAgents = currentAgents.OrderByDescending(p => p.Skidka).ToList();
                    break;
                case 5:
                    currentAgents = currentAgents.OrderByDescending(p => p.Priority).ToList();
                    break;
            }
            if (tbPoisk.Text != "")
                currentAgents = currentAgents.Where(p => p.NameCompany.ToLower().Contains(tbPoisk.Text.ToLower()) ||
                p.Phone.ToLower().Contains(tbPoisk.Text.ToLower()) || p.Email.ToLower().Contains(tbPoisk.Text.ToLower())).ToList();
            var count = stackPan.Children.Count;
            var agentsLength = currentAgents.Count();
            int agentsPerPage = 10;
            var pages = agentsLength / agentsPerPage;
            updateButtons(pages, currentAgents);
            lvAgents.ItemsSource = currentAgents;
        }

        //public async Task<IList<SavedSearch>> FindAllSavedSearches(int page, int limit)
        //{
        //    if (page == 0)
        //        page = 1;

        //    if (limit == 0)
        //        limit = int.MaxValue;

        //    var skip = (page - 1) * limit;

        //    var savedSearches = _databaseContext.SavedSearches.Skip(skip).Take(limit).Include(x => x.Parameters);
        //    return await savedSearches.ToArrayAsync();
        //}

        public void updateButtons(int pages, List<Agents> agents)
        {
            stackPan.Children.Clear();
            for (int i = 1; i <= pages; i++)
            {
                Button btnToAdd = new Button();
                btnToAdd.Content = i;
                btnToAdd.Style = (Style)Resources["pageBtn"];
                var skip = (i - 1) * 10;
                btnToAdd.Click += (object sender, RoutedEventArgs e) =>
                {
                    var currentAgents = NiceRustleEntities.GetContext().Agents.OrderBy(a => a.Id).Skip(skip).Take(10).ToList();
                    lvAgents.ItemsSource = currentAgents;
                };
                //if (i == clickedBtnIndex)
                //{
                //    btnToAdd.FontWeight = FontWeights.Bold;
                //}
                stackPan.Children.Add(btnToAdd);
            }
        }

        private void Item_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Window1 agentics = new Window1(lvAgents.SelectedItem, lvAgents.SelectedItem as Agents);
            agentics.Owner = this;
            agentics.Title = "Редактирование агента";
            agentics.ShowDialog();
            UpdateAgents();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            List<Agents> agent = lvAgents.SelectedItems.Cast<Agents>().ToList();
            if (agent == null)
            {
                MessageBox.Show("Выберите агента");
                return;
            }
            Document doc = new Document();
            BaseFont baseFont = BaseFont.CreateFont("C:/Windows/Fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font font = new Font(baseFont, Font.DEFAULTSIZE, Font.NORMAL);
            Font font1 = new Font(baseFont, 16, Font.BOLD);
            Font font2 = new Font(baseFont, Font.DEFAULTSIZE, Font.BOLDITALIC);
            using (var writer = PdfWriter.GetInstance(doc, new FileStream("pdfReport.pdf", FileMode.Create)))
            {
                doc.Open();
                foreach (Agents a in agent)
                {
                    
                    doc.AddTitle("Отчет");
                    doc.NewPage();
                    doc.Add(new Paragraph("АГЕНТ", font1));
                    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(a.Logo);
                    image.ScaleAbsoluteHeight(150);
                    image.ScaleAbsoluteWidth(150);
                    doc.Add(image);
                    doc.Add(new Paragraph($"{a.AgentTypes.TypeName} | {a.NameCompany}", font));
                    doc.Add(new Paragraph($"Электронная почта: {a.Email}", font));
                    doc.Add(new Paragraph($"Телефон: {a.Phone}", font));
                    doc.Add(new Paragraph($"Адрес: {a.Adress}", font));
                    doc.Add(new Paragraph($"Приоритет: {a.Priority}", font));
                    doc.Add(new Paragraph($"Директор: {a.Director}", font));
                    doc.Add(new Paragraph($"ИНН: {a.INN}", font));
                    doc.Add(new Paragraph($"КПП: {a.KPP}", font));
                   
                } 
                doc.Close();
            }
            MessageBox.Show("Документ записан");
            Process.Start("pdfReport.pdf");
        }

        private void btDiagram_Click(object sender, RoutedEventArgs e)
        {
            Diagram diag = new Diagram(agents);
            diag.Show();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var paymentsForRemoving = lvAgents.SelectedItems.Cast<Agents>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить {paymentsForRemoving.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    NiceRustleEntities.GetContext().Agents.RemoveRange(paymentsForRemoving);
                    NiceRustleEntities.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены");
                    UpdateAgents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
        }
    }
}
