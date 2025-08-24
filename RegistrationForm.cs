using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementApplication.Entities.DTOs;
using TaskManagementApplication.Entities.Entities;
using TaskManagementApplication.Entities.Services;

namespace TaskManagementApplication
{
    public partial class RegistrationForm : Form
    {
        private readonly IUserService _userService;
        private readonly IValidationService _validationService;
        private readonly IReminderPersonalityService _personalityService;

        // UI controls
        private TextBox usernameTextBox;
        private TextBox emailTextBox;
        private TextBox firstNameTextBox;
        private TextBox lastNameTextBox;
        private DateTimePicker dateOfBirthPicker;
        private TextBox ageTextBox;
        private TextBox passwordTextBox;
        private TextBox confirmPasswordTextBox;
        private TextBox securityQuestionTextBox;
        private TextBox securityAnswerTextBox;
        private ComboBox personalityComboBox;
        private Button registerButton;

        public RegistrationForm(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _validationService = serviceProvider.GetRequiredService<IValidationService>();
            _personalityService = serviceProvider.GetRequiredService<IReminderPersonalityService>();

            LoadPersonalities();
        }

        private void InitializeComponent()
        {
            usernameTextBox = new TextBox { Location = new System.Drawing.Point(10, 10), Width = 200 };
            emailTextBox = new TextBox { Location = new System.Drawing.Point(10, 40), Width = 200 };
            firstNameTextBox = new TextBox { Location = new System.Drawing.Point(10, 70), Width = 200 };
            lastNameTextBox = new TextBox { Location = new System.Drawing.Point(10, 100), Width = 200 };
            dateOfBirthPicker = new DateTimePicker { Location = new System.Drawing.Point(10, 130), Width = 200 };
            ageTextBox = new TextBox { Location = new System.Drawing.Point(10, 160), Width = 200, ReadOnly = true };
            passwordTextBox = new TextBox { Location = new System.Drawing.Point(10, 190), Width = 200, PasswordChar = '*' };
            confirmPasswordTextBox = new TextBox { Location = new System.Drawing.Point(10, 220), Width = 200, PasswordChar = '*' };
            securityQuestionTextBox = new TextBox { Location = new System.Drawing.Point(10, 250), Width = 200 };
            securityAnswerTextBox = new TextBox { Location = new System.Drawing.Point(10, 280), Width = 200 };
            personalityComboBox = new ComboBox { Location = new System.Drawing.Point(10, 310), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            registerButton = new Button { Location = new System.Drawing.Point(10, 340), Text = "Register" };

            dateOfBirthPicker.ValueChanged += DateOfBirthPicker_ValueChanged;
            registerButton.Click += RegisterButton_Click;

            Controls.Add(usernameTextBox);
            Controls.Add(emailTextBox);
            Controls.Add(firstNameTextBox);
            Controls.Add(lastNameTextBox);
            Controls.Add(dateOfBirthPicker);
            Controls.Add(ageTextBox);
            Controls.Add(passwordTextBox);
            Controls.Add(confirmPasswordTextBox);
            Controls.Add(securityQuestionTextBox);
            Controls.Add(securityAnswerTextBox);
            Controls.Add(personalityComboBox);
            Controls.Add(registerButton);
        }

        private void LoadPersonalities()
        {
            var personalities = Enum.GetValues(typeof(PersonalityType)).Cast<PersonalityType>();
            personalityComboBox.Items.Clear();
            foreach (var personality in personalities)
            {
                var description = _personalityService.GetPersonalityDescription(personality);
                personalityComboBox.Items.Add(new ComboBoxItem(personality, description));
            }
            if (personalityComboBox.Items.Count > 0)
                personalityComboBox.SelectedIndex = 0;
        }

        private void DateOfBirthPicker_ValueChanged(object sender, EventArgs e)
        {
            var age = DateTime.Now.Year - dateOfBirthPicker.Value.Year;
            if (DateTime.Now.DayOfYear < dateOfBirthPicker.Value.DayOfYear)
                age--;

            ageTextBox.Text = age.ToString();
        }

        private PersonalityType GetSelectedPersonality()
        {
            if (personalityComboBox.SelectedItem is ComboBoxItem item)
                return item.Personality;
            return PersonalityType.FlirtyPlayful; // Default fallback
        }

        private async void RegisterButton_Click(object sender, EventArgs e)
        {
            try
            {
                var dto = new UserRegistrationDto
                {
                    Username = usernameTextBox.Text.Trim(),
                    Email = emailTextBox.Text.Trim(),
                    FirstName = firstNameTextBox.Text.Trim(),
                    LastName = lastNameTextBox.Text.Trim(),
                    DateOfBirth = dateOfBirthPicker.Value,
                    Password = passwordTextBox.Text,
                    ConfirmPassword = confirmPasswordTextBox.Text,
                    SecurityQuestion = securityQuestionTextBox.Text.Trim(),
                    SecurityAnswer = securityAnswerTextBox.Text.Trim(),
                    ReminderPersonality = GetSelectedPersonality()
                };

                var validation = _validationService.ValidateUserRegistration(dto);
                if (!validation.IsValid)
                {
                    MessageBox.Show(string.Join("\n", validation.Errors), "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var user = await _userService.RegisterAsync(dto);
                if (user != null)
                {
                    MessageBox.Show("Registration successful! You can now log in.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Registration Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper class for ComboBox items
        private class ComboBoxItem
        {
            public PersonalityType Personality { get; }
            public string Description { get; }

            public ComboBoxItem(PersonalityType personality, string description)
            {
                Personality = personality;
                Description = description;
            }

            public override string ToString() => Description;
        }
    }
}
