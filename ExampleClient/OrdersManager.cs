﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExampleClient {
	public partial class OrdersManager : FormForAuthorizedUser {

		public OrdersManager() {
			InitializeComponent();
		}

		private void OnLoad(object sender, EventArgs e) {
			dgv.ColumnCount = 5;
			dgv.Columns[0].Name = "ID";
			dgv.Columns[1].Name = "Описание";
			dgv.Columns[2].Name = "Адрес";
			dgv.Columns[3].Name = "ФИО";
			dgv.Columns[4].Name = "Дата";

			UpdateOrders();
			ShowWorkersInAddOrderForm();
		}

		private void UpdateOrders() {
			new APIRequest().makeAPIRequest("getOrders", new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("authstr", CurrentAuthString)
			}, delegate (List<OrderWithWorker> data) {
				dgv.Rows.Clear();
				foreach (OrderWithWorker item in data) {
					addRow(item);
				}
			});
		}

		private void addRow(OrderWithWorker item) {
			dgv.Rows.Add(item.order.id_order, item.order.order, item.order.address, item.worker.name + " " + item.worker.surname + " " + item.worker.midname, item.order.date);
		}

		private void ShowWorkersInAddOrderForm() {
			new APIRequest().makeAPIRequest("getWorkers", new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("authstr", CurrentAuthString)
			}, delegate (List<Worker> items) {
				List<ComboItem> src = new List<ComboItem>();

				foreach (Worker w in items) {
					src.Add(new ComboItem(w.name, w.id_worker));
				}

				add_worker.DataSource = src;
				add_worker.DisplayMember = "display";
				add_worker.ValueMember = "value";
			});
		}

		private void button_update_Click(object sender, EventArgs e) {
			UpdateOrders();
		}

		private void button_edit_Click(object sender, EventArgs e) {

		}

		private void button_delete_Click(object sender, EventArgs e) {
			if (dgv.SelectedRows.Count != 1) {
				return;
			}

			new APIRequest().makeAPIRequest("deleteOrder", new List<KeyValuePair<string, string>>() {
				new KeyValuePair<string, string>("id_order", dgv.SelectedRows[0].Cells[0].Value.ToString()),
				new KeyValuePair<string, string>("authstr", CurrentAuthString)
			}, delegate (int data) {
				UpdateOrders();
			});
		}

		private void button_add_Click(object sender, EventArgs e) {
			new APIRequest().makeAPIRequest("addOrder", new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("id_worker", add_worker.SelectedValue.ToString()),
				new KeyValuePair<string, string>("address", textBox1.Text.ToString()),
				new KeyValuePair<string, string>("order", textBox2.Text.ToString()),
				new KeyValuePair<string, string>("authstr", CurrentAuthString)
			}, delegate (int data) {
				UpdateOrders();
			});
		}

		private void NewOrders_FormClosed(object sender, FormClosedEventArgs e) {
			OnCloseForm();
		}
	}
}
