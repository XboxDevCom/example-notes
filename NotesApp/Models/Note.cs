using System;

namespace NotesApp.Models
{
    /// <summary>
    /// Stellt eine einzelne Notiz mit Titel, Inhalt und Zeitstempeln dar.
    /// </summary>
    public sealed class Note
    {
        /// <summary>
        /// Ruft den eindeutigen Bezeichner der Notiz ab oder legt diesen fest.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ruft den Titel der Notiz ab oder legt diesen fest.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Ruft den Inhalt der Notiz ab oder legt diesen fest.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ruft den Erstellungszeitpunkt der Notiz ab oder legt diesen fest.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Ruft den Zeitpunkt der letzten Änderung der Notiz ab oder legt diesen fest.
        /// </summary>
        public DateTime ModifiedAt { get; set; }

        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="Note"/>-Klasse
        /// und generiert einen neuen Bezeichner sowie die aktuellen Zeitstempel.
        /// </summary>
        public Note()
        {
            Id = Guid.NewGuid();
            Title = string.Empty;
            Content = string.Empty;
            CreatedAt = DateTime.Now;
            ModifiedAt = DateTime.Now;
        }
    }
}
