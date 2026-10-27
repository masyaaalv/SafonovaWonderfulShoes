using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SafonovaWonderfulShoes
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private User _currentUser;
        private int _userRole;

        // Кэшируем список всех товаров, чтобы не обращаться к базе данных при каждом нажатии клавиши в поиске
        private List<Product> _allProducts;

        public ProductPage(User user)
        {
            try
            {
                InitializeComponent();

                _currentUser = user;
                _userRole = user?.ID_Role ?? 0;

                // Загружаем данные из БД один раз при инициализации страницы
                _allProducts = SafonovaShoesEntities.GetContext().Products.ToList();

                // Устанавливаем значения по умолчанию для комбобоксов
                FilterCBox.SelectedIndex = 0;
                SortTB.SelectedIndex = 0;

                // Обновляем информацию о пользователе и список товаров
                UpdateUserInfo();
                UpdateProduct();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка инициализации", $"Не удалось загрузить страницу товаров.\nПодробности: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновляет текст с ФИО и ролью пользователя в правом верхнем углу
        /// </summary>
        private void UpdateUserInfo()
        {
            if (_currentUser != null)
            {
                // .Trim() убирает лишние пробелы, если какое-то поле (например, отчество) пустое
                FIOTB.Text = $"{_currentUser.Surname} {_currentUser.Name} {_currentUser.Patronymic}".Trim();

                switch (_userRole)
                {
                    case 1:
                        RoleTB.Text = "Администратор";
                        break;
                    case 2:
                        RoleTB.Text = "Менеджер";
                        break;
                    case 3:
                        RoleTB.Text = "Авторизованный пользователь";
                        break;
                    default:
                        RoleTB.Text = "Неизвестная роль";
                        break;
                }
            }
            else
            {
                FIOTB.Text = "Гость";
                RoleTB.Text = "Гость";
            }
        }

        /// <summary>
        /// Метод обновления списка товаров с применением фильтров, сортировки и поиска в реальном времени
        /// </summary>
        private void UpdateProduct()
        {
            try
            {
                // Начинаем с полного списка товаров
                IEnumerable<Product> filteredProducts = _allProducts;

                // 1. Применение поиска по наименованию и описанию
                if (!string.IsNullOrWhiteSpace(SearchTBox.Text))
                {
                    string searchText = SearchTBox.Text.ToLower().Replace('ё', 'е');

                    filteredProducts = filteredProducts.Where(p =>
                    {
                        bool nameMatch = p.Product_Name != null && p.Product_Name.ToLower().Replace('ё', 'е').Contains(searchText);
                        bool descMatch = p.Discribtion != null && p.Discribtion.ToLower().Replace('ё', 'е').Contains(searchText);

                        return nameMatch || descMatch;
                    });
                }

                // 2. Применение сортировки
                if (SortTB.SelectedIndex > 0 && SortTB.SelectedItem is ComboBoxItem selectedSort)
                {
                    string sortType = selectedSort.Content.ToString();

                    switch (sortType)
                    {
                        case "по наименованию от А до Я":
                            filteredProducts = filteredProducts.OrderBy(p => p.Product_Name);
                            break;
                        case "по наименованию от Я до А":
                            filteredProducts = filteredProducts.OrderByDescending(p => p.Product_Name);
                            break;
                        case "По возрастанию цены":
                            filteredProducts = filteredProducts.OrderBy(p => p.Cost);
                            break;
                        case "По убыванию цены":
                            filteredProducts = filteredProducts.OrderByDescending(p => p.Cost);
                            break;
                    }
                }

                // 3. Применение фильтрации по категории
                if (FilterCBox.SelectedIndex > 0 && FilterCBox.SelectedItem is ComboBoxItem selectedFilter)
                {
                    string category = selectedFilter.Content.ToString();
                    filteredProducts = filteredProducts.Where(p => p.CategoryTitle == category);
                }

                // 4. Обновление источника данных для ListView (преобразуем IEnumerable обратно в List)
                ProductListView.ItemsSource = filteredProducts.ToList();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка фильтрации", $"Не удалось обновить список товаров.\nПодробности: {ex.Message}");
            }
        }

       
        private void ShowErrorMessage(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        
        private void SortTB_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => UpdateProduct();

        private void FilterCBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
            => UpdateProduct();

        private void SearchTBox_TextChanged(object sender, TextChangedEventArgs e)
            => UpdateProduct();

        
    }
}