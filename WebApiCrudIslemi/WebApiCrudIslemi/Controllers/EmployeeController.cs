using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiCrudIslemi.Controllers
{
    public class EmployeeController : ApiController
    {
        #region dbnesnesi
        EmployeeEntities db = new EmployeeEntities();
        //newleyerek gerekli işlemleri yapmak için db yi çektik
        #endregion


        public HttpResponseMessage Get(string gender = "all", int? top = 0)
        {
            IQueryable<Employee> query = db.Employees;//sorguyu oluşturdum ama execute etmedim 
            //ıqueryable select * from 
            gender = gender.ToLower();
            switch (gender)
            {
                case "all":
                    break;
                case "male":
                case "famale":
                    query = query.Where(x => x.Gender.ToLower() == gender);//select sorguma where sorgumuda ekledim
                    break;

                default:
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"{gender} is not a valid gender. please use all,male or famale.");
            }
            if (top > 0)
            {
                query = query.Take(top.Value);//dönen sorguya top çekmiş oldum
            }
            return Request.CreateResponse(HttpStatusCode.OK, query.ToList());//sorguya execute edilmeişlemi
        }

        public HttpResponseMessage Get(int id)
        {
            Employee employee = db.Employees.FirstOrDefault(x => x.Id == id);
            if (employee == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"Id'si {id} olan çalışan bulunamadı");
            }
            return Request.CreateErrorResponse(HttpStatusCode.OK, employee.ToString());
        }

        public HttpResponseMessage Post(Employee employee)
        {
            try
            {
                db.Employees.Add(employee);
                if (db.SaveChanges() > 0)
                {
                    HttpResponseMessage message = Request.CreateErrorResponse(HttpStatusCode.Created, employee.ToString());
                    message.Headers.Location = new Uri(Request.RequestUri + "/" + employee.Id);//insert edilen employee nin location bilgisi
                    return message;//işlem başarılı olursa message geri döncek
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Insert işlemi yapılamadı");
                }

            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        public HttpResponseMessage Put([FromBody] MyBodyType myType, [FromUri] Employee employee)
        {
            try
            {
                Employee emp = db.Employees.FirstOrDefault(x => x.Id == employee.Id);

                if (emp == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee ıd:" + employee.Id);
                }
                else
                {
                    emp.Name = employee.Name;
                    emp.Surname = employee.Surname;
                    emp.Salary = employee.Salary;
                    emp.Gender = employee.Gender;
                    if (db.SaveChanges() > 0)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.Created, employee.ToString());
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Güncelleme yapılamadı");
                    }
                }

            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        public HttpResponseMessage Delete(int id)
        {
            try
            {
                Employee emp = db.Employees.FirstOrDefault(x => x.Id == id);

                if (emp == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee ıd:" + id);
                }
                else
                {
                    db.Employees.Remove(emp);
                    if (db.SaveChanges() > 0)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "Employee id:" + id);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "silme işlemi yapılamadı");
                    }
                }
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public class MyBodyType
        {
            public int id { get; set; }
            public string second { get; set; }
        }

    }
}
