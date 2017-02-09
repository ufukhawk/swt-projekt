﻿using Lingvo.Common.Entities;
using System;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Lingvo.MobileApp.Entities
{
    /// <summary>
    /// Teacher memo: teacher can record a scentence or word the student cant pronounce easily.
    /// </summary>
	[Table("TeacherMemos")]
    public class TeacherMemo : IExercise
    {
        [ForeignKey(typeof(Recording))]
        public int RecordingId { get; set; }

        [ForeignKey(typeof(Recording))]
        public int? StudentTrackId { get; set; }

        public String Name { get; set; }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [OneToOne]
        public Recording TeacherTrack { get; set; }

        [OneToOne]
        public Recording StudentTrack { get; set; }

        public TeacherMemo()
        {
        }

        public void DeleteStudentRecording()
        {
            StudentTrack = null;
            StudentTrackId = 0;
        }
    }
}
