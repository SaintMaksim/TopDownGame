using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TopDownGame.Models;
using System.Linq;

namespace TopDownGame.Views
{
    public class InventoryForm : Form
    {
        private readonly Inventory _inventory;
        private DataGridView _itemsGrid;
        private Label _goldLabel;

        public InventoryForm(Inventory inventory)
        {
            _inventory = inventory;
            InitializeComponents();
            UpdateInventoryView();
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.I) this.Close(); };
        }

        private void InitializeComponents()
        {
            this.Text = "Инвентарь";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            _goldLabel = new Label
            {
                Text = $"Золото: {_inventory.Gold}",
                Location = new Point(20, 20),
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true
            };

            _itemsGrid = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(340, 180),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                ScrollBars = ScrollBars.Vertical,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };

            _itemsGrid.Columns.Add("Item", "Предмет");
            _itemsGrid.Columns.Add("Count", "Количество");

            this.Controls.Add(_goldLabel);
            this.Controls.Add(_itemsGrid);
        }

        public void UpdateInventoryView()
        {
            _goldLabel.Text = $"Золото: {_inventory.Gold}";
            _itemsGrid.Rows.Clear();
            foreach (var item in _inventory.GetAllItems())
            {
                _itemsGrid.Rows.Add(item.Key, item.Value);
            }
        }
    }
}