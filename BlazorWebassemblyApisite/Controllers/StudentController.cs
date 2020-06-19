using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorWebassemblyApisite.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlazorWebassemblyApisite.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private IStudentRepository _studentRepository;
        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public List<Student> Get()
        {
            return _studentRepository.List();
        }

        [HttpGet("{id}")]
        public Student Get(int id)
        {
            return _studentRepository.Get(id);
        }

        [HttpPost]
        public Student Post(Student model)
        {
            _studentRepository.Add(model);

            return model;
        }

        [HttpPut]
        public Student Put(Student model)
        {
            _studentRepository.Update(model);

            return model;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _studentRepository.Delete(id);
        }
    }
}
