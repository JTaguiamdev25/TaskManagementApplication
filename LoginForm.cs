using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using TaskManagementApplication.Entities.DTOs;
using TaskManagementApplication.Entities.Services;

public partial class LoginForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserService _userService;

    // UI controls
    private TextBox usernameTextBox;
    private TextBox passwordTextBox;
    private CheckBox showPasswordCheckBox;
    private Button loginButton;

    public LoginForm(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _userService = serviceProvider.GetRequiredService<IUserService>();
    }

    private void InitializeComponent()
    {
        usernameTextBox = new TextBox { Location = new System.Drawing.Point(10, 10), Width = 200 };
        passwordTextBox = new TextBox { Location = new System.Drawing.Point(10, 40), Width = 200, PasswordChar = '*' };
        showPasswordCheckBox = new CheckBox { Location = new System.Drawing.Point(10, 70), Text = "Show Password" };
        loginButton = new Button { Location = new System.Drawing.Point(10, 100), Text = "Login" };

        showPasswordCheckBox.CheckedChanged += ShowPasswordCheckBox_CheckedChanged;
        loginButton.Click += LoginButton_Click;

        Controls.Add(usernameTextBox);
        Controls.Add(passwordTextBox);
        Controls.Add(showPasswordCheckBox);
        Controls.Add(loginButton);
    }

    private async void LoginButton_Click(object sender, EventArgs e)
    {
        try
        {
            var loginDto = new UserLoginDto
            {
                Username = usernameTextBox.Text.Trim(),
                Password = passwordTextBox.Text
            };

            var user = await _userService.LoginAsync(loginDto);
            if (user != null)
            {
                var notificationService = _serviceProvider.GetRequiredService<INotificationService>();
                await notificationService.SendWelcomeNotificationAsync(user);

                var dashboardForm = new DashboardForm(_serviceProvider, user);
                this.Hide();
                dashboardForm.Show();
            }
            else
            {
                MessageBox.Show("Invalid username or password!", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Login error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        passwordTextBox.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
    }
}