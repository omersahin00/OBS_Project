using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_API.Concrete;
using Web_API.Entities;

namespace Web_API.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    public class NotesController : Controller
    {
        protected readonly SqlDbContext _context;

        public NotesController(SqlDbContext context)
        {
            _context = context;
        }


        [HttpGet("api/Notes/Get/WithNumber/{Number}")]
        public ActionResult<IEnumerable<Notes>> GetStudentAllNotes(string Number)
        {
            // Öğrencinin aldığı tüm notları döndürüyor:
            try
            {
                var student = _context.Students.FirstOrDefault(x => x.Number == Number);
                List<Notes> studentNoteList;
                //fghjk -> Aşe yazdı.
                if (student != null)
                {
                    studentNoteList = _context.Notes.Where(x => x.StudentNotesID == student.StudentNotesID).ToList();
                    return Ok(studentNoteList);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }
        }



        [HttpPost("api/Notes/Post/Create")]
        public ActionResult<Notes> CreateStudentNote(Student student)
        {
            if (ModelState.IsValid)
            {
                student.IsActive = true;
                _context.Students.Add(student);
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                return UnprocessableEntity();
            }
        }



        [HttpPost("api/Notes/Post/Update")]
        public ActionResult<Notes> UpdateNote(Notes note)
        {
            try
            {
                if (ModelState.IsValid && _context.Notes != null)
                {
                    Notes? noteData;

                    if (note.ID != 0)
                        noteData = _context.Notes.FirstOrDefault(x => x.ID == note.ID);

                    else if (note.StudentNotesID != 0)
                        noteData = _context.Notes.FirstOrDefault(x => x.StudentNotesID == note.StudentNotesID && x.CourseID == note.CourseID);

                    else
                        return UnprocessableEntity();

                    if (noteData != null)
                    {
                        noteData.Score = note.Score;
                        noteData.LetterScore = note.LetterScore;
                        noteData.IsActive = note.IsActive;
                        _context.Notes.Update(noteData);
                        _context.SaveChanges();
                        return NoContent();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return UnprocessableEntity();
                }
            }
            catch (Exception)
            {
                return UnprocessableEntity();
            }
        }

    }
}

