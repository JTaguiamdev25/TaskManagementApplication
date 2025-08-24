using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using TaskManagementApplication.Entities.Infrastructure.Data;

namespace TaskManagementApplication
{
    public partial class DatabaseTestForm : Form
    {
        private readonly AppDbContext _context;

        public DatabaseTestForm(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                // Test connection
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    MessageBox.Show("✅ Database connection successful!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("❌ Cannot connect to database", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnCreateSampleData_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if database exists and create sample data
                await _context.Database.EnsureCreatedAsync();
                MessageBox.Show("✅ Database created successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
