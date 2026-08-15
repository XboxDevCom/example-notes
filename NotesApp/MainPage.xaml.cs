using System;
using System.Collections.ObjectModel;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using NotesApp.Models;

namespace NotesApp
{
    /// <summary>
    /// Hauptseite der Notiz-App mit Listen- und Bearbeitungsansicht.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly ObservableCollection<Note> _notes;
        private readonly NoteStorage _storage;
        private Note _currentNote;

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="MainPage"/>-Klasse.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            _notes = new ObservableCollection<Note>();
            _storage = new NoteStorage();
            this.KeyDown += MainPage_KeyDown;
        }

        /// <summary>
        /// Behandelt die B-Taste des Gamepads, um das Bearbeitungsfeld zu schließen.
        /// </summary>
        private void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.GamepadB && EditPanel.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                EditPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                EmptyState.Visibility = Windows.UI.Xaml.Visibility.Visible;
                _currentNote = null;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Seite navigiert wird, und lädt die gespeicherten Notizen.
        /// </summary>
        /// <param name="e">Ereignisdaten für das Navigationsereignis.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await LoadNotesAsync();
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf die Schaltfläche "Neue Notiz" geklickt wird.
        /// Erstellt eine neue Notiz und zeigt das Bearbeitungsfeld an.
        /// </summary>
        /// <param name="sender">Die Quelle des Ereignisses.</param>
        /// <param name="e">Ereignisdaten.</param>
        private void NewNoteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _currentNote = new Note();
            DisplayNote(_currentNote);
            EditPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            EmptyState.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf ein Element in der Notizenliste geklickt wird.
        /// Lädt die ausgewählte Notiz in das Bearbeitungsfeld.
        /// </summary>
        /// <param name="sender">Die Quelle des Ereignisses.</param>
        /// <param name="e">Ereignisdaten mit der angeklickten Notiz.</param>
        private void NotesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            _currentNote = e.ClickedItem as Note;
            if (_currentNote != null)
            {
                DisplayNote(_currentNote);
                EditPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                EmptyState.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf die Schaltfläche "Speichern" geklickt wird.
        /// Speichert die aktuelle Notiz und aktualisiert die Liste.
        /// </summary>
        /// <param name="sender">Die Quelle des Ereignisses.</param>
        /// <param name="e">Ereignisdaten.</param>
        private async void SaveButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_currentNote == null)
                return;

            _currentNote.Title = string.IsNullOrWhiteSpace(TitleBox.Text) ? "Ohne Titel" : TitleBox.Text;
            _currentNote.Content = ContentBox.Text;
            _currentNote.ModifiedAt = DateTime.Now;

            await _storage.SaveNoteAsync(_currentNote);
            await LoadNotesAsync();
        }

        /// <summary>
        /// Wird aufgerufen, wenn auf die Schaltfläche "Löschen" geklickt wird.
        /// Löscht die aktuelle Notiz und aktualisiert die Liste.
        /// </summary>
        /// <param name="sender">Die Quelle des Ereignisses.</param>
        /// <param name="e">Ereignisdaten.</param>
        private async void DeleteButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_currentNote == null)
                return;

            await _storage.DeleteNoteAsync(_currentNote.Id);
            _currentNote = null;
            EditPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            EmptyState.Visibility = Windows.UI.Xaml.Visibility.Visible;
            await LoadNotesAsync();
        }

        /// <summary>
        /// Lädt alle Notizen aus dem Speicher und aktualisiert die Listenansicht.
        /// </summary>
        private async System.Threading.Tasks.Task LoadNotesAsync()
        {
            var loadedNotes = await _storage.LoadNotesAsync();
            _notes.Clear();
            foreach (var note in loadedNotes)
                _notes.Add(note);
            NotesList.ItemsSource = _notes;
        }

        /// <summary>
        /// Zeigt die Daten einer Notiz im Bearbeitungsfeld an.
        /// </summary>
        /// <param name="note">Die anzuzeigende Notiz.</param>
        private void DisplayNote(Note note)
        {
            TitleBox.Text = note.Title ?? string.Empty;
            ContentBox.Text = note.Content ?? string.Empty;
        }
    }
}
