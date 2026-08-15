using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NotesApp.Models
{
    /// <summary>
    /// Verwaltet das lokale Speichern und Laden von Notizen über Windows.Storage.
    /// </summary>
    public sealed class NoteStorage
    {
        private readonly StorageFolder _localFolder;

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="NoteStorage"/>-Klasse.
        /// </summary>
        public NoteStorage()
        {
            _localFolder = ApplicationData.Current.LocalFolder;
        }

        /// <summary>
        /// Lädt alle gespeicherten Notizen aus dem lokalen Ordner.
        /// </summary>
        /// <returns>Eine nach Änderungsdatum absteigend sortierte Liste von Notizen.</returns>
        public async Task<List<Note>> LoadNotesAsync()
        {
            var notes = new List<Note>();
            var files = await _localFolder.GetFilesAsync();

            foreach (var file in files)
            {
                if (!file.Name.EndsWith(".json"))
                    continue;

                string jsonText = await FileIO.ReadTextAsync(file);
                var note = ParseNote(jsonText);
                if (note != null)
                    notes.Add(note);
            }

            return notes.OrderByDescending(n => n.ModifiedAt).ToList();
        }

        /// <summary>
        /// Speichert eine einzelne Notiz als JSON-Datei im lokalen Ordner.
        /// </summary>
        /// <param name="note">Die zu speichernde Notiz.</param>
        public async Task SaveNoteAsync(Note note)
        {
            string jsonText = SerializeNote(note);
            string fileName = note.Id.ToString() + ".json";
            var file = await _localFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, jsonText);
        }

        /// <summary>
        /// Löscht die einer GUID zugeordnete Notizdatei aus dem lokalen Ordner.
        /// </summary>
        /// <param name="id">Der Bezeichner der zu löschenden Notiz.</param>
        public async Task DeleteNoteAsync(Guid id)
        {
            string fileName = id.ToString() + ".json";
            var file = await _localFolder.TryGetItemAsync(fileName) as StorageFile;
            if (file != null)
                await file.DeleteAsync();
        }

        /// <summary>
        /// Deserialisiert einen JSON-String in ein <see cref="Note"/>-Objekt.
        /// </summary>
        /// <param name="jsonText">Der JSON-String.</param>
        /// <returns>Die deserialisierte Notiz oder null bei einem Fehler.</returns>
        private Note ParseNote(string jsonText)
        {
            try
            {
                JsonObject jsonObject;
                if (!JsonObject.TryParse(jsonText, out jsonObject))
                    return null;

                var note = new Note();

                if (jsonObject.ContainsKey("Id"))
                    note.Id = Guid.Parse(jsonObject.GetNamedString("Id"));
                if (jsonObject.ContainsKey("Title"))
                    note.Title = jsonObject.GetNamedString("Title");
                if (jsonObject.ContainsKey("Content"))
                    note.Content = jsonObject.GetNamedString("Content");
                if (jsonObject.ContainsKey("CreatedAt"))
                    note.CreatedAt = DateTime.Parse(jsonObject.GetNamedString("CreatedAt"));
                if (jsonObject.ContainsKey("ModifiedAt"))
                    note.ModifiedAt = DateTime.Parse(jsonObject.GetNamedString("ModifiedAt"));

                return note;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Serialisiert ein <see cref="Note"/>-Objekt in einen JSON-String.
        /// </summary>
        /// <param name="note">Die zu serialisierende Notiz.</param>
        /// <returns>Der JSON-String der Notiz.</returns>
        private string SerializeNote(Note note)
        {
            var jsonObject = new JsonObject();
            jsonObject.Add("Id", JsonValue.CreateStringValue(note.Id.ToString()));
            jsonObject.Add("Title", JsonValue.CreateStringValue(note.Title ?? string.Empty));
            jsonObject.Add("Content", JsonValue.CreateStringValue(note.Content ?? string.Empty));
            jsonObject.Add("CreatedAt", JsonValue.CreateStringValue(note.CreatedAt.ToString("o")));
            jsonObject.Add("ModifiedAt", JsonValue.CreateStringValue(note.ModifiedAt.ToString("o")));
            return jsonObject.Stringify();
        }
    }
}
