﻿using System;
using System.Collections.Generic;


namespace ExampleClient {

	public partial class ScheduleManage : FormForAuthorizedUser {

		public ScheduleManage() {
			InitializeComponent();
		}

		private void Form2_Load(object sender, EventArgs e) {
			dgv.ColumnCount = 6;
			dgv.Columns[0].Name = "ID";
			dgv.Columns[1].Name = "Имя";
			dgv.Columns[2].Name = "Фамилия";
			dgv.Columns[3].Name = "Отчество";
			dgv.Columns[4].Name = "Время";
			dgv.Columns[5].Name = "Дата";

			updateSchedule();
			showWorkersInAddShiftForm();
			showSmensInAddShiftForm();
		}

		/**
		 * Обновление раписания
		 */
		private void updateSchedule() {
			new APIRequest().makeAPIRequest("getSchedule", new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("authstr", CurrentAuthString)
			}, delegate (List<WorkShift> data) {
				dgv.Rows.Clear();
				foreach (WorkShift item in data) {
					addRow(item);
				}
			});
		}

		private void addRow(WorkShift item) {
			dgv.Rows.Add(item.smena.id_smena, item.worker.name, item.worker.surname, item.worker.midname, item.smena.start + "-" + item.smena.end, item.smena.date);
		}

		/**
		 * Клик по кнопке добавления
		 */
		private void button2_Click(object sender, EventArgs e) {
			new APIRequest().makeAPIRequest("addSmena", new List<KeyValuePair<string, string>>() {
				new KeyValuePair<string, string>("id_worker", add_worker.SelectedValue.ToString()),
				new KeyValuePair<string, string>("date", add_date.Value.ToShortDateString()),
				new KeyValuePair<string, string>("id_rasp", add_rasp.SelectedValue.ToString()),
				new KeyValuePair<string, string>("authstr", CurrentAuthString)
			}, delegate (int data) {
				updateSchedule();
			});
			showSmensInAddShiftForm();
		}

		/**
		 * Клик по кнопке обновления
		 */
		private void button1_Click(object sender, EventArgs e) {
			updateSchedule();
		}

		private void button4_Click(object sender, EventArgs e) {

		}

		private void showWorkersInAddShiftForm() {
			new APIRequest().makeAPIRequest("getWorkers", new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("authstr", CurrentAuthString)
			}, delegate (List<Worker> items) {
				List<ComboItem> src = new List<ComboItem>();
				foreach (Worker w in items) {
					src.Add(new ComboItem(w.name + " " + w.surname + " " + w.midname, w.id_worker));
				}
				add_worker.DataSource = src;
				add_worker.DisplayMember = "display";
				add_worker.ValueMember = "value";
			});
		}

		private void showSmensInAddShiftForm() {
			new APIRequest().makeAPIRequest("getAllSmena", new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("authstr", CurrentAuthString)
			}, delegate (List<Smena> items) {
				List<ComboItem> src = new List<ComboItem>();

				foreach (Smena w in items) {
					src.Add(new ComboItem(w.start + "-" + w.end + ":" + w.date, w.id_rasp));
				}

				add_rasp.DataSource = src;
				add_rasp.DisplayMember = "display";
				add_rasp.ValueMember = "value";
			});
		}

		private void button_delete_Click(object sender, EventArgs e) {
			if (dgv.SelectedRows.Count != 1) {
				return;
			}

			if (dgv.SelectedRows[0].Cells[0].Value == null) {
				return;
			}

			new APIRequest().makeAPIRequest("deleteSmena", new List<KeyValuePair<string, string>>() {
				new KeyValuePair<string, string>("id_smena", dgv.SelectedRows[0].Cells[0].Value.ToString()),
				new KeyValuePair<string, string>("authstr", CurrentAuthString)
			}, delegate (int data) {
				updateSchedule();
			});
		}

		private void button_edit_Click(object sender, EventArgs e) {

		}

		private void ScheduleManage_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e) {
			OnCloseForm();
		}
	}
}