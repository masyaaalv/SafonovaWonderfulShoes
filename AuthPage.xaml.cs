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

namespace SafonovaWonderfulShoes
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage(null));
            MessageBox.Show("Вошли как гость");

        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = TBLogin.Text.Trim();

                // Валидация поля ввода
                if (string.IsNullOrWhiteSpace(login))
                {
                    MessageBox.Show("Пожалуйста, введите логин!",
                        "Ошибка ввода",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                User user = SafonovaShoesEntities.GetContext()
                    .Users.FirstOrDefault(p => p.Login == login);

                if (user != null)
                {
                    Manager.MainFrame.Navigate(new ProductPage(user));
                    TBLogin.Text = "";
                    MessageBox.Show($"Добро пожаловать, {user.Surname} {user.Name}!",
                        "Успешная авторизация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Пользователь с таким логином не найден.\nПроверьте правильность ввода логина.",
                        "Ошибка авторизации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при подключении к базе данных:\n{ex.Message}\n\nПопробуйте позже или обратитесь к администратору.",
                    "Ошибка системы",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
