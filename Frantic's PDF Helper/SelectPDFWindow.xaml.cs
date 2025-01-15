﻿using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Frantics_PDF_Helper.Utilities;

using Window = System.Windows.Window;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Frantics_PDF_Helper
{
	/// <summary>
	/// Interaction logic for SelectPDFWindow.xaml
	/// </summary>
	public partial class SelectPDFWindow : Window
	{
		OpenFileDialog fileDialog = new();
		MainWindow mainWindow = new();

		/// <summary>
		/// Is this window safe to close?
		/// If not, we will shut down the application instead.
		/// Otherwise the app may keep running in the background.
		/// </summary>
		public bool isSafeToClose = false;
		public SelectPDFWindow()
		{
			InitializeComponent();

			Localisation.LocaliseWindow(this);

			//this.Title = Localisation.GetLocalisedString("_AppName");
			//Localisation.SetTaggedButtonContent(loadFileButton, true);
			//Localisation.SetTaggedButtonContent(browseFileButton, true);

			foreach (PDFCacheMode mode in Enum.GetValues(typeof(PDFCacheMode)))
			{
				cacheModeComboBox.Items.Add(Localisation.GetLocalisedEnumString<PDFCacheMode>(mode));
			}

			// This is done in SplashWindow
			//Settings.LoadSettings();
			cacheModeComboBox.SelectedIndex = (int)Settings.Instance.CacheMode;
			pdfFileDirTextBox.Text = Settings.Instance.LastPDFPath;

		}

		private void BrowseFileButton_Click (object sender, RoutedEventArgs e)
		{
			// Open file dialog
			fileDialog.Filter = Localisation.GetLocalisedString("OpenFileDialog.pdfFiles") + " (*.pdf)|*.pdf|" + Localisation.GetLocalisedString("OpenFileDialog.allFiles") + " (*.*)|*.*";
			fileDialog.DefaultDirectory = !Path.IsPathFullyQualified(pdfFileDirTextBox.Text) ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) : Path.GetDirectoryName(pdfFileDirTextBox.Text);
			fileDialog.InitialDirectory = !Path.IsPathFullyQualified(pdfFileDirTextBox.Text) ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) : Path.GetDirectoryName(pdfFileDirTextBox.Text);
			if (fileDialog.ShowDialog() == true)
			{
				pdfFileDirTextBox.Text = fileDialog.FileName;
			}
		}

		private void LoadFileButton_Click (object sender, RoutedEventArgs e)
		{
			if (!File.Exists(pdfFileDirTextBox.Text))
			{
				return;
			}

			Settings.Instance.LastPDFPath = pdfFileDirTextBox.Text;
			Settings.Instance.CacheMode = (PDFCacheMode)cacheModeComboBox.SelectedIndex;
			Settings.SaveSettings();

			// Load PDF file
			mainWindow.LoadPDF(pdfFileDirTextBox.Text);
			mainWindow.Show();
			isSafeToClose = true;
			this.Close();
		}

		private void Window_Closing (object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (!isSafeToClose)
			{
				Application.Current.Shutdown();
			}
		}
	}
}
