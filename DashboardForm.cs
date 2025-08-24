using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskManagementApplication.Entities;
using TaskManagementApplication.Entities.Services;

public partial class DashboardForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITaskService _taskService;
    private readonly INotificationService _notificationService;
    private readonly User _currentUser;

    // UI controls
    private Label usernameLabel;
    private ListView tasksListView;
    private Label totalTasksLabel;
    private Label completedTasksLabel;
    private Label pendingTasksLabel;
    private ProgressBar progressBar;
    private Label progressLabel;
    private Button addTaskButton;

    public DashboardForm(IServiceProvider serviceProvider, User currentUser)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        _taskService = serviceProvider.GetRequiredService<ITaskService>();
        _notificationService = serviceProvider.GetRequiredService<INotificationService>();
        _currentUser = currentUser;

        LoadDashboard();
    }

    private void InitializeComponent()
    {
        usernameLabel = new Label();
        tasksListView = new ListView();
        totalTasksLabel = new Label();
        completedTasksLabel = new Label();
        pendingTasksLabel = new Label();
        progressBar = new ProgressBar();
        progressLabel = new Label();
        addTaskButton = new Button();
        SuspendLayout();
        // 
        // usernameLabel
        // 
        usernameLabel.Location = new Point(0, 0);
        usernameLabel.Name = "usernameLabel";
        usernameLabel.Size = new Size(100, 23);
        usernameLabel.TabIndex = 0;
        // 
        // tasksListView
        // 
        tasksListView.Location = new Point(0, 0);
        tasksListView.Name = "tasksListView";
        tasksListView.Size = new Size(121, 97);
        tasksListView.TabIndex = 1;
        tasksListView.UseCompatibleStateImageBehavior = false;
        // 
        // totalTasksLabel
        // 
        totalTasksLabel.Location = new Point(0, 0);
        totalTasksLabel.Name = "totalTasksLabel";
        totalTasksLabel.Size = new Size(100, 23);
        totalTasksLabel.TabIndex = 2;
        // 
        // completedTasksLabel
        // 
        completedTasksLabel.Location = new Point(0, 0);
        completedTasksLabel.Name = "completedTasksLabel";
        completedTasksLabel.Size = new Size(100, 23);
        completedTasksLabel.TabIndex = 3;
        // 
        // pendingTasksLabel
        // 
        pendingTasksLabel.Location = new Point(0, 0);
        pendingTasksLabel.Name = "pendingTasksLabel";
        pendingTasksLabel.Size = new Size(100, 23);
        pendingTasksLabel.TabIndex = 4;
        // 
        // progressBar
        // 
        progressBar.Location = new Point(0, 0);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(100, 23);
        progressBar.TabIndex = 5;
        // 
        // progressLabel
        // 
        progressLabel.Location = new Point(0, 0);
        progressLabel.Name = "progressLabel";
        progressLabel.Size = new Size(100, 23);
        progressLabel.TabIndex = 6;
        // 
        // addTaskButton
        // 
        addTaskButton.Location = new Point(0, 0);
        addTaskButton.Name = "addTaskButton";
        addTaskButton.Size = new Size(75, 23);
        addTaskButton.TabIndex = 7;
        addTaskButton.Click += AddTaskButton_Click;
        // 
        // DashboardForm
        // 
        ClientSize = new Size(665, 345);
        Controls.Add(usernameLabel);
        Controls.Add(tasksListView);
        Controls.Add(totalTasksLabel);
        Controls.Add(completedTasksLabel);
        Controls.Add(pendingTasksLabel);
        Controls.Add(progressBar);
        Controls.Add(progressLabel);
        Controls.Add(addTaskButton);
        Name = "DashboardForm";
        Load += DashboardForm_Load;
        ResumeLayout(false);
    }

    private async void LoadDashboard()
    {
        // Display username
        usernameLabel.Text = $"Welcome, {_currentUser.FirstName}!";

        // Load tasks
        await LoadTasksAsync();

        // Load progress
        await LoadProgressAsync();

        // Load upcoming tasks
        await LoadUpcomingTasksAsync();
    }

    private async Task LoadTasksAsync()
    {
        var tasks = await _taskService.GetUserTasksAsync(_currentUser.Id);

        tasksListView.Items.Clear();
        foreach (var task in tasks)
        {
            var item = new ListViewItem(task.Title);
            item.SubItems.Add(task.DueDate.ToShortDateString());
            item.SubItems.Add(task.IsCompleted ? "Completed" : "Pending");
            item.Tag = task;
            tasksListView.Items.Add(item);
        }
    }

    private async Task LoadProgressAsync()
    {
        var progress = await _taskService.GetTaskProgressAsync(_currentUser.Id);

        // Update progress UI
        totalTasksLabel.Text = $"Total: {progress.TotalTasks}";
        completedTasksLabel.Text = $"Completed: {progress.CompletedTasks}";
        pendingTasksLabel.Text = $"Pending: {progress.PendingTasks}";

        // Update progress bar
        progressBar.Value = (int)progress.CompletionPercentage;
        progressLabel.Text = $"{progress.CompletionPercentage:F1}%";
    }

    private async Task LoadUpcomingTasksAsync()
    {
        // Example implementation: you may want to display these in a separate control
        var upcomingTasks = await _taskService.GetUpcomingTasksAsync(_currentUser.Id);
        // You can add code here to display upcoming tasks if needed
    }

    private async void AddTaskButton_Click(object sender, EventArgs e)
    {
        // You need to implement AddTaskForm in your project for this to work
        // using (var addTaskForm = new AddTaskForm(_serviceProvider, _currentUser.Id))
        // {
        //     if (addTaskForm.ShowDialog() == DialogResult.OK)
        //     {
        //         await LoadTasksAsync();
        //         await LoadProgressAsync();
        //     }
        // }
    }

    private void DashboardForm_Load(object sender, EventArgs e)
    {

    }
}
